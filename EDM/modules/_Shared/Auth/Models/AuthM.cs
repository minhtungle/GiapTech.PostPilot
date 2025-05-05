using EDM_DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace Auth.Models
{
    public class AuthM
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public bool GhiNho { get; set; } = false;

        public string MaXacThuc { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class ForgotM {
        public AuthM TaiKhoan { get; set; } = new AuthM();
        public string DuongDanKhaiThac { get; set; } = string.Empty;
        public string MaXacNhan { get; set; } = string.Empty;
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
    }
    public class ThongBaoThietBiDangNhapM
    {
        public tbNguoiDung NguoiDung { get; set; }
       public ThongTinThietBi ThongTinThietBi { get; set; } = new ThongTinThietBi();
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
    }
    public class ThongTinThietBi
    {
        public string TenTrinhDuyet { get; set; } = string.Empty;
        public string PhienBan { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}