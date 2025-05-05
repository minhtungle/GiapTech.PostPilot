using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDoanhThu.Dtos
{
    public class DoanhThuMoiNhatDto
    {
        public tbKhachHang_DonHang_ThanhToan ThanhToan {  get; set; }
        public string TenNguoiDung { get; set; } = string.Empty;
        public string TenKhachHang { get; set; } = string.Empty;
        public string TenSanPham { get; set; } = string.Empty;
        public int PhanTramThanhToan { get; set; } = 0;
    }
}