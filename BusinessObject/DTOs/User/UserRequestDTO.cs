using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.User
{
    public class UserRequestDTO
    {
        [Required]
        Guid id;

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full name must be between {2} and {1} characters long.")]
        public string FullName { get; set; }

        //[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password must be at least 8 characters long with at least one letter and one number.")]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "Email is required.")]
        //[EmailAddress(ErrorMessage = "Invalid email format.")]
        //public string Email { get; set; }


        [Required(ErrorMessage = "Birthdate is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDay { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between {2} and {1} characters long.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Skill is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Skill must be between {2} and {1} characters long.")]
        public string Skill { get; set; }

        [StringLength(500, ErrorMessage = "Other contact must be less than {1} characters long.")]
        public string OtherContact { get; set; }

        [StringLength(200, ErrorMessage = "Avatar must be less than {1} characters long.")]
        public string Avatar { get; set; }

        [StringLength(200, ErrorMessage = "Theme must be less than {1} characters long.")]
        public string Theme { get; set; }


        [EnumDataType(typeof(UserStatus))]
        public UserStatus Status { get; set; }

        public List<Guid> MajorIdList { get; set; }


    }
}
