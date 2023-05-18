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
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public int GroupSize { get; set; }

        [Required]
        public int MemberCount { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Skill { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [ForeignKey(nameof(CurrentMilestone))]
        public Guid? CurrentMilestoneId { get; set; }

        public Milestone? CurrentMilestone { get; set; }

        [InverseProperty(nameof(Milestone.Group))]
        public List<Milestone> Milestones { get; set; }

        public List<Task> Tasks { get; set; }

        public List<GroupMajor> GroupMajors { get; set; }

        public List<Application> Applications { get; set; }

        public List<Member> Members { get; set; }
    }
}
