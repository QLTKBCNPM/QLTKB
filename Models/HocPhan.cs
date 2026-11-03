using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTKB.Models
{
    [Table("HocPhan")]
    public class HocPhan
    {
        [Key]
        public int HocPhanID { get; set; }

        [Required(ErrorMessage = "Mã học phần không được để trống")]
        [StringLength(20, ErrorMessage = "Mã học phần tối đa 20 ký tự")]
        [Display(Name = "Mã học phần")]
        public string MaHocPhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên học phần không được để trống")]
        [StringLength(150, ErrorMessage = "Tên học phần tối đa 150 ký tự")]
        [Display(Name = "Tên học phần")]
        public string TenHocPhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số tín chỉ không được để trống")]
        [Range(1, 15, ErrorMessage = "Số tín chỉ từ 1 đến 15")]
        [Display(Name = "Số tín chỉ")]
        public int SoTinChi { get; set; } = 3;

        [Range(0, 300, ErrorMessage = "Số tiết lý thuyết từ 0 đến 300")]
        [Display(Name = "Số tiết lý thuyết")]
        public int SoTietLyThuyet { get; set; } = 0;

        [Range(0, 300, ErrorMessage = "Số tiết thực hành từ 0 đến 300")]
        [Display(Name = "Số tiết thực hành")]
        public int SoTietThucHanh { get; set; } = 0;

        [StringLength(500)]
        [Display(Name = "Mô tả / Đề cương")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;
    }
}

