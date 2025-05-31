using Applications._Others.Interfaces;
using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAITool.Dtos;
using Applications.QuanLyAITool.Interfaces;
using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Enums;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyBaiDang.Controllers
{
    [CustomAuthorize]
    public class QuanLyBaiDangController : StaticArgController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyBaiDang";
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
        private List<tbNenTang> NENTANGs
        {
            get
            {
                return Session["NENTANGs"] as List<tbNenTang> ?? new List<tbNenTang>();
            }
            set
            {
                Session["NENTANGs"] = value;
            }
        }
        // GET: Default
        private readonly IQuanLyBaiDangAppService _quanLyBaiDangService;
        private readonly IQuanLyAIToolAppService _quanLyAIToolAppService;
        private readonly IQuanLyAIBotAppService _quanLyAIBotAppService;
        private readonly IOtherAppService _otherAppService;
        public QuanLyBaiDangController(
            IQuanLyBaiDangAppService quanLyBaiDangService,
            IQuanLyAIToolAppService quanLyAIToolAppService,
            IQuanLyAIBotAppService quanLyAIBotAppService,
            IOtherAppService otherAppService)
        {
            _quanLyBaiDangService = quanLyBaiDangService;
            _quanLyAIToolAppService = quanLyAIToolAppService;
            _quanLyAIBotAppService = quanLyAIBotAppService;
            _otherAppService = otherAppService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            var thaoTacs = _quanLyBaiDangService.GetThaoTacs(maChucNang: "QuanLyBaiDang");
            #endregion

            #region ##
            var nenTangs = await _otherAppService.GetNenTangs();
            var aiTools = await _quanLyAIToolAppService.GetAITools();
            var aiBots = await _quanLyAIBotAppService.GetAIBots();

            var aiBots_ = aiBots.Select(x => x.AIBot).ToList();
            #endregion

            #endregion

            THAOTACs = thaoTacs.ToList();
            var output = new Applications.QuanLyBaiDang.Dtos.Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
                NenTangs = nenTangs,
                AITools = aiTools,
                AIBots = aiBots_
            };
            return View($"{VIEW_PATH}/baidang.cshtml", output);
        }

        [HttpGet]
        public async Task<ActionResult> getList_BaiDang(Applications.QuanLyBaiDang.Dtos.LocThongTinDto locThongTin)
        {
            var baiDangs = await _quanLyBaiDangService.GetBaiDangs(loai: "all", locThongTin: locThongTin);
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-getList.cshtml", baiDangs);
        }
        [HttpPost]
        public ActionResult displayModal_CRUD_BaiDang(DisplayModel_CRUD_BaiDang_Input_Dto input)
        {
            var baiDang = new tbBaiDangExtend();
            var output = new DisplayModel_CRUD_BaiDang_Output_Dto
            {
                Loai = input.Loai,
                BaiDang = baiDang,
            };
            return PartialView($"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/baidang-crud.cshtml", output);
        }
        [HttpGet]
        public async Task<ActionResult> addBanGhi_Modal_CRUD()
        {
            var baiDang = new tbBaiDangExtend { BaiDang = new tbBaiDang() };
            var nenTangs = await _otherAppService.GetNenTangs();
            var aiTools = await _quanLyAIToolAppService.GetAITools();
            var aiBots = await _quanLyAIBotAppService.GetAIBots();

            var aiBots_ = aiBots.Select(x => x.AIBot).ToList();
            var html_baidang_row = Public.Handle.RenderViewToString(
                this,
                $"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/form-addbaidang.cshtml",
                new FormAddBaiDangDto
                {
                    LoaiView = "row",
                    BaiDang = baiDang,
                });
            var html_baidang_read = Public.Handle.RenderViewToString(
                this,
                $"{VIEW_PATH}/quanlybaidang-tab/baidang/baidang-crud/form-addbaidang.cshtml",
                new FormAddBaiDangDto
                {
                    LoaiView = "read",
                    BaiDang = baiDang,
                    NenTangs = nenTangs,
                    AITools = aiTools,
                    AIBots = aiBots_
                });
            var output = new
            {
                html_baidang_row,
                html_baidang_read
            };
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> create_BaiDang(HttpPostedFileBase[] files, Guid[] rowNumbers)
        {
            try
            {
                var baiDang_NEWs = JsonConvert.DeserializeObject<List<tbBaiDangExtend>>(Request.Form["baiDangs"]);
                if (baiDang_NEWs == null || !baiDang_NEWs.Any())
                {
                    return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);
                }

                await _quanLyBaiDangService.Create_BaiDang(baiDangs: baiDang_NEWs, files: files, rowNumbers: rowNumbers);

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
                await _quanLyBaiDangService.Delete_BaiDangs(idBaiDangs: idBaiDangs);

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