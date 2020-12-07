namespace Tapas.Web.MappingConfig
{
    using AutoMapper;
    using Tapas.Data.Models;
    using Tapas.Web.ViewModels.Administration.Categories;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Category, CategoryViewModel>().ReverseMap();
        }
    }
}
