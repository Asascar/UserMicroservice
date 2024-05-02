using Microsoft.EntityFrameworkCore;
using UserMicroservice.Models;

namespace UserMicroservice.Repositories
{
    public class AppDbContext : DbContext
    {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

            public DbSet<User> Users { get; set; }
            
    }

    public class DbContextFactory
    {
        public static AppDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new AppDbContext(options);
        }
    }
}
