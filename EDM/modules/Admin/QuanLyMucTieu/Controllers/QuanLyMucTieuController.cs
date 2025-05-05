using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyMucTieu.Controllers
{
    public class QuanLyMucTieuController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyMucTieu";
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
        private List<tbNguoiDung> NGUOIDUNGs
        {
            get
            {
                return Session["NGUOIDUNGs"] as List<tbNguoiDung> ?? new List<tbNguoiDung>();
            }
            set
            {
                Session["NGUOIDUNGs"] = value;
            }
        }
        public QuanLyMucTieuController()
        {

        }
        #endregion
        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyMucTieu").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #region Người dùng
            var nguoiDungs = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList() ?? new List<tbNguoiDung>();
            #endregion

            #endregion

            return View();
        }
    }
}