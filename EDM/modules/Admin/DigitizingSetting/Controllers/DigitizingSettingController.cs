using ClosedXML.Excel;
using DigitizingSetting.Models;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitizingSetting.Controllers
{
    public class DigitizingSettingController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_DirectorySetting/DigitizingSetting/";
        private List<tbBieuMauExtend> EXCEL_BIEUMAUs_UPLOAD
        {
            get
            {
                return Session["EXCEL_BIEUMAUs_UPLOAD"] as List<tbBieuMauExtend> ?? new List<tbBieuMauExtend>();
            }
            set
            {
                Session["EXCEL_BIEUMAUs_UPLOAD"] = value;
            }
        }
        private List<default_tbLoaiBieuMau> LOAIBIEUMAUs
        {
            get
            {
                return Session["LOAIBIEUMAUs"] as List<default_tbLoaiBieuMau> ?? new List<default_tbLoaiBieuMau>();
            }
            set
            {
                Session["LOAIBIEUMAUs"] = value;
            }
        }
        private List<tbBieuMauExtend> EXCEL_BIEUMAUs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_BIEUMAUs_DOWNLOAD"] as List<tbBieuMauExtend> ?? new List<tbBieuMauExtend>();
            }
            set
            {
                Session["EXCEL_BIEUMAUs_DOWNLOAD"] = value;
            }
        }
        #endregion
        public ActionResult Index()
        {
            List<default_tbLoaiBieuMau> loaiBieuMaus = db.default_tbLoaiBieuMau.Where(x => x.TrangThai == 1).ToList() ?? new List<default_tbLoaiBieuMau>();
            LOAIBIEUMAUs = loaiBieuMaus;
            return View($"{VIEW_PATH}/digitizingsetting.cshtml");
        }
        [HttpGet]
        public ActionResult getList()
        {
            List<tbBieuMauExtend> bieuMaus = get_BieuMaus(loai: "all");
            return Json(new
            {
                data = bieuMaus
            }, JsonRequestBehavior.AllowGet);
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
        public ActionResult displayModal_CRUD(string loai, int idBieuMau)
        {
            tbBieuMau bieuMau = new tbBieuMau();
            List<default_tbLoaiBieuMau> loaiBieuMaus = new List<default_tbLoaiBieuMau>();
            List<tbBieuMau_TruongDuLieu> bieuMau_TruongDuLieus = new List<tbBieuMau_TruongDuLieu>();
            loaiBieuMaus = db.default_tbLoaiBieuMau.Where(x => x.TrangThai == 1).ToList() ?? new List<default_tbLoaiBieuMau>();
            if (loai != "create" && idBieuMau != 0)
            {
                bieuMau = db.tbBieuMaus.Find(idBieuMau);
                bieuMau_TruongDuLieus = db.tbBieuMau_TruongDuLieu.Where(x => x.IdBieuMau == bieuMau.IdBieuMau && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1).ToList() ?? new List<tbBieuMau_TruongDuLieu>();
            };

            ViewBag.bieuMau = bieuMau;
            ViewBag.bieuMau_TruongDuLieus = bieuMau_TruongDuLieus;
            ViewBag.loaiBieuMaus = loaiBieuMaus;
            ViewBag.loai = loai;
            return PartialView($"{VIEW_PATH}/digitizingsetting-crud.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_Delete()
        {
            List<tbBieuMauExtend> bieuMaus = get_BieuMaus(loai: "all");
            ViewBag.bieuMaus = bieuMaus;
            string str_idBieuMaus_XOA = Request.Form["str_idBieuMaus_XOA"];
            ViewBag.idBieuMaus_XOA = str_idBieuMaus_XOA.Split(',').ToList();
            return PartialView($"{VIEW_PATH}/digitizingsetting-delete.cshtml");
        }
        public List<tbBieuMauExtend> get_BieuMaus(string loai, string idBieuMaus = "")
        {
            List<tbBieuMauExtend> bieuMaus = new List<tbBieuMauExtend>();
            if (loai == "all")
            {
                string getSql = $@"select * from tbBieuMau
                where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai = 1 order by NgayTao desc";
                bieuMaus = db.Database.SqlQuery<tbBieuMauExtend>(getSql).ToList();
            }
            else
            {
                if (idBieuMaus != "")
                {
                    string getSql = $@"select * from tbBieuMau where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai = 1 and IdBieuMau in ({idBieuMaus}) order by NgayTao desc";
                    bieuMaus = db.Database.SqlQuery<tbBieuMauExtend>(getSql).ToList();
                }
            }
            foreach (tbBieuMauExtend bieuMau in bieuMaus)
            {
                bieuMau.LoaiBieuMau = db.default_tbLoaiBieuMau.Find(bieuMau.IdLoaiBieuMau) ?? new default_tbLoaiBieuMau();
            };
            return bieuMaus;
        }
        [HttpPost]
        public tbBieuMauExtend kiemTra_BieuMau(tbBieuMauExtend bieuMau)
        {
            List<string> ketQuas = new List<string>();
            // Kiểm tra còn hồ sơ khác có trùng mã không
            tbBieuMau bieuMau_OLD = db.tbBieuMaus.FirstOrDefault(x => x.TenBieuMau == bieuMau.TenBieuMau && x.IdBieuMau != bieuMau.IdBieuMau && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (bieuMau_OLD != null)
            {
                ketQuas.Add("Biểu mẫu đã tồn tại");
                bieuMau.KiemTraExcel.TrangThai = 0;
            };
            if (bieuMau.TenBieuMau.Length > 30) {
                ketQuas.Add("Tên biểu mẫu không được vượt quá 30 ký tự");
                bieuMau.KiemTraExcel.TrangThai = 0;
            };
            bieuMau.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            return bieuMau;
        }
        [HttpPost]
        public ActionResult create_BieuMau(string str_bieuMau)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbBieuMauExtend bieuMau_NEW = JsonConvert.DeserializeObject<tbBieuMauExtend>(str_bieuMau ?? "");
                    if (bieuMau_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_BieuMau(bieuMau: bieuMau_NEW).KiemTraExcel.TrangThai == 0)
                        {
                            status = "warning";
                            mess = bieuMau_NEW.KiemTraExcel.KetQua;
                        }
                        else
                        {
                            // Tạo biểu mẫu
                            tbBieuMau bieuMau = new tbBieuMau
                            {
                                TenBieuMau = bieuMau_NEW.TenBieuMau,
                                IdLoaiBieuMau = bieuMau_NEW.IdLoaiBieuMau,

                                TrangThai = 1,
                                NguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbBieuMaus.Add(bieuMau);
                            db.SaveChanges();
                            // Tạo trường dữ liệu
                            foreach (tbBieuMau_TruongDuLieu truongDuLieu_NEW in bieuMau_NEW.TruongDuLieus)
                            {
                                tbBieuMau_TruongDuLieu truongDuLieu = new tbBieuMau_TruongDuLieu
                                {
                                    IdBieuMau = bieuMau.IdBieuMau,
                                    //MaTruong = truongDuLieu_NEW.MaTruong,
                                    TenTruong = truongDuLieu_NEW.TenTruong,

                                    TrangThai = 1,
                                    NguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbBieuMau_TruongDuLieu.Add(truongDuLieu);
                            }
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Thiết lập số hóa",
                                ThaoTac = "Thêm mới",
                                NoiDungChiTiet = "Thêm mới biểu mẫu",

                                NgayTao = DateTime.Now,
                                IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            });
                            db.SaveChanges();
                            scope.Commit();
                        };
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
        public ActionResult update_BieuMau(string str_bieuMau)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbBieuMauExtend bieuMau_NEW = JsonConvert.DeserializeObject<tbBieuMauExtend>(str_bieuMau ?? "");
                    if (bieuMau_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_BieuMau(bieuMau: bieuMau_NEW).KiemTraExcel.TrangThai == 0)
                        {
                            status = "warning";
                            mess = bieuMau_NEW.KiemTraExcel.KetQua;
                        }
                        else
                        {
                            // Biểu mẫu
                            tbBieuMau bieuMau_OLD = db.tbBieuMaus.Find(bieuMau_NEW.IdBieuMau);
                            bieuMau_OLD.TenBieuMau = bieuMau_NEW.TenBieuMau;
                            bieuMau_OLD.IdLoaiBieuMau = bieuMau_NEW.IdLoaiBieuMau;

                            bieuMau_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                            bieuMau_OLD.NgaySua = DateTime.Now;
                            #region Trường dữ liệu
                            string capNhat_TruongDuLieu_SQL = $@"
                            -- Lấy chuỗi id trường dữ liệu cũ
                            declare @id_truongDuLieu VARCHAR(MAX);
                            set @id_truongDuLieu = 
                                (select string_agg(IdTruongDuLieu, ',') from tbBieuMau_TruongDuLieu 
                                where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdBieuMau in ({bieuMau_OLD.IdBieuMau}))
                            -- Tạo bảng tạm để chứa các giá trị INT từ chuỗi
                            declare @tblTruongDuLieu table (Id INT);
                            -- Chia chuỗi thành các phần tử và chèn vào bảng tạm
                            INSERT INTO @tblTruongDuLieu (Id)
                            SELECT value FROM STRING_SPLIT(@id_truongDuLieu, ',');

                            -- Xóa trường dữ liệu cũ
                            update tbBieuMau_TruongDuLieu 
                            set 
                                TrangThai = 0, NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                            where 
                                MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdTruongDuLieu in (SELECT Id FROM @tblTruongDuLieu)

                            -- Xóa dữ liệu số cũ
                            update tbHoSo_VanBan_DuLieuSo
                            set 
                                TrangThai = 0 , NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                            where 
                                 MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdTruongDuLieu in (SELECT Id FROM @tblTruongDuLieu)
                            ";
                            db.Database.ExecuteSqlCommand(capNhat_TruongDuLieu_SQL);
                            foreach (tbBieuMau_TruongDuLieu truongDuLieu_NEW in bieuMau_NEW.TruongDuLieus)
                            {
                                // Cập nhật lại
                                var truongDuLieu_OLD = db.tbBieuMau_TruongDuLieu.Find(truongDuLieu_NEW.IdTruongDuLieu);
                                if (truongDuLieu_OLD != null)
                                {
                                    //truongDuLieu_OLD.MaTruong = truongDuLieu_NEW.MaTruong;
                                    truongDuLieu_OLD.TenTruong = truongDuLieu_NEW.TenTruong;
                                    truongDuLieu_OLD.TrangThai = 1; // Khôi phục trường dữ liệu vừa xóa

                                    truongDuLieu_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                                    truongDuLieu_OLD.NgaySua = DateTime.Now;
                                    // Khôi phục dữ liệu số
                                    string capNhat_DuLieuSo_SQL = $@"
                                    update tbHoSo_VanBan_DuLieuSo
                                     set 
                                        TrangThai = 1 , NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                                    where 
                                        MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdTruongDuLieu = {truongDuLieu_OLD.IdTruongDuLieu}
                                    ";
                                }
                                else
                                {
                                    tbBieuMau_TruongDuLieu truongDuLieu = new tbBieuMau_TruongDuLieu
                                    {
                                        IdBieuMau = bieuMau_OLD.IdBieuMau,
                                        //MaTruong = truongDuLieu_NEW.MaTruong,
                                        TenTruong = truongDuLieu_NEW.TenTruong,

                                        TrangThai = 1,
                                        NguoiTao = per.NguoiDung.IdNguoiDung,
                                        NgayTao = DateTime.Now,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                    };
                                    db.tbBieuMau_TruongDuLieu.Add(truongDuLieu);
                                }
                            }
                            #endregion
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Thiết lập số hóa",
                                ThaoTac = "Cập nhật",
                                NoiDungChiTiet = "Cập nhật biểu mẫu",

                                NgayTao = DateTime.Now,
                                IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            });
                            db.SaveChanges();
                            scope.Commit();
                        };
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
        public JsonResult delete_BieuMaus()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idBieuMaus_XOA = Request.Form["str_idBieuMaus_XOA"];
                    List<int> idBieuMaus_XOA = str_idBieuMaus_XOA.Split(',').Select(Int32.Parse).ToList();
                    int idBieuMau_THAYTHE = int.Parse(Request.Form["idBieuMau_THAYTHE"]);
                    if (str_idBieuMaus_XOA != "")
                    {
                        string capNhatSQL = $@"
                        -- Xóa biểu mẫu
                        update tbBieuMau 
                        set 
                            TrangThai = 0, NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                        where 
                            MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdBieuMau in ({str_idBieuMaus_XOA})
                        
                        -- Xóa trường dữ liệu
                        update tbBieuMau_TruongDuLieu
                        set 
                            TrangThai = 0, NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                        where 
                            MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdBieuMau in ({str_idBieuMaus_XOA})

                        -- Cập nhật văn bản
                         update tbHoSo_VanBan
                        set 
                            IdBieuMau = {idBieuMau_THAYTHE}, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' 
                        where 
                            IdBieuMau in({idBieuMau_THAYTHE}) and TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}

                        -- Cập nhật dữ liệu số
                         update tbHoSo_VanBan_DuLieuSo 
                        set 
                            TrangThai = 0, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' 
                        where 
                            MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdBieuMau in ({str_idBieuMaus_XOA})
                        ";
                        db.Database.ExecuteSqlCommand(capNhatSQL);
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Quản lý đơn vị",
                            ThaoTac = "Xóa",
                            NoiDungChiTiet = "Xóa phông lưu trữ và cập nhật phông thay thế cho hồ sơ, vị trí lưu trữ, danh mục hồ sơ đang sử dụng",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        db.SaveChanges();
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Thiết lập số hóa",
                            ThaoTac = "Xóa",
                            NoiDungChiTiet = "Xóa biểu mẫu",

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
        #endregion
        #region Excel
        public tbBieuMauExtend kiemTra_Excel_BieuMau(tbBieuMauExtend bieuMau)
        {
            List<string> ketQuas = new List<string>();
            /**
             * Các thông tin cần kiểm tra
             * Tên biểu mẫu
             * Tên loại biểu mẫu
             * Tên trường
             */
            if (kiemTra_BieuMau(bieuMau: bieuMau).KiemTraExcel.TrangThai == 0)
            {
                ketQuas.Add(bieuMau.KiemTraExcel.KetQua);
                bieuMau.KiemTraExcel.TrangThai = 0;
            };
            if (bieuMau.TenBieuMau == "" || bieuMau.LoaiBieuMau.TenLoaiBieuMau == "" || bieuMau.TruongDuLieus.Count == 0)
            {
                ketQuas.Add("Thiếu thông tin");
                bieuMau.KiemTraExcel.TrangThai = 0;
            };
            bieuMau.KiemTraExcel.KetQua = string.Join(",", ketQuas);
            return bieuMau;
        }
        [HttpPost]
        public ActionResult upload_Excel_BieuMau(HttpPostedFileBase[] files)
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
                        EXCEL_BIEUMAUs_UPLOAD = new List<tbBieuMauExtend>();
                        foreach (var sheet in workSheets)
                        {
                            if (sheet.Name.Contains("BieuMau"))
                            {
                                /**
                                 * Xóa bảng đang có vì nó chiếm vùng dữ liệu nhưng không đầy đủ
                                 * Bảng này chỉ chưa dữ liệu được tạo mặc định trong hàm download
                                 * (Trước đó phải chuyển đổi chuỗi tên table cho giống kiểu chuỗi tên sheet)
                                 */
                                sheet.Tables.Remove(sheet.Name.Replace(" ", String.Empty));
                                var table = sheet.RangeUsed().AsTable(); // Tạo bảng mới trên vùng dữ liệu đầy đủ
                                foreach (var row in table.DataRange.Rows())
                                {
                                    if (!row.IsEmpty())
                                    {
                                        tbBieuMauExtend bieuMau = new tbBieuMauExtend();
                                        bieuMau.TenBieuMau = row.Field("Tên-biểu-mẫu").GetString().Trim();
                                        bieuMau.LoaiBieuMau.TenLoaiBieuMau = row.Field("Tên-loại-biểu-mẫu").GetString().Trim();
                                        string[] tenTruongs = row.Field("Tên-trường-dữ-liệu").GetString().Trim().Split(',');
                                        foreach (string tenTruong in tenTruongs)
                                        {
                                            if (tenTruong != "")
                                            {
                                                bieuMau.TruongDuLieus.Add(new tbBieuMau_TruongDuLieu
                                                {
                                                    TenTruong = tenTruong,
                                                });
                                            };
                                        };
                                        EXCEL_BIEUMAUs_UPLOAD.Add(bieuMau);
                                    };
                                };
                            };
                        };
                        #endregion
                    };
                }
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
        public ActionResult download_Excel_BieuMau()
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Tạo excel
            using (var workBook = new XLWorkbook())
            {
                #region Tạo sheet biểu mẫu
                DataTable tbBieuMau = new DataTable();
                tbBieuMau.Columns.Add("Tên-biểu-mẫu", typeof(string)); // 1
                tbBieuMau.Columns.Add("Tên-loại-biểu-mẫu", typeof(string)); // 2
                tbBieuMau.Columns.Add("Tên-trường-dữ-liệu", typeof(string)); // 3
                #region Thêm dữ liệu
                foreach (tbBieuMauExtend bieuMau in EXCEL_BIEUMAUs_DOWNLOAD)
                {
                    tbBieuMau.Rows.Add(
                       bieuMau.TenBieuMau,
                       bieuMau.LoaiBieuMau.TenLoaiBieuMau,
                       string.Join(",", bieuMau.TruongDuLieus.Select(x => x.TenTruong)));
                };
                int EXCEL_BIEUMAUs_DOWNLOAD_COUNT = EXCEL_BIEUMAUs_DOWNLOAD.Count;
                for (int i = 0; i < EXCEL_BIEUMAUs_DOWNLOAD_COUNT; i++)
                {
                    tbBieuMauExtend bieuMau = EXCEL_BIEUMAUs_DOWNLOAD[i];
                    if (tbBieuMau.Rows.Count <= i)
                        tbBieuMau.Rows.Add(
                           bieuMau.TenBieuMau,
                           bieuMau.LoaiBieuMau.TenLoaiBieuMau,
                           string.Join(",", bieuMau.TruongDuLieus.Select(x => x.TenTruong)));
                };
                #endregion
                #endregion
                #region Tạo sheet danh sách
                DataTable tbDanhSach = new DataTable();
                tbDanhSach.Columns.Add("TenLoaiBieuMau", typeof(string));
                #region Thêm dữ liệu
                int tenLoaiBieuMau_Count = LOAIBIEUMAUs.Count();
                for (int i = 0; i < tenLoaiBieuMau_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][0] = LOAIBIEUMAUs[i].TenLoaiBieuMau;
                };
                #endregion
                #endregion
                #region Tạo file excel
                workBook.Worksheets.Add(tbBieuMau, "BieuMau");
                workBook.Worksheets.Add(tbDanhSach, "DanhSach");
                for (int i = 1; i <= tbBieuMau.Rows.Count + 1; i++)
                {
                    workBook.Worksheets.First().Cell(i, 2).CreateDataValidation().List($"=OFFSET(DanhSach!$A$2,0,0,COUNTA(DanhSach!$A:$A),1)");
                };
                #endregion
                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Position = 0;
                workBook.SaveAs(memoryStream);
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode("BIEUMAU.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/DocumentDigitizing/Index");
        }
        public ActionResult save_Excel_BieuMau()
        {
            string status = "";
            string mess = "";
            List<tbBieuMauExtend> bieuMau_HopLes = new List<tbBieuMauExtend>();
            List<tbBieuMauExtend> bieuMau_KhongHopLes = new List<tbBieuMauExtend>();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (EXCEL_BIEUMAUs_DOWNLOAD.Count == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        foreach (tbBieuMauExtend bieuMau_NEW in EXCEL_BIEUMAUs_DOWNLOAD)
                        {
                            // Kiểm tra excel
                            tbBieuMauExtend bieuMau_KhongHopLe = kiemTra_Excel_BieuMau(bieuMau_NEW);
                            if (bieuMau_KhongHopLe.KiemTraExcel.TrangThai == 0)
                            {
                                bieuMau_KhongHopLes.Add(bieuMau_KhongHopLe);
                            }
                            else
                            {
                                // Kiểm tra tên biểu mẫu này đã được thêm ở bản ghi trước đó chưa
                                if (bieuMau_HopLes.Any(x => x.TenBieuMau == bieuMau_NEW.TenBieuMau))
                                {
                                    bieuMau_NEW.KiemTraExcel.TrangThai = 0;
                                    bieuMau_NEW.KiemTraExcel.KetQua = "Trùng tên biểu mẫu";
                                    bieuMau_KhongHopLes.Add(bieuMau_NEW);
                                }
                                else
                                {
                                    // Tạo biểu mẫu
                                    tbBieuMau bieuMau = new tbBieuMau
                                    {
                                        TenBieuMau = bieuMau_NEW.TenBieuMau,
                                        IdLoaiBieuMau = bieuMau_NEW.IdLoaiBieuMau,

                                        TrangThai = 1,
                                        NguoiTao = per.NguoiDung.IdNguoiDung,
                                        NgayTao = DateTime.Now,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                    };
                                    db.tbBieuMaus.Add(bieuMau);
                                    db.SaveChanges();
                                    // Tạo trường dữ liệu
                                    foreach (tbBieuMau_TruongDuLieu truongDuLieu_NEW in bieuMau_NEW.TruongDuLieus)
                                    {
                                        tbBieuMau_TruongDuLieu truongDuLieu = new tbBieuMau_TruongDuLieu
                                        {
                                            IdBieuMau = bieuMau.IdBieuMau,
                                            TenTruong = truongDuLieu_NEW.TenTruong,

                                            TrangThai = 1,
                                            NguoiTao = per.NguoiDung.IdNguoiDung,
                                            NgayTao = DateTime.Now,
                                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                        };
                                        db.tbBieuMau_TruongDuLieu.Add(truongDuLieu);
                                    };
                                    bieuMau_HopLes.Add(bieuMau_NEW);
                                };
                            };
                        };
                        if (bieuMau_KhongHopLes.Count == 0)
                        { // Thêm bản ghi thành công và không tồn tại bản ghi không hợp lệ
                            status = "success";
                            mess = "Thêm mới bản ghi thành công";
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Thiết lập số hóa",
                                ThaoTac = "Thêm mới",
                                NoiDungChiTiet = "Thêm mới bằng tệp",

                                NgayTao = DateTime.Now,
                                IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            });
                            db.SaveChanges();
                            scope.Commit();

                        }
                        else
                        { // Khi thêm thành công, thay thế EXCEL_BIEUMAUs_UPLOAD bằng bieuMau_KhongHopLes
                            if (bieuMau_KhongHopLes.Count == EXCEL_BIEUMAUs_DOWNLOAD.Count)
                            { // Tất cả đều không hợp lệ
                                status = "error-1";
                                mess = "Thêm mới bản ghi không thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                            }
                            else
                            {
                                status = "warning";
                                mess = "Thêm mới bản ghi [HỢP LỆ] thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                                db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                                {
                                    TenModule = "Thiết lập số hóa",
                                    ThaoTac = "Thêm mới",
                                    NoiDungChiTiet = "Thêm mới bằng tệp",

                                    NgayTao = DateTime.Now,
                                    IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                });
                                db.SaveChanges();
                                scope.Commit();
                            };
                            // Trả lại danh sách bản ghi không hợp lệ
                            EXCEL_BIEUMAUs_UPLOAD = new List<tbBieuMauExtend>();
                            EXCEL_BIEUMAUs_UPLOAD.AddRange(bieuMau_KhongHopLes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = "error-0";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                bieuMau_KhongHopLes,
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public void get_BieuMaus_download()
        {
            string loaiTaiXuong = Request.Form["loaiTaiXuong"];
            string str_bieuMaus = Request.Form["str_bieuMau"];
            EXCEL_BIEUMAUs_DOWNLOAD = new List<tbBieuMauExtend>();
            EXCEL_BIEUMAUs_DOWNLOAD.Add(new tbBieuMauExtend
            {
                TenBieuMau = "Ví dụ ...",
                LoaiBieuMau = new default_tbLoaiBieuMau
                {
                    IdLoaiBieuMau = LOAIBIEUMAUs.FirstOrDefault().IdLoaiBieuMau,
                    TenLoaiBieuMau = LOAIBIEUMAUs.FirstOrDefault().TenLoaiBieuMau
                },
                TruongDuLieus = new List<tbBieuMau_TruongDuLieu> {
                    new tbBieuMau_TruongDuLieu {
                        TenTruong = "Trường 1"
                    },
                    new tbBieuMau_TruongDuLieu {
                        TenTruong = "Trường 2"
                    },
                    new tbBieuMau_TruongDuLieu {
                        TenTruong = "Trường 3"
                    },
                    new tbBieuMau_TruongDuLieu {
                        TenTruong = "Trường 4"
                    },
                    new tbBieuMau_TruongDuLieu {
                        TenTruong = "Trường 5"
                    },
                    new tbBieuMau_TruongDuLieu {
                        TenTruong = "Trường 6"
                    }
                }
            });
            if (loaiTaiXuong == "data")
            {
                EXCEL_BIEUMAUs_DOWNLOAD = JsonConvert.DeserializeObject<List<tbBieuMauExtend>>(str_bieuMaus ?? "") ?? new List<tbBieuMauExtend>();
            };
        }
        public ActionResult getList_Excel_BieuMau(string loai)
        {
            if (loai == "reload") EXCEL_BIEUMAUs_UPLOAD = new List<tbBieuMauExtend>();
            return PartialView($"{VIEW_PATH}/digitizingsetting-excel.bieumau/excel.bieumau-getList.cshtml");
        }
        #endregion
    }
}