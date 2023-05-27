using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IGroupMajorRepository _groupMajorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMajorRepository _majorRepository;

        public GroupService(IGroupRepository groupRepository, IMemberRepository memberRepository, IGroupMajorRepository groupMajorRepository, IUserRepository userRepository, IMajorRepository majorRepository)
        {
            _groupRepository = groupRepository;
            _memberRepository = memberRepository;
            _groupMajorRepository = groupMajorRepository;
            _userRepository = userRepository;
            _majorRepository = majorRepository;
        }

        public Guid CreateGroup(Guid createrId, GroupDTOForCreating groupDTOForCreating)
        {
            int totalMemberNeeded = groupDTOForCreating.GroupMajorDTOs.Select(gm => gm.MemberCount).Sum();
            if (totalMemberNeeded > groupDTOForCreating.GroupSize - 1)
                throw new Exception("Total member needed must smaller than the team's size since it also count the creater.");

            Group group = _groupRepository.CreateGroup(groupDTOForCreating);

            _memberRepository.CreateMember(createrId, group.Id, MemberRole.LEADER);

            foreach(GroupMajorDTO groupMajorDTO in groupDTOForCreating.GroupMajorDTOs)
            {
                if (_majorRepository.FindByID(groupMajorDTO.MajorId) != null)
                    _groupMajorRepository.CreateGroupMajor(group.Id, groupMajorDTO);
                else throw new Exception("Major with Id: " + groupMajorDTO.MajorId + " is not exist.");
            }
            return group.Id;
        }
    }
}
