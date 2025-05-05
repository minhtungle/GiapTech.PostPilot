using DocumentFormation.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Models
{
    public class ReportM
    {
    }
    public class ThongTinTimKiemM : tbHoSoExtend
    {
        public List<int> IdDanhMucHoSos { get; set; } = new List<int>();
        public List<int> IdViTriLuuTrus { get; set; } = new List<int>();
        public List<int> IdCoCauToChucs { get; set; } = new List<int>();
        public tbNguoiDung _NguoiTao { get; set; } = new tbNguoiDung();
        public tbDonViSuDung _DonViSuDung { get; set; } = new tbDonViSuDung();
    }
    public class tbDanhMucHoSoExtend
    {
        public tbDanhMucHoSo DanhMucHoSo { get; set; } = new tbDanhMucHoSo();
        public List<tbHoSoExtend> HoSos { get; set; } = new List<tbHoSoExtend>();
    }
    public class DuLieuSoM
    {
        public int soLuong_DuLieuSo_BangBieu { get; set; } = 0;
        public int soLuong_DuLieuSo_VanBan { get; set; } = 0;
    }
    public class KetQuaTimKiemM
    {
        public List<tbHoSoExtend> HoSos { get; set; } = new List<tbHoSoExtend>();
        public List<tbDanhMucHoSo> DanhMucHoSos { get; set; } = new List<tbDanhMucHoSo>();
        public List<tbViTriLuuTru> ViTriLuuTrus { get; set; } = new List<tbViTriLuuTru>();
        public List<tbCoCauToChuc> CoCauToChucs { get; set; } = new List<tbCoCauToChuc>();
    }
}