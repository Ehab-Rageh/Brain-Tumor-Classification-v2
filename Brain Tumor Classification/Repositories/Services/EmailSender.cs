using System.Net;
using System.Net.Mail;

namespace Brain_Tumor_Classification.Repositories.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string receptor, string subject, string body)
        {
            var email = _configuration.GetValue<string>("EMAIL_CONFIGRATION:EMAIL");
            var password = _configuration.GetValue<string>("EMAIL_CONFIGRATION:PASSWORD");
            var host = _configuration.GetValue<string>("EMAIL_CONFIGRATION:HOST");
            var port = _configuration.GetValue<int>("EMAIL_CONFIGRATION:PORT");

            var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(email!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };

            mailMessage.To.Add(receptor);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
        }
    }
}
