namespace ProblemSolvingTracker.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Difficulty? Difficulty { get; set; }
        public Tag? Tag { get; set; }
    }
}
