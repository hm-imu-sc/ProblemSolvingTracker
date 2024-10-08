namespace ProblemSolvingTracker.Models
{
    public class Count
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Topic? Topic { get; set; }
        public int SolveCount { get; set; }
    }
}
