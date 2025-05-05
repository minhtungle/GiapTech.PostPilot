using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDoanhThu.Dtos
{
    public class ThongKeTongQuanOutput_Dto
    {
        public int Thang {  get; set; }
        public int Nam { get; set; }

        #region Doanh thu
        public long DoanhThuMucTieu { get; set; }
        public long DoanhThuThucTe { get; set; }
        public long DoanhThuTiengAnh { get; set; }
        public long DoanhThuTiengDuc { get; set; }

        public decimal PhanTramHoanThanhMucTieu {  get; set; }
        #endregion

        #region Nhân viên kinh doanh
        public List<DoanhThuNVKD_Dto> NVKD_TopDoanhThu { get; set; }
        public List<DoanhThuNVKD_Dto> NVKD_KhongCoDoanhThu { get; set; }
        #endregion

        #region Khách hàng & Đơn hàng
        public int SoLuongDonHang { get; set; }
        public int SoLuongThanhToan { get; set; }
        public int SoLuongKhachHangMoi {  get; set; }
        public int SoLuongKhachHangUpSell { get; set; }
        public int SoLuongKhachHangHocThu { get; set; }
        public int SoLuongKhachHangHocChinh { get; set; }
        public int SoLuongKhachHangHocThuSangHocChinh { get; set; }
        public int SoLuongKhachHangChuaDangKyHoc { get; set; }
        public int SoLuongKhachHangDangHoc { get; set; }
        public int SoLuongKhachHangDaHocXong { get; set; }
        public int SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang { get; set; }
        public int SoLuongKhachHangThanhToanTronGoi { get; set; }
        
        public decimal PhanTramThanhToanNhieuLan { get; set; }
        public decimal PhanTramUpSellChung { get; set; }
        public decimal PhanTramKhachHang_HocThuSangHocChinh { get; set; }
        #endregion

        #region Sản phẩm
        public int SoLuongSanPham_TiengAnh { get; set; }
        public int SoLuongSanPham_TiengDuc { get; set; }
        public List<SanPhamDaBan_Dto> SanPham_BanChay { get; set; }
        public List<SanPhamDaBan_Dto> SanPham_TopDoanhThu { get; set; }
        #endregion

    }
    public class DoanhThuNVKD_Dto
    {
        public tbNguoiDung NguoiDung {  get; set; }
        public long TongDoanhThu { get; set; }
    }

    public class SanPhamDaBan_Dto
    {
        public tbSanPham SanPham { get; set; }
        public int SoLuongDaBan { get; set; }
        public long TongDoanhThu { get; set ; }
    }
}