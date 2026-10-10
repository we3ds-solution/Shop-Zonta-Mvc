namespace market_mvc.Features.Auth.Email
{
    /// <summary>
    /// Email delivery abstraction (issue #4). No credentials in code:
    /// SMTP comes from configuration/secret manager; without it the dev
    /// sender writes .eml files to App_Data/mail (never used in production).
    /// </summary>
    public interface IEmailSender
    {
        bool IsConfigured { get; }
        Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
    }

    public class FileEmailSender : IEmailSender
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileEmailSender> _logger;

        public bool IsConfigured => false;

        public FileEmailSender(IWebHostEnvironment env, ILogger<FileEmailSender> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var dir = Path.Combine(_env.ContentRootPath, "App_Data", "mail");
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}.eml");
            await File.WriteAllTextAsync(file,
                $"To: {to}\nSubject: {subject}\nDate: {DateTime.UtcNow:o}\n\n{htmlBody}", ct);
            _logger.LogWarning("Email to {To} written to {File} (dev sender; configure Smtp:* for production).", to, file);
        }
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;

        public bool IsConfigured => !string.IsNullOrWhiteSpace(_config["Smtp:Host"]);

        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var host = _config["Smtp:Host"];
            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("SMTP is not configured (Smtp:Host).");

            var port = _config.GetValue("Smtp:Port", 587);
            var user = _config["Smtp:User"];
            var pass = _config["Smtp:Pass"];
            var from = _config["Smtp:From"] ?? user;
            var ssl = _config.GetValue("Smtp:Ssl", true);

            using var client = new System.Net.Mail.SmtpClient(host, port)
            {
                EnableSsl = ssl,
                Credentials = string.IsNullOrWhiteSpace(user)
                    ? System.Net.CredentialCache.DefaultNetworkCredentials
                    : new System.Net.NetworkCredential(user, pass)
            };
            using var message = new System.Net.Mail.MailMessage(from!, to, subject, htmlBody)
            {
                IsBodyHtml = true
            };
            await client.SendMailAsync(message, ct);
            _logger.LogInformation("Email sent to {To} with subject {Subject}.", to, subject);
        }
    }
}
