using BusinessObject.DTOs.Anoucement;
using BusinessObject.Enums;
using BusinessObject.Models;

namespace API_JoinIn.Utils.Email
{
    public interface IEmailService
    {
        public  System.Threading.Tasks.Task SendConfirmationEmail(string toEmail, string emailVerificationLink);
        public  System.Threading.Tasks.Task SendRecoveryPasswordEmail(string toEmail, string passwordRecoveryLink);

        public System.Threading.Tasks.Task SendEmailNotifyAssignTaskToMember(string toEmail,
                                                          string Name,
                                                          string TaskName,
                                                          string atGroup,
                                                          string Description,
                                                          string StartDate,
                                                          string Deadline,
                                                          ImportantLevel importantLevel
                                                          );

        public  System.Threading.Tasks.Task SendNotificationEmail(string toEmail,string Name, List<AnoucementDTO> notification);
    }
}
