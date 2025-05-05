using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLySanPham.Models
{
    public class tbSanPham_LichSuExtend
    {
        public tbSanPham_LichSu LichSu { get; set; }
        public tbNguoiDung ThongTinNguoiTao { get; set; } = new tbNguoiDung();
    }
}