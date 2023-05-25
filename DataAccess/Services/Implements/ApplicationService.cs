using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationMajorRepository _applicationMajorRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IGroupMajorRepository _groupMajorRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly IUserMajorRepository _userMajorRepository;

        public ApplicationService(IApplicationRepository applicationRepository, IApplicationMajorRepository applicationMajorRepository, IGroupRepository groupRepository, IMemberRepository memberRepository, IGroupMajorRepository groupMajorRepository, IMajorRepository majorRepository, IUserMajorRepository userMajorRepository)
        {
            _applicationRepository = applicationRepository;
            _applicationMajorRepository = applicationMajorRepository;
            _groupRepository = groupRepository;
            _memberRepository = memberRepository;
            _groupMajorRepository = groupMajorRepository;
            _majorRepository = majorRepository;
            _userMajorRepository = userMajorRepository;
        }

        public Guid? ConfirmApplication(Guid leaderId, ConfirmedApplicationDTO confirmedApplicationDTO)
        {
            if (confirmedApplicationDTO.Status == ApplicationStatus.WAITING)
                throw new Exception("Status to confirm must be APPROVED or DISAPPROVED");

            Application application = _applicationRepository.FindById(confirmedApplicationDTO.ApplicationId);
            if (application == null)
                throw new Exception("Applcation does not exist.");

            if (application.Status != ApplicationStatus.WAITING)
                throw new Exception("Applcation was confirmed.");

            if (_memberRepository.GetRoleInThisGroup(leaderId, application.GroupId) != MemberRole.LEADER)
                throw new Exception("This user is not leader in this group.");

            Guid? resultId = _applicationRepository.ConfirmApplication(confirmedApplicationDTO);
            if (resultId == null) throw new Exception("Confirm application fail");

            _groupRepository.IncreaseCurrentMemberCount(application.GroupId, 1);

            List<GroupMajor> groupMajors = _groupMajorRepository.FindByGroupId(application.GroupId);
            List<ApplicationMajor> applicationMajors = _applicationMajorRepository.FindByApplicationId(resultId.Value);
            if (applicationMajors.GroupBy(x => x.MajorId).Any(g => g.Count() > 1))
                throw new Exception("Exist duplicated major Id.");
            bool IsMatch;
            foreach (ApplicationMajor appliedMajor in applicationMajors)
            {
                if (_majorRepository.FindByID(appliedMajor.MajorId) == null)
                    throw new Exception("Major with Id: " + appliedMajor.MajorId + " is not exist.");
                IsMatch = false;
                foreach (GroupMajor groupMajor in groupMajors)
                {
                    if (appliedMajor.MajorId == groupMajor.MajorId)
                    {
                        if (!(groupMajor.MemberCount > 0 && groupMajor.Status == GroupMajorStatus.OPEN))
                            throw new Exception("This group no longer need member has major with Id: " + appliedMajor.MajorId);
                        IsMatch = true;
                        _groupMajorRepository.DecreaseCurrentNeededMemberCount(groupMajor, 1);
                        break;
                    }
                }
                if (!IsMatch)
                    throw new Exception("Applied major with Id: " + appliedMajor.MajorId + " does not match with group's application needs.");
            }
            _memberRepository.CreateMember(application.UserId, application.GroupId, MemberRole.MEMBER);
            return resultId.Value;
        }

        public Guid CreateApplication(Guid userId, SentApplicationDTO sentApplicationDTO)
        {
            if(sentApplicationDTO.MajorIds.GroupBy(x => x).Any(g => g.Count() > 1))
                throw new Exception("Exist duplicated major Id.");
            if (_groupRepository.FindById(sentApplicationDTO.GroupId) == null)
                throw new Exception("Group does not exist.");
            if (_memberRepository.FindByUserIdAndGroupId(userId, sentApplicationDTO.GroupId) != null)
                throw new Exception("User is already a member of this group.");
            List<GroupMajor> groupMajors = _groupMajorRepository.FindByGroupId(sentApplicationDTO.GroupId);
            List<UserMajor> userMajors = _userMajorRepository.FindByUserId(userId);
            bool IsMatch;
            foreach (Guid majorAppliedId in sentApplicationDTO.MajorIds)
            {
                if (_majorRepository.FindByID(majorAppliedId) == null)
                    throw new Exception("Major with Id: " + majorAppliedId + " is not exist.");
                IsMatch = false;
                foreach (GroupMajor groupMajor in groupMajors)
                {
                    if (majorAppliedId == groupMajor.MajorId)
                    {
                        if(!userMajors.Select(um => um.MajorId).Contains(majorAppliedId))
                            throw new Exception("Your application's major with Id: " + majorAppliedId + " does not match with your major.");
                        if (!(groupMajor.MemberCount > 0 && groupMajor.Status == GroupMajorStatus.OPEN))
                            throw new Exception("This group no longer need member has major with Id: " + majorAppliedId);
                        IsMatch = true;
                        break;
                    }
                }
                if (!IsMatch)
                    throw new Exception("Applied major with Id: " + majorAppliedId + " does not match with group's application needs.");
            }
            Guid newApplicationId = _applicationRepository.CreateApplication(userId, sentApplicationDTO).Id;
            _applicationMajorRepository.CreateApplicationMajors(newApplicationId, sentApplicationDTO.MajorIds);
            return newApplicationId;
        }
    }
}
