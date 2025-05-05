using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Models
{
    public class tbLopHocExtend
    {
        public tbLopHoc LopHoc { get; set; } = new tbLopHoc();
        public List<tbDonHangExtend> DonHangs { get; set; } = new List<tbDonHangExtend>();
        public List<tbLopHoc_BuoiHocExtend> BuoiHocs { get; set; } = new List<tbLopHoc_BuoiHocExtend>();
        public List<tbLopHoc_TaiLieu> TaiLieus { get; set; } = new List<tbLopHoc_TaiLieu>();
        public List<tbKhachHang> KhachHangs { get; set; } = new List<tbKhachHang>();
        public List<tbNguoiDung> GiaoViens { get; set; } = new List<tbNguoiDung>();
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
    }
}