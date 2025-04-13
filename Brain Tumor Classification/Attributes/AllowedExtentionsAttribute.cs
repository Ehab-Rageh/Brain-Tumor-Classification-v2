namespace Brain_Tumor_Classification.Attributes;

public class AllowedExtentionsAttribute : ValidationAttribute
{
    private readonly string _allowedExtentios;

    public AllowedExtentionsAttribute(string allowedExtentions)
    {
        _allowedExtentios = allowedExtentions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;

        if (file is not null)
        {
            var extention = Path.GetExtension(file.FileName);

            bool isAllowed = _allowedExtentios.Split(',').Contains(extention, StringComparer.OrdinalIgnoreCase);

            if (!isAllowed)
            {
                return new ValidationResult($"Only {_allowedExtentios} Extentions is allowed.");
            }
        }
        return ValidationResult.Success;
    }
}
