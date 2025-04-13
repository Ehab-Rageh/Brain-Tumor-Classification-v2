namespace Brain_Tumor_Classification.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]  
    public string Name { get; set; }

    [MaxLength(10)]  
    public string Gender { get; set; }

    [DataType(DataType.Date)]  
    public DateTime BirthDate { get; set; }

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

