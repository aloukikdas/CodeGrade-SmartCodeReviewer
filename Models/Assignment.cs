using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeReviewer.Api.Models
{
    public class Assignment
    {
        [Key]
        public int Id {get;set;}

        [Required]
        [StringLength(200)]
        public string Title {get;set;} = string.Empty;

        [Required]
        public string Description {get;set;} = string.Empty;

        public int? ClassroomId { get; set; }

        [ForeignKey("ClassroomId")]
        public Classroom? Classroom { get; set; }

        public string AllowedLanguage { get; set; } = "Any";
    }
}