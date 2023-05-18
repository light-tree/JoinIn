using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace BusinessObject.Data
{
    public class Context : DbContext
    {
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationMajor> ApplicationMajors { get; set; }
        public DbSet<AssignedTask> AssignedTasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMajor> GroupMajors { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserMajor> UserMajors { get; set; }

        public Context()
        {

        }

        public Context(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(local);Uid=sa;Pwd=123456;Database=JoinIn;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationMajor>().HasKey(a => new { a.ApplicationId, a.MajorId });
            modelBuilder.Entity<GroupMajor>().HasKey(g => new { g.GroupId, g.MajorId });
            modelBuilder.Entity<AssignedTask>().HasKey(a => new { a.AssignedById, a.AssignedForId, a.TaskId });
            modelBuilder.Entity<UserMajor>().HasKey(u => new { u.UserId, u.MajorId });

            /*Disable cascade delete: ensures that deleting a User will not cascade-delete related Feedbacks.*/
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.FeedbackedFor)
                .WithMany(u => u.ReceivedFeedbacks)
                .HasForeignKey(f => f.FeedbackedForId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.FeedbackedBy)
                .WithMany(u => u.SentFeedbacks)
                .HasForeignKey(f => f.FeedbackedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Task>()
                .HasOne(m => m.MainTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.MainTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedTask>()
                .HasOne(m => m.Task)
                .WithMany(t => t.AssignedTasks)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedTask>()
                .HasOne(f => f.AssignedFor)
                .WithMany(u => u.AssignedTasksFor)
                .HasForeignKey(f => f.AssignedForId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignedTask>()
                .HasOne(f => f.AssignedBy)
                .WithMany(u => u.AssignedTasksBy)
                .HasForeignKey(f => f.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Group>()
                .HasOne(m => m.CurrentMilestone)
                .WithOne(t => t.GroupForCurrent)
                .OnDelete(DeleteBehavior.Restrict);
            /*Use ON DELETE NO ACTION: no action should be taken on related entities during a delete operation. 
             *However, this means you need to manually handle any dependencies and ensure data integrity.*/
            //modelBuilder.Entity<Feedback>()
            //    .HasOne(f => f.FeedbackedFor)
            //    .WithMany(u => u.ReceivedFeedbacks)
            //    .HasForeignKey(f => f.FeedbackedForId)
            //    .OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<Feedback>()
            //    .HasOne(f => f.FeedbackedBy)
            //    .WithMany(u => u.SentFeedbacks)
            //    .HasForeignKey(f => f.FeedbackedById)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
