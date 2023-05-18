using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class UserMajor
    {
        [Key]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Key]
        [ForeignKey(nameof(Major))]
        public Guid MajorId { get; set; }

        public User User { get; set; }

        public Major Major { get; set; }
    }
}
