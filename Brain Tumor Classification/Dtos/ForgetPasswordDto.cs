namespace Brain_Tumor_Classification.Dtos
{
    public class ForgetPasswordDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
