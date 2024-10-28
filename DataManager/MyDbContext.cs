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
        public DbSet<Topic> Topics => Set<Topic>();
        public DbSet<Count> Counts => Set<Count>();
        public DbSet<StudyMaterial> StudyMaterials => Set<StudyMaterial>();
        public DbSet<TopicStudyMaterial> TopicStudyMaterials => Set<TopicStudyMaterial>();
        public DbSet<TopicTag> TopicTags => Set<TopicTag>();
    }
}
