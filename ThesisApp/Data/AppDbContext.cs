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
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Thesis>()
                .HasMany(e => e.Students)
                .WithOne(e => e.SelectedThesis)
                .HasForeignKey(e => e.SelectedThesisId)
                .IsRequired();
        }

    }
}