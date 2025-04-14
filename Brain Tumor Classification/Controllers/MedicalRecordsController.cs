using Microsoft.AspNetCore.Authorization;

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

        [HttpPost("upload/{id}")]
        public async Task<IActionResult> AddMedicalRecordAsync(string id, [FromForm] MedicalRecordDto dto)
        {
            var response = await _medicalRecordService.AddMedicalRecordAsync(id, dto);

            if (!string.IsNullOrEmpty(response))
                return BadRequest(response);

            return Ok();
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetMedicalRecordByIdAsync(int id)
        {
            var medicalRecord = await _medicalRecordService.GetMedicalRecordByIdAsync(id);

            if (medicalRecord is null)
                return NotFound("Medical Record not found!");

            return Ok(medicalRecord);
        }

        [HttpGet("getByPatientId/{id}")]
        public async Task<IActionResult> GetMedicalRecordsBypatientIdAsync(string id)
        {
            var medicalRecord = await _medicalRecordService.GetMedicalRecordsByPatientIdAsync(id);

            if (medicalRecord is null)
                return NotFound("There is no Medical Records!");

            return Ok(medicalRecord);
        }
    }
}
