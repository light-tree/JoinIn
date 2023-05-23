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
        public async Task<Major> findMajorById(string id)
        {
            var rs = await majorRepository.findByID(id); ;
            return rs;

        }
    }
}
