using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDoanhThu.Dtos
{
    public class BangXepHangDto
    {
        public int XepHang {  get; set; }
        public string TenNguoiDung { get; set; } = string.Empty;
        public string TenChucVu { get; set; } = string.Empty;
        public long TongDoanhThu { get; set; } = 0;
    }
}