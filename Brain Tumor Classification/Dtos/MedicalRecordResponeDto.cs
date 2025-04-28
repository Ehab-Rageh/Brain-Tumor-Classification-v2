namespace Brain_Tumor_Classification.Dtos
{
    public class MedicalRecordResponeDto
    {
        public int MedicalRecordId { get; set; }
        public string ImageURL { get; set; }
        public string PatientId { get; set; }
        public bool HasTumor { get; set; }
        public string? TumorType { get; set; } = string.Empty;
    }
}
