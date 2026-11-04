using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTKB.Models
{
    [Table("SinhVien")]
    public class SinhVien
    {
        [Key]
        public int SinhVienID { get; set; }

        public int TaiKhoanID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
        [StringLength(20)]
        public string MaSinhVien { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Lop { get; set; }

        [StringLength(100)]
        public string? Nganh { get; set; }

        [StringLength(20)]
        public string? KhoaHoc { get; set; }

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [ForeignKey("TaiKhoanID")]
        public TaiKhoan? TaiKhoan { get; set; }
    }
}
