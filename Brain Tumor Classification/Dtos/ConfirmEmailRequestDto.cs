namespace Brain_Tumor_Classification.Dtos
{
    public class ConfirmEmailRequestDto
    {
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
