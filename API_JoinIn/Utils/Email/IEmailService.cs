namespace API_JoinIn.Utils.Email
{
    public interface IEmailService
    {
        public  Task SendConfirmationEmail(string toEmail, string emailVerificationLink);
        public Task SendRecoveryPasswordEmail(string toEmail, string passwordRecoveryLink);
    }
}
