using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UnitManage.Controllers {
    public class UnitManageController : RouteConfigController {
        private readonly string VIEW_PATH = "~/Views/Admin/UnitManage/";

        private List<tbDonViSuDung_DonViLienKet> DONVILIENKETs {
            get {
                return Session["DONVILIENKETs"] as List<tbDonViSuDung_DonViLienKet> ?? new List<tbDonViSuDung_DonViLienKet>();
            }
            set {
                Session["DONVILIENKETs"] = value;
            }
        }
        public ActionResult Index() {
            tbDonViSuDung donViSuDung = per.DonViSuDung;
            ViewBag.donViSuDung = donViSuDung;
            return View($"{VIEW_PATH}/unitmanage.cshtml");
        }
        [HttpGet]
        public ActionResult getList_DonViSuDung() {
            ViewBag.donViSuDung = per.DonViSuDung;
            return PartialView($"{VIEW_PATH}/unitmanage-donvisudung.getList.cshtml");
        }
        [HttpPost]
        public ActionResult update_DonViSuDung(HttpPostedFileBase logo, HttpPostedFileBase banner) {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction()) {
                try {
                    string str_donViSuDung = Request.Form["str_donViSuDung"];
                    tbDonViSuDung donViSuDung_NEW = JsonConvert.DeserializeObject<tbDonViSuDung>(str_donViSuDung ?? "");
                    if (donViSuDung_NEW == null) {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    } else {
                        string folderPath = string.Format("{0}", $"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/THIETLAPCHUNG/");
                        string folderPath_SERVER = Request.MapPath(folderPath);
                        if (!System.IO.Directory.Exists(folderPath_SERVER)) {
                            System.IO.Directory.CreateDirectory(folderPath_SERVER); // Tạo folder
                        };
                        tbDonViSuDung donViSuDung_OLD = db.tbDonViSuDungs.Find(per.DonViSuDung.MaDonViSuDung);
                        donViSuDung_OLD.TenDonViSuDung = donViSuDung_NEW.TenDonViSuDung;
                        donViSuDung_OLD.Logo = donViSuDung_NEW.Logo;
                        donViSuDung_OLD.Banner = donViSuDung_NEW.Banner;
                        donViSuDung_OLD.TieuDeTrangChu = donViSuDung_NEW.TieuDeTrangChu;
                        donViSuDung_OLD.GiaoDien = donViSuDung_NEW.GiaoDien;
                        donViSuDung_OLD.SoDienThoai = donViSuDung_NEW.SoDienThoai;
                        donViSuDung_OLD.Email = donViSuDung_NEW.Email;
                        donViSuDung_OLD.DiaChi = donViSuDung_NEW.DiaChi;
                        donViSuDung_OLD.SuDungTrangNguoiDung = donViSuDung_NEW.SuDungTrangNguoiDung;

                        donViSuDung_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        donViSuDung_OLD.NgaySua = DateTime.Now;
                        // Thêm tệp
                        if (logo != null) {
                            string inputFileName = Public.Handle.ConvertToUnSign(s: Path.GetFileName(logo.FileName), khoangCach: "-");
                            string inputFilePath = string.Format("{0}/[{1}]{2}", folderPath, "LOGO", inputFileName);
                            string inputFilePath_SERVER = Request.MapPath(inputFilePath);
                            donViSuDung_OLD.Logo = inputFilePath;
                            if (System.IO.File.Exists(inputFilePath_SERVER)) {
                                System.IO.File.Delete(inputFilePath_SERVER);
                            };
                            logo.SaveAs(inputFilePath_SERVER);
                        };
                        if (banner != null) {
                            string inputFileName = Public.Handle.ConvertToUnSign(s: Path.GetFileName(banner.FileName), khoangCach: "-");
                            string inputFilePath = string.Format("{0}/[{1}]{2}", folderPath, "BANNER", inputFileName);
                            string inputFilePath_SERVER = Request.MapPath(inputFilePath);
                            donViSuDung_OLD.Banner = inputFilePath;
                            if (System.IO.File.Exists(inputFilePath_SERVER)) {
                                System.IO.File.Delete(inputFilePath_SERVER);
                            };
                            banner.SaveAs(inputFilePath_SERVER);
                        };
                        /**
                         * Gán lại đơn vị sử dụng
                         */
                        per.DonViSuDung = donViSuDung_OLD;
                        
                        db.SaveChanges();
                        scope.Commit();
                    };
                } catch (Exception ex) {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                };
            };
            return Json(new {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
    }
}