

namespace Brain_Tumor_Classification.Repositories.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _imagePath;

        public MedicalRecordService(ApplicationDbContext context, HttpClient httpClient, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _httpClient = httpClient;
            _webHostEnvironment = webHostEnvironment;
            _imagePath = $"{_webHostEnvironment.WebRootPath}/images";
        }

        public async Task<string> AddMedicalRecordAsync(string id, MedicalRecordDto dto)
        {
            if (dto.MRIImage is null || dto.MRIImage.Length == 0)
                return"No file uploaded!";

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await dto.MRIImage.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            string aiResponse = "";

            //ToDo: Save the response from the AI model to the database
            aiResponse = await SendToAiModelAsync(fileBytes);

            var imageName = await SaveImageAsync(dto.MRIImage);

            var medicalRecord = new MedicalRecord
            {
                MRIImage = imageName,
                CreatedOn = DateTime.UtcNow,
                PatientId = id,
                //ToDo: Set the Tumor property based on the AI model response
            };

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            //ToDo: Return the AI model response
            return aiResponse;
        }

        public async Task<byte[]?> GetMedicalRecordByIdAsync(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);

            if (medicalRecord is null)
                return null;

            var image = await File.ReadAllBytesAsync(Path.Combine(_imagePath, medicalRecord.MRIImage));

            return image;
        }

        public async Task<List<byte[]>?> GetMedicalRecordsByPatientIdAsync(string patientId)
        {
            var medicalRecords = _context.MedicalRecords
                .Where(m => m.PatientId == patientId)
                .Select(m => m.MRIImage)
                .ToList();

            if (medicalRecords.Count == 0)
                return null;

            var images = new List<byte[]>();

            foreach (var record in medicalRecords)
            {
                images.Add(await File.ReadAllBytesAsync(Path.Combine(_imagePath, record)));
            }

            return images;
        }

        private async Task<string> SendToAiModelAsync(byte[] fileBytes)
        {
            // 1. Create a stream content for the binary data
            using var fileContent = new ByteArrayContent(fileBytes);

            // 2. Set the content type (e.g., for JPEG)
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpg");

            // 3. Prepare multipart form data
            using var formData = new MultipartFormDataContent();
            formData.Add(fileContent, "file", "mri_image.jpg");

            // 4. Send with PostAsync (not PostAsJsonAsync!)
            var response = await _httpClient.PostAsync(
                "https://a06a-104-196-146-144.ngrok-free.app/predict",
                formData
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

            var imagePath = Path.Combine(_imagePath, imageName);

            using var stream = File.Create(imagePath);
            await image.CopyToAsync(stream);
            stream.Dispose();

            return imageName;
        }
    }
}
