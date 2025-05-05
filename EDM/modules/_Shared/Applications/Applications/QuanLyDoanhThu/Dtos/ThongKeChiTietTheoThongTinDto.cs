using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDoanhThu.Dtos
{
    public class ThongKeChiTietTheoThongTinDto
    {
        public string LoaiThongKe { get; set; } = "NVKD";
        public Guid IdThongTin { get; set; }
        public string ThoiGian { get; set; } = DateTime.Now.ToString("MM/yyyy");
    }
    public class LayThongTinThongKeTheoNVKD_Output_Dto
    {
        public tbNguoiDung NhanVienKinhDoanh { get; set; }
        public List<tbKhachHang_DonHang_ThanhToan> ThanhToans { get; set; }
    }
    public class LayThongTinThongKeTheoSanPham_Output_Dto
    {
        public tbNguoiDung NhanVienKinhDoanh { get; set; }
        public List<tbKhachHang_DonHang_ThanhToan> ThanhToans { get; set; }
    }
}