using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BusinessObject.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public DateTime BirthDay { get; set; }

        [Required]
        public bool Gender { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Skill { get; set; }

        [Required]
        public string OtherContact { get; set;}

        [Required]
        public string Avatar { get; set; }

        [Required]
        public string Theme { get; set; }

        [Required]
        public int Status { get; set; }

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
