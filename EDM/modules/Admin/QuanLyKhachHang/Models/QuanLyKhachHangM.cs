using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhachHang.Models
{
    public class QuanLyKhachHangM
    {
    }
    public class tbKhachHangExtend : tbKhachHang
    {
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbNguoiDung ThongTinNguoiTao { get; set; } = new tbNguoiDung();
        public List<tbKhachHang_DonHangExtend> DonHangs { get; set; } = new List<tbKhachHang_DonHangExtend>();
        public List<tbKhachHang_LichSuExtend> LichSus { get; set; } = new List<tbKhachHang_LichSuExtend>();
        public tbKhachHang_LoaiKhachHang LoaiKhachHang { get; set; } = new tbKhachHang_LoaiKhachHang();
        public tbGoiChamSoc GoiChamSoc { get; set; } = new tbGoiChamSoc();
        public tbPhuongThucThanhToan PhuongThucThanhToan { get; set; } = new tbPhuongThucThanhToan();
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
    }

    public class tbKhachHang_LichSuExtend : tbKhachHang_LichSu
    {
        public tbNguoiDung ThongTinNguoiTao { get; set; } = new tbNguoiDung();
    }
    public class tbKhachHang_DonHangExtend : tbKhachHang_DonHang
    {
        public tbSanPham SanPham { get; set; } = new tbSanPham();
        public List<tbKhachHang_DonHang_ThanhToan> ThanhToans { get; set; } = new List<tbKhachHang_DonHang_ThanhToan>();
    }
}