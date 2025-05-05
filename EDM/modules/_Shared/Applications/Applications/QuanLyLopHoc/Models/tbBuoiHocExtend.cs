using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Models
{
    public class tbLopHoc_BuoiHocExtend
    {
        public tbLopHoc_BuoiHoc BuoiHoc { get; set; } = new tbLopHoc_BuoiHoc();
        public List<tbLopHoc_BuoiHoc_HinhAnh> HinhAnhBuoiHocs { get; set; } = new List<tbLopHoc_BuoiHoc_HinhAnh>();
        public List<tbKhachHang> KhachHangs { get; set; } = new List<tbKhachHang>();
        public List<tbNguoiDung> GiaoViens { get; set; } = new List<tbNguoiDung>();
    }
}