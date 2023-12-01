using Microsoft.EntityFrameworkCore;
using WebAppVideoCamersOperzal.Models.Entities;

namespace WebAppVideoCamersOperzal.Models
{
    /// <summary>
    /// Контекст базы данных
    /// </summary>
    public class ApplicationContext: DbContext
    {
        /// <summary>
        /// Организации
        /// </summary>
        public DbSet<Organization> Organizations => Set<Organization>();

        /// <summary>
        /// Камеры
        /// </summary>
        public DbSet<VideoCamera> VideoCameras => Set<VideoCamera>();   
        
        /// <summary>
        /// Посещения
        /// </summary>
        public DbSet<Visit> Visits => Set<Visit>();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="options"></param>
        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {
            Database.EnsureCreated();
        }       
       
        /// <summary>
        /// Настройки
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organization>()
                .Ignore(t => t.dateEndStr)
                .HasMany(e => e.camers)
                .WithOne(e => e.organization)
                .HasForeignKey(e => e.orgCode)
                .HasPrincipalKey(e => e.code)
                .OnDelete(DeleteBehavior.Cascade);
        }        

    }
}
