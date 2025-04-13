namespace Brain_Tumor_Classification.Settings;

public class FileSettings
{
    public const string ImagePath = "/assets/images";
    public const string AllowedExtentions = ".jpg";
    public const int MaxFileSizeInMB = 1;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
}
