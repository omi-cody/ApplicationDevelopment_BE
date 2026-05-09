using Bike360.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Bike360.Infrastructure.Services
{
    
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtp = _config.GetSection("Smtp");

                using var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"] ?? "587"))
                {
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = true
                };

                var mail = new MailMessage
                {
                    From       = new MailAddress(smtp["From"]!, smtp["DisplayName"] ?? "Vehicle Parts"),
                    Subject    = subject,
                    Body       = body,
                    IsBodyHtml = false
                };

                mail.To.Add(toEmail);
                await client.SendMailAsync(mail);

                _logger.LogInformation("Email sent to {Email} — Subject: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                // Log but do NOT crash the caller — email is non-critical
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }
    }
}