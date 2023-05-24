using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements    
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly Context _context;

        public ApplicationRepository(Context context)
        {
            _context = context;
        }

        public Application CreateApplication(Guid userId, SentApplicationDTO sentApplicationDTO)
        {
            Application application = new Application
            {
                CreatedDate = DateTime.Now,
                UserId = userId,
                Status = BusinessObject.Enums.ApplicationStatus.WAITING,
                Description = sentApplicationDTO.Description,
                GroupId = sentApplicationDTO.GroupId
            };
            _context.Applications.Add(application);
            _context.SaveChanges();
            return application;
        }

        public Application FindById(Guid id)
        {
            return _context.Applications.FirstOrDefault(a => a.Id == id);
        }

        public Guid? ConfirmApplication(ConfirmedApplicationDTO confirmedApplicationDTO)
        {
            Application application = FindById(confirmedApplicationDTO.ApplicationId);
            application.Status = confirmedApplicationDTO.Status;
            _context.Update(application);
            if (_context.SaveChanges() != 1) return null;
            else return application.Id;
        }
    }
}
