using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Milestone
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Status { get; set;}

        [Required]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }

        public Group Group { get; set; }

        public Group GroupForCurrent { get; set; }
    }
}
