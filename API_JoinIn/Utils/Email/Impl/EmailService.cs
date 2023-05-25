using System.Net.Mail;
using System.Net;
using FirebaseAdmin.Auth;
using FirebaseAdmin;

namespace API_JoinIn.Utils.Email.Impl
{
    public class EmailService : IEmailService
    {
       
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        public EmailService(IConfiguration configuration)
        {
            _smtpHost = configuration["SmtpConfig:Host"];
            _smtpPort = int.Parse(configuration["SmtpConfig:Port"]);
            _smtpUsername = configuration["SmtpConfig:Username"];
            _smtpPassword = configuration["SmtpConfig:Password"];

        }

        public async Task SendConfirmationEmail(string toEmail, string emailVerificationLink)
        {
            try
            {
                //// tạo confirmlink 
                //var emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(toEmail);
                //var defaultApp = FirebaseApp.DefaultInstance;


                // gửi email confirm
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                var subject = "Xác nhận email";
                var body = $"<p>Nhấp vào liên kết sau để xác nhận email: {emailVerificationLink}</p>";

                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;

                        await smtpClient.SendMailAsync(message);
                    }
                }
            } catch(Exception ex) {
               
            }

        }

        public async Task SendRecoveryPasswordEmail(string toEmail, string passwordRecoveryLink)
        {
            try
            {
                //// tạo confirmlink 
                //var emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(toEmail);
                //var defaultApp = FirebaseApp.DefaultInstance;


                // gửi email confirm
                var from = new MailAddress(_smtpUsername);
                var to = new MailAddress(toEmail);
                var subject = "Xác nhận email";
                var body = $"<p>Nhấp vào liên kết sau để xác nhận thay đổi mật khẩu: {passwordRecoveryLink}</p>";

                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;


                    using (var message = new MailMessage(from, to))
                    {
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;

                        await smtpClient.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
