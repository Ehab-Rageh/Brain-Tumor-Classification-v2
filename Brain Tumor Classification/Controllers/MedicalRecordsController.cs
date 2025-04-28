namespace Brain_Tumor_Classification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordsController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> AddMedicalRecordAsync([FromForm] MedicalRecordDto dto)
        {
            var userId = User.FindFirst("uid")?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await _medicalRecordService.AddMedicalRecordAsync(userId, dto);

            if (result is null)
                return BadRequest("Something wrong happend. try again later.");

            return Ok(result);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetMedicalRecordByIdAsync(int id)
        {
            var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(id);

            if (medicalRecord is null)
                return NotFound("Medical Record not found!");

            var response = new MedicalRecordResponeDto
            {
                MedicalRecordId = medicalRecord.Id,
                ImageURL = medicalRecord.ImageURL,
                PatientId = medicalRecord.PatientId,
                HasTumor = medicalRecord.Tumor.HasTumor,
                TumorType = medicalRecord.Tumor.TumorType
            };

            return Ok(response);
        }

        [HttpGet("GetCurrentUserMedicalRecords")]
        public async Task<IActionResult> GetMedicalRecordsBypatientIdAsync()
        {
            var userId = User.FindFirst("uid")?.Value;

            if (userId == null)
                return Unauthorized();

            var medicalRecord = await _medicalRecordService.GetMedicalRecordsByPatientIdAsync(userId);

            if (medicalRecord is null)
                return NotFound("There is no Medical Records!");

            var response = new List<MedicalRecordResponeDto>();
            foreach (var record in medicalRecord)
            {
                response.Add(new MedicalRecordResponeDto
                {
                    MedicalRecordId = record.Id,
                    ImageURL = record.ImageURL,
                    PatientId = record.PatientId,
                    HasTumor = record.Tumor.HasTumor,
                    TumorType = record.Tumor.TumorType
                });
            }

            return Ok(response);
        }
    }
}
