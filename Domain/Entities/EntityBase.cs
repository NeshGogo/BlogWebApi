using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [MaxLength(200)]
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        [MaxLength(200)]
        public string? UpdatedBy { get; set; }
    }
}
