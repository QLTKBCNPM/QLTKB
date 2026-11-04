
using Microsoft.EntityFrameworkCore;
using QLTKB.Models;

namespace QLTKB.Data
{
    public class QLTKBContext : DbContext
    {
        public QLTKBContext(DbContextOptions<QLTKBContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoan { get; set; }

        public DbSet<VaiTro> VaiTro { get; set; }

        public DbSet<HocPhan> HocPhan { get; set; }
        public DbSet<GiangVien> GiangVien { get; set; }
        public DbSet<SinhVien> SinhVien { get; set; }
        public DbSet<PhongHoc> PhongHoc { get; set; }
    }
}