using System.ComponentModel.DataAnnotations;

namespace Domain.Config
{
    public abstract class BaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; private set; }

        [Required]
        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public DateTime? DeletedAt { get; private set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            DeletedAt = DateTime.UtcNow;
        }
    }
}
