using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZaitovaLibrary.Models
{
    [Table("sales_points_zaitova", Schema = "app")]
    public class SalesPoint
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("partner_id")]
        public int PartnerId { get; set; }

        [Required]
        [Column("address")]
        public string Address { get; set; } = string.Empty;

        [Column("point_type")]
        [MaxLength(50)]
        public string? PointType { get; set; }

        [ForeignKey("PartnerId")]
        public virtual Partner? Partner { get; set; }
    }
}