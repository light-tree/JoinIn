using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Major
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<ApplicationMajor> ApplicationMajors { get; set; }

        public List<GroupMajor> GroupMajors { get; set; }

        public List<UserMajor> UserMajors { get; set; }
    }
}
