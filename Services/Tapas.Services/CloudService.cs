namespace Tapas.Services
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using Tapas.Services.Contracts;

    public class CloudService : ICloudService
    {
        private readonly Account account;

        public CloudService(string cloudName, string apiKey, string apiSecret)
        {
            this.account = new Account()
            {
                Cloud = cloudName,
                ApiKey = apiKey,
                ApiSecret = apiSecret,
            };
        }

        public async Task<string> UploadImageFromForm(IFormFile formFile)
        {
            Stream stream = formFile.OpenReadStream();
            var imageName = Guid.NewGuid().ToString();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(imageName, stream),
            };

            var uploadResult = this.Cloudinary().Upload(uploadParams);

            // await this.DownloadWebPImageFromCloudinary(uploadResult.PublicId);
            var url = uploadResult.Uri.AbsolutePath;
            var index = url.LastIndexOf("/");
            url = url.Substring(index + 1, url.Length - (index + 1));
            return await Task.FromResult<string>(url);
        }

        public async Task<string> UploadImageFromResources(string fileDirectory)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileDirectory),
            };

            var uploadResult = this.Cloudinary().Upload(uploadParams);
            var url = uploadResult.Uri.AbsolutePath;
            var index = url.LastIndexOf("/");
            url = url.Substring(index + 1, url.Length - (index + 1));
            return await Task.FromResult<string>(url);
        }

        public async Task DownloadWebPImageFromCloudinary(string publicId)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"https://res.cloudinary.com/duxtyuzpy/image/upload/c_scale,h_140,w_200,f_auto/v1587540778/{publicId}");
                var resultContent = await result.Content.ReadAsStreamAsync();
            }
        }

        private Cloudinary Cloudinary() => new Cloudinary(this.account);
    }
}
