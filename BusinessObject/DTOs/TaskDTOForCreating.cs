using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class TaskDTOForCreating
    {
        public string Name { get; set; }

        public DateTime StartDateDeadline { get; set; }

        public DateTime EndDateDeadline { get; set; }

        public ImportantLevel ImpotantLevel { get; set; }

        public int EstimatedDays { get; set; }

        public string Description { get; set; }

        public Enums.TaskStatus Status { get; set; }

        public Guid? MainTaskId { get; set; }

        public Guid GroupId { get; set; }

        public List<Guid> AssignedForIds { get; set; }
    }
}
