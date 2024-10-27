using ManagerClasses;

namespace ProblemSolvingTracker.Models
{
    public class Count : Entity
    {
        public DateTime Date { get; set; }
        public Topic? Topic { get; set; }
        public int SolveCount { get; set; }
    }
}
