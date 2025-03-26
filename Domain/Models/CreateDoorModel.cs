using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CreateDoorModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Door number must be greater than 0.")]
        public int DoorNumber { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Door type must be 0 or positive.")]
        public int DoorType { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Door name must have at least 3 characters.")]
        public string DoorName { get; set; } = string.Empty;
    }
}
