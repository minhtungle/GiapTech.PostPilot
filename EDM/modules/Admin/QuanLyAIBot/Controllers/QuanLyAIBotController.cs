using EDM_DB;
using Newtonsoft.Json;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyAIBot.Controllers
{
    public class QuanLyAIBotController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyAIBot";
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

        public QuanLyAIBotController()
        {
            //_openAIApiService = new OpenAIApiService(); // Hoặc inject bằng DI nếu bạn dùng Autofac
        }
        #endregion

        //public ActionResult Index(Guid idChienDich)
        //{
        //    #region Lấy các danh sách

        //    #region Thao tác
        //    List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
        //    List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyAIBot").ThaoTacs ?? new List<ThaoTac>();
        //    #endregion

        //    #endregion

        //    THAOTACs = thaoTacs;
        //    var output = new IndexOutPut_Dto
        //    {
        //        ThaoTacs = thaoTacs,
        //    };
        //    //var a = _quanLyDangBaiAppService.GetListAsync();
        //    return View($"{VIEW_PATH}/baidang.cshtml", output);
        //}

        //[HttpGet]
        //public ActionResult getList_AIBot(Guid idChienDich)
        //{
        //    List<tbAIBotExtend> baiDangs = getAIBots(loai: "all", idChienDich: idChienDich);
        //    return PartialView($"{VIEW_PATH}/baidang-getList.cshtml", baiDangs);
        //}

        //public List<tbAIBotExtend> getAIBots(Guid idChienDich, string loai = "all", List<Guid> idAIBots = null, LocThongTinDto locThongTin = null)
        //{
        //    var baiDangRepo = db.tbAIBots.Where(x => x.IdChienDich == idChienDich &&
        //    x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
        //        ?? new List<tbAIBot>();

        //    var baiDangs = baiDangRepo
        //        .Where(x => loai != "single" || idAIBots.Contains(x.IdAIBot))
        //        .Select(g => new tbAIBotExtend
        //        {
        //            AIBot = g,
        //        })
        //        .OrderByDescending(x => x.AIBot.ThoiGian)
        //        .ToList() ?? new List<tbAIBotExtend>();

        //    return baiDangs;
        //}
    }
}