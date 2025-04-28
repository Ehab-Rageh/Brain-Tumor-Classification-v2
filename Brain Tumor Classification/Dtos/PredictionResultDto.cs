using System.Text.Json.Serialization;

namespace Brain_Tumor_Classification.Dtos
{
    public class PredictionResultDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("predicted_class")]
        public string PredictedClass { get; set; }

        [JsonPropertyName("confidence")]
        public float Confidence { get; set; }
    }
}
