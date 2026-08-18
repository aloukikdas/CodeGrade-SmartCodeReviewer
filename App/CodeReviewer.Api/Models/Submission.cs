using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeReviewer.Api.Models
{
    public class Submission
    {
        [Key]
        public int Id {get;set;}

        [Required]
        public int StudentId {get;set;}

        [Required]
        public int AssignmentId {get;set;}

        [Required]
        public string CodeText {get;set;} = string.Empty;

        public DateTime SubmissionDate {get;set;} = DateTime.UtcNow;

        [ForeignKey("StudentId")]
        public User? Student {get;set;}

        [ForeignKey("AssignmentId")]
        public Assignment? Assignment {get;set;}

        public string Language { get; set; } = "csharp";
        public int? Grade { get; set; }
        public string? AiFeedback { get; set; }
    }
}