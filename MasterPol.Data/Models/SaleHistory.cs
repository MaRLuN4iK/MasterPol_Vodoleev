using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterPol.Data.Models
{
    [Table("sale_histories", Schema = "app")]
    public class SaleHistory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("partner_id")]
        public int PartnerId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("sale_date")]
        public DateTime SaleDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public SaleHistory()
        {
            CreatedAt = DateTime.Now;
            SaleDate = DateTime.Now;
        }
    }
}