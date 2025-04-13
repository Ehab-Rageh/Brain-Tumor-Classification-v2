namespace Brain_Tumor_Classification.Attributes;

public class MaxSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;

    public MaxSizeAttribute(int maxFileSize)
    {
        this._maxFileSize = maxFileSize;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;

        if (file is not null && file.Length > _maxFileSize)
        {
            return new ValidationResult($"Maximum Size Allowed {FileSettings.MaxFileSizeInMB} MB.");
        }

        return ValidationResult.Success;
    }
}
