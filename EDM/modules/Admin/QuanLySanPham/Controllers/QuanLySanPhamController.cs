using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDM_DB;
using Public.Controllers;
using Public.Models;
using UserType.Models;
using Applications.QuanLySanPham.Models;
using Applications.QuanLySanPham.Dtos;

namespace QuanLySanPham.Controllers
{
    public class QuanLySanPhamController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLySanPham";
        private List<tbSanPham_LoaiSanPham> LOAISANPHAMs
        {
            get
            {
                return Session["LOAISANPHAMs"] as List<tbSanPham_LoaiSanPham> ?? new List<tbSanPham_LoaiSanPham>();
            }
            set
            {
                Session["LOAISANPHAMs"] = value;
            }
        }
        private List<tbLoaiKhoaHoc> LOAIKHOAHOCs
        {
            get
            {
                return Session["LOAIKHOAHOCs"] as List<tbLoaiKhoaHoc> ?? new List<tbLoaiKhoaHoc>();
            }
            set
            {
                Session["LOAIKHOAHOCs"] = value;
            }
        }
        public QuanLySanPhamController()
        {

        }
        #endregion
        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLySanPham").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #region Loại sản phẩm
            List<tbSanPham_LoaiSanPham> loaiSanPhams = db.tbSanPham_LoaiSanPham.Where(x => x.TrangThai != 0).ToList() ?? new List<tbSanPham_LoaiSanPham>();
            #endregion

            #region Loại khóa học
            List<tbLoaiKhoaHoc> loaiKhoaHoc = db.tbLoaiKhoaHocs.Where(x => x.TrangThai != 0).ToList() ?? new List<tbLoaiKhoaHoc>();
            #endregion

            #endregion

