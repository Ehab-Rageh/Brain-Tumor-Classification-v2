namespace Brain_Tumor_Classification.Repositories.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string receptor, string subject, string body);
    }
}