using BusinessObject.Data;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class MajorRepository : IMajorRepository
    {
        Context _context;
        public MajorRepository(Context context)
        {
            _context = context;
        }

        public Major FindByID(Guid id)
        {
            try
            {
                return _context.Majors.FirstOrDefault(m => m.Id == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
