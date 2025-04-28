using static System.Net.Mime.MediaTypeNames;

namespace Brain_Tumor_Classification.Repositories.Interfaces
{
    public interface IImageService
    {
        bool ValidateFileUpload(IFormFile image);
        Task<KeyValuePair<string, string>> SaveImageAsync(IFormFile image);
    }
}
