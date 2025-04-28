namespace Brain_Tumor_Classification.Dtos
{
    public class ResendConfirmationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
