namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Tapas.Common;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Data.Models.Enums;
    using Tapas.Services.Contracts;
    using Tapas.Services.Data.Contracts;
    using Tapas.Services.Dto.Mistral;
    using Tapas.Web.ViewModels.Addreses;
    using Tapas.Web.ViewModels.Administration.Sizes;
    using Tapas.Web.ViewModels.Extras;
    using Tapas.Web.ViewModels.Orders;
    using Tapas.Web.ViewModels.ShopingCartItems;

    public class OrdersService : IOrdersService
    {
        private const int LocationId = 1;
        private const int StoreId = 1;
        private const decimal Qtty = 1m;
        private readonly IRepository<Order> ordersRepository;
        private readonly IDeletableEntityRepository<ShopingCart> cartRepository;
        private readonly IDeletableEntityRepository<ShopingCartItem> itemsRepository;
        private readonly IDeletableEntityRepository<ProductSize> sizeRepository;
        private readonly IDeletableEntityRepository<DeliveryAddress> addressRepository;
        private readonly IDeletableEntityRepository<DeliveryTax> deliveryTaxRepository;
        private readonly IMistralService mistralService;

        public OrdersService(
            IRepository<Order> ordersRepository,
            IDeletableEntityRepository<ShopingCart> cartRepository,
            IDeletableEntityRepository<ShopingCartItem> itemsRepository,
            IDeletableEntityRepository<ProductSize> sizeRepository,
            IDeletableEntityRepository<DeliveryAddress> addressRepository,
            IDeletableEntityRepository<DeliveryTax> deliveryTaxRepository,
            IMistralService mistralService)
        {
            this.ordersRepository = ordersRepository;
            this.cartRepository = cartRepository;
            this.itemsRepository = itemsRepository;
            this.sizeRepository = sizeRepository;
            this.addressRepository = addressRepository;
            this.deliveryTaxRepository = deliveryTaxRepository;
            this.mistralService = mistralService;
        }

        public async Task<string> ChangeStatusAsync(string status, string orderId, string setTime, string taxId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ArgumentNullException("OrderId is null.");
            }

            if (int.TryParse(orderId, out int id))
            {
                if (Enum.TryParse(typeof(OrderStatus), status, out object statusResult))
                {
                    try
                    {
                        var order = this.ordersRepository.All().Where(x => x.Id == id).FirstOrDefault();
                        if (order is null)
                        {
                            throw new ArgumentException("Order is not exist.");
                        }

                        order.Status = (OrderStatus)statusResult;

                        if (int.TryParse(setTime, out int minutesToDelivery))
                        {
                            order.MinutesForDelivery = minutesToDelivery;
                        }

                        switch (statusResult)
                        {
                            case OrderStatus.Processed:
                                if (!string.IsNullOrEmpty(taxId) && int.TryParse(taxId, out int t))
                                {
                                    order.DeliveryTaxId = t;
                                }

                                order.ProcessingTime = DateTime.UtcNow;
                                await this.SendOrderToMistraalAsync(order);
                                break;
                            case OrderStatus.OnDelivery: order.OnDeliveryTime = DateTime.UtcNow; break;
                            case OrderStatus.Delivered: order.DeliveredTime = DateTime.UtcNow; break;
                            default: break;
                        }

                        await this.ordersRepository.SaveChangesAsync();
                        return order.UserId;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }

                throw new ArgumentException("Status is not valid.");
            }
            else
            {
                throw new ArgumentException("OrderId is not valid integer.");
            }
        }

        // Post Orders/Create
        public async Task<int> CreateAsync(ApplicationUser user, OrderInpitModel model)
        {
            if (model.TakeAway)
            {
                model.AddressId = this.addressRepository.All().Where(a => a.DisplayName == GlobalConstants.TakeAway).FirstOrDefault()?.Id;
            }

            var order = new Order()
            {
                AddInfo = model.AddInfo,
                AddressId = model.AddressId,
                UserId = user.Id,
                Status = OrderStatus.Unprocessed,
                CreatedOn = DateTime.UtcNow,
                TakeAway = model.TakeAway,
                Cutlery = model.CutleryCount,
            };

            foreach (var item in user.ShopingCart.CartItems)
            {
                order.Bag.CartItems.Add(item);
            }

            await this.ordersRepository.AddAsync(order);
            await this.ordersRepository.SaveChangesAsync();

            user.ShopingCart.CartItems.Clear();

            return order.Id;
        }

        // Ajax Orders/Details
        public OrderDetailsViewModel GetDetailsById(int id)
        {
            var order = this.ordersRepository.All().Where(x => x.Id == id).FirstOrDefault();

            if (order is null)
            {
                throw new ArgumentException("Order not exists.");
            }

            var model = new OrderDetailsViewModel()
            {
                CreatedOn = order.CreatedOn.ToLocalTime().ToString("dd/MM/yy HH:mm"),
                OrderId = order.Id,
                DisplayAddress = order.Address.DisplayName,
                AddressInfo = order.Address.AddInfo,
                UserUserName = order.User.UserName,
                UserPhone = order.User.PhoneNumber,
                AddInfo = order.AddInfo,
                TakeAway = order.TakeAway,
                CutleryCount = order.Cutlery,
                CartItems = order.Bag.CartItems
                    .Select(x => new ShopingItemsViewModel()
                    {
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductPrice = x.Size.Price,
                        Quantity = x.Quantity,
                        Description = x.Description,
                        Extras = x.ExtraItems
                                  ?.Select(e => new ExtraCartItemModel()
                                  {
                                      Name = e.Extra.Name,
                                      Price = e.Extra.Price,
                                      Quantity = e.Quantity,
                                  }).ToList(),
                        Size = new ProductSizeViewModel()
                        {
                            SizeName = this.sizeRepository
                                           .All()
                                           .Where(s => s.MenuProductId == x.ProductId)
                                           .Count() > 1 ? x.Size.SizeName : null,
                        },
                    }).ToList(),
                Status = order.Status,
                PackagesPrice = order.Bag.CartItems.Sum(x => (decimal)Math.Ceiling((double)x.Size.MaxProductsInPackage / x.Quantity) * x.Size.Package.Price),
            };
            model.TotalPrice = model.CartItems.Sum(x => x.ItemPrice) + model.PackagesPrice;

            if (model.TotalPrice < GlobalConstants.MOPTCDF && !order.TakeAway && order.Status == OrderStatus.Unprocessed)
            {
                model.Taxes = this.deliveryTaxRepository
                  .All()
                  .OrderBy(x => x.Price)
                  .Select(x => new SelectListItem()
                  {
                      Text = x.MistralName,
                      Value = x.Id.ToString(),
                      Selected = false,
                  }).ToList();
            }

            if (order.DeliveryTaxId.HasValue)
            {
                var x = this.deliveryTaxRepository.All().Where(x => x.Id == order.DeliveryTaxId).FirstOrDefault().Price;
                model.TotalPrice += x;
                model.DeliveryFee = x;
            }

            if (model.Status != OrderStatus.Unprocessed)
            {
                model.TimeForDelivery = order.ProcessingTime.ToLocalTime().AddMinutes((double)order.MinutesForDelivery).ToString("dd/MM/yy HH:mm");
            }

            return model;
        }

        // Orders/Index
        public ICollection<OrdersViewModel> GetDailyOrders()
        {
            return this.ordersRepository.All().AsEnumerable()
                .Where(x => x.CreatedOn.ToLocalTime().Date == DateTime.Now.Date)
                .OrderByDescending(x => x.Id)
                .Select(x => new OrdersViewModel()
                {
                    Id = x.Id,
                    Status = x.Status.ToString(),
                }).ToList();
        }

        // Orders/Create
        public OrderInpitModel GetOrderInputModel(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException();
            }

            var model = new OrderInpitModel()
            {
                TakeAway = false,
                AddInfo = string.Empty,
                ApplicationUserId = user.Id,
                Addresses = user.Addresses
                    .Select(x => new AddressViewModel()
                    {
                        Id = x.Id,
                        AddInfo = x.AddInfo,
                        Street = x.Street,
                        StreetNumber = x.StreetNumber,
                        DisplayName = x.DisplayName,
                    }).ToList(),
                CartItems = this.cartRepository
                    .All()
                    .Where(x => x.Id == user.ShopingCart.Id)
                    .FirstOrDefault()
                    ?.CartItems
                    .Select(x => new ShopingItemsViewModel()
                    {
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductPrice = x.Size.Price,
                        Quantity = x.Quantity,
                        Description = x.Description,
                        Extras = x.ExtraItems
                                  ?.Select(e => new ExtraCartItemModel()
                                  {
                                      Name = e.Extra.Name,
                                      Price = e.Extra.Price,
                                      Quantity = e.Quantity,
                                  }).ToList(),
                        Size = new ProductSizeViewModel()
                        {
                            SizeName = this.sizeRepository
                                           .All()
                                           .Where(s => s.MenuProductId == x.ProductId)
                                           .Count() > 1 ? x.Size.SizeName : null,
                        },
                    }).ToList(),
                PackegesPrice = this.itemsRepository
                    .All()
                    .Where(x => x.ShopingCartId == user.ShopingCart.Id)
                    .Sum(x => Math.Ceiling((decimal)x.Quantity / x.Size.MaxProductsInPackage) * x.Size.Package.Price),
            };

            model.OrderPrice = model.CartItems.Sum(x => x.ItemPrice) + model.PackegesPrice + model.DeliveryFee;

            return model;
        }

        public bool IsExists(int id) => this.ordersRepository.All().Any(x => x.Id == id);

        // Orders/All
        public ICollection<OrderCollectionViewModel> GetAll()
        {
            return this.ordersRepository.All()
                .Select(x => new OrderCollectionViewModel()
                {
                    Id = x.Id,
                    UserName = x.User.UserName,
                    DateTime = x.CreatedOn.ToLocalTime(),
                }).OrderByDescending(x => x.Id).ToList();
        }

        // Orders/All => OrdersByUser
        public ICollection<OrdersViewModel> GetOrdersByUserName(string userName)
        {
            return this.ordersRepository.All()
                .Where(x => x.User.UserName == userName)
                .Select(x => new OrdersViewModel()
                {
                    Id = x.Id,
                    Status = x.Status.ToString(),
                }).OrderByDescending(x => x.Id).ToList();
        }

        // Orders/UserOrders
        public ICollection<UserOrderViewModel> GetMyOrders(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException("User is null!");
            }

            return this.ordersRepository.All()
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new UserOrderViewModel()
                {
                    OrderId = x.Id,
                    Status = x.Status.ToString(),
                    ArriveTime = x.ProcessingTime.ToLocalTime().AddMinutes((double)x.MinutesForDelivery).ToString("dd/MM/yyyy HH:mm"),
                    CreatedOn = x.CreatedOn.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                    TakeAway = x.TakeAway,
                }).Take(10).ToList();
        }

        // /Orders/UserOrders/UserOrderDetails
        public UserOrderDetailsViewModel GetUserDetailsById(int id)
        {
            var order = this.ordersRepository.All().Where(x => x.Id == id).FirstOrDefault();

            if (order is null)
            {
                throw new ArgumentException("Order not exists.");
            }

            var model = new UserOrderDetailsViewModel()
            {
                CreatedOn = order.CreatedOn.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                OrderId = order.Id,
                TakeAway = order.TakeAway,
                CutleryCount = order.Cutlery,
                CartItems = order.Bag.CartItems
                    .Select(x => new ShopingItemsViewModel()
                    {
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductPrice = x.Size.Price,
                        Quantity = x.Quantity,
                        Description = x.Description,
                        Size = new ProductSizeViewModel()
                        {
                            SizeName = this.sizeRepository
                                           .All()
                                           .Where(s => s.MenuProductId == x.ProductId)
                                           .Count() > 1 ? x.Size.SizeName : null,
                        },
                        Extras = x.ExtraItems
                                  ?.Select(e => new ExtraCartItemModel()
                                  {
                                      Name = e.Extra.Name,
                                      Price = e.Extra.Price,
                                      Quantity = e.Quantity,
                                  }).ToList(),
                    }).ToList(),
                Status = order.Status,
                PackagesPrice = order.Bag.CartItems.Sum(x => (decimal)Math.Ceiling((double)x.Size.MaxProductsInPackage / x.Quantity) * x.Size.Package.Price),
            };

            model.TotalPrice = model.CartItems.Sum(x => x.ItemPrice) + model.PackagesPrice;

            if (order.DeliveryTaxId.HasValue)
            {
                var x = this.deliveryTaxRepository.All().Where(x => x.Id == order.DeliveryTaxId).FirstOrDefault().Price;
                model.TotalPrice += x;
                model.DeliveryFee = x;
            }

            if (model.Status != OrderStatus.Unprocessed)
            {
                model.TimeForDelivery = order.ProcessingTime.ToLocalTime().AddMinutes((double)order.MinutesForDelivery).ToString("dd/MM/yyyy HH:mm");
            }

            return model;
        }

        private async Task SendOrderToMistraalAsync(Order order)
        {
            var orderDto = new OrderMDto()
            {
                LocationId = LocationId,
                StoreId = StoreId,
                Date = DateTime.Now,
                Info = order.AddInfo,
                Sales = new List<OrderItemMDto>(),
            };

            foreach (var item in order.Bag.CartItems)
            {
                orderDto.Sales.Add(new OrderItemMDto()
                {
                    Code = item.Size.MistralCode,
                    Name = item.Size.MistralName,
                    SalesPrice = item.Size.Price,
                    Qtty = item.Quantity,
                });

                foreach (var extra in item.ExtraItems)
                {
                    orderDto.Sales.Add(new OrderItemMDto()
                    {
                        Code = extra.Extra.MistralCode,
                        Name = extra.Extra.MistralName,
                        SalesPrice = extra.Extra.Price,
                        Qtty = extra.Quantity,
                    });
                }

                orderDto.Sales.Add(new OrderItemMDto()
                {
                    Code = item.Size.Package.MistralCode,
                    Name = item.Size.Package.MistralName,
                    SalesPrice = item.Size.Package.Price,
                    Qtty = Math.Ceiling((decimal)item.Quantity / item.Size.MaxProductsInPackage),
                });
            }

            if (!order.TakeAway && order.DeliveryTaxId.HasValue)
            {
                var tax = this.deliveryTaxRepository
                    .All()
                    .Where(x => x.Id == order.DeliveryTaxId)
                    .FirstOrDefault();
                if (tax != null)
                {
                    orderDto.Sales.Add(new OrderItemMDto()
                    {
                        Code = tax.MistralCode,
                        Name = tax.MistralName,
                        SalesPrice = tax.Price,
                        Qtty = Qtty,
                    });
                }
            }

            await this.mistralService.SaveWebOrder(orderDto);
        }
    }
}
