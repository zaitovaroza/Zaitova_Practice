using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZaitovaLibrary.Models
{
    [Table("partner_types_zaitova", Schema = "app")]
    public class PartnerType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}