using Microsoft.EntityFrameworkCore;
using ProblemSolvingTracker.Models;

namespace ProblemSolvingTracker.DataManager
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            
        }

        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Difficulty> Difficulties => Set<Difficulty>();
        public DbSet<Topic> Topics => Set<Topic>();
        public DbSet<Count> Counts => Set<Count>();
    }
}
