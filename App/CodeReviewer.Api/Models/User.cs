using System.ComponentModel.DataAnnotations;

namespace CodeReviewer.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Role {get;set;} = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name {get;set;}=string.Empty;

        [Required]
        [EmailAddress]
        public string Email {get;set;} = string.Empty;

        [Required]
        public string Password {get;set;} = string.Empty;
    }
}