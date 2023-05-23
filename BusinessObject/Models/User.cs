using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BusinessObject.Enums;

namespace BusinessObject.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string? Password { get; set; }

        [Required]
        public string Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public DateTime BirthDay { get; set; }

        [Required]
        public bool Gender { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Skill { get; set; }

        public string? Token { get; set; }

        public string? OtherContact { get; set;}

        public string? Avatar { get; set; }

        public string? Theme { get; set; }

        [Required]
        public UserStatus Status { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        [InverseProperty(nameof(Feedback.FeedbackedFor))]
        public List<Feedback> ReceivedFeedbacks { get; set; }

        [InverseProperty(nameof(Feedback.FeedbackedBy))]
        public List<Feedback> SentFeedbacks { get; set; }

        public List<Transaction> Transactions { get; set; }

        public List<UserMajor> UserMajors { get; set; }

        public List<Member> Members { get; set; }

        public List<Application> Applications { get; set; }
    }
}
