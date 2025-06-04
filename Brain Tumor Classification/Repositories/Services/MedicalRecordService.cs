using System.Text.Json;

namespace Brain_Tumor_Classification.Repositories.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageService imageService;
        private readonly string _imagePath;

        public MedicalRecordService
        (
            ApplicationDbContext context,
            HttpClient httpClient,
            IWebHostEnvironment webHostEnvironment,
            IImageService imageService
        )
        {
            _context = context;
            _httpClient = httpClient;
            _webHostEnvironment = webHostEnvironment;
            this.imageService = imageService;
            _imagePath = $"{_webHostEnvironment.WebRootPath}/images";
        }

        public async Task<MedicalRecordResponeDto?> AddMedicalRecordAsync(string id, MedicalRecordDto dto)
        {
            if (dto.MRIImage is null || dto.MRIImage.Length == 0 || ! imageService.ValidateFileUpload(dto.MRIImage))
                return null;

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await dto.MRIImage.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Send to AI model and get prediction
            var aiResponse = await SendToAiModelAsync(fileBytes, dto.MRIImage.FileName);
            if (aiResponse is null) return null;

            var imageInfo = await imageService.SaveImageAsync(dto.MRIImage);

            // 1. Create and save the MedicalRecord first
            var medicalRecord = new MedicalRecord
            {                
                MRIImage = imageInfo.Key,
                ImageURL = imageInfo.Value,
                CreatedOn = DateTime.UtcNow,
                PatientId = id,
            };

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            // 2. Now create PredictionResult and link it to the saved MedicalRecord
            var predictionResult = new PredictionResult
            {
                Status = aiResponse.Status,
                PredictedClass = aiResponse.PredictedClass,
                Confidence = aiResponse.Confidence,
                MedicalRecordId = medicalRecord.Id 
            };

            _context.PredictionResults.Add(predictionResult);

            // 3. Now create Tumor and link it to the saved MedicalRecord
            var tumor = new Tumor
            {
                HasTumor = aiResponse.PredictedClass == "no_tumor" ? false : true,
                TumorType = aiResponse.PredictedClass,
                MedicalRecordId = medicalRecord.Id
            };

            _context.Tumors.Add(tumor);
            await _context.SaveChangesAsync();

            var response = new MedicalRecordResponeDto
            {
                MedicalRecordId = medicalRecord.Id,
                ImageURL = medicalRecord.ImageURL,
                PatientId = medicalRecord.PatientId,
                HasTumor = tumor.HasTumor,
                TumorType = tumor.TumorType,
            };

            return response;
        }

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(int id)
        {
            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Tumor)
                .FirstOrDefaultAsync(m => m.Id == id);

            return medicalRecord;
        }

        public async Task<List<MedicalRecord>?> GetMedicalRecordsByPatientIdAsync(string patientId)
        {
            var medicalRecords = _context.MedicalRecords
                .Include(m => m.Tumor)
                .Where(m => m.PatientId == patientId)
                .ToList();

            return medicalRecords;
        }

        private async Task<PredictionResultDto?> SendToAiModelAsync(byte[] fileBytes, string fileName)
        {
            using var fileContent = new ByteArrayContent(fileBytes);

            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => throw new NotSupportedException($"Unsupported file extension: {extension}")
            };

            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

            using var formData = new MultipartFormDataContent();
            formData.Add(fileContent, "file", fileName);

            using var response = await _httpClient.PostAsync(
                "https://51fc-35-202-197-253.ngrok-free.app/predict",
                formData
            );

            if (!response.IsSuccessStatusCode) return null;
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PredictionResultDto>(responseContent);

            return result;
        }
    }
}
