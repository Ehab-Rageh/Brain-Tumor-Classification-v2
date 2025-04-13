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

            var imageName = await SaveImageAsync(dto.MRIImage);

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await dto.MRIImage.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            //ToDo: Save the response from the AI model to the database
            //var aiResponse = await SendToAiModelAsync(fileBytes);

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
            return string.Empty;
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
            using var content = new ByteArrayContent(fileBytes);

            var formData = new MultipartFormDataContent
            {
                { content, "file", "mri_image.jpg" }
            };

            //ToDo: Change the URL to the AI model endpoint
            var response = await _httpClient.PostAsJsonAsync("https://localhost:5001/api/ai-model/predict", formData);

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
