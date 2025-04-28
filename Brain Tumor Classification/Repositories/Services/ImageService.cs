
using Microsoft.AspNetCore.Hosting;

namespace Brain_Tumor_Classification.Repositories.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ImageService
        (
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor
        )
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<KeyValuePair<string, string>> SaveImageAsync(IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName);
            var imageName = $"{Guid.NewGuid()}{extension}";

            var localPass = Path.Combine($"{webHostEnvironment.WebRootPath}/images",
                $"{imageName}");

            using var stream = File.Create(localPass);
            await image.CopyToAsync(stream);
            stream.Dispose();

            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}" +
                  $"{httpContextAccessor.HttpContext.Request.PathBase}/images/{imageName}";

            return new KeyValuePair<string, string>(imageName, urlFilePath);
        }

        public bool ValidateFileUpload(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(image.FileName);

            if (!allowedExtensions.Contains(extension))
            {
                return false;
            }

            if (image.Length > 10485760)
            {
                return false;
            }

            return true;
        }
    }
    
}
