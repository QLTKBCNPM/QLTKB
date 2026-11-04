using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTKB.Models
{
    [Table("GiangVien")]
    public class GiangVien
    {
        [Key]
        public int GiangVienID { get; set; }

        public int TaiKhoanID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảng viên")]
        [StringLength(20)]
        public string MaGiangVien { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Khoa { get; set; }

        [StringLength(100)]
        public string? BoMon { get; set; }

        [StringLength(50)]
        public string? HocVi { get; set; }

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [ForeignKey("TaiKhoanID")]
        public TaiKhoan? TaiKhoan { get; set; }
    }
}
