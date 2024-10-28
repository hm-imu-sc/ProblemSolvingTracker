using ManagerClasses;

namespace ProblemSolvingTracker.Models
{
    public class TopicStudyMaterial : Entity
    {
        public Topic? Topic { get; set; }
        public StudyMaterial? StudyMaterial { get; set; }
    }
}
