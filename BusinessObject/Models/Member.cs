using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Member
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }

        [Required]
        public DateTime JoinedDate { get; set; }

        [Required]
        public DateTime LeftDate { get; set; }

        [Required]
        public int Role { get; set; }

        public User User { get; set; }

        public Group Group { get; set; }

        [InverseProperty(nameof(AssignedTask.AssignedFor))]
        public List<AssignedTask> AssignedTasksFor { get; set; }

        [InverseProperty(nameof(AssignedTask.AssignedBy))]
        public List<AssignedTask> AssignedTasksBy { get; set; }

        public List<Task> Tasks { get; set; }
    }
}
