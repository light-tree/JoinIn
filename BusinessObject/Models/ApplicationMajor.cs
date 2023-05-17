using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class ApplicationMajor
    {
        [Key]
        [ForeignKey(nameof(Application))]
        public long ApplicationId { get; set; }

        [Key]
        [ForeignKey(nameof(Major))]
        public long MajorId { get; set; }

        public Application Application { get; set; }

        public Major Major { get; set; }
    }
}
