using Applications._Others.Interfaces;
using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAITool.Dtos;
using Applications.QuanLyAITool.Interfaces;
using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyBaiDang.Models;
using Applications.QuanLyChienDich.Dtos;
using Applications.QuanLyChienDich.Interfaces;
using Applications.QuanLyChienDich.Models;
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
using LocThongTin_BaiDang = Applications.QuanLyBaiDang.Dtos.LocThongTinDto;
using LocThongTin_ChienDich = Applications.QuanLyChienDich.Dtos.LocThongTinDto;

namespace QuanLyBaiDang.Controllers
{
    [CustomAuthorize]
    public class QuanLyBaiDangController : StaticArgController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyBaiDang";
        private readonly IQuanLyBaiDangAppService _quanLyBaiDangAppService;
        private readonly IQuanLyChienDichAppService _quanLyChienDichAppService;
        private readonly IQuanLyAIToolAppService _quanLyAIToolAppService;
        private readonly IQuanLyAIBotAppService _quanLyAIBotAppService;
        private readonly IOtherAppService _otherAppService;
        public QuanLyBaiDangController(
            IQuanLyBaiDangAppService quanLyBaiDangService,
            IQuanLyChienDichAppService quanLyChienDichService,
            IQuanLyAIToolAppService quanLyAIToolAppService,
            IQuanLyAIBotAppService quanLyAIBotAppService,
            IOtherAppService otherAppService)
        {
            _quanLyBaiDangAppService = quanLyBaiDangService;
            _quanLyChienDichAppService = quanLyChienDichService;
            _quanLyAIToolAppService = quanLyAIToolAppService;
            _quanLyAIBotAppService = quanLyAIBotAppService;
            _otherAppService = otherAppService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            var output = await _quanLyBaiDangAppService.Index_OutPut();
            return View($"{VIEW_PATH}/baidang.cshtml", output);
        }

