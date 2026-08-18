namespace CodeReviewer.Api.DTOs
{
    public class CreateClassroomDto
    {
        public string Name { get; set; } = string.Empty;
        public int TeacherId { get; set; }
    }

    public class EnrollDto
    {
        public string JoinCode { get; set; } = string.Empty;
        public int StudentId { get; set; }
    }

    public class CreateAssignmentDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ClassroomId { get; set; }
        public string AllowedLanguage { get; set; } = "Any";
    }
}