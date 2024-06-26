using Microsoft.EntityFrameworkCore;
using ThesisApp.Entities;

namespace Municipality.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Thesis> Theses { get; set; }
        public DbSet<ConfirmationCode> ConfirmationCodes { get; set; }
        public DbSet<Deadline> Deadlines { get; set; }
        public DbSet<RequestToThesis> RequestToTheses { get; set; }
        public DbSet<Notification> Notifications { get; set; } 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

    }
}