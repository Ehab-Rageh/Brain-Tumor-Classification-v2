using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brain_Tumor_Classification.Models;

public class Tumor
{
    [Key]  
    public int Id { get; set; }

    [Required]  
    public bool HasTumor { get; set; } = false;

    [Required]  
    [MaxLength(100)]  
    public string TumorType { get; set; } = string.Empty;

    [Required]
    public int MedicalRecordId { get; set; }

    [ForeignKey("MedicalRecordId")]
    public MedicalRecord MedicalRecord { get; set; } = default!;
}
