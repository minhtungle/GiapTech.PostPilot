using Applications.QuanLyTaiLieu.Dtos;
using Applications.QuanLyTaiLieu.Models;
using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Public.Controllers;
using Public.Dtos;
using Public.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaiLieu_DB;

namespace QuanLyTaiLieu.Controllers
{
    public class QuanLyTaiLieuController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyTaiLieu";
        IsoDateTimeConverter DATETIMECONVERTER = new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" };
        private List<ThaoTac> THAOTACs
        {
            get
            {
                return Session["THAOTACs"] as List<ThaoTac> ?? new List<ThaoTac>();
            }
            set
            {
                Session["THAOTACs"] = value;
            }
        }

        public QuanLyTaiLieuController()
        {

        }
        #endregion
        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyTaiLieu").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #endregion

            THAOTACs = thaoTacs;

            return View($"{VIEW_PATH}/quanlytailieu.cshtml");
        }
        [HttpGet]
        public ActionResult getList_TaiLieu()
        {
            List<tbTaiLieuExtend> taiLieus = get_TaiLieus(loai: "all");
            return PartialView($"{VIEW_PATH}/tailieu/tailieu-getList.cshtml", taiLieus);
        }
        [HttpGet]
        public ActionResult getList_LoaiTaiLieu()
        {
            List<tbLoaiTaiLieu> loaiTaiLieus = get_LoaiTaiLieu(loai: "all");
            return PartialView($"{VIEW_PATH}/nhandan/nhandan-getList.cshtml", loaiTaiLieus);
        }
        public List<tbTaiLieuExtend> get_TaiLieus(string loai = "all", List<Guid> idTaiLieus = null, LocThongTinDto locThongTin = null)
        {
            // Chỉ hiển thị bản ghi có quyền truy cập
            var taiLieuRepo = tailieu_db.tbTaiLieux.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();
            var nhanDanRepo = tailieu_db.tbLoaiTaiLieux.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();

            var taiLieus = (from tl in taiLieuRepo
                                //join nd in nhanDanRepo on tl.IdNhanDan equals nd.IdNhanDan
                            where (loai != "single" || idTaiLieus.Contains(tl.IdTaiLieu))
                            orderby tl.NgayTao descending, tl.NgaySua descending
                            select new tbTaiLieuExtend
                            {
                                TaiLieu = tl,
                                //NhanDan = nd
                            }).ToList();

            return taiLieus;
        }
        public List<tbLoaiTaiLieu> get_LoaiTaiLieu(string loai = "all", List<Guid> idNhanDans = null, LocThongTinDto locThongTin = null)
        {
            // Chỉ hiển thị bản ghi có quyền truy cập
            var loaiTaiLieuRepo = tailieu_db.tbLoaiTaiLieux
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();

            var loaiTaiLieus = (from nd in loaiTaiLieuRepo
                            where (loai != "single" || idNhanDans.Contains(nd.IdLoaiTaiLieu))
                            orderby nd.NgayTao descending, nd.NgaySua descending
                            select nd).ToList();

            return loaiTaiLieus;
        }
        [HttpPost]
        public ActionResult displayModal_XemChiTiet(Guid idTaiLieu)
        {
            var taiLieu = get_TaiLieus(loai: "single", idTaiLieus: new List<Guid> { idTaiLieu })?.FirstOrDefault();

            //Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
            //string hostName = uri.GetLeftPart(UriPartial.Authority);

            //if (taiLieu.TaiLieu.LoaiTep.Contains("pdf"))
            //    ViewBag.iframeHtml = $"<iframe src=\"{hostName}/XemPDF?duongDanTep={idHoSo}&idVanBan={idVanBan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh\"></iframe>";
            //else if (taiLieu.TaiLieu.LoaiTep.Contains("mp4"))
            //    ViewBag.iframeHtml = $"<video src=\"{taiLieu.DuongDan}\" controls style=\"width: 100%; height: 70vh; border: 1px solid var(--bs-body-color)\"></video>";
            //else
            //    ViewBag.iframeHtml = $"<iframe src=\"{taiLieu.DuongDan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh; border: 1px solid var(--bs-body-color)\"></iframe>";
            return PartialView($"{VIEW_PATH}/taiLieu/taiLieu-xemchitiet.cshtml", taiLieu);
        }
        //public ActionResult XemPDF(string duongDanTep, bool quyenTaiXuong = true)
        //{
        //    var model = new XemPDF_Output_Dto
        //    {
        //        DuongDanTep = duongDanTep,
        //        QuyenTaiXuong = quyenTaiXuong
        //    };

        //    return View("~/Views/_Shared/_lib/pdf_viewer.cshtml", model);
        //}
        [HttpPost]
        public ActionResult create_TaiLieu(HttpPostedFileBase[] files)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = tailieu_db.Database.BeginTransaction())
            {
                try
                {
                    if (files == null || files.Length == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        var fAnhDaiDien = Request.Files["anhdaidien"];
                        var fAnhMoTas = Request.Files["anhmota"];

                        foreach (HttpPostedFileBase f in files)
                        {
                            /**
                             * Mô tả sơ đồ tệp
                             * Ghi chú: 
                             *  - ext: loại tệp (doc|docx|xls|xlsx|png|jpg|jpeg|mp4) không bao gồm pdf
                             * ---------------------------------------------
                             * (1) |(2)   |(3)      |(4)         |(5)
                             * HoSo|
                             *     |VanBan|VanBan.ext
                             *            |CHUYENDOI|[.docx]     |VanBan.pdf
                             *                      |[.xlsx]     |VanBan.pdf
                             *                      ...
                             *            |PHIEUMUON|(Mã phiếu 1)|VanBan.pdf
                             *                      |(Mã phiếu 2)|VanBan.pdf
                             *                      ...
                             */
                            string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/TAILIEU", per.DonViSuDung.MaDonViSuDung);
                            string tenTaiLieu_BANDAU = Path.GetFileName(f.FileName);
                            var duongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMucGoc, tenTep_BANDAU: tenTaiLieu_BANDAU);
                            #region Nếu chưa có văn bản này thì tạo mới trong db
                            tbTaiLieu taiLieu_OLD = tailieu_db.tbTaiLieux.FirstOrDefault(x =>
                            x.TenTaiLieuGoc == duongDanTep.TenTep_KHONGDAU_KHONGLOAI &&
                            //x.TenVanBan_BanDau == Path.GetFileNameWithoutExtension(tenVanBan_BANDAU) &&
                            x.LoaiTep == duongDanTep.LoaiTep &&
                            x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                            if (taiLieu_OLD != null)
                            {
                                taiLieu_OLD.TrangThai = 1; // Khôi phục dữ liệu này nếu đang xóa
                            }
                            else
                            {
                                var taiLieu = new tbTaiLieu
                                {
                                    IdTaiLieu = Guid.NewGuid(),
                                    TenTaiLieuGoc = Path.GetFileNameWithoutExtension(tenTaiLieu_BANDAU),
                                    TenTaiLieuMoi = duongDanTep.TenTep_KHONGDAU_KHONGLOAI,
                                    LoaiTep = duongDanTep.LoaiTep,
                                    DuongDanVatLy= duongDanTep.DuongDanTep_BANDAU,
                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                tailieu_db.tbTaiLieux.Add(taiLieu);
                            }
                            ;
                            #endregion

                            #region Tạo và lưu
                            // Tạo thư mục
                            if (!System.IO.Directory.Exists(duongDanTep.DuongDanThuMuc_BANDAU_SERVER))
                                System.IO.Directory.CreateDirectory(duongDanTep.DuongDanThuMuc_BANDAU_SERVER);
                            // (Nếu có rồi thì xóa)
                            if (System.IO.File.Exists(duongDanTep.DuongDanTep_BANDAU_SERVER))
                                System.IO.File.Delete(duongDanTep.DuongDanTep_BANDAU_SERVER);
                            f.SaveAs(duongDanTep.DuongDanTep_BANDAU_SERVER);
                            #endregion
                        }
                        ;
                        tailieu_db.SaveChanges();
                        scope.Commit();
                    }
                    ;
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult delete_TaiLieu()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = tailieu_db.Database.BeginTransaction())
            {
                try
                {
                    var idTaiLieus = Request.Form["idTaiLieus"].Split(',').Select(x => Guid.Parse(x)).ToList();
                    foreach (var idTaiLieu in idTaiLieus)
                    {
                        var taiLieu = tailieu_db.tbTaiLieux.Find(idTaiLieu);
                        taiLieu.TrangThai = 0;
                        taiLieu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        taiLieu.NgaySua = DateTime.Now;

                        #region Xóa tệp
                        string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/TAILIEU", per.DonViSuDung.MaDonViSuDung);
                        var duongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMucGoc, tenTep_BANDAU: taiLieu.TenTaiLieuGoc);
                        if (System.IO.Directory.Exists(duongDanTep.DuongDanThuMuc_BANDAU_SERVER))
                            System.IO.Directory.Delete(duongDanTep.DuongDanThuMuc_BANDAU_SERVER, true);
                        #endregion
                    }
                    ;
                    tailieu_db.SaveChanges();
                    scope.Commit();
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
    }
}