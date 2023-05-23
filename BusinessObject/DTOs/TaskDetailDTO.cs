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
    public class TaskDetailDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string StartDateDeadline { get; set; }

        [Required]
        public string EndDateDeadline { get; set; }

        public string? FinishedDate { get; set; }

        [Required]
        public string ImpotantLevel { get; set; }

        [Required]
        public string EstimatedDays { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        public UserDTOForTaskList? CreatedBy { get; set; }

        public TaskRecordDTO? MainTask { get; set; }

        public List<TaskRecordDTO>? SubTasks { get; set; }

        public List<UserDTOForTaskList>? AssignedFor { get; set; }

        public List<Comment>? Comments { get; set; }
    }
}
