using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Models
{
    public class tbDonHangExtend
    {
        public tbKhachHang_DonHang DonHang { get; set; } = new tbKhachHang_DonHang();
        public tbSanPham SanPham { get; set; } = new tbSanPham();
        public tbSanPham_LoaiSanPham_TrinhDo TrinhDoDauVao { get; set; } = new tbSanPham_LoaiSanPham_TrinhDo();
        public tbSanPham_LoaiSanPham_TrinhDo TrinhDoDauRa { get; set; } = new tbSanPham_LoaiSanPham_TrinhDo();
        public tbKhachHang KhachHang { get; set; } = new tbKhachHang();
        public tbNguoiDung ThongTinNguoiTao { get; set; } = new tbNguoiDung();
    }
}