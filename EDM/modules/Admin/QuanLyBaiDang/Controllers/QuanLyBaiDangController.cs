using Applications.OpenAIApi.AppServices;
using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Interfaces;
using Applications.QuanLyBaiDang.Models;
using Applications.QuanLyBaiDang.Serivices;
using Applications.QuanLyChienDich.Models;
using EDM_DB;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Public.Controllers;
using Public.Enums;
using Public.Extensions;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        private readonly OpenAIApiService _openAIApiService;
        private readonly IQuanLyBaiDangAppService _baiDangService;
        public QuanLyBaiDangController(IQuanLyBaiDangAppService baiDangService)
        {
            _openAIApiService = new OpenAIApiService(); // Hoặc inject bằng DI nếu bạn dùng Autofac
            _baiDangService = baiDangService;
        }
        #endregion

        public ActionResult Index(Guid idChienDich)
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyBaiDang").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #region Chiến dịch
            var chienDichRepo = db.tbChienDiches.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbChienDich>();

            var chienDich = chienDichRepo
                .FirstOrDefault(x => x.IdChienDich == idChienDich) ?? new tbChienDich();
            #endregion

            #region Nền tảng
            var nenTangs = db.tbNenTangs.Where(x => x.TrangThai != 0
            //&& x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
            ).ToList()
                ?? new List<tbNenTang>();
            #endregion

            #endregion

            THAOTACs = thaoTacs;
            NENTANGs = nenTangs;
            var output = new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
                ChienDich = chienDich,
            };
            //var a = _quanLyDangBaiAppService.GetListAsync();
            return View($"{VIEW_PATH}/baidang.cshtml", output);
        }

        [HttpGet]
        public async Task<ActionResult> getList_BaiDang(Guid idChienDich)
        {
            //var baiDangs = getBaiDangs(loai: "all", idChienDich: idChienDich);
            var baiDangs = await _baiDangService.GetBaiDangs(loai: "all", idChienDich: idChienDich);
            return PartialView($"{VIEW_PATH}/baidang-getList.cshtml", baiDangs);
        }
        [HttpPost]
        public ActionResult displayModal_CRUD_BaiDang(DisplayModel_CRUD_BaiDang_Input_Dto input)
        {
            //var baiDang = getChienDichs(loai: "single", idChienDichs: new List<Guid> { input.IdBaiDang })?.FirstOrDefault() ?? new tbBaiDangExtend();
            var baiDang = new tbBaiDangExtend();
            var output = new DisplayModel_CRUD_BaiDang_Output_Dto
            {
                Loai = input.Loai,
                BaiDang = baiDang,
            };
            return PartialView($"{VIEW_PATH}/baidang-crud/baidang-crud.cshtml", output);
        }
        [HttpGet]
        public ActionResult addBanGhi_Modal_CRUD()
        {
            var baiDang = new tbBaiDangExtend { BaiDang = new tbBaiDang() };
            var html_baidang_row = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/baidang-crud/form-addbaidang.cshtml",
                new FormAddBaiDangDto
                {
                    BaiDang = baiDang,
                    LoaiView = "row"
                });
            var html_baidang_read = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/baidang-crud/form-addbaidang.cshtml",
                new FormAddBaiDangDto
                {
                    BaiDang = baiDang,
                    LoaiView = "read"
                });
            var output = new
            {
                html_baidang_row,
                html_baidang_read
            };
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> create_BaiDang(HttpPostedFileBase[] files, Guid[] rowNumbers, HttpClient httpClient)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var baiDang_NEWs = JsonConvert.DeserializeObject<List<tbBaiDangExtend>>(Request.Form["baiDangs"]);

                    if (baiDang_NEWs == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        foreach (var baiDang_NEW in baiDang_NEWs)
                        {
                            List<string> _linkTeps = new List<string>();

                            #region Lưu db

                            // Thêm mới
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
                                    // Lấy ảnh của bài đăng
                                    if (baiDang_NEW.RowNumber == rowNumber)
                                    {
                                        var file = files[i];

                                        if (file == null || file.ContentLength <= 0)
                                        {
                                            status = "error";
                                            mess = "Chưa có file nào được chọn";
                                            return Json(new
                                            {
                                                status,
                                                mess
                                            }, JsonRequestBehavior.AllowGet);
                                        }

                                        using (var ms = new MemoryStream())
                                        {
                                            file.InputStream.CopyTo(ms);
                                            var fileBytes = ms.ToArray();
                                            var base64Image = Convert.ToBase64String(fileBytes);

                                            //string apiKey = "1e400d7c7e3474f17176880df3027c34";
                                            string _apiKey = GetDecryptedCredential("Imgbb", "ApiKey");
                                            var formData = new MultipartFormDataContent();
                                            formData.Add(new StringContent(_apiKey), "key");
                                            formData.Add(new StringContent(base64Image), "image");

                                            var response = await httpClient.PostAsync("https://api.imgbb.com/1/upload", formData);
                                            if (response.IsSuccessStatusCode)
                                            {
                                                var jsonString = await response.Content.ReadAsStringAsync();
                                                dynamic result = JsonConvert.DeserializeObject(jsonString);
                                                string imageUrl = result.data.url;

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
                                            else
                                            {
                                                // Xử lý lỗi nếu cần
                                                ModelState.AddModelError("", "Upload ảnh thất bại.");
                                            }
                                        };
                                    }
                                    ;
                                }
                            }
                ;
                            #endregion
                        }
                        ;

                        db.SaveChanges();
                        scope.Commit();
                    }
                    ;
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> create_BaiDangAsync(HttpPostedFileBase[] files, Guid[] rowNumbers, HttpClient httpClient)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var baiDang_NEWs = JsonConvert.DeserializeObject<List<tbBaiDangExtend>>(Request.Form["baiDangs"]);

                    if (baiDang_NEWs == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        foreach (var baiDang_NEW in baiDang_NEWs)
                        {
                            List<string> _linkTeps = new List<string>();

                            #region Lưu db

                            // Thêm mới
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
                                    // Lấy ảnh của bài đăng
                                    if (baiDang_NEW.RowNumber == rowNumber)
                                    {
                                        var file = files[i];

                                        if (file == null || file.ContentLength <= 0)
                                        {
                                            status = "error";
                                            mess = "Chưa có file nào được chọn";
                                            return Json(new
                                            {
                                                status,
                                                mess
                                            }, JsonRequestBehavior.AllowGet);
                                        }

                                        using (var ms = new MemoryStream())
                                        {
                                            file.InputStream.CopyTo(ms);
                                            var fileBytes = ms.ToArray();
                                            var base64Image = Convert.ToBase64String(fileBytes);

                                            string apiKey = "1e400d7c7e3474f17176880df3027c34";
                                            var formData = new MultipartFormDataContent();
                                            formData.Add(new StringContent(apiKey), "key");
                                            formData.Add(new StringContent(base64Image), "image");

                                            var response = await httpClient.PostAsync("https://api.imgbb.com/1/upload", formData);
                                            if (response.IsSuccessStatusCode)
                                            {
                                                var jsonString = await response.Content.ReadAsStringAsync();
                                                dynamic result = JsonConvert.DeserializeObject(jsonString);
                                                string imageUrl = result.data.url;
                                                //imageUrls.Add(imageUrl);
                                            }
                                            else
                                            {
                                                // Xử lý lỗi nếu cần
                                                ModelState.AddModelError("", "Upload ảnh thất bại.");
                                            }
                                        }

                                        var tepDinhKem = new tbTepDinhKem
                                        {
                                            IdTep = Guid.NewGuid(),
                                            FileName = Path.GetFileNameWithoutExtension(file.FileName),
                                            //FileNameUpdate = duongDanTep.TenTep_CHUYENDOI,
                                            //FileExtension = duongDanTep.LoaiTep,
                                            //DuongDanTepVatLy = duongDanTep.DuongDanTep_BANDAU,
                                            //ByteData = imgData,

                                            TrangThai = 1,
                                            IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                            NgayTao = DateTime.Now,
                                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                        };

                                        #region Lưu file trong server
                                        byte[] imgData = null;
                                        using (var binaryReader = new BinaryReader(file.InputStream))
                                        {
                                            imgData = binaryReader.ReadBytes(file.ContentLength);
                                        }
                                    ;

                                        // Kiểm tra max dung lượng
                                        // Lưu
                                        string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/TEPDINHKEM/{1}/{2}/{3}",
                                       per.DonViSuDung.MaDonViSuDung, baiDang.IdChienDich, baiDang.IdBaiDang, tepDinhKem.IdTep);

                                        string tenTaiLieu_BANDAU = Path.GetFileName(file.FileName);
                                        var duongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMucGoc, tenTep_BANDAU: tenTaiLieu_BANDAU);

                                        string inputFileName = Public.Handle.ConvertToUnSign(s: Path.GetFileName(file.FileName), khoangCach: "-");
                                        string filePath = string.Format("/{0}/{1}", duongDanThuMucGoc, inputFileName);
                                        string folderPath_SERVER = Request.MapPath(duongDanThuMucGoc);
                                        string inputFilePath_SERVER = Request.MapPath(filePath);
                                        try
                                        {
                                            // Tạo thư mục
                                            if (!System.IO.Directory.Exists(folderPath_SERVER))
                                                System.IO.Directory.CreateDirectory(folderPath_SERVER);
                                            // (Nếu có rồi thì xóa)
                                            if (System.IO.File.Exists(inputFilePath_SERVER))
                                                System.IO.File.Delete(inputFilePath_SERVER);
                                            System.IO.File.WriteAllBytes(inputFilePath_SERVER, imgData);

                                            //f.SaveAs(inputFilePath_SERVER); // Lưu cách này k được

                                            // Đoạn này khóa vì không còn lưu vào drive
                                            //string currentDomain = Request.Url.Host.ToLower();
                                            //string link = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, filePath);
                                            //_linkTeps.Add(link);
                                        }
                                        catch (Exception ex)
                                        {
                                            // Xóa toàn bộ thư mục
                                            if (!System.IO.Directory.Exists(folderPath_SERVER)) System.IO.Directory.Delete(folderPath_SERVER, true);
                                        }
                                        #endregion

                                        tepDinhKem.FileNameUpdate = duongDanTep.TenTep_CHUYENDOI;
                                        tepDinhKem.FileExtension = duongDanTep.LoaiTep;
                                        tepDinhKem.DuongDanTepVatLy = filePath;
                                        tepDinhKem.ByteData = imgData;

                                        db.tbTepDinhKems.Add(tepDinhKem);

                                        var baiDangTepDinhKem = new tbBaiDangTepDinhKem
                                        {
                                            IdBaiDangTepDinhKem = Guid.NewGuid(),
                                            IdBaiDang = baiDang.IdBaiDang,
                                            IdTepDinhKem = tepDinhKem.IdTep,
                                        };

                                        db.tbBaiDangTepDinhKems.Add(baiDangTepDinhKem);
                                    }
                                    ;
                                }
                            }
                ;
                            #endregion

                            #region Chuyển vào sheet (Không cần nữa)
                            // Đường dẫn thực tế của file JSON đã upload
                            //string jsonPath = Server.MapPath("~/App_Data/ggc-drive.json");

                            //// Khởi tạo credential
                            //GoogleCredential credential;
                            //using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
                            //{
                            //    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                            //}

                            //// Khởi tạo Sheets API service
                            //var service = new SheetsService(new BaseClientService.Initializer()
                            //{
                            //    HttpClientInitializer = credential,
                            //    ApplicationName = ApplicationName,
                            //});

                            //// Tạo dữ liệu mẫu
                            //var values = new List<IList<object>>();
                            //values.Add(new List<object> {
                            //    baiDang_NEW.BaiDang.IdBaiDang,
                            //    baiDang_NEW.BaiDang.ThoiGian.Value.ToString(),
                            //    baiDang_NEW.BaiDang.NoiDung,
                            //    (bool)baiDang_NEW.BaiDang.TuTaoAnhAI ? "TRUE" : "FALSE",
                            //     string.Join(",", _linkTeps),
                            //    baiDang.TrangThaiDangBai == 0 ? "waiting" : "done",
                            //    ""
                            //});
                            //// Tạo yêu cầu ghi dữ liệu
                            //var valueRange = new ValueRange
                            //{
                            //    Values = values
                            //};

                            //var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, $"{sheetName}!A:E");
                            //appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                            //var appendResponse = appendRequest.Execute();
                            #endregion
                        }
                        ;

                        db.SaveChanges();
                        scope.Commit();
                    }
                    ;
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult delete_BaiDangs()
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
        public async Task<JsonResult> taoNoiDungAI(string prompt)
        {
            string status = "success";
            string mess = "Đã tạo nội dung AI";
            string noiDung = "";
            try
            {
                // 🔒 Lấy key đã được giải mã từ DB
                string _apiKey = GetDecryptedCredential("OpenAI", "ApiKey");
                noiDung = await _openAIApiService.GetCompletionAsync(prompt: prompt, _apiKey: _apiKey);
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
        public string GetDecryptedCredential(string serviceName, string credentialType, Guid? userId = null)
        {
            var cred = db.tbApiCredentials
                .FirstOrDefault(x => x.ServiceName == serviceName
                                  && x.CredentialType == credentialType
                                  //&& (userId == null || x.IdNguoiDung == userId)
                                  );

            if (cred == null) throw new Exception("Không tìm thấy dữ liệu!");

            return CryptoHelper.Decrypt(cred.KeyJson);
        }

        public void SaveEncryptedCredential(string serviceName, string credentialType, string rawKeyJson, Guid? userId = null)
        {
            var encrypted = CryptoHelper.Encrypt(rawKeyJson);

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
            db.SaveChanges();
        }
        public void SaveEncryptedKeys()
        {
            // OpenAI Key
            SaveEncryptedCredential("Imgbb", "ApiKey", "1e400d7c7e3474f17176880df3027c34");

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