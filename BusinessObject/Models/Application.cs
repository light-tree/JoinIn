using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public DateTime ConfirmedDate { get; set;}

        [Required]
        public string Description { get; set;}

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set;}

        [Required]
        [ForeignKey(nameof(Group))]
        public long GroupId { get; set; }

        public User User { get; set; }

        public Group Group { get; set;}

        public List<ApplicationMajor> ApplicationMajors { get; set;}
    }
}
