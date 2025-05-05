using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoaHoc.Controllers
{
    public class QuanLyKhoaHocController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyKhoaHoc";
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

        public QuanLyKhoaHocController()
        {

        }
        #endregion
        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyKhoaHoc").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #endregion

            THAOTACs = thaoTacs;

            return View($"{VIEW_PATH}/quanlykhoahoc.cshtml");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}