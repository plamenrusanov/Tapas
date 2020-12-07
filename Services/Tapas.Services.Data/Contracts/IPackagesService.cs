namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Web.ViewModels.Administration.Packages;

    public interface IPackagesService
    {
        Task AddAsync(PackageInputModel model);

        IEnumerable<PackageViewModel> All();

        PackageViewModel GetPackageViewModelById(int packageId);

        Task EditAsync(PackageViewModel packageViewModel);

        Task RemoveAsync(int packageId);
    }
}
