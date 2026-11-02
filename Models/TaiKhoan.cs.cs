
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTKB.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        public int TaiKhoanID { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        public int VaiTroID { get; set; }

        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; }

        [ForeignKey("VaiTroID")]
        public VaiTro? VaiTro { get; set; }
    }
}

