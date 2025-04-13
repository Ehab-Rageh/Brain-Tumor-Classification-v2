namespace Brain_Tumor_Classification.Models;

public class MedicalRecord
{
    public int Id { get; set; }
    public string MRIImage { get; set; } = default!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Tumor? Tumor { get; set; } = default!;
    public string PatientId { get; set; }

    [ForeignKey("PatientId")]
    public ApplicationUser? Patient { get; set; } = default!;
}
