using BusinessObject.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class MajorService : IMajorService
    {
        IMajorRepository majorRepository;
        public MajorService(IMajorRepository majorRepository)
        {
            this.majorRepository = majorRepository;
        }
        public Major FindMajorById(Guid id)
        {
            var rs = majorRepository.FindByID(id); ;
            return rs;

        }
    }
}
