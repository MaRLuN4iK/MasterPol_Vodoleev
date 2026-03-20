using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterPol.Data.Models
{
    [Table("partner_types", Schema = "app")]
    public class PartnerType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Навигационное свойство
        public virtual ICollection<Partner> Partners { get; set; }

        public PartnerType()
        {
            Partners = new HashSet<Partner>();
        }
    }
}