namespace Brain_Tumor_Classification.Dtos;

public class MedicalRecordDto
{
    [AllowedExtentions(FileSettings.AllowedExtentions),
        MaxSize(FileSettings.MaxFileSizeInBytes)]
    public IFormFile MRIImage { get; set; } = default!;
}
