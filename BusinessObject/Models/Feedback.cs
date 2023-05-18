﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public float Rating { get; set; }

        [Required]
        public DateTime FeedbackedDate { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [ForeignKey(nameof(FeedbackedBy))]
        public long FeedbackedById { get; set; }

        [Required]
        [ForeignKey(nameof(FeedbackedFor))]
        public long FeedbackedForId { get; set; }

        public User FeedbackedBy { get; set; }

        public User FeedbackedFor { get; set; }
    }
}