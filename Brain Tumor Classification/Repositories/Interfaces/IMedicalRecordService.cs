namespace Brain_Tumor_Classification.Repositories.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<string> AddMedicalRecordAsync(string id, MedicalRecordDto dto);
        Task<byte[]> GetMedicalRecordByIdAsync(int id);
        Task<List<byte[]>> GetMedicalRecordsByPatientIdAsync(string patientId);
    }
}
