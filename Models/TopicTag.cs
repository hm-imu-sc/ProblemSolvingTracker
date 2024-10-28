using ManagerClasses;

namespace ProblemSolvingTracker.Models
{
    public class TopicTag : Entity
    {
        public Topic? Topic { get; set; }
        public Tag? Tag { get; set; }
    }
}
