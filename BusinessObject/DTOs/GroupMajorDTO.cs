using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class GroupMajorDTO
    {
        public Guid GroupId { get; set; }

        public Guid MajorId { get; set; }

        public int MemberCount { get; set; }

        public GroupMajorStatus Status { get; set; }
    }
}
