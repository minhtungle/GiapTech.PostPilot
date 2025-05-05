using EDM_DB;
using System;
using System.Collections.Generic;

namespace Applications.QuanLyDoanhThu.Dtos
{
    public class LocThongTinChiTietDto
    {
        public string LoaiThongKe { get; set; }
        public string ThoiGian { get; set; }
        public List<Guid> IdDanhMucChas { get; set; } = new List<Guid>();
    }
    public class LocThongTinTongQuanDto
    {
        public string LoaiThongKe { get; set; }
        public string ThoiGian { get; set; }
    }
    public class DanhMucThongKeTheoSanPham
    {
        public tbSanPham SanPham { get; set; }
        public long DoanhThu { get; set; }
    }
    public class DanhMucThongKeTheoNVKD
    {
        public tbNguoiDung NguoiDung { get; set; }
        public tbNguoiDung_DoanhThu DoanhThu { get; set; }
    }
}