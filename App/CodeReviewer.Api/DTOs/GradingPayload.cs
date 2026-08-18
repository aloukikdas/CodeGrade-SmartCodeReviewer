namespace CodeReviewer.Api.Models
{
    public class GradingPayload
    {
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public int Grade { get; set; }
        public string? AiFeedback { get; set; }
    }
}