using Microsoft.EntityFrameworkCore;
using Task_Management_API.Entities;

namespace Task_Management_API.Services
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(
            DbContextOptions<TaskDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
    }
}
