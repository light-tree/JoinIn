using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Enums;

namespace BusinessObject.Models
{
    public class Feedback
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public float Rating { get; set; }

        [Required]
        public DateTime FeedbackedDate { get; set; }

        [Required]
        public FeedbackStatus Status { get; set; }

        [Required]
        [ForeignKey(nameof(FeedbackedBy))]
        public Guid FeedbackedById { get; set; }

        [Required]
        [ForeignKey(nameof(FeedbackedFor))]
        public Guid FeedbackedForId { get; set; }

        public User FeedbackedBy { get; set; }

        public User FeedbackedFor { get; set; }
    }
}
