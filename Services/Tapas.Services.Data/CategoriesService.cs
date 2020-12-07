namespace Tapas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;
    using Tapas.Data.Common.Repositories;
    using Tapas.Data.Models;
    using Tapas.Services.Data.Contracts;
    using Tapas.Web.ViewModels.Administration.Categories;

    public class CategoriesService : ICategoriesService
    {
        private readonly IRepository<Category> categoriesRepository;
        private readonly IMapper mapper;

        public CategoriesService(IRepository<Category> categoriesRepository, IMapper mapper)
        {
            this.categoriesRepository = categoriesRepository;
            this.mapper = mapper;
        }

        public async Task AddAsync(string name)
        {
            var catg = new Category()
            {
                Name = name,
                Position = this.categoriesRepository.All().ToList().Count + 1,
            };
            await this.categoriesRepository.AddAsync(catg);
            await this.categoriesRepository.SaveChangesAsync();
        }

        public IEnumerable<CategoryViewModel> All()
        {
            return this.categoriesRepository.All()
                .OrderBy(x => x.Position)
                .Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Position = x.Position,
                }).ToList();
        }

        public void Edit(CategoryViewModel categoryViewModel)
        {
            if (string.IsNullOrEmpty(categoryViewModel.Id))
            {
                throw new ArgumentNullException();
            }

            var category = this.categoriesRepository.All()
                .Where(x => x.Id == categoryViewModel.Id)
                .FirstOrDefault();

            if (category is null)
            {
                throw new Exception();
            }

            category.Name = categoryViewModel.Name;
            this.categoriesRepository.SaveChanges();
        }

        public CategoryViewModel GetCategoryViewModelById(string categoryId)
        {
            if (!this.ExistCategoryById(categoryId))
            {
                throw new ArgumentException();
            }

            return this.categoriesRepository.All()
                .Where(x => x.Id == categoryId)
                .Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .FirstOrDefault();
        }

        public bool ExistCategoryByName(string categoryName)
        {
            return this.categoriesRepository.All().Any(x => x.Name == categoryName);
        }

        public bool ExistCategoryById(string categoryId)
        {
            return this.categoriesRepository.All().Any(x => x.Id == categoryId);
        }

        public void Remove(string categoryId)
        {
            if (!this.ExistCategoryById(categoryId))
            {
                throw new ArgumentException();
            }

            var category = this.GetCategoryById(categoryId);

            this.categoriesRepository.Delete(category);
            this.categoriesRepository
                .All()
                .Where(x => x.Position > category.Position)
                .AsEnumerable()
                .Select(x => x.Position--)
                .ToList();

            this.categoriesRepository.SaveChanges();
        }

        public string GetCategoryNameById(string categoryId)
        {
            if (!this.ExistCategoryById(categoryId))
            {
                throw new ArgumentException();
            }

            return this.categoriesRepository.All()
                .Where(x => x.Id == categoryId)
                .Select(x => x.Name)
                .FirstOrDefault();
        }

        public async Task SavePositions(IEnumerable<CategoryViewModel> models)
        {
            foreach (var model in models)
            {
                var category = this.mapper.Map<Category>(model);
                this.categoriesRepository
                    .Update(category);
            }

            await this.categoriesRepository.SaveChangesAsync();
        }

        private Category GetCategoryById(string categoryId)
            => this.categoriesRepository.All().FirstOrDefault(x => x.Id == categoryId);
    }
}
