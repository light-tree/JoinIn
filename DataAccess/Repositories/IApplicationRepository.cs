﻿using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IApplicationRepository
    {
        Application CreateApplication(Guid userId, SentApplicationDTO sentApplicationDTO);
        Application FindById(Guid applicationId);
        Guid? ConfirmApplication(ConfirmedApplicationDTO confirmedApplicationDTO);
    }
}
