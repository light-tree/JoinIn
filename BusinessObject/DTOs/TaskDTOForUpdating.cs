using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class TaskDTOForUpdating
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDateDeadline { get; set; }

        public DateTime EndDateDeadline { get; set; }

        public ImportantLevel ImpotantLevel { get; set; }

        public int EstimatedDays { get; set; }

        public string Description { get; set; }

        public Enums.TaskStatus Status { get; set; }

        public List<Guid> AssignedForIds { get; set; }
    }
}
