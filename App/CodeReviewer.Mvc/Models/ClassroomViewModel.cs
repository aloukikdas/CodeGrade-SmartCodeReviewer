using System.ComponentModel.DataAnnotations;

namespace CodeReviewer.Mvc.Models
{
    public class ClassroomViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string JoinCode { get; set; } = string.Empty;
        public int TeacherId { get; set; }
    }

    public class CreateClassroomViewModel
    {
        [Required(ErrorMessage = "Classroom name is required.")]
        [Display(Name = "Classroom Name")]
        public string Name { get; set; } = string.Empty;
    }

    public class JoinClassroomViewModel
    {
        [Required(ErrorMessage = "Please enter a valid 6-character Join Code.")]
        [Display(Name = "Class Join Code")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 characters.")]
        public string JoinCode { get; set; } = string.Empty;
    }
}