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
    public class Task
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDateDeadline { get; set; }

        [Required]
        public DateTime EndDateDeadline { get; set; }

        [Required]
        public DateTime FinishedDate { get; set; }


        [Required]
        public string Description { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }

        [Required]
        [ForeignKey(nameof(CreatedBy))]
        public Guid CreatedById { get; set; }

        [Required]
        [ForeignKey(nameof(MainTask))]
        public Guid? MainTaskId { get; set; }

        public Group Group { get; set; }

        public User CreatedBy { get; set; }

        public Task? MainTask { get; set; }

        [InverseProperty(nameof(MainTask))]
        public List<Task> SubTasks { get; set;}

        public List<AssignedTask> AssignedTasks { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
