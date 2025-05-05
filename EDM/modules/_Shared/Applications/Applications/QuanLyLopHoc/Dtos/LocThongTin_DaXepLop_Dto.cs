using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Dtos
{
    public class LocThongTin_DaXepLop_Dto
    {
        public string ThoiGian { get; set; }
        public string TenLopHoc { get; set; }
        public Guid? IdTenLopHoc { get; set; }
        public Guid? IdLoaiSanPham { get; set; }
        public Guid? IdSanPham { get; set; }
        public Guid? IdGiaoVien { get; set; }
        public Guid? IdKhachHang { get; set; }
        public Guid? IdTrangThaiHoc{ get; set; }
    }
}