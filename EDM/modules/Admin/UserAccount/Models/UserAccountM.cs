using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserAccount.Models
{
    public class UserAccountM
    {
    }
    public class tbNguoiDungExtend : tbNguoiDung
    {
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbKieuNguoiDung KieuNguoiDung { get; set; } = new tbKieuNguoiDung();
        public tbCoCauToChuc CoCauToChuc { get; set; } = new tbCoCauToChuc();
        public default_tbChucVu ChucVu { get; set; } = new default_tbChucVu();
        public string MatKhauCu { get; set; } = string.Empty;
        public string MatKhauMoi { get; set; } = string.Empty;
    }
    public class CapNhatTaiKhoanMailM<T>
    {
        public T NguoiDung_OLD { get; set; }
        public T NguoiDung_NEW { get; set; }
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
        public string HinhThucCapNhat { get; set; } = "thongtinnguoidung";
    }

    public class ThongTinThietBiLuuTru
    {
        public string IPAddress { get; set; }
        public string DeviceName { get; set; }
    }
    public class NoiDungBanQuyen
    {
        public string MaBanQuyen { get; set; }
        public string KeyDonVi { get; set; }
        public string TenDonViSuDung { get; set; }
        public string TenNguoiDangKy { get; set; }
        public DateTime NgayKichHoat { get; set; }
        public DateTime NgayHetHan { get; set; }
        public int TongThoiGian { get; set; }
    }
}