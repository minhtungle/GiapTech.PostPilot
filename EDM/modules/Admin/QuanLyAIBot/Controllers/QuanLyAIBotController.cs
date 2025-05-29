using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Interfaces;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyAIBot.Controllers
{
    [CustomAuthorize]
    public class QuanLyAIBotController : StaticArgController
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
        public readonly IQuanLyAIBotAppService _quanLyAIBotAppService;
        public QuanLyAIBotController(IQuanLyAIBotAppService quanLyAIBotAppService)
        {
            _quanLyAIBotAppService = quanLyAIBotAppService;
        }
        #endregion

        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            var thaoTacs = _quanLyAIBotAppService.GetThaoTacs(maChucNang: "QuanLyAIBot").ToList();
            #endregion

            #endregion

            THAOTACs = thaoTacs;
            var output = new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
            };
            return View($"{VIEW_PATH}/baidang.cshtml", output);
        }

        [HttpGet]
        public async Task<ActionResult> getList_AIBot()
        {
            var data = await _quanLyAIBotAppService.GetAIBots(loai: "all");
            return PartialView($"{VIEW_PATH}/aibot-getList.cshtml", data);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_BaiDang(DisplayModel_CRUD_AIBot_Input_Dto input)
        {
            var rs = await _quanLyAIBotAppService.GetAIBots(loai: "single", idAIBot: new List<Guid> { input.IdAIBot });
            var output = new DisplayModel_CRUD_AIBot_Output_Dto
            {
                Loai = input.Loai,
                AIBot = rs.FirstOrDefault(),
            };
            return PartialView($"{VIEW_PATH}/aibot-crud.cshtml", output);
        }
    }
}