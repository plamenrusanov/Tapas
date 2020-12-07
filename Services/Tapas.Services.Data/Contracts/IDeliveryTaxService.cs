namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Web.ViewModels.Administration.DeliveryTax;

    public interface IDeliveryTaxService
    {
        Task<IEnumerable<DeliveryTaxViewModel>> AllAsync(bool isDeleted);

        Task AddAsync(DeliveryTaxInpitModel inputModel);

        DeliveryTaxViewModel GetDeliveryTaxViewModelById(int deliveryTaxId);

        Task EditAsync(DeliveryTaxViewModel viewModel);

        Task RemoveAsync(int deliveryTaxId);

        Task ActivateAsync(int deliveryTaxId);
    }
}