        #region Bài đăng
        [HttpPost]
        public async Task<ActionResult> getList_BaiDang(LocThongTin_BaiDang input)
        {
            var baiDangs = await _quanLyBaiDangAppService.GetBaiDangs(loai: "all", locThongTin: input);
            var output = new GetList_BaiDang_Output_Dto
            {
                BaiDangs = baiDangs.ToList(),
                ThaoTacs = _quanLyAIBotAppService.GetThaoTacs(maChucNang: "QuanLyBaiDang"),
            };
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-getList.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_BaiDang(DisplayModel_CRUD_BaiDang_Input_Dto input)
        {
            //var baiDang = await _quanLyBaiDangAppService.GetBaiDangs(loai: "single", idBaiDangs: new List<Guid> { input.IdBaiDang });
            var output = new DisplayModel_CRUD_BaiDang_Output_Dto
            {
                Loai = input.Loai,
                //BaiDang = baiDang.FirstOrDefault().BaiDang,
            };
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/baidang-crud.cshtml", output);
        }
        [HttpGet]
        public async Task<ActionResult> addBanGhi_Modal_CRUD()
        {
            var output = await _quanLyBaiDangAppService.AddBanGhi_Modal_CRUD_Output();

            output.LoaiView = "row";
            var html_baidang_row = Public.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/form-addbaidang.cshtml",
                model: output);

            output.LoaiView = "read";
            var html_baidang_read = Public.Handle.RenderViewToString(
                controller: this,
                viewName: $"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/form-addbaidang.cshtml",
                model: output);
            return Json(new
            {
                html_baidang_row,
                html_baidang_read
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> create_BaiDang(HttpPostedFileBase[] files, Guid[] rowNumbers)
        {
            try
            {
                var baiDang_NEWs = JsonConvert.DeserializeObject<List<tbBaiDangExtend>>(Request.Form["baiDangs"]);
                var loai = Request.Form["loai"];
                if (baiDang_NEWs == null || !baiDang_NEWs.Any())
                {
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);
                }

                await _quanLyBaiDangAppService.Create_BaiDang(loai: loai,baiDangs: baiDang_NEWs, files: files, rowNumbers: rowNumbers);

                return Json(new { status = "success", mess = "Thêm mới bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> delete_BaiDangs()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";

            try
            {
                var idBaiDangs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idBaiDangs"]);
                if (idBaiDangs == null || idBaiDangs.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyBaiDangAppService.Delete_BaiDangs(idBaiDangs: idBaiDangs);

                return Json(new { status, mess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> taoNoiDungAI(TaoNoiDungAI_Input_Dto input)
        {
            string status = "success";
            string mess = "Đã tạo nội dung AI";
            string noiDung = "";
            try
            {
                var aiBot = await _quanLyAIBotAppService.GetAIBots(loai: "single", idAIBot: new List<Guid> { input.IdAIBot });
                if (aiBot == null || !aiBot.Any() || aiBot.FirstOrDefault().AIBot.IdAIBot == Guid.Empty)
                {
                    return Json(new
                    {
                        status = "error",
                        mess = "Không tìm thấy thông tin AI Bot"
                    });
                }
                string prompt = aiBot.FirstOrDefault().AIBot.Prompt += string.Format("\n {0}: {1}",
                     "[THÔNG TIN CUNG CẤP] (nếu không có gì thì bỏ qua)",
                     input.Keywords);
                noiDung = await _quanLyAIToolAppService.WorkWithAITool(input: new WorkWithAITool_Input_Dto
                {
                    IdAITool = input.IdAITool,
                    Prompt = prompt,
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
        [HttpPost]
        public async Task<JsonResult> chonLoaiAIBot(Guid idAIBot)
        {
            try
            {
                var aiBot = await _quanLyAIBotAppService.GetAIBots(loai: "single", idAIBot: new List<Guid> { idAIBot });
                if (aiBot != null && aiBot.FirstOrDefault().AIBot.IdAIBot != Guid.Empty)
                    return Json(new
                    {
                        status = "success",
                        mess = "Lựa chọn AI Bot thành công, hãy nhập keywords để tạo nội dung",
                        Keywords = aiBot.FirstOrDefault().AIBot.Keywords,
                    });
                return Json(new
                {
                    status = "warning",
                    mess = "Không tìm thấy thông tin AI Bot",
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    mess = ex.ToString()
                });
            }
            ;

        }
        #endregion

        #region Chiến dịch
        [HttpPost]
        public async Task<ActionResult> getList_ChienDich(LocThongTin_ChienDich input)
        {
            var chienDichs = await _quanLyChienDichAppService.GetChienDichs(loai: "all", locThongTin: input);
            var output = new GetList_ChienDich_Output_Dto
            {
                ChienDichs = chienDichs.ToList(),
                ThaoTacs = _quanLyAIBotAppService.GetThaoTacs(maChucNang: "QuanLyChienDich"),
            };
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/chiendich/chiendich-getList.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> displayModal_CRUD_ChienDich(DisplayModel_CRUD_ChienDich_Input_Dto input)
        {
            var chienDich = await _quanLyChienDichAppService.GetChienDichs(loai: "single", idChienDichs: new List<Guid> { input.IdChienDich });
            var output = new DisplayModel_CRUD_ChienDich_Output_Dto
            {
                Loai = input.Loai,
                ChienDich = chienDich.FirstOrDefault(),
            };
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/chiendich/chiendich-crud.cshtml", output);
        }
        [HttpPost]
        public async Task<ActionResult> create_ChienDich()
        {
            try
            {
                var chienDich_NEW = JsonConvert.DeserializeObject<tbChienDich>(Request.Form["chienDich"]);
                if (chienDich_NEW == null)
                {
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);
                }

                await _quanLyChienDichAppService.Create_ChienDich(chienDich: chienDich_NEW);

                return Json(new { status = "success", mess = "Thêm mới bản ghi thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> delete_ChienDichs()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";

            try
            {
                var idChienDichs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idChienDichs"]);
                if (idChienDichs == null || idChienDichs.Count == 0)
                    return Json(new { status = "error", mess = "Chưa chọn bản ghi nào." }, JsonRequestBehavior.AllowGet);

                // Gọi AppService xử lý logic chính
                await _quanLyChienDichAppService.Delete_ChienDichs(idChienDichs: idChienDichs);

                return Json(new { status, mess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", mess = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public void SaveEncryptedCredential(string serviceName, string credentialType, string rawKeyJson, Guid? userId = null)
        {
            var encrypted = CryptoHelper.Encrypt(rawKeyJson);
            var oldCred = db.tbApiCredentials.FirstOrDefault(x => x.ServiceName == serviceName);
            if (oldCred == null)
            {
                var newCred = new tbApiCredential
                {
                    IdApiCredentials = Guid.NewGuid(),
                    IdNguoiDung = Guid.Empty,
                    ServiceName = serviceName,
                    CredentialType = credentialType,
                    KeyJson = encrypted,
                    TrangThai = 1,
                    NgayTao = DateTime.Now,
                    IdNguoiTao = userId
                };
                db.tbApiCredentials.Add(newCred);
            }
            else
            {
                oldCred.KeyJson = encrypted;
            }
            db.SaveChanges();
        }
        public void SaveEncryptedKeys()
        {
            // OpenAI Key
            //SaveEncryptedCredential("Imgbb", "ApiKey", "1e400d7c7e3474f17176880df3027c34");
            SaveEncryptedCredential("FreeImage", "ApiKey", "6d207e02198a847aa98d0a2a901485a5");

            // Google JSON (nội dung file)
            //string jsonFilePath = Server.MapPath("~/App_Data/ggc-drive.json");
            //if (System.IO.File.Exists(jsonFilePath))
            //{
            //    string jsonContent = System.IO.File.ReadAllText(jsonFilePath);
            //    SaveEncryptedCredential("Google", "ServiceAccountJson", jsonContent);
            //}
            //else
            //{
            //    throw new FileNotFoundException("The specified file does not exist.", jsonFilePath);
            //}
        }
    }
}