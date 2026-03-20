using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterPol.Data.Models
{
    [Table("products", Schema = "app")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("article")]
        [MaxLength(50)]
        public string Article { get; set; }

        [Column("type")]
        [MaxLength(100)]
        public string Type { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [Column("min_partner_price")]
        public decimal MinPartnerPrice { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Навигационное свойство
        public virtual ICollection<SaleHistory> SaleHistories { get; set; }

        public Product()
        {
            SaleHistories = new HashSet<SaleHistory>();
            CreatedAt = DateTime.Now;
        }
    }
}