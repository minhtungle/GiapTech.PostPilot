using Applications.QuanLyLuong.Dtos;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.EMMA;
using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Public.Controllers;
using Public.Models;
using QuanLyLuong.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;

namespace QuanLyLuong.Controllers
{
    public class QuanLyLuongController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyLuong";
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
        private List<default_tbChucVu> CHUCVUs
        {
            get
            {
                return Session["CHUCVUs"] as List<default_tbChucVu> ?? new List<default_tbChucVu>();
            }
            set
            {
                Session["CHUCVUs"] = value;
            }
        }
        #endregion
        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyLuong").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #region Chức vụ
            List<default_tbChucVu> chucVus = db.default_tbChucVu.Where(x => x.TrangThai != 0
            //&& (x.MaChucVu == "GV" || x.MaChucVu == "NVKD")
            ).ToList() ?? new List<default_tbChucVu>();
            #endregion

            #endregion

            THAOTACs = thaoTacs;
            CHUCVUs = chucVus;

            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;

            return View($"{VIEW_PATH}/quanlyluong.cshtml");
        }
        [HttpPost]
        public JsonResult getList(LocThongTinDto locThongTin)
        {
            IQueryable<tbNguoiDungExtend> nguoiDungs = get_NguoiDungs(loai: "all", locThongTin: locThongTin);
            return Json(new
            {
                data = nguoiDungs
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

        #region Lấy danh sách dữ liệu
        public IQueryable<tbNguoiDungExtend> get_NguoiDungs(
            string loai = "all", bool layThongTinPhu = true, List<Guid> idNguoiDungs = null,
            LocThongTinDto locThongTin = null)
        {
            IQueryable<tbNguoiDungExtend> nguoiDungs = null;
            var nds = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();
            var tienLuongs = db.tbNguoiDung_TienLuong.Where(x => x.TrangThai != 0
            && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
            && x.ThoiGian == locThongTin.ThoiGian).ToList() ?? new List<tbNguoiDung_TienLuong>();
            nguoiDungs = (from nd in nds
                          join cv in CHUCVUs on nd.IdChucVu equals cv.IdChucVu into cvGroup

                          from cv in cvGroup.DefaultIfEmpty() // LEFT JOIN với bảng CHUCVUs
                          join tl in tienLuongs on nd.IdNguoiDung equals tl.IdNguoiDung into tlGroup

                          from tl in tlGroup.DefaultIfEmpty() // LEFT JOIN với bảng tbTienLuong
                          where (locThongTin.ChucVu == "GV" ? cv.MaChucVu == "GV" : cv.MaChucVu == "NVKD")
                          orderby nd.NgayTao descending
                          select new tbNguoiDungExtend
                          {
                              IdNguoiDung = nd.IdNguoiDung,
                              TenNguoiDung = nd.TenNguoiDung,
                              TienLuong = new tbNguoiDung_TienLuong
                              {
                                  IdNguoiDung_TienLuong = tl != null ? tl.IdNguoiDung_TienLuong : Guid.Empty,
                                  TongTienLuong = tl != null ? tl.TongTienLuong : null,
                              },
                              ChucVu = new default_tbChucVu
                              {
                                  MaChucVu = cv != null ? cv.MaChucVu : "",
                                  TenChucVu = cv != null ? cv.TenChucVu : ""
                              },
                          }).AsQueryable();

            if (locThongTin != null)
            {
                if (locThongTin.DaTinhLuong)
                    nguoiDungs = nguoiDungs.Where(x => x.TienLuong.IdNguoiDung_TienLuong != Guid.Empty).AsQueryable();
                else nguoiDungs = nguoiDungs.Where(x => x.TienLuong.IdNguoiDung_TienLuong == Guid.Empty).AsQueryable();

                if (locThongTin.ChucVu == "GV")
                    nguoiDungs = nguoiDungs.Where(x => x.ChucVu.MaChucVu == "GV").AsQueryable();
                else nguoiDungs = nguoiDungs.Where(x => x.ChucVu.MaChucVu == "NVKD").AsQueryable();
            };

            if (loai == "single") nguoiDungs = nguoiDungs.Where(x => idNguoiDungs.Contains(x.IdNguoiDung)).AsQueryable();

            return nguoiDungs;
        }
        #endregion

        #region CRUD
        [HttpPost]
        public ActionResult displayModal_XemChiTiet_NguoiDung(Guid idNguoiDung, LocThongTinDto locThongTin)
        {
            tbNguoiDungExtend nguoiDung = get_NguoiDungs(loai: "single", idNguoiDungs: new List<Guid> { idNguoiDung }, locThongTin: locThongTin).FirstOrDefault();
            if (locThongTin.ChucVu == "GV")
            {
                // Chuyển đổi mốc thời gian thành đầu tháng và cuối tháng
                DateTime dauThang = DateTime.ParseExact(locThongTin.ThoiGian, "MM/yyyy", null);
                DateTime cuoiThang = dauThang.AddMonths(1).AddDays(-1); // Ngày cuối cùng của tháng

                // Lấy các buổi học giáo viên đó tham gia trong thời gian được chọn
                var buoiHocRepo = db.tbLopHoc_BuoiHoc.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                && x.TrangThai != 0 && x.ThoiGianBatDau >= dauThang && x.ThoiGianBatDau <= cuoiThang
                && x.IdGiaoVien.Contains(idNguoiDung.ToString())).ToList();
                var lopHocRepo = db.tbLopHocs.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                && x.TrangThai != 0).ToList();
                nguoiDung.LopHocs = (from bh in buoiHocRepo
                                     join lh in lopHocRepo on bh.IdLopHoc equals lh.IdLopHoc
                                     orderby lh.NgayTao descending
                                     group new { bh, lh } by bh.IdLopHoc into grouped
                                     select new tbLopHocExtend
                                     {
                                         IdLopHoc = (Guid)grouped.Key,
                                         TenLopHoc = grouped.FirstOrDefault().lh.TenLopHoc,
                                         BuoiHocs = grouped.Select(x => x.bh).ToList(),
                                     }
                            ).ToList();

                var model = new NguoiDungCanTinhTongLuongM
                {
                    ThoiGian = locThongTin.ThoiGian,
                };
                model.NguoiDungs.Add(nguoiDung);
                string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/giaovien/giaovien-xemchitiet.cshtml", model);
                return Json(new
                {
                    view = viewAsString,
                    model
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView($"{VIEW_PATH}/nhanvienkinhdoanh/nhanvienkinhdoanh-crud.cshtml");
            };
        }
        [HttpPost]
        public ActionResult displayModal_TinhTongLuong()
        {
            List<Guid> idNguoiDungs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idNguoiDungs"]).ToList();
            LocThongTinDto locThongTin = JsonConvert.DeserializeObject<LocThongTinDto>(Request.Form["locThongTin"]);

            // Chuyển đổi mốc thời gian thành đầu tháng và cuối tháng
            DateTime dauThang = DateTime.ParseExact(locThongTin.ThoiGian, "MM/yyyy", null);
            DateTime cuoiThang = dauThang.AddMonths(1).AddDays(-1); // Ngày cuối cùng của tháng

            var nguoiDungs = get_NguoiDungs(loai: "single", idNguoiDungs: idNguoiDungs, locThongTin: locThongTin);
            if (locThongTin.ChucVu == "GV")
            {
                var model = new NguoiDungCanTinhTongLuongM
                {
                    ThoiGian = locThongTin.ThoiGian,
                    NguoiDungs = new List<tbNguoiDungExtend>(),
                };
                foreach (var nguoiDung in nguoiDungs)
                {
                    // Lấy các buổi học giáo viên đó tham gia trong thời gian được chọn
                    var buoiHocRepo = db.tbLopHoc_BuoiHoc.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                    && x.TrangThai != 0 && x.ThoiGianBatDau >= dauThang && x.ThoiGianBatDau <= cuoiThang
                    && x.IdGiaoVien.Contains(nguoiDung.IdNguoiDung.ToString())).ToList();
                    var lopHocRepo = db.tbLopHocs.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                    && x.TrangThai != 0).ToList();

                    nguoiDung.LopHocs = (from bh in buoiHocRepo
                                         join lh in lopHocRepo on bh.IdLopHoc equals lh.IdLopHoc
                                         orderby lh.NgayTao descending
                                         group new { bh, lh } by bh.IdLopHoc into grouped
                                         select new tbLopHocExtend
                                         {
                                             IdLopHoc = (Guid)grouped.Key,
                                             TenLopHoc = grouped.FirstOrDefault().lh.TenLopHoc,
                                             BuoiHocs = grouped.Select(x => x.bh).ToList(),
                                         }
                                ).ToList();
                    var luong = db.tbNguoiDung_TienLuong
                        .FirstOrDefault(x => x.IdNguoiDung == nguoiDung.IdNguoiDung && x.ThoiGian == locThongTin.ThoiGian
                        && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung) ?? new tbNguoiDung_TienLuong();

                    nguoiDung.CongThucTinhLuong = db.tbNguoiDung_TienLuong_CongThuc
                        .FirstOrDefault(x => x.IdNguoiDung_TienLuong == luong.IdNguoiDung_TienLuong
                        && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung) ?? new tbNguoiDung_TienLuong_CongThuc
                        {
                            TienThuongThem = 0,
                            HeSo_ChuaDiemDanh = 0,
                            HeSo_DaDiemDanh = 1,
                            HeSo_HocVienNghiKhongPhep = 0.5M,
                            HeSo_HocVienNghiCoPhep = 0.5M
                        };

                    model.NguoiDungs.Add(nguoiDung);
                };
                string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/quanlyluong-tinhtongluong.cshtml", model);
                return Json(new
                {
                    view = viewAsString,
                    model
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView($"{VIEW_PATH}/nhanvienkinhdoanh/nhanvienkinhdoanh-crud.cshtml");
            };
        }
        [HttpPost]
        public ActionResult tinhTongLuong()
        {
            var nguoiDung = JsonConvert.DeserializeObject<tbNguoiDungExtend>(Request.Form["nguoiDung"]);

            var tienLuong = tinhTongLuong(nguoiDung: nguoiDung);

            return Json(new
            {
                tienLuong
            }, JsonRequestBehavior.AllowGet);
        }
        private TienLuongM tinhTongLuong(tbNguoiDungExtend nguoiDung)
        {
            var tienLuong = new TienLuongM
            {
                TongTienLuong = 0,
                LuongTheoLops = new List<LuongTheoLop>(),
            };
            foreach (var lopHoc in nguoiDung.LopHocs)
            {
                var luongTheoLop = new LuongTheoLop
                {
                    IdLopHoc = lopHoc.IdLopHoc,
                    LuongTheoBuois = new List<LuongTheoBuoi>(),
                    TongTienLuong = 0
                };

                foreach (var buoiHoc in lopHoc.BuoiHocs)
                {
                    var luongTheoBuoi = new LuongTheoBuoi
                    {
                        IdBuoiHoc = buoiHoc.IdLopHoc_BuoiHoc,
                        TongTienLuong = 0
                    };

                    if (buoiHoc.DiemDanh == 1)
                        luongTheoBuoi.TongTienLuong = (long)(buoiHoc.LuongTheoBuoi * nguoiDung.CongThucTinhLuong.HeSo_DaDiemDanh);
                    else if (buoiHoc.DiemDanh == 2)
                        luongTheoBuoi.TongTienLuong = (long)(buoiHoc.LuongTheoBuoi * nguoiDung.CongThucTinhLuong.HeSo_HocVienNghiKhongPhep);
                    else if (buoiHoc.DiemDanh == 3)
                        luongTheoBuoi.TongTienLuong = (long)(buoiHoc.LuongTheoBuoi * nguoiDung.CongThucTinhLuong.HeSo_HocVienNghiCoPhep);

                    luongTheoLop.TongTienLuong += luongTheoBuoi.TongTienLuong;
                    luongTheoLop.LuongTheoBuois.Add(luongTheoBuoi);
                };
                tienLuong.TongTienLuong += luongTheoLop.TongTienLuong;
                tienLuong.LuongTheoLops.Add(luongTheoLop);
            };
            tienLuong.TongTienLuong += nguoiDung.CongThucTinhLuong.TienThuongThem.Value;
            return tienLuong;
        }
        [HttpPost]
        public ActionResult save()
        {
            string status = "success";
            string mess = "Tính lương thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var nguoiDungs = JsonConvert.DeserializeObject<List<tbNguoiDungExtend>>(Request.Form["str_nguoiDungs"] ?? "");
                    var locThongTin = JsonConvert.DeserializeObject<LocThongTinDto>(Request.Form["locThongTin"] ?? "");
                    if (nguoiDungs == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        foreach (var nguoiDung in nguoiDungs)
                        {
                            var tongTienLuong = tinhTongLuong(nguoiDung: nguoiDung);
                            // Lương
                            var nguoiDung_TienLuong_OLD = db.tbNguoiDung_TienLuong
                                .FirstOrDefault(x => x.IdNguoiDung == nguoiDung.IdNguoiDung && x.ThoiGian == locThongTin.ThoiGian);
                            if (nguoiDung_TienLuong_OLD != null)
                            {
                                nguoiDung_TienLuong_OLD.TongTienLuong = tongTienLuong.TongTienLuong;

                                nguoiDung_TienLuong_OLD.TrangThai = 1;
                                nguoiDung_TienLuong_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                nguoiDung_TienLuong_OLD.NgaySua = DateTime.Now;

                                var congThucTinhLuong = db.tbNguoiDung_TienLuong_CongThuc
                                    .FirstOrDefault(x => x.IdNguoiDung_TienLuong == nguoiDung_TienLuong_OLD.IdNguoiDung_TienLuong);
                                if (congThucTinhLuong != null)
                                {
                                    congThucTinhLuong.TienThuongThem = nguoiDung.CongThucTinhLuong.TienThuongThem;
                                    congThucTinhLuong.GhiChu = nguoiDung.CongThucTinhLuong.GhiChu;
                                    congThucTinhLuong.HeSo_ChuaDiemDanh = nguoiDung.CongThucTinhLuong.HeSo_ChuaDiemDanh;
                                    congThucTinhLuong.HeSo_DaDiemDanh = nguoiDung.CongThucTinhLuong.HeSo_DaDiemDanh;
                                    congThucTinhLuong.HeSo_HocVienNghiKhongPhep = nguoiDung.CongThucTinhLuong.HeSo_HocVienNghiKhongPhep;
                                    congThucTinhLuong.HeSo_HocVienNghiCoPhep = nguoiDung.CongThucTinhLuong.HeSo_HocVienNghiCoPhep;

                                    congThucTinhLuong.TrangThai = 1;
                                    congThucTinhLuong.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                    congThucTinhLuong.NgaySua = DateTime.Now;
                                };
                            }
                            else
                            {
                                var nguoiDung_TienLuong_NEW = new tbNguoiDung_TienLuong
                                {
                                    IdNguoiDung_TienLuong = Guid.NewGuid(),
                                    IdNguoiDung = nguoiDung.IdNguoiDung,

                                    TongTienLuong = tongTienLuong.TongTienLuong,
                                    ThoiGian = locThongTin.ThoiGian,

                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbNguoiDung_TienLuong.Add(nguoiDung_TienLuong_NEW);
                                var congThucTinhLuong = new tbNguoiDung_TienLuong_CongThuc
                                {
                                    IdCongThucTinhLuong = Guid.NewGuid(),
                                    IdNguoiDung_TienLuong = nguoiDung_TienLuong_NEW.IdNguoiDung_TienLuong,

                                    TienThuongThem = nguoiDung.CongThucTinhLuong.TienThuongThem,
                                    GhiChu = nguoiDung.CongThucTinhLuong.GhiChu,
                                    HeSo_ChuaDiemDanh = nguoiDung.CongThucTinhLuong.HeSo_ChuaDiemDanh,
                                    HeSo_DaDiemDanh = nguoiDung.CongThucTinhLuong.HeSo_DaDiemDanh,
                                    HeSo_HocVienNghiKhongPhep = nguoiDung.CongThucTinhLuong.HeSo_HocVienNghiKhongPhep,
                                    HeSo_HocVienNghiCoPhep = nguoiDung.CongThucTinhLuong.HeSo_HocVienNghiCoPhep,

                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbNguoiDung_TienLuong_CongThuc.Add(congThucTinhLuong);
                            };
                            db.SaveChanges();
                            //scope.Commit();
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
        #endregion

        #region Excel
        public ActionResult download_Luong_NguoiDung(string chucVu = "GV", string thoiGian = "")
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Tạo excel
            string duongDanTepMau_SERVER = Request.MapPath("/Assets/files/[Mẫu danh sách lương].xlsx");
            using (var workBook = new XLWorkbook(duongDanTepMau_SERVER))
            {
                // Lấy sheet cần chỉnh sửa (ví dụ: Sheet1)
                var sheet = workBook.Worksheet("Sheet1");

                // Tìm dòng trống đầu tiên để ghi dữ liệu mới
                var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
                var newRow = lastRow + 1;

                // Thêm dữ liệu mới
                for (int i = 0; i < 5; i++)
                {
                    sheet.Cell(newRow + i, 1).Value = $"Row {newRow + i}";
                    sheet.Cell(newRow + i, 2).Value = $"Data {i}";
                    sheet.Cell(newRow + i, 3).Value = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");
                };

                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.Position = 0;
                workBook.SaveAs(memoryStream);
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode($"Lương giáo viên tháng {thoiGian}.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/UserAccount/Index");
        }
        #endregion
    }
}