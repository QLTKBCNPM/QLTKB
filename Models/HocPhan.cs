
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTKB.Models
{
    [Table("HocPhan")]
    public class HocPhan
    {
        [Key]
        public int HocPhanID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã học phần")]
        [StringLength(20)]
        public string MaHocPhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên học phần")]
        [StringLength(150)]
        public string TenHocPhan { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Số tín chỉ phải từ 1 đến 20")]
        public int SoTinChi { get; set; } = 3;

        [Range(0, 500)]
        public int SoTietLyThuyet { get; set; }

        [Range(0, 500)]
        public int SoTietThucHanh { get; set; }

        [StringLength(500)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;
    }
}

