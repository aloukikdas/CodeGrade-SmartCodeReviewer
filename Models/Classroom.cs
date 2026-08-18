using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeReviewer.Api.Models
{
    public class Classroom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string JoinCode { get; set; } = string.Empty;

        [Required]
        public int TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public User? Teacher { get; set; }
    }
}