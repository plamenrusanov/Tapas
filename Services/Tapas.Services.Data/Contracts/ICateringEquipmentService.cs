namespace Tapas.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Tapas.Web.ViewModels.Administration.CateringEquipment;

    public interface ICateringEquipmentService
    {
        IEnumerable<IndexEquipmentModel> GetAll();

        Task ActivateAsync(string productId);

        IEnumerable<Deleted> GetDeletedProducts();

        Task Delete(string id);

        Task SetEditModel(Edit model);

        Edit GetEditModel(string id);

        Details GetDetailsById(string id);

        Task AddEquipmentAsync(Create model);

        Create CreateInputModel();
    }
}
