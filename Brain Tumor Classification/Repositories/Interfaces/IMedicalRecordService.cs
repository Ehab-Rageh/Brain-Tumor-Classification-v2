namespace Brain_Tumor_Classification.Repositories.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordResponeDto?> AddMedicalRecordAsync(string id, MedicalRecordDto dto);
        Task<MedicalRecord> GetMedicalRecordByIdAsync(int id);
        Task<List<MedicalRecord>?> GetMedicalRecordsByPatientIdAsync(string patientId);
    }
}
