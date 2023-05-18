using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class GroupMajor
    {
        [Key]
        [ForeignKey(nameof(Group))]
        public Guid GroupId { get; set; }

        [Key]
        [ForeignKey(nameof(Major))]
        public Guid MajorId { get; set; }

        [Required]
        public int MemberCount { get; set; }

        public Group Group { get; set; }

        public Major Major { get; set; }
    }
}
