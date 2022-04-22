using System.Net;
using System.Net.Mail;
using System.Text;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void SendEmail(string email, string subject, string message)
    {
        string to = email;
        string from = _config["EmailSetting:EmailFrom"];

        var mailMessage = new MailMessage(from, to)
        {
            Subject = subject,
            BodyEncoding = Encoding.UTF8,
            Body = message,
            IsBodyHtml = true
        };

        var smtpClient = new SmtpClient(_config["EmailSetting:EmailServer"], int.Parse(_config["EmailSetting:EmailPort"]))
        {
            Port = Convert.ToInt32(_config["EmailSetting:EmailPort"]),
            Credentials = new NetworkCredential(from, _config["EmailSetting:EmailPassword"]),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        try
        {
            _logger.LogInformation("Sending email");

            smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
