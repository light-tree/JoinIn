using BusinessObject.Data;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class CommentRepository : ICommentRepository
    {
        private readonly Context _context;

        public CommentRepository(Context context)
        {
            _context = context;
        }

        public int DeleteByTaskId(Guid taskId)
        {
            List<Comment> deletedComments = _context.Comments.Where(c => c.TaskId == taskId).ToList();
            _context.Comments.RemoveRange(deletedComments);
            return _context.SaveChanges();
        }
    }
}
