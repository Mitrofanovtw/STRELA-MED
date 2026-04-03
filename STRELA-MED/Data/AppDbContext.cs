using Microsoft.EntityFrameworkCore;
using STRELA_MED.Models;

namespace STRELA_MED.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<MedicalExam> MedicalExams { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=strela_med_db;Username=postgres;Password=123");
        }
    }
}