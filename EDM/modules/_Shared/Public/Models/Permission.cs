using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Models
{
    public class Permission
    {
        public tbNguoiDung NguoiDung { get; set; } = new tbNguoiDung();
        public default_tbChucVu ChucVu { get; set; } = new default_tbChucVu();
        public tbCapDo_DoanhThu CapDo_DoanhThu { get; set; } = new tbCapDo_DoanhThu();
        public tbKieuNguoiDung KieuNguoiDung { get; set; } = new tbKieuNguoiDung();
        public tbCoCauToChuc CoCauToChuc { get; set; } = new tbCoCauToChuc();
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
        public string Role { get; set; } = string.Empty;
    }
}