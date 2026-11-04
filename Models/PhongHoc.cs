using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLTKB.Models
{
    [Table("PhongHoc")]
    public class PhongHoc
    {
        [Key]
        public int PhongHocID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã phòng")]
        [StringLength(20)]
        public string MaPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên phòng")]
        [StringLength(100)]
        public string TenPhong { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ToaNha { get; set; }

        [Range(1, 1000, ErrorMessage = "Sức chứa phải từ 1 đến 1000")]
        public int SucChua { get; set; } = 40;

        [StringLength(50)]
        public string? LoaiPhong { get; set; }

        public bool TrangThai { get; set; } = true;
    }
}
