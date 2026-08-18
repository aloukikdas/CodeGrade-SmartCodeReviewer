using CodeReviewer.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeReviewer.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users {get;set;}
        public DbSet<Assignment> Assignments {get;set;}
        public DbSet<Submission> Submissions {get;set;}
        public DbSet<Classroom> Classrooms {get;set;}
        public DbSet<Enrollment> Enrollments {get;set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}