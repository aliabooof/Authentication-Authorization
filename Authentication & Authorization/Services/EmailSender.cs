using Authentication___Authorization.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace Authentication___Authorization.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SendGridClient _sendGridClient;
        private readonly EmailAddress _fromAddress;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            var apiKey = _configuration["SendGrid:ApiKey"];
            var SenderEmail = _configuration["SendGrid:SenderEmail"];
            var SenderName = _configuration["SendGrid:SenderName"] ?? "No-Reply";

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("SendGrid API Key is missing");
            }
            if (string.IsNullOrEmpty(SenderEmail))
            {
                _logger.LogError("SendGrid Sender Email is missing");
            }

            _sendGridClient = new SendGridClient(apiKey);
            _fromAddress = new EmailAddress(SenderEmail, SenderName);

        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if(string.IsNullOrEmpty(email))
            {
                new ArgumentException("Email address is missing", nameof(email));
                
            }
            if (string.IsNullOrEmpty(subject))
            {
                new ArgumentException("Email subject is missing", nameof(subject));
               
            }
            if (string.IsNullOrEmpty(htmlMessage))
            {
                throw new ArgumentException( "Email message is missing", nameof(htmlMessage));
               
            }

            var toAddress = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(
                from: _fromAddress,
                to: toAddress,
                subject: subject,
                plainTextContent: "Please view this email in a client that supports HTML emails.",
                htmlContent:htmlMessage);

            try
            {
                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to send email to {email} with status code {response.StatusCode}");
                }
                else
                {
                    var errorMessage = await response.Body.ReadAsStringAsync().ConfigureAwait(false);

                    _logger.LogError(
                        "Failed to send email to {Email}. Status Code: {StatusCode}, Reason: {ErrorMessage}",
                        email, response.StatusCode, errorMessage);

                    

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {email} with error: {ex.Message}");
                return;
            }
        }
    }
}