            LOAISANPHAMs = loaiSanPhams;
            LOAIKHOAHOCs = loaiKhoaHoc;

            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;
            ViewBag.thaoTacs = thaoTacs;
            return View($"{VIEW_PATH}/quanlysanpham.cshtml");
        }
        [HttpGet]
        public ActionResult getList()
        {
            List<tbSanPhamExtend> sanPhams = get_SanPhams(loai: "all");
            return PartialView($"{VIEW_PATH}/quanlysanpham-getList.cshtml", sanPhams);
        }

        #region Lấy các danh sách dữ liệu
        public List<tbSanPhamExtend> get_SanPhams(string loai = "all", List<Guid> idSanPhams = null, LocThongTinDto locThongTin = null)
        {
            var sanPhamRepo = db.tbSanPhams.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();

            var sanPhams = sanPhamRepo
               .Where(x =>
               (loai != "single" || idSanPhams.Contains(x.IdSanPham)))
               .Join(LOAISANPHAMs,
                    sp => sp.IdLoaiSanPham,
                    lsp => lsp.IdLoaiSanPham,
                    (sp, lsp) => new tbSanPhamExtend
                    {
                        SanPham = sp,
                        LoaiSanPham = lsp
                    })
               .Join(LOAIKHOAHOCs,
                    sp => sp.SanPham.IdLoaiKhoaHoc,
                    lkh => lkh.IdLoaiKhoaHoc,
                    (sp, lkh) => new tbSanPhamExtend
                    {
                        SanPham = sp.SanPham,
                        LoaiKhoaHoc = lkh,
                        LoaiSanPham = sp.LoaiSanPham
                    })
               .OrderByDescending(x => x.SanPham.NgayTao)
               .ThenBy(x => x.SanPham.NgaySua)
               .ToList() ?? new List<tbSanPhamExtend>(); // Đây là kết quả trung gian (var a)

            return sanPhams;
        }
        public List<tbSanPham_LichSuExtend> get_KhachHang_LichSus(tbSanPhamExtend sanPham)
        {
            List<tbSanPham_LichSuExtend> sanPham_LichSus = db.Database.SqlQuery<tbSanPham_LichSuExtend>($@"
                    select * 
                    from tbSanPham_LichSu
                    where IdSanPham = '{sanPham.SanPham.IdSanPham}' and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                    order by NgayTao desc
                    ").ToList() ?? new List<tbSanPham_LichSuExtend>();
            foreach (tbSanPham_LichSuExtend khachHang_LichSu in sanPham.LichSus)
            {
                khachHang_LichSu.ThongTinNguoiTao = db.tbNguoiDungs.FirstOrDefault(x =>
                x.IdNguoiDung == khachHang_LichSu.LichSu.IdNguoiTao && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung) ?? new tbNguoiDung();
            }
            ;
            return sanPham_LichSus;
        }
        #endregion

        #region CRUD
        [HttpPost]
        public ActionResult displayModal_CRUD(string loai, Guid idSanPham)
        {
            #region Thông tin chung sản phẩm
            tbSanPhamExtend sanPham = new tbSanPhamExtend();

            if (loai != "create" && idSanPham != Guid.Empty)
            {
                sanPham = get_SanPhams(loai: "single", idSanPhams: new List<Guid> { idSanPham }).FirstOrDefault();
            }
            ;
            #endregion

            var model = new DisplayModel_CRUD_SanPham_Output_Dto
            {
                SanPham = sanPham,
                Loai = loai
            };
            return PartialView($"{VIEW_PATH}/quanlysanpham-crud.cshtml", model);
        }

        [HttpPost]
        public tbSanPhamExtend kiemTra_SanPham(tbSanPhamExtend sanPham)
        {
            List<string> ketQuas = new List<string>();
            // Kiểm tra còn sản phẩm khác có trùng mã không
            tbSanPham sanPham_OLD = db.tbSanPhams.FirstOrDefault(x =>
            //x.MaSanPham == sanPham.MaSanPham && 
            x.TenSanPham == sanPham.SanPham.TenSanPham && x.IdLoaiSanPham == sanPham.SanPham.IdLoaiSanPham &&
            x.IdSanPham != sanPham.SanPham.IdSanPham &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (sanPham_OLD != null)
            {
                ketQuas.Add("Sản phẩm đã tồn tại");
                sanPham.KiemTraExcel.TrangThai = 0;
            }
            ;
            sanPham.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            return sanPham;
        }
        [HttpPost]
        public ActionResult create_SanPham(string str_SanPham)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbSanPhamExtend sanPham_NEW = JsonConvert.DeserializeObject<tbSanPhamExtend>(str_SanPham);
                    if (sanPham_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_SanPham(sanPham: sanPham_NEW).KiemTraExcel.TrangThai == 0)
                        {
                            status = "warning";
                            mess = sanPham_NEW.KiemTraExcel.KetQua;
                        }
                        else
                        {
                            // Thêm mới
                            tbSanPham sanPham = new tbSanPham
                            {
                                IdSanPham = Guid.NewGuid(),
                                IdLoaiSanPham = sanPham_NEW.SanPham.IdLoaiSanPham,
                                IdLoaiKhoaHoc = sanPham_NEW.SanPham.IdLoaiKhoaHoc,
                                TenSanPham = sanPham_NEW.SanPham.TenSanPham,
                                GiaTienTungBuoi = sanPham_NEW.SanPham.GiaTienTungBuoi,
                                SoBuoi = sanPham_NEW.SanPham.SoBuoi,
                                GiaTien = sanPham_NEW.SanPham.GiaTienTungBuoi * sanPham_NEW.SanPham.SoBuoi,
                                ThoiGianBuoiHoc = sanPham_NEW.SanPham.ThoiGianBuoiHoc,

                                GhiChu = sanPham_NEW.SanPham.GhiChu,
                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbSanPhams.Add(sanPham);
                            db.tbSanPham_LichSu.Add(new tbSanPham_LichSu
                            {
                                IdLichSu = Guid.NewGuid(),
                                IdSanPham = sanPham.IdSanPham,
                                NoiDung = "Khởi tạo sản phẩm",
                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            });
                            db.SaveChanges();
                            scope.Commit();
                        }
                    }
                    ;
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
                ;
            }
            ;
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult update_SanPham(string str_SanPham)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbSanPhamExtend sanPham_NEW = JsonConvert.DeserializeObject<tbSanPhamExtend>(str_SanPham);
                    // Kiểm tra có trùng bản ghi nào khác không
                    if (kiemTra_SanPham(sanPham: sanPham_NEW).KiemTraExcel.TrangThai == 0)
                    {
                        status = "warning";
                        mess = sanPham_NEW.KiemTraExcel.KetQua;
                    }
                    else
                    {
                        tbSanPham sanPham_OLD = db.tbSanPhams.Find(sanPham_NEW.SanPham.IdSanPham);

                        sanPham_OLD.TenSanPham = sanPham_NEW.SanPham.TenSanPham;
                        sanPham_OLD.IdLoaiSanPham = sanPham_NEW.SanPham.IdLoaiSanPham;
                        sanPham_OLD.IdLoaiKhoaHoc = sanPham_NEW.SanPham.IdLoaiKhoaHoc;

                        sanPham_OLD.GiaTienTungBuoi = sanPham_NEW.SanPham.GiaTienTungBuoi;
                        sanPham_OLD.SoBuoi = sanPham_NEW.SanPham.SoBuoi;
                        sanPham_OLD.GiaTien = sanPham_NEW.SanPham.GiaTienTungBuoi * sanPham_NEW.SanPham.SoBuoi;
                        sanPham_OLD.ThoiGianBuoiHoc = sanPham_NEW.SanPham.ThoiGianBuoiHoc;

                        sanPham_OLD.GhiChu = sanPham_NEW.SanPham.GhiChu;
                        sanPham_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        sanPham_OLD.NgaySua = DateTime.Now;

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
        public JsonResult delete_SanPhams()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idSanPhams = Request.Form["str_idSanPhams"];
                    //List<Guid> idSanPhams = str_idSanPhams.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                    List<Guid> idSanPhams = JsonConvert.DeserializeObject<List<Guid>>(str_idSanPhams);
                    foreach (Guid idSanPham in idSanPhams)
                    {
                        tbSanPham sanPham = db.tbSanPhams.Find(idSanPham);

                        // Xóa bản ghi trong db
                        sanPham.TrangThai = 0;
                        sanPham.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        sanPham.NgaySua = DateTime.Now;

                        db.SaveChanges();
                    }
                    ;
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
        #endregion
    }
}