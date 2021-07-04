namespace Tapas.Services.Contracts
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IImageService
    {
        Task<string> AddImage(IFormFile image, string directory);
    }
}
