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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        [ForeignKey(nameof(Group))]
        public long GroupId { get; set; }

        [Required]
        public DateTime JoinedDate { get; set; }

        [Required]
        public DateTime LeftDate { get; set; }

        [Required]
        public int Role { get; set; }

        public User User { get; set; }

        public Group Group { get; set; }

        public List<AssignedTask > AssignedTasks { get; set; }

        public List<Task> Tasks { get; set; }
    }
}
