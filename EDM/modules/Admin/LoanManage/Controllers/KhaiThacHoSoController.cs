using DocumentFormation.Models;
using EDM_DB;
using LoanManage.Models;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoanManage.Controllers {
    public class KhaiThacHoSoController : StaticArgController {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/LoanManage";
        #endregion
        public KhaiThacHoSoController() {
        }
        public void LayQuyen() {
            string currentDomain = Request.Url.Host.ToLower();
            PermissionM per = new PermissionM {
                DonViSuDung = db.tbDonViSuDungs.FirstOrDefault(k => k.TenMien == currentDomain && k.TrangThai == 1) ?? new tbDonViSuDung(),
                Role = "USER"
            };
            Session["Permission"] = per; // Phải set như này thì từ sau mới sử dụng được session
        }
        public ActionResult Index(string code = "") {
            LayQuyen();
            // Lấy tên miền
            {
                Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
                string hostName = uri.GetLeftPart(UriPartial.Authority);
                ViewBag.hostName = hostName;
            };
            // Xác thực
            {
                string[] decodeSplit = Public.Handle.DecodeFrom64(code).Split('-');
                string maXacThuc = decodeSplit[0];
                string idPhieuMuon = decodeSplit[1];
                tbPhieuMuonExtend phieuMuon = get_PhieuMuons(idPhieuMuon: int.Parse(idPhieuMuon)).FirstOrDefault();
                // Đã xóa hoặc hủy duyệt thì không truy cập được
                if (phieuMuon.TrangThai == 0 || phieuMuon.TrangThai == 3) {
                    return null;
                };
                string trangThaiMuon = "conhan";
                int thoiHanMuon = 0;
                if (phieuMuon == null) {
                    trangThaiMuon = "khongtontai";
                } else {
                    TimeSpan khoangCach = phieuMuon.NgayHenTra.Value - DateTime.Now;
                    thoiHanMuon = khoangCach.Days;
                    if (DateTime.Now >= phieuMuon.NgayHenTra.Value.AddDays(1)) {
                        thoiHanMuon = 0;
                        trangThaiMuon = "hethan";
                    };
                };
                ViewBag.phieuMuon = phieuMuon;
                ViewBag.thoiHanMuon = thoiHanMuon;
                ViewBag.trangThaiMuon = trangThaiMuon;
            };
            return View($@"{VIEW_PATH}/khaithachoso/khaithachoso.cshtml");
        }
        public List<tbPhieuMuonExtend> get_PhieuMuons(int idPhieuMuon) {
            List<tbPhieuMuonExtend> phieuMuons = new List<tbPhieuMuonExtend>();
            string get_PhieuMuonsSQL = $@"select * from tbPhieuMuon where TrangThai <> 0 and IdPhieuMuon = {idPhieuMuon} order by NgayTao desc";
            phieuMuons = db.Database.SqlQuery<tbPhieuMuonExtend>(get_PhieuMuonsSQL).ToList() ?? new List<tbPhieuMuonExtend>();
            foreach (tbPhieuMuonExtend phieuMuon in phieuMuons) {
                // Lấy hình thức mượn
                phieuMuon.HinhThucMuon = db.default_tbHinhThucMuon.FirstOrDefault(x => x.IdHinhThucMuon == phieuMuon.IdHinhThucMuon && x.TrangThai == 1) ?? new default_tbHinhThucMuon();
                // Lấy danh sách văn bản
                phieuMuon.PhieuMuon_VanBans = get_PhieuMuon_VanBans(idPhieuMuon: phieuMuon.IdPhieuMuon);
            };
            return phieuMuons;
        }
        public List<tbPhieuMuon_VanBanExtend> get_PhieuMuon_VanBans(int idPhieuMuon) {
            string get_PhieuMuon_VanBansSQL = $@"select * from tbPhieuMuon_VanBan where TrangThai <> 0 and IdPhieuMuon = {idPhieuMuon}";
            List<tbPhieuMuon_VanBanExtend> phieuMuon_VanBans = db.Database.SqlQuery<tbPhieuMuon_VanBanExtend>(get_PhieuMuon_VanBansSQL).ToList() ?? new List<tbPhieuMuon_VanBanExtend>();
            foreach (tbPhieuMuon_VanBanExtend vanBan in phieuMuon_VanBans) {
                // Tìm văn bản
                string get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai = 1 and IdVanBan in ({vanBan.IdVanBan})";
                vanBan.VanBan = db.Database.SqlQuery<tbHoSo_VanBanExtend>(get_VanBanSQL).FirstOrDefault() ?? new tbHoSo_VanBanExtend();
                vanBan.HoSo = db.tbHoSoes.Find(vanBan.VanBan.IdHoSo);
            };
            return phieuMuon_VanBans;
        }
        public ActionResult xemPDF(int idPhieuMuon = 0, int idVanBan = 0) {
            tbPhieuMuonExtend phieuMuon = get_PhieuMuons(idPhieuMuon: idPhieuMuon).FirstOrDefault();
            if (phieuMuon == null) return View();
            // Trả view xem pdf
            ViewBag.DuongDanFile = phieuMuon.PhieuMuon_VanBans.FirstOrDefault(x=> x.IdVanBan == idVanBan).DuongDanFile_DaXuLy;
            // Chỉ khi đăng ký mượn và không phải mượn đọc thì mới cho tải
            bool quyenTaiXuong = false;
            if (phieuMuon.HinhThucMuon.IdHinhThucMuon == 2 || phieuMuon.HinhThucMuon.IdHinhThucMuon == 3) {
                quyenTaiXuong = true;
            };
            ViewBag.QuyenTaiXuong = quyenTaiXuong;
            return View("~/Views/_Shared/_lib/pdf_viewer.cshtml");
        }
    }
}