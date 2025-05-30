using Applications.QuanLyAITool.Dtos;
using Applications.QuanLyAITool.Interfaces;
using EDM_DB;
using Newtonsoft.Json;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyAITool.Controllers
{
    public class QuanLyAIToolController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyAITool";
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
        public readonly IQuanLyAIToolAppService _quanLyAIToolAppService;
        public QuanLyAIToolController(IQuanLyAIToolAppService quanLyAIToolAppService)
        {
            _quanLyAIToolAppService = quanLyAIToolAppService;
        }
        #endregion

        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            var thaoTacs = _quanLyAIToolAppService.GetThaoTacs(maChucNang: "QuanLyAITool").ToList();
            #endregion

            #endregion

            THAOTACs = thaoTacs;
            var output = new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
            };
            return View($"{VIEW_PATH}/aitool.cshtml", output);
        }

        [HttpGet]
        public async Task<ActionResult> getList_AITool()
        {
            var data = await _quanLyAIToolAppService.GetAITools(loai: "all");
            return PartialView($"{VIEW_PATH}/aitool-getList.cshtml", data);
        }
       
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_AITool(DisplayModel_CRUD_AITool_Input_Dto input)
        {
            var aiTool = await _quanLyAIToolAppService.GetAITools(loai: "single", idAITool: new List<Guid> { input.IdAITool });
            var loaiAITool = await _quanLyAIToolAppService.GetAITools(loai: "all");
            var output = new DisplayModel_CRUD_AITool_Output_Dto
            {
                Loai = input.Loai,
                AITool = aiTool.FirstOrDefault() ?? new tbAITool(),
            };
            return PartialView($"{VIEW_PATH}/aitool-crud.cshtml", output);
        }
       
        [HttpPost]
        public async Task<ActionResult> create_AITool()
        {
            try
            {
                var aiTool_NEW = JsonConvert.DeserializeObject<tbAITool>(Request.Form["aiTool"]);
                if (aiTool_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIToolAppService.IsExisted_AITool(aiTool: aiTool_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIToolAppService.Create_AITool(aiTool: aiTool_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
    }
}