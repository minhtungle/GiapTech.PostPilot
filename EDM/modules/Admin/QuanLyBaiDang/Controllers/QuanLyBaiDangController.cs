using Applications._Others.Interfaces;
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
        private readonly IOtherAppService _otherAppService;
        public QuanLyBaiDangController(
            IQuanLyBaiDangAppService quanLyBaiDangService,
            IQuanLyAIToolAppService quanLyAIToolAppService,
            IOtherAppService otherAppService)
        {
            _quanLyBaiDangService = quanLyBaiDangService;
            _quanLyAIToolAppService = quanLyAIToolAppService;
            _otherAppService = otherAppService;
        }
        #endregion

        public async Task<ActionResult> Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            var thaoTacs = _quanLyBaiDangService.GetThaoTacs(maChucNang: "QuanLyBaiDang");
            #endregion
            
            #region Thao tác
            var nenTangs = await _otherAppService.GetNenTangs();
            #endregion

            #endregion

            THAOTACs = thaoTacs.ToList();
            var output = new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
                NenTangs = nenTangs,
            };
            return View($"{VIEW_PATH}/baidang.cshtml", output);
        }

        [HttpGet]
        public async Task<ActionResult> getList_BaiDang(LocThongTinDto locThongTin)
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
            var html_baidang_row = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/baidang-crud/form-addbaidang.cshtml",
                new FormAddBaiDangDto
                {
                    LoaiView = "row",
                    BaiDang = baiDang,
                    NenTangs = nenTangs,
                });
            var html_baidang_read = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/baidang-crud/form-addbaidang.cshtml",
                new FormAddBaiDangDto
                {
                    LoaiView = "read",
                    BaiDang = baiDang,
                    NenTangs = nenTangs,
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
        public async Task<ActionResult> _create_BaiDang(HttpPostedFileBase[] files, Guid[] rowNumbers)
        {
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var baiDang_NEWs = JsonConvert.DeserializeObject<List<tbBaiDangExtend>>(Request.Form["baiDangs"]);

                    if (baiDang_NEWs == null)
                    {
                        return Json(new { status = "error", mess = "Chưa có bản ghi nào" }, JsonRequestBehavior.AllowGet);
                    }

                    foreach (var baiDang_NEW in baiDang_NEWs)
                    {
                        var baiDang = new tbBaiDang
                        {
                            IdBaiDang = Guid.NewGuid(),
                            IdChienDich = baiDang_NEW.BaiDang.IdChienDich,
                            IdNenTang = baiDang_NEW.BaiDang.IdNenTang,
                            Prompt = baiDang_NEW.BaiDang.Prompt,
                            NoiDung = baiDang_NEW.BaiDang.NoiDung,
                            ThoiGian = baiDang_NEW.BaiDang.ThoiGian,
                            TuTaoAnhAI = baiDang_NEW.BaiDang.TuTaoAnhAI,
                            TrangThaiDangBai = (int?)TrangThaiDangBaiEnum.WaitToPost,
                            TrangThai = 1,
                            IdNguoiTao = per.NguoiDung.IdNguoiDung,
                            NgayTao = DateTime.Now,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        };
                        db.tbBaiDangs.Add(baiDang);

                        if (files != null && (baiDang_NEW.BaiDang.TuTaoAnhAI.HasValue && !baiDang_NEW.BaiDang.TuTaoAnhAI.Value))
                        {
                            for (int i = 0; i < files.Length; i++)
                            {
                                var rowNumber = rowNumbers[i];
                                if (baiDang_NEW.RowNumber == rowNumber)
                                {
                                    var file = files[i];
                                    if (file == null || file.ContentLength <= 0)
                                    {
                                        return Json(new { status = "error", mess = "Chưa có file nào được chọn" }, JsonRequestBehavior.AllowGet);
                                    }

                                    var result = await _quanLyBaiDangService.UploadToFreeImageHost(file: file);

                                    if (result == null)
                                    {
                                        return Json(new { status = "error", mess = "Không nhận được phản hồi từ server." }, JsonRequestBehavior.AllowGet);
                                    }

                                    if (result.StatusCode != 200)
                                    {
                                        return Json(new { status = "error", mess = "Upload thất bại: " + (result.StatusText ?? "Lỗi không xác định") }, JsonRequestBehavior.AllowGet);
                                    }

                                    string imageUrl = result.Image.Url;
                                    var tepDinhKem = new tbTepDinhKem
                                    {
                                        IdTep = Guid.NewGuid(),
                                        FileName = Path.GetFileNameWithoutExtension(file.FileName),
                                        DuongDanTepOnline = imageUrl,
                                        TrangThai = 1,
                                        IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                        NgayTao = DateTime.Now,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                    };
                                    db.tbTepDinhKems.Add(tepDinhKem);

                                    var baiDangTepDinhKem = new tbBaiDangTepDinhKem
                                    {
                                        IdBaiDangTepDinhKem = Guid.NewGuid(),
                                        IdBaiDang = baiDang.IdBaiDang,
                                        IdTepDinhKem = tepDinhKem.IdTep,
                                    };
                                    db.tbBaiDangTepDinhKems.Add(baiDangTepDinhKem);
                                }
                            }
                        }
                    }

                    db.SaveChanges();
                    scope.Commit();

                    return Json(new { status = "success", mess = "Thêm mới bản ghi thành công" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", mess = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpPost]
        public async Task<JsonResult> _delete_BaiDangs()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    List<Guid> idBaiDangs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idBaiDangs"]);
                    if (idBaiDangs.Count > 0)
                    {
                        foreach (var idBaiDang in idBaiDangs)
                        {
                            var baiDang_OLD = db.tbBaiDangs.Find(idBaiDang);
                            if (baiDang_OLD != null)
                            {
                                baiDang_OLD.TrangThaiDangBai = (int?)TrangThaiDangBaiEnum.WaitToDelete; // Chờ xóa trên nền tảng
                                baiDang_OLD.TrangThai = 0;
                                baiDang_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                baiDang_OLD.NgaySua = DateTime.Now;

                                var baiDangTepDinhKems = db.tbBaiDangTepDinhKems
                                       .Where(x => x.IdBaiDang == baiDang_OLD.IdBaiDang)
                                       .ToList();

                                // Ép danh sách ID tệp về danh sách Guid
                                var tepIds = baiDangTepDinhKems.Select(y => y.IdTepDinhKem).ToList();

                                var tepDinhKems = db.tbTepDinhKems
                                    .Where(x => tepIds.Contains(x.IdTep))
                                    .ToList();
                                foreach (var tepDinhKem in tepDinhKems)
                                {
                                    tepDinhKem.TrangThai = 0;
                                    tepDinhKem.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                    tepDinhKem.NgaySua = DateTime.Now;
                                    // Xóa file trong server
                                    string duongDanTepVatLy = tepDinhKem.DuongDanTepVatLy;
                                    string duongDanThuMucGoc = Path.GetDirectoryName(duongDanTepVatLy);
                                    string inputFilePath_SERVER = Request.MapPath(duongDanTepVatLy);
                                    if (System.IO.File.Exists(inputFilePath_SERVER))
                                        System.IO.File.Delete(inputFilePath_SERVER);
                                    // Xóa bản ghi trong DB
                                    db.tbTepDinhKems.Remove(tepDinhKem);
                                }
                                ;
                            }
                            ;
                        }
                        ;
                        db.SaveChanges();
                        scope.Commit();
                    }
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> taoNoiDungAI(string toolCode = "OpenAI_ChatGPT", string prompt = "")
        {
            string status = "success";
            string mess = "Đã tạo nội dung AI";
            string noiDung = "";
            try
            {
                noiDung = await _quanLyAIToolAppService.WorkWithAITool(toolCode: toolCode, prompt: prompt);
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