using Aspose.Cells;
using Aspose.Pdf;
using Aspose.Words;
using Aspose.Words.Saving;
using ClosedXML.Excel;
using DocumentDigitizing.Models;
using DocumentFormation.Controllers;
using DocumentFormation.Models;
using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserType.Models;

namespace DocumentDigitizing.Controllers
{
    public class DocumentDigitizingController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_DocumentManage/DocumentDigitizing";
        IsoDateTimeConverter DATETIMECONVERTER = new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" };
        private List<tbHoSoExtend> EXCEL_DULIEUSOs_UPLOAD
        {
            get
            {
                return Session["EXCEL_DULIEUSOs_UPLOAD"] as List<tbHoSoExtend> ?? new List<tbHoSoExtend>();
            }
            set
            {
                Session["EXCEL_DULIEUSOs_UPLOAD"] = value;
            }
        }
        private List<tbHoSoExtend> EXCEL_DULIEUSOs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_DULIEUSOs_DOWNLOAD"] as List<tbHoSoExtend> ?? new List<tbHoSoExtend>();
            }
            set
            {
                Session["EXCEL_DULIEUSOs_DOWNLOAD"] = value;
            }
        }
        private List<tbBieuMauExtend> BIEUMAUs_EXCEL
        {
            get
            {
                return Session["BIEUMAUs_EXCEL"] as List<tbBieuMauExtend> ?? new List<tbBieuMauExtend>();
            }
            set
            {
                Session["BIEUMAUs_EXCEL"] = value;
            }
        }
        private List<tbBieuMauExtend> BIEUMAUs
        {
            get
            {
                return Session["BIEUMAUs"] as List<tbBieuMauExtend> ?? new List<tbBieuMauExtend>();
            }
            set
            {
                Session["BIEUMAUs"] = value;
            }
        }
        private tbHoSoExtend HOSO
        {
            get
            {
                return Session["HOSO"] as tbHoSoExtend ?? new tbHoSoExtend();
            }
            set
            {
                Session["HOSO"] = value;
            }
        }
        #endregion
        public ActionResult Index(int idHoSo = 0)
        {
            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "DocumentFormation").ThaoTacs;
            #endregion
            // Danh sách các biểu mẫu
            BIEUMAUs = get_BieuMaus(loai: "all");
            // Thông tin hồ sơ đang chọn
            HOSO = get_HoSos(loai: "single", idHoSos: idHoSo.ToString()).FirstOrDefault();
            HOSO.VanBans = get_VanBans(hoSo: HOSO, loai: "all");
            ViewBag.thaoTacs = thaoTacs;
            return PartialView($"{VIEW_PATH}/vanban.cshtml");
        }
        [HttpGet]
        public ActionResult getList()
        {
            List<tbHoSo_VanBanExtend> vanBans = get_VanBans(hoSo: HOSO, loai: "all");
            HOSO.VanBans = vanBans;
            return PartialView($"{VIEW_PATH}/vanban.getList.cshtml");
        }
        public List<tbHoSoExtend> get_HoSos(string loai, string idHoSos = "")
        {
            List<tbHoSoExtend> hoSos = new List<tbHoSoExtend>();
            // Chỉ hiển thị bản ghi có quyền truy cập
            if (loai == "all")
            {
                hoSos = db.Database.SqlQuery<tbHoSoExtend>($@"
                select * from tbHoSo 
                where 
                    MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai in (1,2) and
                    exists (select 1 from string_split(QuyenTruyCap, ',') where value = '{per.NguoiDung.IdNguoiDung}')
                order by NgayTao desc").ToList();
            }
            else
            {
                if (idHoSos != "")
                {
                    string getSql = $@"select * from tbHoSo where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} 
                    and TrangThai in (1,2) and IdHoSo in ({idHoSos}) order by NgayTao desc";
                    hoSos = db.Database.SqlQuery<tbHoSoExtend>(getSql).ToList();
                }
            };
            foreach (tbHoSoExtend hoSo in hoSos)
            {
                hoSo.VanBans = db.Database.SqlQuery<tbHoSo_VanBanExtend>($@"
                select * from tbHoSo_VanBan 
                where IdHoSo = {hoSo.IdHoSo} and TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}").ToList() ?? new List<tbHoSo_VanBanExtend>();
                hoSo.ViTriLuuTru = db.tbViTriLuuTrus.Find(hoSo.IdViTriLuuTru) ?? new tbViTriLuuTru();
                hoSo.PhongLuuTru = db.tbDonViSuDung_PhongLuuTru.Find(hoSo.IdPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                // Lấy danh mục từ danh sách danh mục đã xử lý tên
                hoSo.DanhMucHoSo = db.tbDanhMucHoSoes.Find(hoSo.IdDanhMucHoSo) ?? new tbDanhMucHoSo();
                hoSo.CheDoSuDung = db.default_tbCheDoSuDung.Find(hoSo.IdCheDoSuDung) ?? new default_tbCheDoSuDung();
                hoSo.ThongTinNguoiTao = db.tbNguoiDungs.Find(hoSo.NguoiTao) ?? new tbNguoiDung();
                hoSo.DonViSuDung = db.tbDonViSuDungs.Find(hoSo.MaDonViSuDung) ?? new tbDonViSuDung();
            };
            return hoSos;
        }
        public List<tbHoSo_VanBanExtend> get_VanBans(tbHoSoExtend hoSo, string loai, string idVanBans = "")
        {
            // Thông tin văn bản của hồ sơ
            string get_VanBanSQL = "";
            if (loai == "all")
            {
                get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdHoSo = {hoSo.IdHoSo}";
            }
            else if (idVanBans != "")
            {
                get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdVanBan in ({idVanBans})";
            };
            List<tbHoSo_VanBanExtend> vanBans = db.Database.SqlQuery<tbHoSo_VanBanExtend>(get_VanBanSQL).ToList() ?? new List<tbHoSo_VanBanExtend>();
            // Lấy thông tin biểu mẫu của văn bản
            foreach (tbHoSo_VanBanExtend vanBan in vanBans)
            {
                string get_BieuMauSQL = $@"select * from tbBieuMau where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdBieuMau = {vanBan.IdBieuMau}";
                vanBan.BieuMau = db.Database.SqlQuery<tbBieuMauExtend>(get_BieuMauSQL).FirstOrDefault() ?? new tbBieuMauExtend();
                #region Lấy đường dẫn văn bản
                string tenVanBan_BANDAU = string.Format("{0}{1}", Path.GetFileName(vanBan.TenVanBan), vanBan.Loai);
                var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU);

                vanBan.DuongDan = duongDanVanBan.duongDanVanBan_BANDAU;
                if (vanBan.Loai.Contains("xls") || vanBan.Loai.Contains("doc"))
                {
                    vanBan.DuongDan = duongDanVanBan.duongDanVanBan_CHUYENDOI;
                };
                #endregion
            };
            return vanBans;
        }
        private void downloadDialog(MemoryStream data, string fileName, string contentType)
        {
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "inline; filename=\"" + fileName + "\"");
            Response.BinaryWrite(data.ToArray());
            Response.Flush();
            Response.End();
        }
        #region CRUD
        [HttpPost]
        public ActionResult displayModal_Digitizing(int idHoSo, string loai, int idVanBan)
        {
            /**
            * Tìm văn bản
            */
            tbHoSo_VanBanExtend vanBan = get_VanBans(hoSo: HOSO, loai: "single", idVanBans: idVanBan.ToString()).FirstOrDefault() ?? new tbHoSo_VanBanExtend();
            // Lấy tên miền
            Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
            string hostName = uri.GetLeftPart(UriPartial.Authority);
            ViewBag.hostName = hostName;
            ViewBag.vanBan = vanBan;
            ViewBag.loai = loai;
            if (vanBan.Loai.Contains("pdf") || vanBan.Loai.Contains("xls") || vanBan.Loai.Contains("doc"))
                ViewBag.iframeHtml = $"<iframe src=\"{hostName}/Search/xemPDF?idHoSo={idHoSo}&idVanBan={idVanBan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh\"></iframe>";
            else if (vanBan.Loai.Contains("mp4"))
                ViewBag.iframeHtml = $"<video src=\"{vanBan.DuongDan}\" controls style=\"width: 100%; height: 70vh; border: 1px solid var(--bs-body-color)\"></video>";
            else
                ViewBag.iframeHtml = $"<iframe src=\"{vanBan.DuongDan}\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh; border: 1px solid var(--bs-body-color)\"></iframe>";
            return PartialView($"{VIEW_PATH}/vanban.detail.cshtml");
        }

        public ActionResult get_DuLieuSos(int idVanBan = 0, int idBieuMau = 0)
        {
            tbBieuMau bieuMau = db.tbBieuMaus.Find(idBieuMau);
            // Tìm trường dữ liệu của biểu mẫu
            List<tbBieuMau_TruongDuLieuExtend> truongDuLieus = db.Database.SqlQuery<tbBieuMau_TruongDuLieuExtend>(
                $@"select truongdulieu.* from tbBieuMau_TruongDuLieu truongdulieu  
                where truongdulieu.IdBieuMau = {idBieuMau} and truongdulieu.TrangThai = 1 and truongdulieu.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}"
                ).ToList() ?? new List<tbBieuMau_TruongDuLieuExtend>();
            /**
             * Tìm dữ liệu số từng trường thuộc văn bản và sắp xếp theo nhóm
             * |---------------------------------------|
             * |   TRUONG1   |   TRUONG2  |   TRUONG3  |
             * |---------------------------------------|
             * |  DULIEUSO1  |  DULIEUSO1 |  DULIEUSO1 |
             * |  DULIEUSO2  |  DULIEUSO2 |  DULIEUSO2 |
             * |  DULIEUSO3  |  DULIEUSO3 |  DULIEUSO3 |
             * |  DULIEUSO4  |  DULIEUSO4 |  DULIEUSO4 |
             * |---------------------------------------|
             */
            foreach (tbBieuMau_TruongDuLieuExtend truongDuLieu in truongDuLieus)
            {
                truongDuLieu.DuLieuSos = db.tbHoSo_VanBan_DuLieuSo.Where(x =>
                x.IdTruongDuLieu == truongDuLieu.IdTruongDuLieu &&
                x.IdVanBan == idVanBan &&
                x.TrangThai == 1 &&
                x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).OrderBy(x => x.Nhom).ToList() ?? new List<tbHoSo_VanBan_DuLieuSo>();
            };

            ViewBag.truongDuLieus = truongDuLieus;
            ViewBag.bieuMau = bieuMau;
            return PartialView($"{VIEW_PATH}/vanban.detail.truongdulieus.cshtml");
        }
        public List<tbBieuMauExtend> get_BieuMaus(string loai, string idBieuMaus = "")
        {
            string get_BieuMauSQL = $@"select * from tbBieuMau where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}";
            if (loai == "single" && idBieuMaus != "")
            {
                get_BieuMauSQL += $@" and IdBieuMau in ({idBieuMaus})";
            }
            List<tbBieuMauExtend> bieuMaus = db.Database.SqlQuery<tbBieuMauExtend>(get_BieuMauSQL).ToList() ?? new List<tbBieuMauExtend>();
            foreach (tbBieuMauExtend bieuMau in bieuMaus)
            {
                bieuMau.TruongDuLieus = db.Database.SqlQuery<tbBieuMau_TruongDuLieuExtend>($@"
                select * from tbBieuMau_TruongDuLieu where IdBieuMau = {bieuMau.IdBieuMau} and TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                ").ToList();
            }
            return bieuMaus;
        }

        [HttpPost]
        public ActionResult create_VanBan(HttpPostedFileBase[] files)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (files == null || files.Length == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {

                        int idHoSo = int.Parse(Request.Form["idHoSo"]);
                        tbHoSoExtend hoSo = db.Database.SqlQuery<tbHoSoExtend>($@"
                        select * from tbHoSo where IdHoSo = {idHoSo}").FirstOrDefault() ?? new tbHoSoExtend();
                        foreach (HttpPostedFileBase f in files)
                        {
                            /**
                             * Mô tả sơ đồ tệp
                             * Ghi chú: 
                             *  - ext: loại tệp (doc|docx|xls|xlsx|png|jpg|jpeg|mp4) không bao gồm pdf
                             * ---------------------------------------------
                             * (1) |(2)   |(3)      |(4)         |(5)
                             * HoSo|
                             *     |VanBan|VanBan.ext
                             *            |CHUYENDOI|[.docx]     |VanBan.pdf
                             *                      |[.xlsx]     |VanBan.pdf
                             *                      ...
                             *            |PHIEUMUON|(Mã phiếu 1)|VanBan.pdf
                             *                      |(Mã phiếu 2)|VanBan.pdf
                             *                      ...
                             */
                            string tenVanBan_BANDAU = Path.GetFileName(f.FileName);
                            var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU);
                            #region Nếu chưa có văn bản này thì tạo mới trong db
                            tbHoSo_VanBan vanBan_OLD = db.tbHoSo_VanBan.FirstOrDefault(x =>
                            x.IdHoSo == idHoSo &&
                            x.TenVanBan == duongDanVanBan.tenVanBan_KHONGDAU_KHONGLOAI &&
                            //x.TenVanBan_BanDau == Path.GetFileNameWithoutExtension(tenVanBan_BANDAU) &&
                            x.Loai == duongDanVanBan.loaiVanBan &&
                            x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                            if (vanBan_OLD != null)
                            {
                                vanBan_OLD.TrangThai = 1; // Khôi phục dữ liệu này nếu đang xóa
                            }
                            else
                            {
                                tbHoSo_VanBan vanBan = new tbHoSo_VanBan
                                {
                                    IdHoSo = idHoSo,
                                    IdBieuMau = 0,
                                    TenVanBan = duongDanVanBan.tenVanBan_KHONGDAU_KHONGLOAI,
                                    TenVanBan_BanDau = Path.GetFileNameWithoutExtension(tenVanBan_BANDAU),
                                    Loai = duongDanVanBan.loaiVanBan,
                                    TrangThai = 1,
                                    NguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbHoSo_VanBan.Add(vanBan);
                            };
                            #endregion

                            #region Tạo và lưu văn bản
                            // Tạo thư mục
                            if (!System.IO.Directory.Exists(duongDanVanBan.duongDanThuMuc_BANDAU_SERVER))
                            {
                                System.IO.Directory.CreateDirectory(duongDanVanBan.duongDanThuMuc_BANDAU_SERVER);
                            };
                            // (Nếu có rồi thì xóa)
                            if (System.IO.File.Exists(duongDanVanBan.duongDanVanBan_BANDAU_SERVER))
                            {
                                System.IO.File.Delete(duongDanVanBan.duongDanVanBan_BANDAU_SERVER);
                            };
                            f.SaveAs(duongDanVanBan.duongDanVanBan_BANDAU_SERVER);
                            #endregion

                            #region Chuyển đổi văn bản dạng office thành PDF
                            if (duongDanVanBan.loaiVanBan.Contains("xls") || duongDanVanBan.loaiVanBan.Contains("doc"))
                            {
                                #region Bước 1: Tạo thư mục lưu PDF
                                if (!System.IO.Directory.Exists(duongDanVanBan.duongDanThuMuc_CHUYENDOI_SERVER))
                                {
                                    System.IO.Directory.CreateDirectory(duongDanVanBan.duongDanThuMuc_CHUYENDOI_SERVER);
                                };
                                #endregion
                                #region Bước 2: Chuyển đổi sang PDF
                                if (duongDanVanBan.loaiVanBan.Contains("xls"))
                                { // Excel
                                  // Tạo một đối tượng Workbook từ tập tin Excel
                                    Workbook workbook = new Workbook(duongDanVanBan.duongDanVanBan_BANDAU_SERVER);
                                    foreach (var worksheet in workbook.Worksheets)
                                    {
                                        // Cho các cell vừa với text bên trong
                                        worksheet.AutoFitColumns();
                                        // Căn lề
                                        worksheet.PageSetup.LeftMargin = 1;
                                        worksheet.PageSetup.RightMargin = 1;
                                        // Cho chiều dài bảng tối đa 2 lề
                                        worksheet.AutoFitRows();
                                    };
                                    // Tạo đối tượng PdfSaveOptions để cấu hình định dạng PDF
                                    Aspose.Cells.PdfSaveOptions pdfSaveOptions = new Aspose.Cells.PdfSaveOptions();
                                    pdfSaveOptions.OnePagePerSheet = true; // Cho các sheet vào chung 1 trang pdf
                                                                           // Lưu PDF vào thư mục
                                    workbook.Save(duongDanVanBan.duongDanVanBan_CHUYENDOI_SERVER, pdfSaveOptions);
                                }
                                else
                                { // Word
                                  // Tạo đối tượng Document từ tệp Word
                                    Aspose.Words.Document document = new Aspose.Words.Document(duongDanVanBan.duongDanVanBan_BANDAU_SERVER);
                                    // Chuyển đổi tài liệu Word sang PDF và lưu vào tệp PDF mới
                                    document.Save(duongDanVanBan.duongDanVanBan_CHUYENDOI_SERVER);
                                };
                                #endregion
                            };
                            #endregion
                        };
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Số hóa hồ sơ",
                            ThaoTac = "Thêm mới",
                            NoiDungChiTiet = "Thêm mới văn bản",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        db.SaveChanges();
                        scope.Commit();
                    };
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
        public JsonResult delete_VanBans()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idVanBans = Request.Form["str_idVanBans"];
                    int idHoSo = int.Parse(Request.Form["idHoSo"]);
                    tbHoSoExtend hoSo = db.Database.SqlQuery<tbHoSoExtend>($@"
                        select * from tbHoSo where IdHoSo = {idHoSo}").FirstOrDefault() ?? new tbHoSoExtend();
                    List<int> idVanBans = str_idVanBans.Split(',').Select(Int32.Parse).ToList();
                    foreach (int idVanBan in idVanBans)
                    {
                        tbHoSo_VanBan vanBan = db.tbHoSo_VanBan.Find(idVanBan);
                        vanBan.TrangThai = 0;
                        vanBan.NguoiSua = per.NguoiDung.IdNguoiDung;
                        vanBan.NgaySua = DateTime.Now;
                        db.Database.ExecuteSqlCommand($@"
                        -- Xóa dữ liệu số
                        update tbHoSo_VanBan_DuLieuSo set TrangThai = 0, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' where IdVanBan = {vanBan.IdVanBan} and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                        -- Xóa phiếu mượn văn bản
                        update tbPhieuMuon_VanBan set TrangThai = 0, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' where IdVanBan = {vanBan.IdVanBan} and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                        ");

                        #region Xóa tệp văn bản
                        string tenVanBan_BANDAU = string.Format("{0}{1}", vanBan.TenVanBan, vanBan.Loai);
                        var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU);
                        if (System.IO.Directory.Exists(duongDanVanBan.duongDanThuMuc_BANDAU_SERVER))
                        {
                            System.IO.Directory.Delete(duongDanVanBan.duongDanThuMuc_BANDAU_SERVER, true);
                        };
                        #endregion
                    };
                    db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                    {
                        TenModule = "Số hóa hồ sơ",
                        ThaoTac = "Xóa",
                        NoiDungChiTiet = "Xóa văn bản và các văn bản thuộc phiếu mượn",

                        NgayTao = DateTime.Now,
                        IdNguoiDung = per.NguoiDung.IdNguoiDung,
                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                    });
                    db.SaveChanges();
                    scope.Commit();
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
        public JsonResult ganBieuMau_VanBans()
        {
            string status = "success";
            string mess = "Gán biểu mẫu thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idVanBans = Request.Form["str_idVanBans"];
                    List<int> ids = str_idVanBans.Split(',').Select(Int32.Parse).ToList();
                    foreach (int id in ids)
                    {
                        tbHoSo_VanBan vanBan = db.tbHoSo_VanBan.Find(id);
                        //vanBan.IdBieuMau = id;
                        // Xóa bản ghi trong db
                        vanBan.TrangThai = 0;
                        vanBan.NguoiSua = per.NguoiDung.IdNguoiDung;
                        vanBan.NgaySua = DateTime.Now;
                    };
                    db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                    {
                        TenModule = "Số hóa hồ sơ",
                        ThaoTac = "Thêm mới",
                        NoiDungChiTiet = "Gán biểu mẫu cho nhiều văn bản",

                        NgayTao = DateTime.Now,
                        IdNguoiDung = per.NguoiDung.IdNguoiDung,
                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                    });
                    db.SaveChanges();
                    scope.Commit();
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
        public JsonResult digitizing()
        {
            string status = "success";
            string mess = "Số hóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_vb = Request.Form["vb"];
                    tbHoSo_VanBanExtend vanBan = JsonConvert.DeserializeObject<tbHoSo_VanBanExtend>(str_vb) ?? new tbHoSo_VanBanExtend();
                    if (vanBan == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        // Cập nhật idBieuMau cho văn bản
                        tbHoSo_VanBan vanBan_OLD = db.tbHoSo_VanBan.Find(vanBan.IdVanBan);
                        vanBan_OLD.IdBieuMau = vanBan.IdBieuMau;
                        vanBan_OLD.NgaySua = DateTime.Now;
                        vanBan_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                        #region Thêm dữ liệu số
                        // Xóa dữ liệu số của biểu mẫu này trong văn bản - Nếu muốn xóa hết dữ liệu số thì bỏ điều kiện IdBieuMau đi
                        string deleteSQL = $@"update tbHoSo_VanBan_DuLieuSo
                        set TrangThai = 0
                        where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdVanBan = {vanBan_OLD.IdVanBan} and IdBieuMau = {vanBan_OLD.IdBieuMau}";
                        db.Database.ExecuteSqlCommand(deleteSQL);
                        // Cập nhật trạng thái bản ghi
                        List<tbBieuMau_TruongDuLieuExtend> truongDuLieus = vanBan.BieuMau.TruongDuLieus;
                        foreach (tbBieuMau_TruongDuLieuExtend truongDuLieu in truongDuLieus)
                        {
                            foreach (tbHoSo_VanBan_DuLieuSo duLieuSo in truongDuLieu.DuLieuSos)
                            {
                                if (duLieuSo.IdDuLieuSo != 0)
                                {
                                    tbHoSo_VanBan_DuLieuSo duLieuSo_OLD = db.tbHoSo_VanBan_DuLieuSo.Find(duLieuSo.IdDuLieuSo);
                                    duLieuSo_OLD.DuLieuSo = duLieuSo.DuLieuSo.Trim();
                                    duLieuSo_OLD.TrangSo = duLieuSo.TrangSo;

                                    duLieuSo_OLD.TrangThai = 1;
                                    duLieuSo_OLD.NgaySua = DateTime.Now;
                                    duLieuSo_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                                }
                                else
                                {
                                    tbHoSo_VanBan_DuLieuSo duLieuSo_NEW = new tbHoSo_VanBan_DuLieuSo
                                    {
                                        IdHoSo = vanBan.IdHoSo,
                                        IdVanBan = vanBan.IdVanBan,
                                        IdBieuMau = vanBan.IdBieuMau,
                                        IdTruongDuLieu = duLieuSo.IdTruongDuLieu,
                                        DuLieuSo = duLieuSo.DuLieuSo.Trim(),
                                        Nhom = duLieuSo.Nhom,
                                        TrangSo = duLieuSo.TrangSo,

                                        TrangThai = 1,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung,
                                        NgayTao = DateTime.Now,
                                        NguoiTao = per.NguoiDung.IdNguoiDung,
                                    };
                                    db.tbHoSo_VanBan_DuLieuSo.Add(duLieuSo_NEW);
                                }
                            }
                        };
                        #endregion
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Số hóa hồ sơ",
                            ThaoTac = "Số hóa",
                            NoiDungChiTiet = "Số hóa văn bản",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
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
        public ActionResult chuyenFileKySoSang()
        {
            string duongDanVanBan_BANDAU = Request.Form["duongDanVanBan_BANDAU"]; // Đường dẫn file gốc chưa ký số
            string duongDanVanBan_BANDAU_SERVER = Request.MapPath(duongDanVanBan_BANDAU); // Đường dẫn file gốc chưa ký số

            string tenVanBan_KYSO = Path.GetFileName(Request.Form["tenVanBan_KYSO"]); // Do phần response của FileUploadHandle đéo trả ra như thiết lập nên phải cắt tên tệp từ đường dẫn
            string duongDanThuMuc_KYSO = Request.Form["duongDanThuMuc_KYSO"];
            string duongDanVanBan_KYSO = string.Format("{0}/{1}", duongDanThuMuc_KYSO, tenVanBan_KYSO);
            string duongDanVanBan_KYSO_SERVER = Request.MapPath(duongDanVanBan_KYSO);

            try
            {
                if (System.IO.File.Exists(duongDanVanBan_BANDAU_SERVER))
                    System.IO.File.Delete(duongDanVanBan_BANDAU_SERVER);
                System.IO.File.Move(duongDanVanBan_KYSO_SERVER, duongDanVanBan_BANDAU_SERVER);


                return Json(new
                {
                    status = "success",
                    mess = "Ký số thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    mess = $"Ký số thất bại: {ex.Message}",
                }, JsonRequestBehavior.AllowGet);
            };
        }
        public ActionResult chuyenFileKySoHangLoatSang()
        {
            try
            {
                string str_tepVanBans = Request.Form["str_tepVanBans"]; // Đường dẫn file gốc chưa ký số

                List<TepVanBanKySoHangLoatM> tepVanBans = JsonConvert.DeserializeObject<List<TepVanBanKySoHangLoatM>>(str_tepVanBans);
                foreach (TepVanBanKySoHangLoatM tepVanBan in tepVanBans)
                {
                    string duongDanVanBan_BANDAU = tepVanBan.FileName;
                    string duongDanVanBan_BANDAU_SERVER = Request.MapPath(tepVanBan.FileName); // Đường dẫn file gốc chưa ký số

                    string tenVanBan_KYSO = Path.GetFileName(duongDanVanBan_BANDAU); // Do phần response của FileUploadHandle đéo trả ra như thiết lập nên phải cắt tên tệp từ đường dẫn
                    string duongDanThuMuc_KYSO = tepVanBan.FileSignedURL;
                    string duongDanVanBan_KYSO = string.Format("{0}/{1}", duongDanThuMuc_KYSO, tenVanBan_KYSO);
                    string duongDanVanBan_KYSO_SERVER = Request.MapPath(duongDanVanBan_KYSO);

                    if (System.IO.File.Exists(duongDanVanBan_BANDAU_SERVER))
                        System.IO.File.Delete(duongDanVanBan_BANDAU_SERVER);
                    System.IO.File.Move(duongDanVanBan_KYSO_SERVER, duongDanVanBan_BANDAU_SERVER);
                };
                return Json(new
                {
                    status = "success",
                    mess = "Ký số thành công",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "error",
                    mess = $"Ký số thất bại: {ex.Message}",
                }, JsonRequestBehavior.AllowGet);
            };
        }
        #endregion
        #region Excel
        [HttpPost]
        public ActionResult upload_Excel_DuLieuSo(HttpPostedFileBase[] files)
        {
            string status = "success";
            string mess = "Thêm mới tệp thành công";
            try
            {
                if (files == null)
                {
                    status = "error";
                    mess = "Tệp đính kèm sai định dạng";
                }
                else
                {
                    foreach (HttpPostedFileBase f in files)
                    {
                        #region Đọc file
                        var workBook = new XLWorkbook(f.InputStream);
                        var workSheets = workBook.Worksheets;
                        EXCEL_DULIEUSOs_UPLOAD = new List<tbHoSoExtend>();
                        BIEUMAUs_EXCEL = new List<tbBieuMauExtend>();
                        foreach (var sheet in workSheets)
                        {
                            // Kiểm tra sheet có thuộc danh sách biểu mẫu
                            tbBieuMauExtend bieuMau = BIEUMAUs.FirstOrDefault(x => x.TenBieuMau == sheet.Name);
                            if (bieuMau != null)
                            {
                                /**
                                 * Xóa bảng đang có vì nó chiếm vùng dữ liệu nhưng không đầy đủ
                                 * Bảng này chỉ chưa dữ liệu được tạo mặc định trong hàm download
                                 * (Trước đó phải chuyển đổi chuỗi tên table cho giống kiểu chuỗi tên sheet)
                                 */
                                sheet.Tables.Remove(sheet.Name.Replace(" ", String.Empty));
                                var table = sheet.RangeUsed().AsTable(); // Tạo bảng mới trên vùng dữ liệu đầy đủ
                                int nhom = 1;
                                bool coDuLieu = false;
                                foreach (var row in table.DataRange.Rows())
                                {
                                    if (!row.IsEmpty())
                                    {
                                        coDuLieu = true;
                                        tbHoSoExtend hoSo = new tbHoSoExtend();
                                        hoSo.MaHoSo = row.Field("Mã-hồ-sơ").GetString().Trim();
                                        hoSo.VanBans.Add(new tbHoSo_VanBanExtend
                                        {
                                            TenVanBan = row.Field("Tên-tệp").GetString().Trim(),
                                            // Gán biểu mẫu và danh sách trường dữ liệu cho hoSo
                                            BieuMau = get_BieuMaus(loai: "single", idBieuMaus: bieuMau.IdBieuMau.ToString()).FirstOrDefault()
                                        });
                                        List<tbBieuMau_TruongDuLieuExtend> truongDuLieus = hoSo.VanBans[0].BieuMau.TruongDuLieus;
                                        foreach (tbBieuMau_TruongDuLieuExtend truongDuLieu in truongDuLieus)
                                        {
                                            truongDuLieu.DuLieuSos.Add(new tbHoSo_VanBan_DuLieuSo
                                            {
                                                IdHoSo = hoSo.IdHoSo,
                                                IdVanBan = hoSo.VanBans[0].IdVanBan,
                                                IdTruongDuLieu = truongDuLieu.IdTruongDuLieu,
                                                IdBieuMau = bieuMau.IdBieuMau,
                                                DuLieuSo = row.Field(truongDuLieu.TenTruong).GetString().Trim(),
                                                Nhom = nhom, // Chọn chỗ này
                                                TrangSo = row.Field("Trang-số").GetString() == "" ? 1 : (int)row.Field("Trang-số").GetDouble(),
                                            });
                                        }
                                        EXCEL_DULIEUSOs_UPLOAD.Add(hoSo);
                                        nhom++;
                                    }
                                };
                                // Nếu sheeet có dữ liệu thì thêm biểu mẫu của sheet
                                if (coDuLieu)
                                {
                                    BIEUMAUs_EXCEL.Add(bieuMau);
                                };
                            };
                        };
                        #endregion
                    };
                };
            }
            catch (Exception ex)
            {
                status = "error";
                mess = ex.Message;
            }
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult download_Excel_DuLieuSo()
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Tạo excel
            using (var workBook = new XLWorkbook())
            {
                #region Tạo sheet hồ sơ cho từng biểu mẫu
                foreach (var bieuMau in BIEUMAUs)
                {
                    DataTable tbDuLieuSo = new DataTable();
                    tbDuLieuSo.Columns.Add("Mã-hồ-sơ", typeof(string)); // 1
                    tbDuLieuSo.Columns.Add("Tên-tệp", typeof(string)); // 2
                    foreach (var truongDuLieu in bieuMau.TruongDuLieus)
                    {
                        tbDuLieuSo.Columns.Add(truongDuLieu.TenTruong, typeof(string));
                    };
                    tbDuLieuSo.Columns.Add("Trang-số", typeof(int)); // 2
                    // Thêm dữ liệu
                    int EXCEL_DULIEUSOs_DOWNLOAD_COUNT = EXCEL_DULIEUSOs_DOWNLOAD.Count;
                    for (int hang = 0; hang < EXCEL_DULIEUSOs_DOWNLOAD_COUNT; hang++)
                    {
                        // Lấy dữ liệu dòng
                        tbHoSoExtend hoSo = EXCEL_DULIEUSOs_DOWNLOAD[hang];
                        // Thêm 1 dòng
                        if (tbDuLieuSo.Rows.Count <= hang) tbDuLieuSo.Rows.Add(tbDuLieuSo.NewRow());
                        // Dữ liệu cột - Mã hồ sơ
                        tbDuLieuSo.Rows[hang][0] = hoSo.MaHoSo;
                        if (hoSo.VanBans.Count > 0)
                        {
                            // Dữ liệu cột - Tên văn bản
                            tbDuLieuSo.Rows[hang][1] = hoSo.VanBans.FirstOrDefault().TenVanBan;
                            // Dữ liệu cột - Trường dữ liệu
                            int soLuongTruongDuLieu = bieuMau.TruongDuLieus.Count;
                            for (int cot = 0; cot < soLuongTruongDuLieu; cot++)
                            {
                                int? trangSo = hoSo.VanBans.FirstOrDefault().BieuMau.TruongDuLieus[cot].DuLieuSos.FirstOrDefault().TrangSo;
                                string duLieuSo = hoSo.VanBans.FirstOrDefault().BieuMau.TruongDuLieus[cot].DuLieuSos.FirstOrDefault().DuLieuSo;
                                tbDuLieuSo.Rows[hang][cot + 2] = duLieuSo; // Dữ liệu số
                                tbDuLieuSo.Rows[hang][soLuongTruongDuLieu + 2] = trangSo; // Trang số
                            };
                        };
                    };
                    workBook.Worksheets.Add(tbDuLieuSo, bieuMau.TenBieuMau);
                    for (int i = 1; i <= tbDuLieuSo.Rows.Count + 1; i++)
                    {
                        // Number
                        workBook.Worksheet(bieuMau.TenBieuMau).Cell(i, (bieuMau.TruongDuLieus.Count + 3)).CreateDataValidation().Decimal.GreaterThan(0);
                    };
                };
                #endregion
                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Position = 0;
                workBook.SaveAs(memoryStream);
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode("DULIEUSO.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/DocumentDigitizing/Index");
        }
        public ActionResult save_Excel_DuLieuSo()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (EXCEL_DULIEUSOs_DOWNLOAD.Count == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        // Gom nhóm hồ sơ
                        foreach (tbHoSoExtend hoSo_NEW in EXCEL_DULIEUSOs_DOWNLOAD)
                        {
                            if (hoSo_NEW.MaHoSo != "")
                            {
                                // Tìm hồ sơ
                                tbHoSo hoSo_OLD = db.tbHoSoes.FirstOrDefault(x => x.MaHoSo == hoSo_NEW.MaHoSo && x.TrangThai == 1 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                                if (hoSo_OLD != null)
                                {
                                    foreach (tbHoSo_VanBanExtend vanBan_NEW in hoSo_NEW.VanBans)
                                    {
                                        // Kiểm tra hồ sơ và văn bản
                                        if (vanBan_NEW.TenVanBan != "")
                                        {
                                            vanBan_NEW.TenVanBan = Public.Handle.ConvertToUnSign(s: vanBan_NEW.TenVanBan, khoangCach: "-");
                                            vanBan_NEW.TenVanBan = Path.GetFileNameWithoutExtension(vanBan_NEW.TenVanBan);
                                            // Cập nhật idBieuMau cho văn bản
                                            tbHoSo_VanBan vanBan_OLD = db.tbHoSo_VanBan.FirstOrDefault(x =>
                                            x.TenVanBan == vanBan_NEW.TenVanBan &&
                                            x.IdHoSo == hoSo_OLD.IdHoSo &&
                                            x.TrangThai == 1 &&
                                            x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                                            if (vanBan_OLD != null)
                                            {
                                                vanBan_OLD.IdBieuMau = vanBan_NEW.BieuMau.IdBieuMau;
                                                vanBan_OLD.NgaySua = DateTime.Now;
                                                vanBan_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                                                #region Thêm dữ liệu số
                                                // Xóa dữ liệu số của biểu mẫu này trong văn bản - Nếu muốn xóa hết dữ liệu số thì bỏ điều kiện IdBieuMau đi
                                                string deleteSQL = $@"update tbHoSo_VanBan_DuLieuSo
                                                set TrangThai = 0
                                                where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdVanBan = {vanBan_OLD.IdVanBan} and IdBieuMau = {vanBan_OLD.IdBieuMau}";
                                                db.Database.ExecuteSqlCommand(deleteSQL);
                                                // Cập nhật trạng thái bản ghi
                                                List<tbBieuMau_TruongDuLieuExtend> truongDuLieus = vanBan_NEW.BieuMau.TruongDuLieus;
                                                foreach (tbBieuMau_TruongDuLieuExtend truongDuLieu in truongDuLieus)
                                                {
                                                    foreach (tbHoSo_VanBan_DuLieuSo duLieuSo in truongDuLieu.DuLieuSos)
                                                    {
                                                        if (duLieuSo.IdDuLieuSo != 0)
                                                        {
                                                            tbHoSo_VanBan_DuLieuSo duLieuSo_OLD = db.tbHoSo_VanBan_DuLieuSo.Find(duLieuSo.IdDuLieuSo);
                                                            duLieuSo_OLD.DuLieuSo = duLieuSo.DuLieuSo.Trim();
                                                            duLieuSo_OLD.TrangSo = duLieuSo.TrangSo;

                                                            duLieuSo_OLD.TrangThai = 1;
                                                            duLieuSo_OLD.NgaySua = DateTime.Now;
                                                            duLieuSo_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                                                        }
                                                        else
                                                        {
                                                            tbHoSo_VanBan_DuLieuSo duLieuSo_NEW = new tbHoSo_VanBan_DuLieuSo
                                                            {
                                                                IdHoSo = hoSo_OLD.IdHoSo,
                                                                IdVanBan = vanBan_OLD.IdVanBan,
                                                                IdBieuMau = vanBan_OLD.IdBieuMau,
                                                                IdTruongDuLieu = truongDuLieu.IdTruongDuLieu,
                                                                DuLieuSo = duLieuSo.DuLieuSo.Trim(),
                                                                Nhom = duLieuSo.Nhom,
                                                                TrangSo = duLieuSo.TrangSo,

                                                                TrangThai = 1,
                                                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung,
                                                                NgayTao = DateTime.Now,
                                                                NguoiTao = per.NguoiDung.IdNguoiDung,
                                                            };
                                                            db.tbHoSo_VanBan_DuLieuSo.Add(duLieuSo_NEW);
                                                        }
                                                    }
                                                };
                                                #endregion
                                            };
                                        };
                                    };
                                };
                            };
                        };
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Số hóa hồ sơ",
                            ThaoTac = "Số hóa",
                            NoiDungChiTiet = "Số hóa văn bản bằng tệp",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        db.SaveChanges();
                        scope.Commit();
                    }
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                };
            };
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public void get_DuLieuSos_download()
        {
            string loaiTaiXuong = Request.Form["loaiTaiXuong"];
            string str_hoSos = Request.Form["hoSos"];
            int idHoSo = int.Parse(Request.Form["idHoSo"]);
            tbHoSoExtend hoSo = db.Database.SqlQuery<tbHoSoExtend>($@"
                        select * from tbHoSo where IdHoSo = {idHoSo}").FirstOrDefault() ?? new tbHoSoExtend();
            EXCEL_DULIEUSOs_DOWNLOAD = new List<tbHoSoExtend>() {
                new tbHoSoExtend {
                MaHoSo = hoSo.MaHoSo,
            }
            };
            if (loaiTaiXuong == "data")
            {
                EXCEL_DULIEUSOs_DOWNLOAD = JsonConvert.DeserializeObject<List<tbHoSoExtend>>(str_hoSos ?? "", DATETIMECONVERTER) ?? new List<tbHoSoExtend>();
                if (EXCEL_DULIEUSOs_DOWNLOAD.Count == 0)
                {
                    EXCEL_DULIEUSOs_DOWNLOAD.Add(new tbHoSoExtend
                    {
                        MaHoSo = hoSo.MaHoSo,
                    });
                };
            }
        }
        public ActionResult getList_Excel_DuLieuSo(string loai)
        {
            List<tbBieuMauExtend> bieuMaus = new List<tbBieuMauExtend>();
            if (loai == "reload")
            {
                EXCEL_DULIEUSOs_UPLOAD = new List<tbHoSoExtend>();
                bieuMaus = BIEUMAUs;
            }
            else
            {
                bieuMaus = BIEUMAUs_EXCEL;
                // Nếu không sheet nào có dữ liệu thì khôi phục danh sách biểu mẫu để không bị trống
                if (BIEUMAUs_EXCEL.Count == 0) bieuMaus = BIEUMAUs;
            }
            ViewBag.loai = loai;
            ViewBag.bieuMaus = bieuMaus;
            return PartialView($"{VIEW_PATH}/documentdigitizing-excel.dulieuso/excel.dulieuso-getList.cshtml");
        }
        #endregion
        #region export2Office
        //public ActionResult exportToOffice(int idHoSo, int idVanBan, string loaiXuat = "pdf") {
        //    Response.Clear();
        //    Response.ClearHeaders();
        //    try {
        //        tbHoSo_VanBanExtend vanBan = get_HoSo(idHoSo: idHoSo, loai: "single", idVanBans: idVanBan.ToString()).VanBans.FirstOrDefault() ?? new tbHoSo_VanBanExtend();
        //        if (vanBan.Loai.Contains("doc") || vanBan.Loai.Contains("xls") || vanBan.Loai.Contains("pdf")) {
        //            /* 
        //             * Chỉ tệp word, excel, pdf mới gọi được vào hàm này
        //             * Tạo đối tượng Document từ file PDF
        //            */
        //            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(Request.MapPath(vanBan.DuongDan)); // Lấy theo đường dẫn tệp PDF
        //            MemoryStream memoryStream = new MemoryStream();
        //            memoryStream.Position = 0;
        //            if (loaiXuat == "word") {
        //                // Ban đầu cùng loại thì lấy file gốc rồi tải về
        //                if (vanBan.Loai.Contains("doc")) {
        //                    FileStream fileStream = new FileStream(Request.MapPath(vanBan.DuongDanBanDau), FileMode.Open);
        //                    fileStream.CopyTo(memoryStream);
        //                    fileStream.Close();
        //                } else {
        //                    Aspose.Words.Saving.DocSaveOptions saveOptions = new Aspose.Words.Saving.DocSaveOptions();
        //                    saveOptions.SaveFormat = DocSaveOptions.DocFormat.DocX; // Định dạng đầu ra là DocX (Word)
        //                    pdfDocument.Save(memoryStream, options: saveOptions);

        //                };
        //                // Thiết lập các tiêu đề phản hồi HTTP để yêu cầu trình duyệt tải xuống
        //                downloadDialog(data: memoryStream, fileName: Server.UrlEncode($"{vanBan.TenVanBan}.docx"), contentType: "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        //            } else if (loaiXuat == "excel") {
        //                // Ban đầu cùng loại thì lấy file gốc rồi tải về
        //                if (vanBan.Loai.Contains("xls")) {
        //                    FileStream fileStream = new FileStream(Request.MapPath(vanBan.DuongDanBanDau), FileMode.Open);
        //                    fileStream.CopyTo(memoryStream);
        //                    fileStream.Close();
        //                } else {
        //                    ExcelSaveOptions saveOptions = new ExcelSaveOptions();
        //                    saveOptions.Format = ExcelSaveOptions.ExcelFormat.XLSX; // Định dạng đầu ra là Xlsx (Excel)
        //                    pdfDocument.Save(memoryStream, options: saveOptions);
        //                };
        //                // Thiết lập các tiêu đề phản hồi HTTP để yêu cầu trình duyệt tải xuống
        //                downloadDialog(data: memoryStream, fileName: Server.UrlEncode($"{vanBan.TenVanBan}.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //            } else {
        //                // Nếu xuất ra PDF thì lấy đúng đường dẫn để xuất luôn vì nó đã là PDF rồi
        //                FileStream fileStream = new FileStream(Request.MapPath(vanBan.DuongDan), FileMode.Open);
        //                fileStream.CopyTo(memoryStream);
        //                fileStream.Close();
        //                // Thiết lập các tiêu đề phản hồi HTTP để yêu cầu trình duyệt tải xuống
        //                downloadDialog(data: memoryStream, fileName: Server.UrlEncode($"{vanBan.TenVanBan}.pdf"), contentType: "application/pdf");
        //            };
        //        };
        //        return Redirect($"/DocumentDigitizing/?idHoSo={idHoSo}");

        //    } catch (Exception ex) {
        //        string mess = ex.Message;
        //        return Redirect($"/DocumentDigitizing/?idHoSo={idHoSo}");
        //    }
        //}
        #endregion
    }
}