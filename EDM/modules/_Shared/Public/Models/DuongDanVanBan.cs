using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Models
{
    public class DuongDanVanBan
    {
        public string LoaiTep { get; set; }
        public string TenTep_KHONGDAU_KHONGLOAI { get; set; }
        public string TenTep_CHUYENDOI { get; set; }

        public string DuongDanThuMuc_BANDAU { get; set; }
        public string DuongDanThuMuc_BANDAU_SERVER { get; set; }
        public string DuongDanTep_BANDAU { get; set; }
        public string DuongDanTep_BANDAU_SERVER { get; set; }

        public string DuongDanThuMuc_CHUYENDOI { get; set; }
        public string DuongDanThuMuc_CHUYENDOI_SERVER { get; set; }
        public string DuongDanTep_CHUYENDOI { get; set; }
        public string DuongDanTep_CHUYENDOI_SERVER { get; set; }

        public string DuongDanThuMuc_PHIEUMUON { get; set; }
        public string DuongDanThuMuc_PHIEUMUON_SERVER { get; set; }
        public string DuongDanTep_PHIEUMUON { get; set; }
        public string DuongDanTep_PHIEUMUON_SERVER { get; set; }

        public string DuongDanThuMuc_MAUCHUNGTHUC { get; set; }
        public string DuongDanThuMuc_MAUCHUNGTHUC_SERVER { get; set; }
        public string DuongDanTep_MAUCHUNGTHUC { get; set; }
        public string DuongDanTep_MAUCHUNGTHUC_SERVER { get; set; }

        public string DuongDanTep_TRANGCHUNGTHUC { get; set; }
        public string DuongDanTep_TRANGCHUNGTHUC_SERVER { get; set; }
    }
}