namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Data.Models;
    using Tapas.Web.ViewModels.Addresses;

    public interface IAddressesService
    {
        ICollection<AddressViewModel> GetMyAddreses(ApplicationUser user);

        Task<AddressInputModel> GetAddressAsync(string latitude, string longitude);

        Task<string> CreateAddressAsync(ApplicationUser user, AddressInputModel model);
    }
}
