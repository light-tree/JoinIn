using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AssignedTask
    {
        [Key]
        [ForeignKey(nameof(Task))]
        public long TaskId { get; set; }

        [Key]
        [ForeignKey(nameof(Member))]
        public long MemberId { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; }

        public Task Task { get; set; }

        public Member Member { get; set; }
    }
}
