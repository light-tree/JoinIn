using BusinessObject.Data;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class AssignedTaskRepository : IAssignedTaskRepository
    {
        private readonly Context _context;

        public AssignedTaskRepository(Context context)
        {
            _context = context;
        }

        public List<AssignedTask> CreateAssignedTasks(List<Guid> assignedForIds, Guid newTaskId, Guid createdById)
        {
            DateTime dateTime = DateTime.Now;
            List<AssignedTask> assignedTasks = new List<AssignedTask>();
            foreach (var assignedForId in assignedForIds)
            {
                if (FindByTaskIdAndAssignedForId(newTaskId, assignedForId) != null)
                    throw new Exception("Member with Id: " + assignedForId + " is assigned duplicately with this task.");
                assignedTasks.Add(new()
                {
                    TaskId = newTaskId,
                    AssignedById = createdById,
                    AssignedForId = assignedForId,
                    AssignedDate = dateTime,
                });
            }
            _context.AssignedTasks.AddRange(assignedTasks);
            _context.SaveChanges();
            return assignedTasks;
        }

        public AssignedTask FindByTaskIdAndAssignedForId(Guid taskId, Guid assignedForId)
        {
            return _context.AssignedTasks.FirstOrDefault(a => a.TaskId == taskId && a.AssignedForId == assignedForId);
        }

        public int DeleteByAssignedForId(Guid lostAssignedForId)
        {
            AssignedTask lostAssignedTask = _context.AssignedTasks.FirstOrDefault(a => a.AssignedForId == lostAssignedForId);
            _context.AssignedTasks.Remove(lostAssignedTask);
            return _context.SaveChanges();
        }

        public int DeleteByTaskId(Guid taskId)
        {
            AssignedTask deletedAssignedTask = _context.AssignedTasks.FirstOrDefault(a => a.TaskId == taskId);
            _context.AssignedTasks.Remove(deletedAssignedTask);
            return _context.SaveChanges();
        }

        public List<AssignedTask> FindByTaskId(Guid id)
        {
            return _context.AssignedTasks.Where(a => a.TaskId == id).ToList();
        }
    }
}
