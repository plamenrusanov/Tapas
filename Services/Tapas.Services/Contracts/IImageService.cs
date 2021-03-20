namespace Tapas.Services.Contracts
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IImageService
    {
        Task SaveImageOnFileSystem(IFormFile formFile, string name, string directory);

        Task<string> AddImage(IFormFile image, string directory);
    }
}
