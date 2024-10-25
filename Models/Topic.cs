namespace ProblemSolvingTracker.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<StudyMaterial> StudyMaterials { get; set; } = new List<StudyMaterial>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
