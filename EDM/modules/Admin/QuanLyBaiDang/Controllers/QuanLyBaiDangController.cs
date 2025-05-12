using Applications.OpenAIApi.AppServices;
using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Models;
using Applications.QuanLyChienDich.Models;
using EDM_DB;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuanLyBaiDang.Controllers
{
    public class QuanLyBaiDangController : RouteConfigController
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
        // GET: Default
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "n8n test";

        // Google Sheet ID (lấy từ link bạn cung cấp)
        static readonly string spreadsheetId = "1ONCqKxCJBwGHyeDY0AZ55ydzOtnypNVQcG4-7vTb7UA";

        // Tên sheet/tab (mặc định thường là "Sheet1")
        static readonly string sheetName = "Sheet1";

        public QuanLyBaiDangController()
        {
            _openAIApiService = new OpenAIApiService(); // Hoặc inject bằng DI nếu bạn dùng Autofac
        }
        private readonly OpenAIApiService _openAIApiService;
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

            #endregion

            THAOTACs = thaoTacs;
            var output = new IndexOutPut_Dto
            {
                ThaoTacs = thaoTacs,
                ChienDich = chienDich,
            };
            //var a = _quanLyDangBaiAppService.GetListAsync();
            return View($"{VIEW_PATH}/baidang.cshtml", output);
        }

        [HttpGet]
        public ActionResult getList_BaiDang(Guid idChienDich)
        {
            List<tbBaiDangExtend> baiDangs = getBaiDangs(loai: "all", idChienDich: idChienDich);
            return PartialView($"{VIEW_PATH}/baidang-getList.cshtml", baiDangs);
        }

        public List<tbBaiDangExtend> getBaiDangs(Guid idChienDich, string loai = "all", List<Guid> idBaiDangs = null, LocThongTinDto locThongTin = null)
        {
            var baiDangRepo = db.tbBaiDangs.Where(x => x.IdChienDich == idChienDich &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbBaiDang>();

            var baiDangs = baiDangRepo
                .Where(x => loai != "single" || idBaiDangs.Contains(x.IdBaiDang))
                .Select(g => new tbBaiDangExtend
                {
                    BaiDang = g,
                })
                .OrderByDescending(x => x.BaiDang.ThoiGian)
                .ToList() ?? new List<tbBaiDangExtend>();

            return baiDangs;
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

        [HttpPost]
        public ActionResult create_BaiDang(HttpPostedFileBase[] files)
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
                            // Thêm mới chiến dịch
                            var chienDich = new tbChienDich
                            {
                                IdChienDich = Guid.NewGuid(),
                                TenChienDich = "",
                                TrangThaiHoatDong = 1,

                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbChienDiches.Add(chienDich);

                            // Thêm mới
                            var baiDang = new tbBaiDang
                            {
                                IdBaiDang = Guid.NewGuid(),
                                IdChienDich = chienDich.IdChienDich,
                                NoiDung = baiDang_NEW.BaiDang.NoiDung,
                                ThoiGian = baiDang_NEW.BaiDang.ThoiGian,
                                TrangThaiDangBai = 1,

                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbBaiDangs.Add(baiDang);

                            foreach (var f in files)
                            {
                                var tepDinhKem = new tbTepDinhKem
                                {
                                    IdTep = Guid.NewGuid(),
                                    FileName = Path.GetFileNameWithoutExtension(f.FileName),
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
                                using (var binaryReader = new BinaryReader(f.InputStream))
                                {
                                    imgData = binaryReader.ReadBytes(f.ContentLength);
                                }
                                ;

                                // Kiểm tra max dung lượng
                                // Lưu
                                string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/THUVIEN_TEPDINHKEM/{1}",
                               per.DonViSuDung.MaDonViSuDung, tepDinhKem.IdTep);
                                string tenTaiLieu_BANDAU = Path.GetFileName(f.FileName);
                                var duongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMucGoc, tenTep_BANDAU: tenTaiLieu_BANDAU);

                                string inputFileName = Public.Handle.ConvertToUnSign(s: Path.GetFileName(f.FileName), khoangCach: "-");
                                string filePath = string.Format("/{0}/{1}",
                                    duongDanThuMucGoc, inputFileName);
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
                                    f.SaveAs(inputFilePath_SERVER);
                                    string currentDomain = Request.Url.Host.ToLower();
                                    string link = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, filePath);
                                    _linkTeps.Add(link);
                                }
                                catch (Exception ex)
                                {
                                    // Xóa toàn bộ thư mục
                                    if (!System.IO.Directory.Exists(folderPath_SERVER)) System.IO.Directory.Delete(folderPath_SERVER, true);
                                }
                                #endregion

                                #region Lưu file trong server
                                tepDinhKem.FileNameUpdate = duongDanTep.TenTep_CHUYENDOI;
                                tepDinhKem.FileExtension = duongDanTep.LoaiTep;
                                tepDinhKem.DuongDanTepVatLy = duongDanTep.DuongDanTep_BANDAU;
                                tepDinhKem.ByteData = imgData;

                                db.tbTepDinhKems.Add(tepDinhKem);

                                var baiDangTepDinhKem = new tbBaiDangTepDinhKem
                                {
                                    IdBaiDang = baiDang.IdBaiDang,
                                    IdTepDinhKem = tepDinhKem.IdTep,
                                };

                                db.tbBaiDangTepDinhKems.Add(baiDangTepDinhKem);
                                #endregion
                            }
                ;

                            #endregion

                            #region Chuyển vào sheet
                            // Đường dẫn thực tế của file JSON đã upload
                            string jsonPath = Server.MapPath("~/App_Data/meta-buckeye-458819-m8-b926e8aa307c.json");

                            // Khởi tạo credential
                            GoogleCredential credential;
                            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
                            {
                                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                            }

                            // Khởi tạo Sheets API service
                            var service = new SheetsService(new BaseClientService.Initializer()
                            {
                                HttpClientInitializer = credential,
                                ApplicationName = ApplicationName,
                            });

                            // Tạo dữ liệu mẫu
                            var values = new List<IList<object>>();
                            values.Add(new List<object> {
                                baiDang_NEW.BaiDang.IdBaiDang,
                                baiDang_NEW.BaiDang.ThoiGian.Value.ToString(),
                                baiDang_NEW.BaiDang.NoiDung,
                                baiDang_NEW.TuTaoAnh == 1 ? "TRUE" : "FALSE",
                                string.Join(",", _linkTeps)
                            });
                            // Tạo yêu cầu ghi dữ liệu
                            var valueRange = new ValueRange
                            {
                                Values = values
                            };

                            var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, $"{sheetName}!A:E");
                            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                            var appendResponse = appendRequest.Execute();
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
        public async Task<JsonResult> taoNoiDungAI(string input)
        {
            string status = "success";
            string mess = "Đã tạo nội dung AI";
            string noiDung = "";
            try
            {
                var email = "email@gmail.com";
                var website = "https://giaptech.com";
                var diaChi = "Hà Nội, Việt Nam";
                var chuDe = input;
                var prompt = $@"
Viết một bài đăng fanpage chất lượng cao theo phong cách sau:

1. **Tiêu đề ngắn gọn, mạnh mẽ và thu hút**, có thể dùng biểu tượng cảm xúc (emoji) phù hợp.
2. **Feedback thực tế hoặc câu chuyện truyền cảm hứng** từ khách hàng hoặc học viên (dạng lời kể, dẫn chứng).
3. **Danh sách gạch đầu dòng** các lợi ích, kết quả cụ thể mà người dùng đạt được.
4. **Lý do tại sao người khác cũng nên lựa chọn dịch vụ/sản phẩm này** (USP – điểm mạnh, cam kết...).
5. **Kêu gọi hành động rõ ràng**: inbox, bình luận, hoặc để lại thông tin để được tư vấn.
6. **Thông tin liên hệ**, bao gồm:
7. **Hashtag liên quan ở cuối bài viết** (4–6 hashtag)
   - Email: {email}
   - Website: {website}
   - Địa chỉ: {diaChi}

Nội dung bài viết nói về: ""{chuDe}""

Yêu cầu:
- Viết bằng tiếng Việt
- Văn phong thuyết phục, tự nhiên, gần gũi, hướng đến hành động, dành cho fanpage
- Có thể sử dụng emoji để làm nổi bật
- Toàn bộ nội dung không quá 500 từ
";
                noiDung = await _openAIApiService.GetCompletionAsync(prompt);
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
    }
}