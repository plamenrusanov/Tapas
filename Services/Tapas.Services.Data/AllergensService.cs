namespace Tapas.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Allergens;

    public class AllergensService : IAllergensService
    {
        private readonly IRepository<Allergen> allergensRepository;

        public AllergensService(IRepository<Allergen> allergensRepository)
        {
            this.allergensRepository = allergensRepository;
        }

        public async Task AddAsync(string allergenName, string path)
        {
            var allergen = new Allergen()
            {
                Name = allergenName,
                ImageUrl = path,
            };

            await this.allergensRepository.AddAsync(allergen);
            await this.allergensRepository.SaveChangesAsync();
        }

        public IEnumerable<AllergenViewModel> All()
        {
            return this.allergensRepository.All().Select(x => new AllergenViewModel()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToList();
        }

        public IEnumerable<string> AllAsString()
        {
            return this.allergensRepository.All().Select(x => x.Name).ToList();
        }

        public bool IsAllergenExist(string allergenName)
        {
            return this.allergensRepository.All().Any(x => x.Name == allergenName);
        }
    }
}
