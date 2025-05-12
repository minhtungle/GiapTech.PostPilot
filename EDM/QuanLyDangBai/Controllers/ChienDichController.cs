using Applications.QuanLyDangBai.AppServices;
using Applications.QuanLyDangBai.Dtos;
using Applications.QuanLyDangBai.Models;
using EDM_DB;
using Google.Apis.Sheets.v4;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyDangBai.Controllers
{
    public class ChienDichController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/ChienDich";
        private List<default_tbChucNang> CHUCNANGs
        {
            get
            {
                return Session["CHUCNANGs"] as List<default_tbChucNang> ?? new List<default_tbChucNang>();
            }
            set
            {
                Session["CHUCNANGs"] = value;
            }
        }
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
        public ChienDichController()
        {
        }
        #endregion
        // GET: ChienDich
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult getList_ChienDich()
        {
            List<tbChienDichExtend> chienDichs = getChienDichs(loai: "all");
            return PartialView($"{VIEW_PATH}/chiendich/chiendich-getList.cshtml", chienDichs);
        }
        public List<tbChienDichExtend> getChienDichs(string loai = "all", List<Guid> idChienDichs = null, LocThongTinDto locThongTin = null)
        {
            var chienDichRepo = db.tbChienDiches.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbChienDich>();

            var chienDichs = chienDichRepo
                .Where(x => loai != "single" || idChienDichs.Contains(x.IdChienDich))
                .Select(g => new tbChienDichExtend
                {
                    ChienDich = g,
                })
                .OrderByDescending(x => x.ChienDich.NgayTao)
                .ToList() ?? new List<tbChienDichExtend>();

            return chienDichs;
        }
    }
}