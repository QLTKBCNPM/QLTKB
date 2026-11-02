
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTKB.Models
{
    [Table("VaiTro")]
    public class VaiTro
    {
        [Key]
        public int VaiTroID { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVaiTro { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }

        public ICollection<TaiKhoan> TaiKhoans { get; set; }
            = new List<TaiKhoan>();
    }
}

