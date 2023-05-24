using BusinessObject.Data;
using BusinessObject.Models;
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

        public async Task<Major> findByID(string id)
        {
            Major major = null;
            try
            {
                
                major = await _context.Majors.FindAsync(id);
                return major;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
