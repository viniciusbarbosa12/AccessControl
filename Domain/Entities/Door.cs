using Domain.Config;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Door : BaseEntity
    {
        [Required]
        public int Number { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

    }
}
