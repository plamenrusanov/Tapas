namespace Tapas.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Tapas.Web.ViewModels.Administration.Allergens;

    public interface IAllergensService
    {
        bool IsAllergenExist(string allergenName);

        Task AddAsync(string allergenName, string path);

        IEnumerable<string> AllAsString();

        IEnumerable<AllergenViewModel> All();
    }
}
