using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZaitovaLibrary.Models
{
    [Table("partners_zaitova", Schema = "app")]
    public class Partner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("type_id")]
        public int TypeId { get; set; }

        [Required]
        [Column("company_name")]
        [MaxLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        [Column("legal_address")]
        public string? LegalAddress { get; set; }

        [Column("inn")]
        [MaxLength(12)]
        public string? Inn { get; set; }

        [Column("director_fullname")]
        [MaxLength(255)]
        public string? DirectorFullname { get; set; }

        [Column("phone")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Column("email")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Column("rating")]
        public int Rating { get; set; }

        [Column("logo_path")]
        public string? LogoPath { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("TypeId")]
        public virtual PartnerType? PartnerType { get; set; }

        public virtual ICollection<SalesHistory> SalesHistories { get; set; } = new List<SalesHistory>();
        public virtual ICollection<SalesPoint> SalesPoints { get; set; } = new List<SalesPoint>();
    }
}