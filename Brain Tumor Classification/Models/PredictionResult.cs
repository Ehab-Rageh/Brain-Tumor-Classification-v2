using System.Text.Json.Serialization;

namespace Brain_Tumor_Classification.Models
{
    public class PredictionResult
    {
        [Key]
        public int Id { get; set; }

        public string Status { get; set; }

        public string PredictedClass { get; set; }

        public float Confidence { get; set; }

        [ForeignKey("MedicalRecord")]
        public int MedicalRecordId { get; set; }

        public MedicalRecord MedicalRecord { get; set; }
    }
}
