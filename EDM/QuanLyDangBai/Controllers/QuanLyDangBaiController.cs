using Applications.QuanLyDangBai.Dtos;
using Applications.QuanLyDangBai.Models;
using EDM_DB;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Sheets.v4.SheetsService;

namespace QuanLyDangBai.Controllers
{
    public class QuanLyDangBaiController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyDangBai";
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


        public QuanLyDangBaiController()
        {
        }
        #endregion

        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyDangBai").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #endregion

            THAOTACs = thaoTacs;
            //var a = _quanLyDangBaiAppService.GetListAsync();
            return View($"{VIEW_PATH}/quanlydangbai.cshtml");
        }
        [HttpGet]
        public ActionResult getList_BaiDang()
        {
            List<tbBaiDangExtend> baiDangs = getBaiDangs(loai: "all") ?? new List<tbBaiDangExtend>();
            return PartialView($"{VIEW_PATH}/quanlydangbai-getList.cshtml", baiDangs);
        }
        public List<tbBaiDangExtend> getBaiDangs(string loai = "all", List<Guid> idLichDangBais = null, LocThongTinDto locThongTin = null)
        {
            var baiDangRepo = db.tbBaiDangs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
                ?? new List<tbBaiDang>();
            var tepDinhKemRepo = db.tbTepDinhKems.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList()
               ?? new List<tbTepDinhKem>();
            //var lichDangBais = lichDangBaiRepo
            //    .Where(lich => loai != "single" || idLichDangBais.Contains(lich.IdLichDangBai))
            //    .Join(baiDangRepo,
            //          lich => lich.IdLichDangBai,
            //          bai => bai.IdBaiDang,
            //          (lich, bai) => new
            //          {
            //              LichDangBai = lich,
            //              BaiDang = bai
            //          })
            //    .Join(tepDinhKemRepo,
            //          input => input.BaiDang.IdBaiDang,
            //          a => a.IdTep,
            //          (tl, a) => new
            //          {
            //              Output = tl,
            //              Anh = a
            //          })
            //    .GroupBy(x => x.Output.BaiDang.IdBaiDang)
            //    .Select(g => new tbBaiDangExtend
            //    {
            //        BaiDang = g.First().Output.BaiDang,
            //        DonViTien = g.First().Output.DonViTien,
            //        AnhMoTas = g.Select(x => x.Anh).ToList()
            //    })
            //    .ToList() ?? new List<tbBaiDangExtend>();

            return new List<tbBaiDangExtend>();
        }
        [HttpPost]
        public ActionResult displayModal_CRUD(DisplayModel_CRUD_BaiDang_Input_Dto input)
        {
            var baiDang = getBaiDangs(loai: "single", idLichDangBais: new List<Guid> { input.IdBaiDang })?.FirstOrDefault() ?? new tbBaiDangExtend();
            var output = new DisplayModel_CRUD_BaiDang_Output_Dto
            {
                Loai = input.Loai,
                BaiDang = baiDang,
            };
            ViewBag.BaiDang = baiDang;
            ViewBag.Loai = input.Loai;
            return PartialView($"{VIEW_PATH}/quanlydangbai-getList - Copy.cshtml", output);
        }

        [HttpPost]
        public ActionResult create_TaiLieu()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            try
            {
                var baiDang_NEW = JsonConvert.DeserializeObject<tbBaiDangExtend>(Request.Form["baiDang"]);
                var files = Request.Files;

                // Đường dẫn thực tế của file JSON đã upload
                string jsonPath = Server.MapPath("~/App_Data/meta-buckeye-458819-m8-2044fdff02b5.json");

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

                string duongDanAnhs = string.Empty;
                for (int i = 0; i < files.Count; i++)
                {
                    var f = Request.Files[i];
                    var key = Request.Files.GetKey(i);

                    byte[] imgData = null;
                    using (var binaryReader = new BinaryReader(f.InputStream))
                    {
                        imgData = binaryReader.ReadBytes(f.ContentLength);
                    }
                    ;
                    duongDanAnhs += Public.Handle.GetImgSrcByByte(data: imgData) + ",";
                }
                ;

                // Tạo dữ liệu mẫu
                var values = new List<IList<object>>();
                for (int i = 1; i <= 10; i++)
                {
                    values.Add(new List<object>
                {
                    $"{i}",
                    baiDang_NEW.BaiDang.ThoiGian.Value.ToString("yyyy-MM-dd"),
                    baiDang_NEW.BaiDang.NoiDung,
                    baiDang_NEW.TuTaoAnh == 1 ? "TRUE" : "FALSE",
                    duongDanAnhs
                });
                }

                // Tạo yêu cầu ghi dữ liệu
                var valueRange = new ValueRange
                {
                    Values = values
                };

                var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, $"{sheetName}!A:E");
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                var appendResponse = appendRequest.Execute();

            }
            catch (DbEntityValidationException ex)
            {
                status = "error";
                mess = ex.Message;

                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine($"Entity: {eve.Entry.Entity.GetType().Name} - State: {eve.Entry.State}");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine($"- Property: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                    }
                }

                throw; // hoặc return lỗi ra view
            }

            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult _create_TaiLieu()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var baiDang_NEW = JsonConvert.DeserializeObject<tbBaiDangExtend>(Request.Form["baiDang"]);
                    var files = Request.Files;

                    if (baiDang_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        //// Thêm mới
                        //var baiDang = new tbBaiDang
                        //{
                        //   IdBaiDang = Guid.NewGuid(),
                        //   NoiDung = baiDang_NEW.BaiDang.NoiDung,
                        //   ThoiGian = baiDang_NEW.BaiDang.ThoiGian,
                        //   TrangThaiDangBai = 1,

                        //    TrangThai = 1,
                        //    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                        //    NgayTao = DateTime.Now,
                        //    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        //};
                        //db.tbBaiDangs.Add(baiDang);

                        //for (int i = 0; i < files.Count; i++)
                        //{
                        //    var f = Request.Files[i];
                        //    var key = Request.Files.GetKey(i);

                        //    string duongDanThuMucGoc = string.Format("/Assets/uploads/{0}/TAILIEU/{1}",
                        //        per.DonViSuDung.MaDonViSuDung, baiDang.IdTaiLieu);
                        //    string tenTaiLieu_BANDAU = Path.GetFileName(f.FileName);
                        //    var duongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMucGoc, tenTep_BANDAU: tenTaiLieu_BANDAU);

                        //    byte[] imgData = null;
                        //    using (var binaryReader = new BinaryReader(f.InputStream))
                        //    {
                        //        imgData = binaryReader.ReadBytes(f.ContentLength);
                        //    }
                        //    ;

                        //    var anhMoTa = new tbAnhMoTa
                        //    {
                        //        IdAnhMoTa = Guid.NewGuid(),
                        //        IdTaiLieu = baiDang.IdTaiLieu,
                        //        LaAnhDaiDien = key == "anhdaidien" ? true : false,
                        //        ImgData = imgData,
                        //        TenTepGoc = Path.GetFileNameWithoutExtension(f.FileName),
                        //        TenTepMoi = duongDanTep.TenTep_CHUYENDOI,
                        //        LoaiTep = duongDanTep.LoaiTep,

                        //        DuongDanTepVatLy = duongDanTep.DuongDanTep_BANDAU,
                        //        TrangThai = 1,
                        //        IdNguoiTao = per.NguoiDung.IdNguoiDung,
                        //        NgayTao = DateTime.Now,
                        //        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        //    };
                        //    db.tbAnhMoTas.Add(anhMoTa);

                        //    #region Tạo và lưu
                        //    // Tạo thư mục
                        //    if (!System.IO.Directory.Exists(duongDanTep.DuongDanThuMuc_BANDAU_SERVER))
                        //        System.IO.Directory.CreateDirectory(duongDanTep.DuongDanThuMuc_BANDAU_SERVER);
                        //    // (Nếu có rồi thì xóa)
                        //    if (System.IO.File.Exists(duongDanTep.DuongDanTep_BANDAU_SERVER))
                        //        System.IO.File.Delete(duongDanTep.DuongDanTep_BANDAU_SERVER);
                        //    f.SaveAs(duongDanTep.DuongDanTep_BANDAU_SERVER);
                        //    #endregion
                        //};
                        //db.SaveChanges();
                        //scope.Commit();
                    }
                    ;
                }
                //catch (Exception ex)
                //{

                //}
                catch (DbEntityValidationException ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();

                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Console.WriteLine($"Entity: {eve.Entry.Entity.GetType().Name} - State: {eve.Entry.State}");
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine($"- Property: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                        }
                    }

                    throw; // hoặc return lỗi ra view
                }
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
    }
}