using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAIBot.Models;
using Applications.QuanLyAITool.Dtos;
using Applications.QuanLyAITool.Interfaces;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuanLyAIBot.Controllers
{
    [CustomAuthorize]
    public class QuanLyAIBotController : Controller
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyAIBot";
        public readonly IQuanLyAIBotAppService _quanLyAIBotAppService;
        private readonly IQuanLyAIToolAppService _quanLyAIToolAppService;
        public QuanLyAIBotController(
            IQuanLyAIBotAppService quanLyAIBotAppService,
            IQuanLyAIToolAppService quanLyAIToolAppService)
        {
            _quanLyAIBotAppService = quanLyAIBotAppService;
            _quanLyAIToolAppService = quanLyAIToolAppService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyAIBotAppService.Index_OutPut();

            return View($"{VIEW_PATH}/aibot.cshtml", output);
        }
        [HttpPost]
        public async Task<JsonResult> taoNoiDungAI(TaoNoiDungAI_Input_Dto input)
        {
            string status = "success";
            string mess = "Đã tạo nội dung AI";
            string noiDung = "";
            try
            {
                input.Prompt += string.Format("{0}: {1}",
                    "[THÔNG TIN CUNG CẤP] (nếu không có gì thì bỏ qua)",
                    input.Keywords);
                noiDung = await _quanLyAIToolAppService.WorkWithAITool(input: new WorkWithAITool_Input_Dto
                {
                    IdAITool = input.IdAITool,
                    Prompt = input.Prompt,
                });
            }
            catch (Exception ex)
            {
                status = "error";
                mess = ex.ToString();
            }
         ;
            return Json(new
            {
                NoiDung = noiDung,
                status,
                mess
            });
        }

        #region AIBot
        [HttpGet]
        public async Task<ActionResult> getList_AIBot()
        {
            var data = await _quanLyAIBotAppService.GetAIBots(loai: "all");
            var output = new GetList_AIBot_Output_Dto
            {
                AIBots = data.ToList(),
                ThaoTacs = _quanLyAIBotAppService.GetThaoTacs(maChucNang: "QuanLyAIBot"),
            };
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/aibot/aibot-getList.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_AIBot(DisplayModel_CRUD_AIBot_Input_Dto input)
        {
            var aiBot = await _quanLyAIBotAppService.GetAIBots(loai: "single", idAIBots: new List<Guid> { input.IdAIBot });
            var loaiAIBot = await _quanLyAIBotAppService.GetLoaiAIBots(loai: "all");
            var aiTools = await _quanLyAIToolAppService.GetAITools();
            var output = new DisplayModel_CRUD_AIBot_Output_Dto
            {
                Loai = input.Loai,
                LoaiAIBot = loaiAIBot.ToList(),
                AIBot = aiBot.FirstOrDefault() ?? new tbAIBotExtend()
                {
                    AIBot = new tbAIBot()
                },
                AITools = aiTools,
            };
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/aibot/aibot-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_AIBot()
        {
            try
            {
                var aiBot_NEW = JsonConvert.DeserializeObject<tbAIBotExtend>(Request.Form["aiBot"]);
                if (aiBot_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIBotAppService.IsExisted_AIBot(
                    aiBot: aiBot_NEW.AIBot);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIBotAppService.Create_AIBot(aiBot: aiBot_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<ActionResult> update_AIBot()
        {
            try
            {
                var aiBot_NEW = JsonConvert.DeserializeObject<tbAIBotExtend>(Request.Form["aiBot"]);
                if (aiBot_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIBotAppService.IsExisted_AIBot(
                    aiBot: aiBot_NEW.AIBot);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIBotAppService.Update_AIBot(aiBot: aiBot_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_AIBot()
        {
            try
            {
                var idAIBots = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idAIBots"]);
                if (idAIBots == null || idAIBots.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyAIBotAppService.Delete_AIBot(idAIBots: idAIBots);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Loại AIBot
        [HttpGet]
        public async Task<ActionResult> getList_LoaiAIBot()
        {
            var data = await _quanLyAIBotAppService.GetLoaiAIBots(loai: "all");
            var output = new GetList_LoaiAIBot_Output_Dto
            {
                LoaiAIBots = data.ToList(),
                ThaoTacs = _quanLyAIBotAppService.GetThaoTacs(maChucNang: "QuanLyAIBot"),
            };
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/loaiaibot/loaiaibot-getList.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_LoaiAIBot(DisplayModel_CRUD_LoaiAIBot_Input_Dto input)
        {
            var loaiAIBot = await _quanLyAIBotAppService.GetLoaiAIBots(loai: "single", idLoaiAIBots: new List<Guid> { input.IdLoaiAIBot });
            var output = new DisplayModel_CRUD_LoaiAIBot_Output_Dto
            {
                Loai = input.Loai,
                LoaiAIBot = loaiAIBot.FirstOrDefault() ?? new tbLoaiAIBot()
            };
            return PartialView($"{VIEW_PATH}/quanlyaibot-tab/loaiaibot/loaiaibot-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_LoaiAIBot()
        {
            try
            {
                var loaiAiBot_NEW = JsonConvert.DeserializeObject<tbLoaiAIBot>(Request.Form["loaiAiBot"]);
                if (loaiAiBot_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIBotAppService.IsExisted_LoaiAIBot(loaiAIBot: loaiAiBot_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIBotAppService.Create_LoaiAIBot(loaiAIBot: loaiAiBot_NEW);
                return Json(new { status = "success", mess = "Thêm mới thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
            ;
        }
        [HttpPost]
        public async Task<ActionResult> update_LoaiAIBot()
        {
            try
            {
                var loaiAIBot_NEW = JsonConvert.DeserializeObject<tbLoaiAIBot>(Request.Form["loaiAIBot"]);
                if (loaiAIBot_NEW == null)
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);

                var isExisted = await _quanLyAIBotAppService.IsExisted_LoaiAIBot(
                    loaiAIBot: loaiAIBot_NEW);
                if (isExisted)
                    return Json(new { status = "error", mess = "Tên đã tồn tại" }, JsonRequestBehavior.AllowGet);

                await _quanLyAIBotAppService.Update_LoaiAIBot(loaiAIBot: loaiAIBot_NEW);
                return Json(new { status = "success", mess = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
       ;
        }
        [HttpPost]
        public async Task<JsonResult> delete_LoaiAIBot()
        {
            try
            {
                var idLoaiAIBots = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idLoaiAIBots"]);
                if (idLoaiAIBots == null || idLoaiAIBots.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyAIBotAppService.Delete_LoaiAIBot(idLoaiAIBots: idLoaiAIBots);

                return Json(new { status = "success", mess = "Xóa bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}