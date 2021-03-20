namespace Tapas.Services
{
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
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

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                return filePath;
            }

            return null;
        }

        public async Task SaveImageOnFileSystem(IFormFile image, string name, string directory)
        {
            /* Check if valid image type (can be extended with more rigorous checks)
            if (image == null)
            {
                throw new Exception();
            }

            if (image.Length < 1)
            {
                throw new Exception();
            }

            string[] allowedImageTypes = new string[] { "image/jpeg", "image/png" };
            if (!allowedImageTypes.Contains(image.ContentType.ToLower()))
            {
                throw new Exception();
            }

            // Prepare paths for saving images
            string imagesPath = Path.Combine(this.hostingEnvironment.WebRootPath, "images");
            string webPFileName = Path.GetFileNameWithoutExtension(image.FileName) + ".webp";
            string normalImagePath = Path.Combine(imagesPath, image.FileName);
            string webPImagePath = Path.Combine(imagesPath, webPFileName);

            // Save the image in its original format for fallback
            using (FileStream normalFileStream = new FileStream(normalImagePath, FileMode.Create))
            {
                image.CopyTo(normalFileStream);
            }

            // Then save in WebP format
            using (FileStream webPFileStream = new FileStream(webPImagePath, FileMode.Create))
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    imageFactory.Load(image.OpenReadStream())
                                .Format(new WebPFormat())
                                .Quality(50)
                                .Save(webPFileStream);
                }

                Bitmap bitmap = new Bitmap(webPFileStream);
                using (var saveImageStream = File.Open(webPImagePath, FileMode.Create))
                {
                    var encoder = new SimpleEncoder();
                    encoder.Encode(bitmap, saveImageStream, 20);
                }
            }
            */
        }
    }
}
