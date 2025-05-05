using EDM_DB;
using Public.Dtos;
using Public.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TaiLieu_DB;

namespace Public.Controllers {
    public class StaticArgController : Controller {
        protected EDM_DBEntities db;
        protected tailieu_dbEntities tailieu_db;
        protected Permission per {
            get {
                return Session["Permission"] as Permission ?? new Permission();
            }
            set {
                Session["Permission"] = value;
            }
        }
        public StaticArgController() {
            db = new EDM_DBEntities();
            tailieu_db = new tailieu_dbEntities();
        }
        public DuongDanVanBan LayDuongDanTep(string duongDanThuMucGoc = "", string tenTep_BANDAU = "", string maXacThuc = "") {
            /**
             * Mô tả sơ đồ tệp
             * Ghi chú: 
             *  - ext: loại tệp (doc|docx|xls|xlsx|png|jpg|jpeg|mp4) không bao gồm pdf
             * ---------------------------------------------
             * (1) |(2)         |(3)      |(4)         |(5)
             * HoSo|
             *     |VanBan[.ext]|VanBan.ext
             *                   CHUYENDOI|[.docx]     |VanBan.pdf
             *                            |[.xlsx]     |VanBan.pdf
             *                            ...
             *                   PHIEUMUON|(Mã phiếu 1)|VanBan.pdf
             *                                         |CHUNGTHUC |chungthuc.png
             *                            |(Mã phiếu 2)|VanBan.pdf
             *                                         |CHUNGTHUC |chungthuc.png
             *                            ...
             */
            string loaiTep = Path.GetExtension(tenTep_BANDAU);
            // Đổi tên: "Tên hồ sơ & Tên hồ sơ" => "Ten-ho-so-Ten-ho-so"
            string tenTep_KHONGDAU_KHONGLOAI = Regex.Replace(Public.Handle.ConvertToUnSign(s: Path.GetFileNameWithoutExtension(tenTep_BANDAU), khoangCach: " "), @"[^\w\d]+", "-");
            string tenTep_CHUYENDOI = string.Format("{0}.pdf", tenTep_KHONGDAU_KHONGLOAI);

            string duongDanThuMuc_BANDAU = string.Format("{0}/{1}[{2}]", duongDanThuMucGoc, tenTep_KHONGDAU_KHONGLOAI, loaiTep); // (2)
            string duongDanThuMuc_BANDAU_SERVER = Request.MapPath(duongDanThuMuc_BANDAU); // (2)
            string duongDanVanBan_BANDAU = string.Format("{0}/{1}{2}", duongDanThuMuc_BANDAU, tenTep_KHONGDAU_KHONGLOAI, loaiTep);  // (3)
            string duongDanVanBan_BANDAU_SERVER = Request.MapPath(duongDanVanBan_BANDAU);  // (3)

            string duongDanThuMuc_CHUYENDOI = string.Format("{0}/CHUYENDOI/[.pdf]", duongDanThuMuc_BANDAU); // (4)
            string duongDanThuMuc_CHUYENDOI_SERVER = Request.MapPath(duongDanThuMuc_CHUYENDOI); // (4)
            string duongDanVanBan_CHUYENDOI = string.Format("{0}/{1}", duongDanThuMuc_CHUYENDOI, tenTep_CHUYENDOI); // (5)
            string duongDanVanBan_CHUYENDOI_SERVER = Request.MapPath(duongDanVanBan_CHUYENDOI); // (5)

            string duongDanThuMuc_PHIEUMUON = string.Format("{0}/PHIEUMUON/{1}", duongDanThuMuc_BANDAU, maXacThuc);
            string duongDanThuMuc_PHIEUMUON_SERVER = Request.MapPath(duongDanThuMuc_PHIEUMUON);
            string duongDanVanBan_PHIEUMUON = string.Format("{0}/{1}{2}", duongDanThuMuc_PHIEUMUON, tenTep_KHONGDAU_KHONGLOAI, loaiTep);
            string duongDanVanBan_PHIEUMUON_SERVER = Request.MapPath(duongDanVanBan_PHIEUMUON);

            string duongDanThuMuc_MAUCHUNGTHUC = string.Format("{0}/PHIEUMUON/{1}/MAUCHUNGTHUC", duongDanThuMuc_BANDAU, maXacThuc);
            string duongDanThuMuc_MAUCHUNGTHUC_SERVER = Request.MapPath(duongDanThuMuc_MAUCHUNGTHUC);
            string duongDanVanBan_MAUCHUNGTHUC = string.Format("{0}/mau-chung-thuc.png", duongDanThuMuc_MAUCHUNGTHUC);
            string duongDanVanBan_MAUCHUNGTHUC_SERVER = Request.MapPath(duongDanVanBan_MAUCHUNGTHUC);

            string duongDanVanBan_TRANGCHUNGTHUC = string.Format("{0}/trang-chung-thuc.pdf", duongDanThuMuc_MAUCHUNGTHUC);
            string duongDanVanBan_TRANGCHUNGTHUC_SERVER = Request.MapPath(duongDanVanBan_TRANGCHUNGTHUC);

            if (loaiTep.Contains("xls") || loaiTep.Contains("doc")) {
                // Lấy đường dẫn tới thư mục văn bản đã chuyển đổi thành PDF
                duongDanVanBan_PHIEUMUON = string.Format("{0}/{1}", duongDanThuMuc_PHIEUMUON, tenTep_CHUYENDOI);
                duongDanVanBan_PHIEUMUON_SERVER = Request.MapPath(duongDanVanBan_PHIEUMUON);
            };
            return new DuongDanVanBan {
                LoaiTep = loaiTep,
                TenTep_KHONGDAU_KHONGLOAI = tenTep_KHONGDAU_KHONGLOAI,
                TenTep_CHUYENDOI = tenTep_CHUYENDOI,

                DuongDanThuMuc_BANDAU = duongDanThuMuc_BANDAU,
                DuongDanThuMuc_BANDAU_SERVER = duongDanThuMuc_BANDAU_SERVER,
                DuongDanTep_BANDAU = duongDanVanBan_BANDAU,
                DuongDanTep_BANDAU_SERVER = duongDanVanBan_BANDAU_SERVER,

                DuongDanThuMuc_CHUYENDOI = duongDanThuMuc_CHUYENDOI,
                DuongDanThuMuc_CHUYENDOI_SERVER = duongDanThuMuc_CHUYENDOI_SERVER,
                DuongDanTep_CHUYENDOI = duongDanVanBan_CHUYENDOI,
                DuongDanTep_CHUYENDOI_SERVER = duongDanVanBan_CHUYENDOI_SERVER,

                DuongDanThuMuc_PHIEUMUON = duongDanThuMuc_PHIEUMUON,
                DuongDanThuMuc_PHIEUMUON_SERVER = duongDanThuMuc_PHIEUMUON_SERVER,
                DuongDanTep_PHIEUMUON = duongDanVanBan_PHIEUMUON,
                DuongDanTep_PHIEUMUON_SERVER = duongDanVanBan_PHIEUMUON_SERVER,

                DuongDanThuMuc_MAUCHUNGTHUC = duongDanThuMuc_MAUCHUNGTHUC,
                DuongDanThuMuc_MAUCHUNGTHUC_SERVER = duongDanThuMuc_MAUCHUNGTHUC_SERVER,
                DuongDanTep_MAUCHUNGTHUC = duongDanVanBan_MAUCHUNGTHUC,
                DuongDanTep_MAUCHUNGTHUC_SERVER = duongDanVanBan_MAUCHUNGTHUC_SERVER,

                DuongDanTep_TRANGCHUNGTHUC = duongDanVanBan_TRANGCHUNGTHUC,
                DuongDanTep_TRANGCHUNGTHUC_SERVER = duongDanVanBan_TRANGCHUNGTHUC_SERVER
            };
        }
        public ActionResult XemPDF(string duongDanTep, bool quyenTaiXuong = false)
        {
            var model = new XemPDF_Output_Dto
            {
                DuongDanTep = duongDanTep,
                QuyenTaiXuong = quyenTaiXuong
            };

            return View("~/Views/_Shared/_lib/pdf_viewer.cshtml", model);
        }
    }
}