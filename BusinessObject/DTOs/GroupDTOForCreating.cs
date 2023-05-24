using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class GroupDTOForCreating
    {
        [Required(ErrorMessage = "Group's name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Group's name must be between {2} and {1} characters long.")]
        public string Name { get; set; }

        public int GroupSize { get; set; }

        [Required(ErrorMessage = "School's name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "School's name must be between {2} and {1} characters long.")]
        public string SchoolName { get; set; }

        [Required(ErrorMessage = "Class's name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Class's name must be between {2} and {1} characters long.")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Subject's name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Subject's name must be between {2} and {1} characters long.")]
        public string SubjectName { get; set; }

        //[Required(ErrorMessage = "Description is required.")]
        //[StringLength(500, MinimumLength = 3, ErrorMessage = "Description must be between {2} and {1} characters long.")]
        public string? Description { get; set; }

        public string? Skill { get; set; }

        public string? Avatar { get; set; }

        public List<GroupMajorDTO> GroupMajorDTOs { get; set; }
    }
}
