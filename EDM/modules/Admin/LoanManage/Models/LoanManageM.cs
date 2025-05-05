using DocumentFormation.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoanManage.Models {
    public class LoanManageM {
    }
    public class tbPhieuMuonExtend : tbPhieuMuon {
        public default_tbHinhThucMuon HinhThucMuon { get; set; } = new default_tbHinhThucMuon();
        public List<tbPhieuMuon_VanBanExtend> PhieuMuon_VanBans { get; set; } = new List<tbPhieuMuon_VanBanExtend>();
    }
    public class tbPhieuMuon_VanBanExtend : tbPhieuMuon_VanBan {
        public tbHoSo HoSo { get; set; } = new tbHoSo();
        public tbHoSo_VanBanExtend VanBan { get; set; } = new tbHoSo_VanBanExtend();
    }
    public class PhieuMuonMailM {
        public string TrangThai { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public string DuongDanKhaiThac { get; set; } = string.Empty;
        public tbPhieuMuonExtend PhieuMuon { get; set; } = new tbPhieuMuonExtend();
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
    }
    public class ThongBaoCoPhieuMuonMailM
    {
        public tbNguoiDung NguoiDung { get; set; } = new tbNguoiDung();
        public string GhiChu { get; set; } = string.Empty;
        public tbPhieuMuonExtend PhieuMuon { get; set; } = new tbPhieuMuonExtend();
        public tbDonViSuDung DonViSuDung { get; set; } = new tbDonViSuDung();
    }
}