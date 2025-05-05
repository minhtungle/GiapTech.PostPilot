using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyLopHoc.Dtos
{
    public class LocThongTin_ChoXepLop_Dto
    {
        public string ThoiGian { get; set; }
        public List<Guid> IdTrangThaiHoc { get; set; }
        public Guid? IdLoaiSanPham { get; set; }
        public Guid? IdSanPham { get; set; }
        public Guid? IdKhachHang { get; set; }
        public Guid? IdTrinhDoDauVao { get; set; }
        public Guid? IdTrinhDoDauRa { get; set; }
    }
}