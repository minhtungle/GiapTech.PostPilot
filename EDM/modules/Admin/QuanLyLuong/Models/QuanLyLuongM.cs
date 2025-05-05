using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;

namespace QuanLyLuong.Models
{
    public class QuanLyLuongM
    {
    }
    public class tbNguoiDungExtend : tbNguoiDung
    {
        public default_tbChucVu ChucVu { get; set; } = new default_tbChucVu();
        public tbNguoiDung_TienLuong TienLuong { get; set; }
        public bool GuiMail { get; set; } = false;
        public List<tbLopHocExtend> LopHocs { get; set; } = new List<tbLopHocExtend>();
        public tbNguoiDung_TienLuong_CongThuc CongThucTinhLuong { get; set; } = new tbNguoiDung_TienLuong_CongThuc
        {
            TienThuongThem = 0,
            HeSo_ChuaDiemDanh = 0,
            HeSo_DaDiemDanh = 1,
            HeSo_HocVienNghiKhongPhep = 0.5M,
            HeSo_HocVienNghiCoPhep = 0.5M
        };
    }
    public class tbLopHocExtend
    {
        public Guid IdLopHoc { get; set; }
        public string TenLopHoc { get; set; }
        public List<tbLopHoc_BuoiHoc> BuoiHocs { get; set; }
    }
    public struct NguoiDungCanTinhTongLuongM
    {
        public List<tbNguoiDungExtend> NguoiDungs { get; set; }
        public string ThoiGian { get; set; }
    }
}