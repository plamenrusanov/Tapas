namespace Tapas.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Tapas.Common;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Data.Models.Enums;
    using Tapas.Services.Contracts;
    using Tapas.Services.Data.Contracts;
    using Tapas.Services.Data.Dto;
    using Tapas.Services.Dto.Mistral;
    using Tapas.Web.ViewModels.Administration.Sizes;
    using Tapas.Web.ViewModels.Extras;
    using Tapas.Web.ViewModels.Orders;
    using Tapas.Web.ViewModels.Rating;
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

                                // await this.SendOrderToMistralAsync(order);
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
            var address = JsonConvert.DeserializeObject<DeliveryAddress>(model.Address);

            var cart = JsonConvert.DeserializeObject<List<ShoppingCartItemDto>>(model.Cart);

            var order = new Order()
            {
                Name = model.Username,
                Phone = model.Phone,
                AddInfo = model.AddInfoOrder,
                AddressId = address.Id,
                Address = address,
                User = user,
                UserId = user.Id,
                Status = OrderStatus.Unprocessed,
                CreatedOn = DateTime.Now,
                TakeAway = model.TakeAway,
                Cutlery = model.CutleryCount,
                Bag = new ShopingCart()
                {
                    CartItems = cart.Select(x => new ShopingCartItem()
                    {
                        SizeId = x.SizeId,
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Description = x.Description,
                        ExtraItems = x.Extras.Select(e => new ExtraItem()
                        {
                            ExtraId = e.Id,
                            Quantity = e.Quantity,
                        }).ToList(),
                    }).ToList(),
                },
            };

            if (model.TakeAway)
            {
                order.Address = this.addressRepository.All().Where(a => a.DisplayName == GlobalConstants.TakeAway).FirstOrDefault();
                order.AddressId = order.Address.Id;
            }

            await this.ordersRepository.AddAsync(order);
            await this.ordersRepository.SaveChangesAsync();

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
                Latitude = order.Address.Latitude,
                Longitude = order.Address.Longitude,
                CreatedOn = order.CreatedOn.ToString("dd/MM/yy HH:mm"),
                OrderId = order.Id,
                DisplayAddress = order.Address.ToString(),
                AddressInfo = order.Address.AddInfo,
                UserUserName = order.Name,
                UserPhone = order.Phone,
                AddInfo = order.AddInfo,
                TakeAway = order.TakeAway,
                CutleryCount = order.Cutlery,
                CustomerComment = order.CustomerComment,
                CartItems = order.Bag.CartItems
                    .Select(x => new ShopingItemsViewModel()
                    {
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductPrice = x.Size.Price,
                        Quantity = x.Quantity,
                        Description = x.Description,
                        Rating = x.Rating,
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
                PackagesPrice = order.Bag.CartItems.Sum(x => (decimal)Math.Ceiling(x.Quantity / (double)x.Size.MaxProductsInPackage) * x.Size.Package.Price),
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
                var deliveryPrice = this.deliveryTaxRepository.All().Where(x => x.Id == order.DeliveryTaxId).FirstOrDefault().Price;
                model.TotalPrice += deliveryPrice;
                model.DeliveryFee = deliveryPrice;
            }

            if (model.Status != OrderStatus.Unprocessed)
            {
                model.TimeForDelivery = order.ProcessingTime.ToLocalTime().AddMinutes((double)order.MinutesForDelivery).ToString("dd/MM/yy HH:mm");
            }

            return model;
        }

        // Orders/Index
        public IEnumerable<OrdersViewModel> GetDailyOrders()
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

        public bool IsExists(int id) => this.ordersRepository.All().Any(x => x.Id == id);

        // Orders/All
        public async Task<IEnumerable<OrderCollectionViewModel>> GetAllAsync()
        {
            return await this.ordersRepository.All()
                .Select(x => new OrderCollectionViewModel()
                {
                    Id = x.Id,
                    UserName = x.User.UserName,
                    DateTime = x.CreatedOn.ToLocalTime(),
                }).OrderByDescending(x => x.Id).ToListAsync();
        }

        // Orders/All => OrdersByUser
        public async Task<IEnumerable<OrdersViewModel>> GetOrdersByUserNameAsync(string userName)
        {
            return await this.ordersRepository.All()
                .Where(x => x.User.UserName == userName)
                .Select(x => new OrdersViewModel()
                {
                    Id = x.Id,
                    Status = x.Status.ToString(),
                }).OrderByDescending(x => x.Id).ToListAsync();
        }

        // Orders/UserOrders
        public async Task<IEnumerable<UserOrderViewModel>> GetMyOrdersAsync(ApplicationUser user)
        {
            if (user is null)
            {
                return new List<UserOrderViewModel>();
            }

            return await this.ordersRepository.All()
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new UserOrderViewModel()
                {
                    OrderId = x.Id,
                    Status = x.Status.ToString(),
                    ArriveTime = x.ProcessingTime.ToLocalTime().AddMinutes((double)x.MinutesForDelivery).ToString("dd/MM/yyyy HH:mm"),
                    CreatedOn = x.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                    TakeAway = x.TakeAway,
                }).Take(10).ToListAsync();
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
                CreatedOn = order.CreatedOn.ToString("dd/MM/yyyy HH:mm"),
                OrderId = order.Id,
                TakeAway = order.TakeAway,
                CutleryCount = order.Cutlery,
                CustomerComment = order.CustomerComment,
                CartItems = order.Bag.CartItems
                    .Select(x => new ShopingItemsViewModel()
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ProductName = x.Product.Name,
                        ProductPrice = x.Size.Price,
                        Quantity = x.Quantity,
                        Description = x.Description,
                        Rating = x.Rating,
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
                PackagesPrice = order.Bag.CartItems.Sum(x => (decimal)Math.Ceiling(x.Quantity / (double)x.Size.MaxProductsInPackage) * x.Size.Package.Price),
            };

            model.TotalPrice = model.CartItems.Sum(x => x.ItemPrice) + model.PackagesPrice;

            if (order.DeliveryTaxId.HasValue)
            {
                var x = this.deliveryTaxRepository.All().Where(x => x.Id == order.DeliveryTaxId).FirstOrDefault()?.Price;
                if (x.HasValue)
                {
                    model.TotalPrice += x.Value;
                    model.DeliveryFee = x.Value;
                }
            }

            if (model.Status != OrderStatus.Unprocessed)
            {
                model.TimeForDelivery = order.ProcessingTime.ToLocalTime().AddMinutes((double)order.MinutesForDelivery).ToString("dd/MM/yyyy HH:mm");
            }

            return model;
        }

        public async Task SetRatingAsync(IEnumerable<RatingItemDto> rating, string message)
        {
            if (rating.ToList().Count > 0)
            {
                var order = this.ordersRepository
                    .All()
                    .FirstOrDefault(x => x.Bag.CartItems.Any(i => i.Id.ToString() == rating.First().ItemId));
                if (order is null)
                {
                    throw new ArgumentException("Order not exist!");
                }

                order.CustomerComment = message;
                this.ordersRepository.Update(order);
                await this.ordersRepository.SaveChangesAsync();

                Stopwatch s = new Stopwatch();
                s.Start();
                foreach (var item in rating)
                {
                    if (byte.TryParse(item.Rating, out byte result) && int.TryParse(item.ItemId, out int itemId))
                    {
                        var shopItem = this.itemsRepository
                            .All()
                            .Where(x => x.Id == itemId)
                            .FirstOrDefault();

                        if (shopItem is null || shopItem.Rating != default)
                        {
                            throw new Exception("Rating is already set!");
                        }

                        shopItem.Rating = result;
                        this.itemsRepository.Update(shopItem);
                        await this.itemsRepository.SaveChangesAsync();
                    }
                }

                s.Stop();
                Console.WriteLine(s.Elapsed);
            }
        }

        private async Task SendOrderToMistralAsync(Order order)
        {
            var orderDto = new OrderMDto()
            {
                LocationId = LocationId,
                StoreId = StoreId,
                Date = DateTime.Now,
                Info = order.AddInfo,
                IsOrder = true,
                TestMode = true,
                Sales = new List<OrderItemMDto>(),
            };

            foreach (var item in order.Bag.CartItems)
            {
                orderDto.Sales.Add(new OrderItemMDto()
                {
                    Code = item.Size.MistralCode.ToString(),
                    Name = item.Size.MistralName,
                    SalesPrice = item.Size.Price,
                    Qtty = item.Quantity,
                });

                foreach (var extra in item.ExtraItems)
                {
                    orderDto.Sales.Add(new OrderItemMDto()
                    {
                        Code = extra.Extra.MistralCode.ToString(),
                        Name = extra.Extra.MistralName,
                        SalesPrice = extra.Extra.Price,
                        Qtty = extra.Quantity,
                    });
                }

                orderDto.Sales.Add(new OrderItemMDto()
                {
                    Code = item.Size.Package.MistralCode.ToString(),
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
                        Code = tax.MistralCode.ToString(),
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
