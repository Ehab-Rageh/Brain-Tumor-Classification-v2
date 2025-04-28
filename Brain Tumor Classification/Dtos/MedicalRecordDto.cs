namespace Brain_Tumor_Classification.Dtos;

public class MedicalRecordDto
{
    [MaxSize(FileSettings.MaxFileSizeInBytes)]
    public IFormFile MRIImage { get; set; } = default!;
}
