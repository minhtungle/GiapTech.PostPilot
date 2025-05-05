using Aspose.Cells;
using Aspose.Words;
using DocumentFormation.Models;
using EDM_DB;
using Public.Controllers;
using Public.Models;
using StorageLocation.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DocumentStorage.Controllers {
    public class DocumentStorageController : RouteConfigController {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_DocumentManage/DocumentStorage";
        private List<tbBieuMauExtend> BIEUMAUs {
            get {
                return Session["BIEUMAUs"] as List<tbBieuMauExtend> ?? new List<tbBieuMauExtend>();
            }
            set {
                Session["BIEUMAUs"] = value;
            }
        }
        private static StorageLocationController storageLocationController = new StorageLocationController();
        #endregion
        public ActionResult Index() {
            return View($"{VIEW_PATH}/documentstorage.cshtml");
        }
        #region Hồ sơ
        [HttpGet]
        public JsonResult get_HoSos(int idViTriLuuTru = 0) {
            List<tbHoSoExtend> hoSos = db.Database.SqlQuery<tbHoSoExtend>($@"select * from tbHoSo where IdViTriLuuTru = {idViTriLuuTru} and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai = 2 order by NgayTao desc").ToList();
            foreach (tbHoSoExtend hoSo in hoSos) {
                hoSo.VanBans = db.Database.SqlQuery<tbHoSo_VanBanExtend>($@"
                select * from tbHoSo_VanBan 
                where IdHoSo = {hoSo.IdHoSo} and TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}").ToList() ?? new List<tbHoSo_VanBanExtend>();
                hoSo.ViTriLuuTru = db.tbViTriLuuTrus.Find(hoSo.IdViTriLuuTru) ?? new tbViTriLuuTru();
                hoSo.PhongLuuTru = db.tbDonViSuDung_PhongLuuTru.Find(hoSo.IdPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                // Lấy danh mục từ danh sách danh mục đã xử lý tên
                hoSo.DanhMucHoSo = db.tbDanhMucHoSoes.Find(hoSo.IdDanhMucHoSo) ?? new tbDanhMucHoSo();
                hoSo.CheDoSuDung = db.default_tbCheDoSuDung.Find(hoSo.IdCheDoSuDung) ?? new default_tbCheDoSuDung();
                hoSo.ThongTinNguoiTao = db.tbNguoiDungs.Find(hoSo.NguoiTao) ?? new tbNguoiDung();
                hoSo.DonViSuDung = db.tbDonViSuDungs.Find(hoSo.MaDonViSuDung) ?? new tbDonViSuDung();
            };
            return Json(new {
                data = hoSos
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult huyNopLuu() {
            string status = "success";
            string mess = "Hủy lưu hồ sơ thành công";
            using (var scope = db.Database.BeginTransaction()) {
                try {
                    string str_ids = Request.Form["str_ids"];
                    List<int> ids = str_ids.Split(',').Select(Int32.Parse).ToList();
                    void _huyNopLuu(tbHoSo hoSo) {
                        hoSo.TrangThai = 1;
                        hoSo.NguoiSua = per.NguoiDung.IdNguoiDung;
                        hoSo.NgaySua = DateTime.Now;
                    }
                    foreach (int id in ids) {
                        tbHoSo hs = db.tbHoSoes.Find(id);
                        _huyNopLuu(hs);
                    };
                    db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap {
                        TenModule = "Lưu trữ hồ sơ",
                        ThaoTac = "Hủy nộp lưu",
                        NoiDungChiTiet = "Hủy nộp lưu hồ sơ",

                        NgayTao = DateTime.Now,
                        IdNguoiDung = per.NguoiDung.IdNguoiDung,
                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                    });
                    db.SaveChanges();
                    scope.Commit();
                } catch (Exception ex) {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult get_ViTriLuuTrus() {
            List<Tree<tbViTriLuuTru>> viTriLuuTrus_Tree = storageLocationController.get_ViTriLuuTrus_Tree(idViTri: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            return Json(viTriLuuTrus_Tree, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Văn bản
        public ActionResult VanBan(int idHoSo = 0) {
            // Danh sách các biểu mẫu
            BIEUMAUs = db.Database.SqlQuery<tbBieuMauExtend>($@"
            select * from tbBieuMau where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
            ").ToList() ?? new List<tbBieuMauExtend>();
            foreach (tbBieuMauExtend bieuMau in BIEUMAUs) {
                bieuMau.TruongDuLieus = db.Database.SqlQuery<tbBieuMau_TruongDuLieuExtend>($@"
                select * from tbBieuMau_TruongDuLieu where IdBieuMau = {bieuMau.IdBieuMau} and TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                ").ToList();
            }
            tbHoSoExtend hoSo = get_HoSo(idHoSo: idHoSo, loai: "all");
            ViewBag.hoSo = hoSo;
            return View($"{VIEW_PATH}/documentstorage-vanban/vanban.cshtml");
        }
        public tbHoSoExtend get_HoSo(int idHoSo, string loai, string idVanBans = "") {
            // Thông tin hồ sơ đang chọn
            tbHoSoExtend hoSo = db.Database.SqlQuery<tbHoSoExtend>($@"
            select * from tbHoSo where IdHoSo = {idHoSo}").FirstOrDefault() ?? new tbHoSoExtend();
            if (hoSo.IdHoSo != 0) {
                // Thông tin vị trí lưu trữ - danh mục hồ sơ - phông lưu trữ - chế độ sử dụng
                hoSo.ViTriLuuTru = db.tbViTriLuuTrus.Find(hoSo.IdViTriLuuTru) ?? new tbViTriLuuTru();
                hoSo.DanhMucHoSo = db.tbDanhMucHoSoes.Find(hoSo.IdDanhMucHoSo) ?? new tbDanhMucHoSo();
                hoSo.PhongLuuTru = db.tbDonViSuDung_PhongLuuTru.Find(hoSo.IdPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                hoSo.CheDoSuDung = db.default_tbCheDoSuDung.Find(hoSo.IdCheDoSuDung) ?? new default_tbCheDoSuDung();
                // Thông tin văn bản của hồ sơ
                string get_VanBanSQL = "";
                if (loai == "all") {
                    get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdHoSo = {idHoSo}";
                } else if (idVanBans != "") {
                    get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdVanBan in ({idVanBans})";
                };
                List<tbHoSo_VanBanExtend> vanBans = db.Database.SqlQuery<tbHoSo_VanBanExtend>(get_VanBanSQL).ToList() ?? new List<tbHoSo_VanBanExtend>();
                // Lấy thông tin biểu mẫu của văn bản
                foreach (tbHoSo_VanBanExtend vanBan in vanBans) {
                    string get_BieuMauSQL = $@"select * from tbBieuMau where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdBieuMau = {vanBan.IdBieuMau}";
                    vanBan.BieuMau = db.Database.SqlQuery<tbBieuMauExtend>(get_BieuMauSQL).FirstOrDefault() ?? new tbBieuMauExtend();
                    #region Lấy đường dẫn văn bản
                    string tenVanBan_BANDAU = string.Format("{0}{1}", Path.GetFileName(vanBan.TenVanBan), vanBan.Loai);
                    var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU);

                    vanBan.DuongDan = duongDanVanBan.duongDanVanBan_BANDAU;
                    if (vanBan.Loai.Contains("xls") || vanBan.Loai.Contains("doc")) {
                        vanBan.DuongDan = duongDanVanBan.duongDanVanBan_CHUYENDOI;
                    };
                    #endregion
                };
                hoSo.VanBans = vanBans;
            }
            return hoSo;
        }
        public ActionResult displayModal_Read_VanBan(int idHoSo, string loai, int idVanBan) {
            /**
             * Tìm văn bản
             */
            tbHoSo_VanBanExtend vanBan = get_HoSo(idHoSo: idHoSo, loai: "single", idVanBans: idVanBan.ToString()).VanBans.FirstOrDefault() ?? new tbHoSo_VanBanExtend();
            // Lấy tên miền
            Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
            string hostName = uri.GetLeftPart(UriPartial.Authority);
            ViewBag.hostName = hostName;
            ViewBag.vanBan = vanBan;
            ViewBag.loai = loai;
            if (vanBan.Loai.Contains("pdf") || vanBan.Loai.Contains("xls") || vanBan.Loai.Contains("doc"))
                ViewBag.iframeHtml = $"<iframe src=\"{hostName}/Search/xemPDF?idHoSo={idHoSo}&idVanBan={idVanBan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh\"></iframe>";
            else if (vanBan.Loai.Contains("mp4"))
                ViewBag.iframeHtml = $"<video src=\"{vanBan.DuongDan}\" controls style=\"width: 100%; height: 70vh; border: 1px solid var(--bs-body-color)\"></video>";
            else
                ViewBag.iframeHtml = $"<iframe src=\"{vanBan.DuongDan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh; border: 1px solid var(--bs-body-color)\"></iframe>";
            return PartialView($"{VIEW_PATH}/documentstorage-vanban/vanban.detail.cshtml");
        }
        public ActionResult get_DuLieuSos(int idVanBan = 0, int idBieuMau = 0) {
            tbBieuMau bieuMau = db.tbBieuMaus.Find(idBieuMau);
            /**
             * Tìm trường dữ liệu của biểu mẫu
             */
            List<tbBieuMau_TruongDuLieuExtend> truongDuLieus = db.Database.SqlQuery<tbBieuMau_TruongDuLieuExtend>(
                $@"select truongdulieu.* from tbBieuMau_TruongDuLieu truongdulieu  
                where truongdulieu.IdBieuMau = {idBieuMau} and truongdulieu.TrangThai = 1 and truongdulieu.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}"
                ).ToList() ?? new List<tbBieuMau_TruongDuLieuExtend>();
            /**
             * Tìm dữ liệu số từng trường thuộc văn bản và sắp xếp theo nhóm
             * |---------------------------------------|
             * |   TRUONG1   |   TRUONG2  |   TRUONG3  |
             * |---------------------------------------|
             * |  DULIEUSO1  |  DULIEUSO1 |  DULIEUSO1 |
             * |  DULIEUSO2  |  DULIEUSO2 |  DULIEUSO2 |
             * |  DULIEUSO3  |  DULIEUSO3 |  DULIEUSO3 |
             * |  DULIEUSO4  |  DULIEUSO4 |  DULIEUSO4 |
             * |---------------------------------------|
             */
            foreach (tbBieuMau_TruongDuLieuExtend truongDuLieu in truongDuLieus) {
                truongDuLieu.DuLieuSos = db.tbHoSo_VanBan_DuLieuSo.Where(x => x.IdTruongDuLieu == truongDuLieu.IdTruongDuLieu && x.IdVanBan == idVanBan && x.TrangThai == 1 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).OrderBy(x => x.Nhom).ToList() ?? new List<tbHoSo_VanBan_DuLieuSo>();
            };

            ViewBag.truongDuLieus = truongDuLieus;
            ViewBag.bieuMau = bieuMau;
            return PartialView($"{VIEW_PATH}/documentstorage-vanban/vanban.detail.truongdulieu.cshtml");
        }
        #endregion
    }
}