namespace Brain_Tumor_Classification.Dtos
{
    public class GetCurrentUserResponseDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }  
    }
}
