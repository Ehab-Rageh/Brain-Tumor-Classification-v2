namespace Brain_Tumor_Classification.Dtos;

public class UpdateUserDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Gender { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }

}
