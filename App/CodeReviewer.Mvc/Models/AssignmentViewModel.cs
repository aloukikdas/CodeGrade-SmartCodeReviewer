using System.ComponentModel.DataAnnotations;

namespace CodeReviewer.Mvc.Models
{
    public class AssignmentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ClassroomId { get; set; }
        public string AllowedLanguage { get; set; } = "Any";
        public bool HasSubmitted { get; set; } = false;
        public int SubmissionId { get; set; }
        public bool HasGrade { get; set; }
    }

    public class CreateAssignmentViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int ClassroomId { get; set; }

        public string AllowedLanguage { get; set; } = "Any";
    }
}