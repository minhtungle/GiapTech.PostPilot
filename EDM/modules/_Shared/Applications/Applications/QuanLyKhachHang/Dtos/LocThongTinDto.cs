using System;
using System.Collections.Generic;

namespace Applications.QuanLyKhachHang.Dtos
{
    public class LocThongTinDto
    {
        public IEnumerable<Guid> IdCoCauToChucs { get; set; }
        public string NgayTao { get; set; } = DateTime.Now.ToString("MM/yyyy");
        public string TenKhachHang { get; set; }
        public string TenNhanVien { get; set; }
        public string Email { get; set; }
        public Nullable<Guid> IdLoaiKhachHang { get; set; }
        public Nullable<Guid> IdGoiChamSoc { get; set; }
    }
}