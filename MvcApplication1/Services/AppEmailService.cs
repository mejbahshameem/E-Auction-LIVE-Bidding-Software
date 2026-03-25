using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace MvcApplication1.Services
{
    public class AppEmailService
    {
        private readonly String _smtpHost;
        private readonly Int32 _smtpPort;
        private readonly String _smtpUser;
        private readonly String _smtpPass;
        private readonly String _smtpFrom;

        public AppEmailService()
        {
            _smtpHost = (ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com").Trim();

            Int32 parsedPort;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out parsedPort))
            {
                parsedPort = 587;
            }

            _smtpPort = parsedPort;
            _smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
            _smtpPass = ConfigurationManager.AppSettings["SmtpPass"];
            _smtpFrom = ConfigurationManager.AppSettings["SmtpFrom"];
        }

        public Boolean TrySend(String toEmail, String subject, String body)
        {
            if (String.IsNullOrWhiteSpace(toEmail))
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(_smtpUser) || String.IsNullOrWhiteSpace(_smtpPass))
            {
                Debug.WriteLine("SMTP skipped: credentials are not configured.");
                return false;
            }

            var fromAddress = !String.IsNullOrWhiteSpace(_smtpFrom) ? _smtpFrom : _smtpUser;

            try
            {
                using (var smtp = new SmtpClient(_smtpHost, _smtpPort))
                using (var message = new MailMessage())
                {
                    smtp.EnableSsl = true;
                    smtp.Timeout = 100000;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_smtpUser, _smtpPass);

                    message.To.Add(toEmail);
                    message.From = new MailAddress(fromAddress);
                    message.Subject = subject ?? String.Empty;
                    message.Body = body ?? String.Empty;

                    smtp.Send(message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SMTP send failed: " + ex.Message);
                return false;
            }
        }
    }
}
