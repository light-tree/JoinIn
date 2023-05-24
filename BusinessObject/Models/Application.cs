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
    public class Application
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public ApplicationStatus Status { get; set; }

        public DateTime? ConfirmedDate { get; set;}

        [Required]
        public string? Description { get; set;}

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set;}

        [Required]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }

        public User User { get; set; }

        public Group Group { get; set;}

        public List<ApplicationMajor> ApplicationMajors { get; set;}
    }
}
