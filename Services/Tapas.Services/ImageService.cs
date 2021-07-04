namespace Tapas.Services
{
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Tapas.Services.Contracts;

    public class ImageService : IImageService
    {
        public async Task<string> AddImage(IFormFile image, string directory)
        {
            if (image.Length > 0)
            {
                var dir = Path.Combine("/", "Images", directory);
                var filePath = Path.Combine(dir, image.FileName);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await image.CopyToAsync(fileStream);
                }

                return filePath;
            }

            return null;
        }
    }
}
