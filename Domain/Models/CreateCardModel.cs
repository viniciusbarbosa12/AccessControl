using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CreateCardModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Card number must be greater than 0.")]
        public int CardNumber { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "First name must have at least 2 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(2, ErrorMessage = "Last name must have at least 2 characters.")]
        public string LastName { get; set; } = string.Empty;
    }
}
