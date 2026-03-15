using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZaitovaLibrary.Models
{
    [Table("products_zaitova", Schema = "app")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("article")]
        [MaxLength(50)]
        public string Article { get; set; } = string.Empty;

        [Required]
        [Column("name")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("min_price")]
        public decimal? MinPrice { get; set; }

        [Column("unit")]
        [MaxLength(20)]
        public string? Unit { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<SalesHistory> SalesHistories { get; set; } = new List<SalesHistory>();
    }
}