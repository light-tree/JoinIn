namespace API_JoinIn.Utils.Email
{
    public interface IEmailService
    {
        public  Task SendConfirmationEmail(string toEmail);
    }
}
