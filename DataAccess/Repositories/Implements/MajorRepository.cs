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

        public async Task<Major> FindByID(Guid id)
        {
            try
            {
                return await _context.Majors.FirstOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
