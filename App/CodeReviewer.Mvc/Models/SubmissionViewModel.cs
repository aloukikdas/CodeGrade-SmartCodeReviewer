using System.ComponentModel.DataAnnotations;

namespace CodeReviewer.Mvc.Models
{
    public class SubmissionViewModel
    {
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public int ClassroomId { get; set; }

        [Required(ErrorMessage = "Please write your code before submitting.")]
        public string CodeText { get; set; } = string.Empty;

        public string AssignmentTitle { get; set; } = string.Empty;
        public string AssignmentDescription { get; set; } = string.Empty;
        public string AllowedLanguage { get; set; } = "Any";
        public string Language { get; set; } = "csharp";
        public int? Grade { get; set; }
        public string? AiFeedback { get; set; }
    }
}