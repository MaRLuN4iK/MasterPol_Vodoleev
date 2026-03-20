using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MasterPol.Data.Models
{
    [Table("partners", Schema = "app")]
    public class Partner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("partner_type_id")]
        public int PartnerTypeId { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column("legal_address")]
        [MaxLength(500)]
        public string LegalAddress { get; set; }

        [Column("inn")]
        [MaxLength(20)]
        public string INN { get; set; }

        [Column("director_name")]
        [MaxLength(255)]
        public string DirectorName { get; set; }

        [Column("phone")]
        [MaxLength(20)]
        public string Phone { get; set; }

        [Column("email")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [Column("rating")]
        public int Rating { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Навигационные свойства
        [ForeignKey("PartnerTypeId")]
        public virtual PartnerType PartnerType { get; set; }

        public virtual ICollection<SaleHistory> SaleHistories { get; set; }

        public Partner()
        {
            SaleHistories = new HashSet<SaleHistory>();
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        // Расчет скидки на основе истории продаж
        public decimal CalculateDiscount()
        {
            if (SaleHistories == null || SaleHistories.Count == 0)
                return 0;

            decimal totalSales = SaleHistories.Sum(sh => sh.Quantity * (sh.Product != null ? sh.Product.MinPartnerPrice : 0));

            if (totalSales < 10000) return 0;
            if (totalSales < 50000) return 5;
            if (totalSales < 300000) return 10;
            return 15;
        }
    }
}