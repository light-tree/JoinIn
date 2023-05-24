using BusinessObject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IApplicationService
    {
        Guid? ConfirmApplication(Guid userId, ConfirmedApplicationDTO confirmedApplicationDTO);
        Guid CreateApplication(Guid userId, SentApplicationDTO sentApplicationDTO);
    }
}
