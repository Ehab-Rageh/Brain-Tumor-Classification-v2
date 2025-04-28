namespace Brain_Tumor_Classification.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
