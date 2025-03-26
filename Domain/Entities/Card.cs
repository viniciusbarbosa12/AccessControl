using Domain.Config;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    [Index(nameof(Code), IsUnique = true)]
    public class Card : BaseEntity
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public int Number { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public List<int> AccessibleDoorNumbers { get; set; } = new(); 
    }
}
