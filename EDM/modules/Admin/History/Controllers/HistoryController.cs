using EDM_DB;
using History.Models;
using Public.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace History.Controllers
{
    public class HistoryController : RouteConfigController
    {
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemUtilities/History";
        public ActionResult Index()
        {
            return View($"{VIEW_PATH}/history.cshtml");
        }
        public ActionResult getList()
        {
            string lichSuTruyCapSQL = $@"select lichsu.*, donvisudung.TenDonViSuDung, nguoidung.TenNguoiDung
            from tbLichSuTruyCap lichsu
                join tbNguoiDung nguoidung on nguoidung.IdNguoiDung = lichsu.IdNguoiDung
                join tbDonViSuDung donvisudung on donvisudung.MaDonViSuDung = lichsu.MaDonViSuDung
            where lichsu.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} 
            order by NgayTao desc";
            List<tbLichSuTruyCapExtend> lichSuTruyCaps = db.Database.SqlQuery<tbLichSuTruyCapExtend>(lichSuTruyCapSQL).ToList();
            return Json(new
            {
                data = lichSuTruyCaps
            }, JsonRequestBehavior.AllowGet);
        }
    }
}