using Applications.QuanLyDoanhThu.Dtos;
using EDM_DB;
using Newtonsoft.Json;
using Organization.Controllers;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserAccount.Models;
using UserType.Models;

namespace QuanLyDoanhThu.Controllers
{
    public class QuanLyDoanhThuController : RouteConfigController
    {
        // GET: QuanLyDoanhThu
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyDoanhThu";
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
        private List<tbNguoiDung> NGUOIDUNGs
        {
            get
            {
                return Session["NGUOIDUNGs"] as List<tbNguoiDung> ?? new List<tbNguoiDung>();
            }
            set
            {
                Session["NGUOIDUNGs"] = value;
            }
        }
        private OrganizationController organizationController = new OrganizationController();
        public QuanLyDoanhThuController()
        {

        }
        #endregion
        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyDoanhThu").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #region Người dùng
            var nguoiDungs = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList() ?? new List<tbNguoiDung>();
            #endregion

            #endregion
            THAOTACs = thaoTacs;
            NGUOIDUNGs = nguoiDungs;

            ViewBag.nhanVienKinhDoanhs = nguoiDungs.Join(
                            db.default_tbChucVu,
                            nguoiDung => nguoiDung.IdChucVu,
                            chucVu => chucVu.IdChucVu,
                            (nguoiDung, chucVu) => new { nguoiDung, chucVu })
                      .Where(x => x.chucVu.MaChucVu == "NVKD")
                      .Select(x => x.nguoiDung) // Chỉ lấy thông tin từ tbNguoiDung
                      .ToList();
            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;
            return View($"{VIEW_PATH}/quanlydoanhthu.cshtml");
        }

        #region CRUD
        public ActionResult displayModal_CRUD()
        {
            int nam = DateTime.Now.Year;
            var doanhThuCoCaus = db.tbCoCauToChuc_DoanhThu
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                && x.NgayLenMucTieu.Substring(3, 4) == nam.ToString())
                .ToList() // Tải dữ liệu về trước
                .Select(x => new
                {
                    Thang = int.Parse(x.NgayLenMucTieu.Substring(0, 2)), // Chuyển đổi sang số
                    DoanhThu = x,
                })
                .ToList();

            // Khởi tạo danh sách 12 tháng với giá trị mặc định là 0
            var mucTieuDoanhThu = Enumerable.Range(1, 12).ToList()
                .Select(_thang => new MucTieuDoanhThuCoCauOutput_Dto
                {
                    Thang = _thang,
                    DoanhThu = doanhThuCoCaus?.FirstOrDefault(x => x.Thang == _thang)?.DoanhThu ?? new tbCoCauToChuc_DoanhThu
                    {
                        IdCoCauToChuc_DoanhThu = Guid.Empty,
                        DoanhThuMucTieu = 0,
                        DoanhThuThucTe = 0,
                        PhanTramHoanThien = 0
                    },
                })
                .ToList();

            ViewBag.MucTieuDoanhThu = mucTieuDoanhThu;
            return PartialView($"{VIEW_PATH}/quanlydoanhthu-crud.cshtml");
        }
        public JsonResult create_MucTieuDoanhThu_CoCau(string str_coCau_DoanhThu)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var mucTieus = JsonConvert.DeserializeObject<List<MucTieuDoanhThuCoCauOutput_Dto>>(Request.Form["str_mucTieus"]);
                    if (mucTieus == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        int nam = DateTime.Now.Year;
                        var doanhThuRepo = db.tbKhachHang_DonHang_ThanhToan
                            .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                                && x.NgayTao.Value.Year == nam)
                            .ToList();
                        foreach (var mucTieu_NEW in mucTieus)
                        {
                            string ngayLenMucTieu = string.Format("{0:D2}/{1}", mucTieu_NEW.Thang, nam);
                            // Lấy doanh thu thực tế tháng đó
                            long? TongDoanhThu = doanhThuRepo
                                .Where(x => x.NgayTao.Value.Month == mucTieu_NEW.Thang)?
                                .Sum(x => x.SoTienDaDong);

                            mucTieu_NEW.DoanhThu.DoanhThuThucTe = TongDoanhThu ?? 0;
                            decimal? phanTramHoanThien = mucTieu_NEW.DoanhThu.DoanhThuMucTieu == 0
                                ? 0
                                : ((decimal)TongDoanhThu / (decimal)mucTieu_NEW.DoanhThu.DoanhThuMucTieu) * 100;
                            mucTieu_NEW.DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                            var mucTieu_OLD = db.tbCoCauToChuc_DoanhThu.Find(mucTieu_NEW.DoanhThu.IdCoCauToChuc_DoanhThu);
                            if (mucTieu_OLD == null)
                            {
                                tbCoCauToChuc_DoanhThu mucTieu = new tbCoCauToChuc_DoanhThu
                                {
                                    IdCoCauToChuc_DoanhThu = Guid.NewGuid(),
                                    IdCoCauToChuc = Guid.Empty,
                                    DoanhThuMucTieu = mucTieu_NEW.DoanhThu.DoanhThuMucTieu,
                                    DoanhThuThucTe = mucTieu_NEW.DoanhThu.DoanhThuThucTe,
                                    PhanTramHoanThien = mucTieu_NEW.DoanhThu.PhanTramHoanThien,
                                    NgayLenMucTieu = ngayLenMucTieu,

                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbCoCauToChuc_DoanhThu.Add(mucTieu);
                            }
                            else
                            {
                                mucTieu_OLD.DoanhThuMucTieu = mucTieu_NEW.DoanhThu.DoanhThuMucTieu;
                                mucTieu_OLD.DoanhThuThucTe = mucTieu_NEW.DoanhThu.DoanhThuThucTe;
                                mucTieu_OLD.PhanTramHoanThien = mucTieu_NEW.DoanhThu.PhanTramHoanThien;

                                mucTieu_OLD.IdNguoiSua = per.NguoiDung.IdNguoiSua;
                                mucTieu_OLD.NgaySua = DateTime.Now;
                            };
                            db.SaveChanges();
                        };
                        scope.Commit();
                    };
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }

                return Json(new
                {
                    status,
                    mess
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Lấy các danh sách dữ liệu

        [HttpGet]
        public JsonResult getMucTieuDoanhThu_CoCau(string ngayLenMucTieu = "")
        {
            List<tbCoCauToChuc_DoanhThu> coCauToChuc_DoanhThus =
                get_MucTieuDoanhThu_CoCau(ngayLenMucTieu: ngayLenMucTieu)
                ?? new List<tbCoCauToChuc_DoanhThu>();
            return Json(new
            {
                coCauToChuc_DoanhThu = coCauToChuc_DoanhThus.FirstOrDefault(),
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Thiết lập mục tiêu doanh thu
        [HttpPost]
        public bool kiemTra_MucTieuDoanhThu_CaNhan(string ngayLenMucTieu = "")
        {
            List<tbNguoiDung_DoanhThu> nguoiDung_DoanhThus =
                get_MucTieuDoanhThu_CaNhan(ngayLenMucTieu: ngayLenMucTieu);
            if (nguoiDung_DoanhThus.Count == 0) return false;
            return true;
        }
        public List<tbNguoiDung_DoanhThu> get_MucTieuDoanhThu_CaNhan(string ngayLenMucTieu = "")
        {
            // Chỉ yêu cầu với nhân viên kinh doanh
            List<tbNguoiDung_DoanhThu> nguoiDung_DoanhThus = db.tbNguoiDung_DoanhThu.Where(x =>
            x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
            x.NgayLenMucTieu == ngayLenMucTieu &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList() ?? new List<tbNguoiDung_DoanhThu>();
            return nguoiDung_DoanhThus;
        }

        public JsonResult thietLapMucTieuDoanhThu_CaNhan(string str_nguoiDung_DoanhThu)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbNguoiDung_DoanhThu nguoiDung_DoanhThu_NEW = JsonConvert.DeserializeObject<tbNguoiDung_DoanhThu>(str_nguoiDung_DoanhThu);

                    if (nguoiDung_DoanhThu_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        string ngayLenMucTieu = DateTime.Now.ToString("MM/yyyy");
                        void taoMucTieuDoanhThu(tbNguoiDung_DoanhThu _nguoiDung_DoanhThu_NEW)
                        {
                            tbNguoiDung_DoanhThu nguoiDung_DoanhThu = new tbNguoiDung_DoanhThu
                            {
                                IdNguoiDung_DoanhThu = Guid.NewGuid(),
                                DoanhThuMucTieu = _nguoiDung_DoanhThu_NEW.DoanhThuMucTieu,
                                DoanhThuThucTe = _nguoiDung_DoanhThu_NEW.DoanhThuThucTe,
                                PhanTramHoanThien = _nguoiDung_DoanhThu_NEW.PhanTramHoanThien,
                                NgayLenMucTieu = ngayLenMucTieu,

                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };

                            if (nguoiDung_DoanhThu_NEW.DoanhThuMucTieu >= (10 ^ 8))
                            {
                                mess = "Rực rỡ luôn ấy chứ, gọi anh em khác là mụn vì cần bạn làm gương 🍾";
                            }
                            else if (nguoiDung_DoanhThu_NEW.DoanhThuMucTieu >= (5 * (10 ^ 7)))
                            {
                                mess = "Cứ nét đều thế này chả mấy mua vf3 🍾";
                            }
                            else if (nguoiDung_DoanhThu_NEW.DoanhThuMucTieu >= (3 * (10 ^ 7)))
                            {
                                mess = "Tuổi 17 bẻ gẫy sừng nyc, tháng sau phải cứng đấy 🍾";
                            }
                            else
                            {
                                mess = "Có công mài sắt có ngày bỏng tay, của ít lòng nhiều. Tháng sau mạnh hơn nhé 🍾";
                            };
                            db.tbNguoiDung_DoanhThu.Add(nguoiDung_DoanhThu);
                            db.SaveChanges();
                        };

                        List<tbNguoiDung_DoanhThu> nguoiDung_DoanhThus =
                            get_MucTieuDoanhThu_CaNhan(ngayLenMucTieu: ngayLenMucTieu)
                            ?? new List<tbNguoiDung_DoanhThu>();
                        if (nguoiDung_DoanhThus.Count > 0)
                        {
                            // Xóa hết mục tiêu cũ
                            db.Database.ExecuteSqlCommand($@"
                            update tbNguoiDung_DoanhThu 
                            set TrangThai = 0 
                            where 
                                IdNguoiTao = '{per.NguoiDung.IdNguoiDung}' 
                                and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}' 
                                and NgayLenMucTieu = '{ngayLenMucTieu}'");

                            // Lấy doanh thu thực tế tháng đó
                            //long? _TongDoanhThu = db.tbKhachHang_DonHang_ThanhToan.Where(x =>
                            //x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                            //x.NgayTao.Value.ToString("MM/yyyy") == ngayLenMucTieu &&
                            //x.IdNguoiTao == per.NguoiDung.IdNguoiDung).Sum(x => x.SoTienDaDong);

                            long? TongDoanhThu = db.Database.SqlQuery<int>($@"
                            select SoTienDaDong
                            from tbKhachHang_DonHang_ThanhToan
                            where 
                                TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                                and IdNguoiTao = '{per.NguoiDung.IdNguoiDung}'
                                and FORMAT(NgayTao, 'MM/yyyy') = '{ngayLenMucTieu}'
                            ").Sum(x => x);

                            nguoiDung_DoanhThu_NEW.DoanhThuThucTe = TongDoanhThu ?? 0;
                            decimal? phanTramHoanThien = ((decimal)nguoiDung_DoanhThu_NEW.DoanhThuThucTe / (decimal)nguoiDung_DoanhThu_NEW.DoanhThuMucTieu) * 100;
                            nguoiDung_DoanhThu_NEW.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                            taoMucTieuDoanhThu(_nguoiDung_DoanhThu_NEW: nguoiDung_DoanhThu_NEW);
                        }
                        else taoMucTieuDoanhThu(_nguoiDung_DoanhThu_NEW: nguoiDung_DoanhThu_NEW);

                        scope.Commit();
                    };
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }

                return Json(new
                {
                    status,
                    mess
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public bool kiemTra_MucTieuDoanhThu_CoCau(string ngayLenMucTieu = "")
        {
            List<tbCoCauToChuc_DoanhThu> coCauToChuc_DoanhThus =
                get_MucTieuDoanhThu_CoCau(ngayLenMucTieu: ngayLenMucTieu);
            if (coCauToChuc_DoanhThus.Count == 0) return false;
            return true;
        }
        public List<tbCoCauToChuc_DoanhThu> get_MucTieuDoanhThu_CoCau(string ngayLenMucTieu = "")
        {
            List<tbCoCauToChuc_DoanhThu> coCauToChuc_DoanhThus = db.tbCoCauToChuc_DoanhThu.Where(x =>
            //x.IdCoCauToChuc == Guid.Empty &&
            //x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
            x.NgayLenMucTieu == ngayLenMucTieu &&
            x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai != 0).ToList() ?? new List<tbCoCauToChuc_DoanhThu>();
            return coCauToChuc_DoanhThus;
        }

        public JsonResult thietLapMucTieuDoanhThu_CoCau(string str_coCau_DoanhThu)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbCoCauToChuc_DoanhThu coCau_DoanhThu_NEW = JsonConvert.DeserializeObject<tbCoCauToChuc_DoanhThu>(str_coCau_DoanhThu);
                    if (coCau_DoanhThu_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        string ngayLenMucTieu = DateTime.Now.ToString("MM/yyyy");
                        void taoMucTieuDoanhThu(tbCoCauToChuc_DoanhThu _coCau_DoanhThu_NEW)
                        {
                            tbCoCauToChuc_DoanhThu coCauToChuc_DoanhThu = new tbCoCauToChuc_DoanhThu
                            {
                                IdCoCauToChuc_DoanhThu = Guid.NewGuid(),
                                IdCoCauToChuc = Guid.Empty,
                                DoanhThuMucTieu = _coCau_DoanhThu_NEW.DoanhThuMucTieu,
                                DoanhThuThucTe = _coCau_DoanhThu_NEW.DoanhThuThucTe,
                                PhanTramHoanThien = _coCau_DoanhThu_NEW.PhanTramHoanThien,
                                NgayLenMucTieu = ngayLenMucTieu,

                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };

                            if (coCauToChuc_DoanhThu.DoanhThuMucTieu >= 700000000)
                            {
                                mess = "Vẫn là vậy, ngai vàng chỉ dành cho VIETGEN 🍾";
                            }
                            else if (coCauToChuc_DoanhThu.DoanhThuMucTieu >= 600000000)
                            {
                                mess = "Tầm này có ngán ai đâu cơ chứ 🍾";
                            }
                            else if (coCauToChuc_DoanhThu.DoanhThuMucTieu >= 500000000)
                            {
                                mess = "Giám đốc cứ mạnh thế này ae thích lắm 🍾";
                            }
                            else if (coCauToChuc_DoanhThu.DoanhThuMucTieu >= 400000000)
                            {
                                mess = "NÉT NÉT NÉT, tháng nào cũng nét 🍾";
                            };

                            db.tbCoCauToChuc_DoanhThu.Add(coCauToChuc_DoanhThu);
                            db.SaveChanges();
                        };

                        List<tbCoCauToChuc_DoanhThu> coCauToChuc_DoanhThus =
                            get_MucTieuDoanhThu_CoCau(ngayLenMucTieu: ngayLenMucTieu)
                            ?? new List<tbCoCauToChuc_DoanhThu>();
                        if (coCauToChuc_DoanhThus.Count > 0)
                        {
                            // Xóa hết mục tiêu cũ
                            db.Database.ExecuteSqlCommand($@"
                            update tbCoCauToChuc_DoanhThu 
                            set TrangThai = 0 
                            where 
                                IdNguoiTao = '{per.NguoiDung.IdNguoiDung}' 
                                and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}' 
                                and NgayLenMucTieu = '{ngayLenMucTieu}'");

                            // Lấy doanh thu thực tế tháng đó
                            long? TongDoanhThu = db.Database.SqlQuery<int>($@"
                            select SoTienDaDong
                            from tbKhachHang_DonHang_ThanhToan
                            where 
                                TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                                and FORMAT(NgayTao, 'MM/yyyy') = '{ngayLenMucTieu}'
                            ").Sum(x => x);

                            coCau_DoanhThu_NEW.DoanhThuThucTe = TongDoanhThu ?? 0;
                            decimal? phanTramHoanThien = ((decimal)TongDoanhThu / (decimal)coCau_DoanhThu_NEW.DoanhThuMucTieu) * 100;
                            coCau_DoanhThu_NEW.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                            taoMucTieuDoanhThu(_coCau_DoanhThu_NEW: coCau_DoanhThu_NEW);
                        }
                        else taoMucTieuDoanhThu(_coCau_DoanhThu_NEW: coCau_DoanhThu_NEW);

                        scope.Commit();
                    };
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }

                return Json(new
                {
                    status,
                    mess
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Thống kê

        #region Trang chủ
        [HttpGet]
        public ActionResult getDoanhThus_MoiNhat()
        {
            var sevenDaysAgo = DateTime.Now.Date.AddDays(-7);
            var today = DateTime.Now.Date;

            var nguoiDungRepo = db.tbNguoiDungs.Where(nd =>
                nd.TrangThai != 0 &&
                nd.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                nd.KichHoat == true);
            var sanPhamRepo = db.tbSanPhams.Where(nd =>
                nd.TrangThai != 0 &&
                nd.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            var khachHangRepo = db.tbKhachHangs.Where(nd =>
                nd.TrangThai != 0 &&
                nd.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);

            var doanhThuMoiNhats = db.tbKhachHang_DonHang_ThanhToan
                .ToList()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                    x.NgayTao.Value.Date >= sevenDaysAgo &&
                    x.NgayTao.Value.Date <= today)
                .Join(nguoiDungRepo,
                    tt => tt.IdNguoiTao,
                    nd => nd.IdNguoiDung,
                    (tt, nd) => new
                    {
                        ThanhToan = tt,
                        NguoiDung = nd
                    })
                .Join(sanPhamRepo,
                    x => x.ThanhToan.IdSanPham,
                    sp => sp.IdSanPham,
                    (x, sp) => new
                    {
                        ThanhToan = x.ThanhToan,
                        NguoiDung = x.NguoiDung,
                        SanPham = sp
                    })
                .Join(khachHangRepo,
                    x => x.ThanhToan.IdKhachHang,
                    kh => kh.IdKhachHang,
                    (x, kh) => new
                    {
                        ThanhToan = x.ThanhToan,
                        NguoiDung = x.NguoiDung,
                        SanPham = x.SanPham,
                        KhachHang = kh
                    })
                .Select(x => new DoanhThuMoiNhatDto
                {
                    ThanhToan = x.ThanhToan,
                    TenNguoiDung = x.NguoiDung.TenNguoiDung ?? string.Empty,
                    TenKhachHang = x.KhachHang.TenKhachHang ?? string.Empty,
                    TenSanPham = x.SanPham.TenSanPham ?? string.Empty,
                    PhanTramThanhToan = (int)x.ThanhToan.PhanTramDaDong
                })
                .OrderByDescending(dto => dto.ThanhToan.NgayTao)
                .ToList() ?? new List<DoanhThuMoiNhatDto>();

            return PartialView($"~/Views/Admin/__Home/TrangChu/quyenkinhdoanh/doanhthumoinhat-getList.cshtml", doanhThuMoiNhats);
        }
        public List<BangXepHangDto> get_TopDoanhThu(string ngayLenMucTieu)
        {
            var nguoiDungRepo = db.tbNguoiDungs.Where(nd =>
                nd.TrangThai != 0 &&
                nd.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                nd.KichHoat == true);
            var doanhThuRepo = db.tbNguoiDung_DoanhThu.Where(nd =>
                nd.TrangThai != 0 &&
                nd.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            var chucVuRepo = db.default_tbChucVu.Where(nd =>
                nd.TrangThai != 0 &&
                nd.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);

            var nguoiDungs = db.tbNguoiDung_DoanhThu
             .Where(doanhThu =>
                 doanhThu.TrangThai != 0 &&
                 doanhThu.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                 (string.IsNullOrEmpty(ngayLenMucTieu) || doanhThu.NgayLenMucTieu == ngayLenMucTieu))
             .Join(nguoiDungRepo,
                 dt => dt.IdNguoiTao,
                 nd => nd.IdNguoiDung,
                 (dt, nd) => new
                 {
                     NguoiDung = nd,
                     DoanhThu = dt
                 })
             .Join(chucVuRepo,
                 nd => nd.NguoiDung.IdChucVu,
                 cv => cv.IdChucVu,
                 (nd, cv) => new
                 {
                     NguoiDung = nd.NguoiDung,
                     DoanhThu = nd.DoanhThu,
                     ChucVu = cv
                 })
             .GroupBy(x => x.NguoiDung.IdNguoiDung)
             .ToList()
             .Select(gr => new BangXepHangDto
             {
                 TenNguoiDung = gr.FirstOrDefault().NguoiDung.TenNguoiDung,
                 TenChucVu = gr.FirstOrDefault().ChucVu.TenChucVu,
                 TongDoanhThu = gr.Sum(dt => dt.DoanhThu.DoanhThuThucTe) ?? 0
             })
             .OrderByDescending(x => x.TongDoanhThu) // Sắp xếp giảm dần theo doanh thu
             .Select((item, index) => new BangXepHangDto // Đánh số thứ tự (index bắt đầu từ 0)
             {
                 XepHang = index + 1,
                 TenNguoiDung = item.TenNguoiDung,
                 TenChucVu = item.TenChucVu,
                 TongDoanhThu = item.TongDoanhThu
             })
             .ToList() ?? new List<BangXepHangDto>();

            return nguoiDungs;
        }
        [HttpGet]
        public ActionResult getNguoiDungs_BangXepHang(string ngayLenMucTieu)
        {
            var nguoiDungs = get_TopDoanhThu(ngayLenMucTieu: ngayLenMucTieu);
            return PartialView($"~/Views/Admin/__Home/TrangChu/quyenkinhdoanh/bangxephang-getList.cshtml", nguoiDungs);
        }

        [HttpGet]
        public JsonResult hienThiThongBaoNoiBat()
        {
            string ngayLenMucTieu = DateTime.Now.AddMonths(-1).ToString("MM/yyyy"); // Lấy tháng trước đó
            List<BangXepHangDto> topDoanhThu_Thang = get_TopDoanhThu(ngayLenMucTieu: ngayLenMucTieu);
            List<BangXepHangDto> topDoanhThu_Tong = get_TopDoanhThu(ngayLenMucTieu: "");
            List<tbCoCauToChuc_DoanhThu> coCauToChuc_DoanhThus =
                get_MucTieuDoanhThu_CoCau(ngayLenMucTieu: ngayLenMucTieu)
                ?? new List<tbCoCauToChuc_DoanhThu>();
            return Json(new
            {
                topDoanhThu_Thang,
                topDoanhThu_Tong,
                tienDoDoanhThu = coCauToChuc_DoanhThus.FirstOrDefault(),
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Tổng quan
        [HttpPost]
        public JsonResult thongKeTongQuan(LocThongTinTongQuanDto locThongTin)
        {
            if (string.IsNullOrEmpty(locThongTin.ThoiGian))
                locThongTin.ThoiGian = DateTime.Now.ToString("MM/yyyy");
            int thang = 0; int.TryParse(locThongTin.ThoiGian.Substring(0, 2), out thang);
            int nam = 0; int.TryParse(locThongTin.ThoiGian.Substring(3, 4), out nam);
            /**
             * Doanh thu tổng (chia theo tháng, quý, năm).
             * Số lượng khách hàng/đơn hàng/thanh toán (theo ngày, tháng, năm).
             * Tỷ lệ sản phẩm học thử vs học chính.
             * Phân bổ sản phẩm theo ngôn ngữ (tiếng Anh/Đức).
             * Tỷ lệ thanh toán hoàn thành vs chưa hoàn thành.
             */
            #region 
            var chucVuRepo = db.default_tbChucVu.Where(x => x.TrangThai != 0);
            var NVKDRepo = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .Join(chucVuRepo,
                    nd => nd.IdChucVu,
                    cv => cv.IdChucVu,
                    (nd, cv) => new
                    {
                        NguoiDung = nd,
                        ChucVu = cv,
                    })
                .Where(x => x.ChucVu.MaChucVu == "NVKD")
                .Select(x => x.NguoiDung);
            var sanPhamRepo = db.tbSanPhams.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            var loaiSanPhamRepo = db.tbSanPham_LoaiSanPham.Where(x => x.TrangThai != 0);

            var doanhThuCoCaus = db.tbCoCauToChuc_DoanhThu
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                && x.NgayLenMucTieu.Substring(3, 4) == nam.ToString())
                .ToList() // Tải dữ liệu về trước
                .Select(x => new
                {
                    Thang = int.Parse(x.NgayLenMucTieu.Substring(0, 2)), // Chuyển đổi sang số
                    DoanhThu = x,
                })
                .ToList();
            var doanhThuNVKDs = db.tbNguoiDung_DoanhThu
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                        && x.NgayLenMucTieu.Substring(3, 4) == nam.ToString())
                .ToList() // Tải dữ liệu về trước
                .Join(NVKDRepo,
                        dt => dt.IdNguoiTao,
                        nd => nd.IdNguoiDung,
                        (dt, nd) => new
                        {
                            DoanhThu = dt,
                            NguoiDung = nd,
                        })
                .Select(x => new
                {
                    Thang = int.Parse(x.DoanhThu.NgayLenMucTieu.Substring(0, 2)), // Chuyển đổi sang số
                    DoanhThu = x.DoanhThu,
                    NguoiDung = x.NguoiDung
                })
                .ToList();
            var khachHangs = db.tbKhachHangs
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                        && x.NgayTao.Value.Year == nam)
                .Select(x => new
                {
                    Thang = x.NgayTao.Value.Month,
                    KhachHang = x
                }).ToList();

            var donHangs = db.tbKhachHang_DonHang
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                        && x.NgayTao.Value.Year == nam)
                .Select(x => new
                {
                    Thang = x.NgayTao.Value.Month,
                    DonHang = x
                }).ToList();

            var thanhToans = db.tbKhachHang_DonHang_ThanhToan
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                        && x.NgayTao.Value.Year == nam)
                .Join(sanPhamRepo,
                        tt => tt.IdSanPham,
                        sp => sp.IdSanPham,
                        (main, sp) => new
                        {
                            ThanhToan = main,
                            SanPham = sp,
                        })
                .Join(loaiSanPhamRepo,
                        sp => sp.SanPham.IdLoaiSanPham,
                        lsp => lsp.IdLoaiSanPham,
                        (main, lsp) => new
                        {
                            ThanhToan = main.ThanhToan,
                            SanPham = main.SanPham,
                            LoaiSanPham = lsp,
                        })
                .Select(x => new
                {
                    Thang = x.ThanhToan.NgayTao.Value.Month,
                    ThanhToan = x.ThanhToan,
                    SanPham = x.SanPham,
                    LoaiSanPham = x.LoaiSanPham,
                })
                .ToList();
            #endregion

            // Khởi tạo danh sách 12 tháng với giá trị mặc định là 0
            var thongKeTongQuan = Enumerable.Range(1, 12).ToList()
                .Select(_thang => new
                {
                    Thang = _thang,
                    DoanhThuMucTieu = doanhThuCoCaus?.Where(x => x.Thang == _thang)?.Sum(x => x.DoanhThu.DoanhThuMucTieu),
                    DoanhThuThucTe = doanhThuCoCaus?.Where(x => x.Thang == _thang)?.Sum(x => x.DoanhThu.DoanhThuThucTe),

                    SoLuongKhachHang = khachHangs?.Where(x => x.Thang == _thang)?.Count(),
                    SoLuongDonHang = donHangs?.Where(x => x.Thang == _thang)?.Count(),
                    SoLuongThanhToan = thanhToans?.Where(x => x.Thang == _thang)?.Count(),
                    SoLuongSanPham_TiengAnh = thanhToans?
                    .Where(x => x.Thang == _thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh")?
                    .Select(x => x.ThanhToan.IdDonHang).Distinct().Count(),
                    SoLuongSanPham_TiengDuc = thanhToans?
                    .Where(x => x.Thang == _thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức")?
                    .Select(x => x.ThanhToan.IdDonHang).Distinct().Count(),
                })
                .ToList();

            var model = new ThongKeTongQuanOutput_Dto
            {
                Thang = thang,
                Nam = nam,
            };

            if (thang == 0 || locThongTin.LoaiThongKe == "theonam") // Cả năm
            {
                #region Doanh thu
                model.DoanhThuMucTieu = (long)thongKeTongQuan.Sum(x => x.DoanhThuMucTieu);
                model.DoanhThuThucTe = (long)thongKeTongQuan.Sum(x => x.DoanhThuThucTe);
                model.DoanhThuTiengAnh = (long)thanhToans.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Sum(x => x.ThanhToan.SoTienDaDong);
                model.DoanhThuTiengDuc = (long)thanhToans.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Sum(x => x.ThanhToan.SoTienDaDong);
                model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
                #endregion

                #region Nhân viên kinh doanh
                var topDoanhThuNguoiDung = doanhThuNVKDs
                    .GroupBy(x => x.NguoiDung) // Nhóm theo NguoiDung
                    .Select(gr => new DoanhThuNVKD_Dto
                    {
                        NguoiDung = gr.Key, // Lấy thông tin người dùng
                        TongDoanhThu = (long)gr.Sum(x => x.DoanhThu.DoanhThuThucTe) // Tính tổng doanh thu
                    })
                    .OrderByDescending(x => x.TongDoanhThu) // Sắp xếp giảm dần theo tổng doanh thu
                    .Take(3) // Lấy top 3
                    .ToList(); // Chuyển thành danh sách

                model.NVKD_TopDoanhThu = topDoanhThuNguoiDung;
                model.NVKD_KhongCoDoanhThu = null;
                #endregion

                #region Khách hàng & Đơn hàng
                model.SoLuongDonHang = donHangs.Count;
                model.SoLuongThanhToan = thanhToans.Count;
                model.SoLuongKhachHangMoi = khachHangs.Count;
                model.SoLuongKhachHangUpSell = khachHangs
                    .Join(donHangs,
                        kh => kh.KhachHang.IdKhachHang,
                        dh => dh.DonHang.IdKhachHang,
                        (kh, dh) => new
                        {
                            KhachHang = kh.KhachHang,
                            DonHang = dh.DonHang,
                        })
                    .GroupBy(x => x.KhachHang)
                    .Select(gr => new
                    {
                        KhachHang = gr.Key,
                        DonHangs = gr.Select(x => x.DonHang).ToList()
                    })
                    .Where(x => x.DonHangs.Count > 1) // Số lượng khách có nhiều hơn 1 đơn hàng
                    .Count();

                var thanhToanHocThus = thanhToans.Where(x => x.ThanhToan.PhanTramDaDong < 10).ToList();
                model.SoLuongKhachHangHocThu = thanhToanHocThus.Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangHocChinh = thanhToans.Where(x => x.ThanhToan.PhanTramDaDong > 10).Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangHocThuSangHocChinh = thanhToanHocThus
                    .Select(x => x.ThanhToan.IdDonHang).Distinct()
                    .Join(donHangs,
                        tt => tt,
                        dh => dh.DonHang.IdDonHang,
                        (tt, dh) => new
                        {
                            DonHang = dh.DonHang,
                        })
                    .Join(thanhToans,
                        dh => dh.DonHang.IdDonHang,
                        tt => tt.ThanhToan.IdDonHang,
                        (dh, tt) => new
                        {
                            IdDonHang = dh.DonHang,
                            ThanhToan = tt.ThanhToan,
                        })
                    .GroupBy(x => x.IdDonHang)
                    .Where(gr => gr.Count() > 1 && gr.Sum(x => x.ThanhToan.PhanTramDaDong) >= 10) // Nhiều hơn 1 lần thanh toán và tổng % đã đóng >= 10
                    .Count();
                model.SoLuongKhachHangChuaDangKyHoc = model.SoLuongKhachHangMoi - thanhToans
                    .Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangChuaDangKyHoc = model.SoLuongKhachHangChuaDangKyHoc < 0 ? 0 : model.SoLuongKhachHangChuaDangKyHoc;

                tbKhachHang_LoaiKhachHang loaiKhachHang_DangHoc = db.tbKhachHang_LoaiKhachHang.FirstOrDefault(x => x.TenLoaiKhachHang == "Đang học") ?? new tbKhachHang_LoaiKhachHang();
                tbKhachHang_LoaiKhachHang loaiKhachHang_DaHocXong = db.tbKhachHang_LoaiKhachHang.FirstOrDefault(x => x.TenLoaiKhachHang == "Đã học xong") ?? new tbKhachHang_LoaiKhachHang();
                model.SoLuongKhachHangDangHoc = khachHangs.Where(x => x.KhachHang.IdLoaiKhachHang == loaiKhachHang_DangHoc.IdLoaiKhachHang).Count();
                model.SoLuongKhachHangDaHocXong = khachHangs.Where(x => x.KhachHang.IdLoaiKhachHang == loaiKhachHang_DaHocXong.IdLoaiKhachHang).Count();

                var donHangThanhToans = thanhToans
                    .GroupBy(x => x.ThanhToan.IdDonHang)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        IdKhachHang = gr.FirstOrDefault().ThanhToan.IdKhachHang,
                        PhanTramThanhToan = gr.Count() == 0 ? 0 : (gr.Sum(x => x.ThanhToan.PhanTramDaDong) / gr.Count()), // Nếu chỉ thanh toán trọn gói thì chỉ có 1 bản ghi
                        SoLanThanhToanTrenDonHang = gr.Count(),
                    }).ToList();
                model.SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang = donHangThanhToans
                    .Where(x => x.SoLanThanhToanTrenDonHang > 1)
                    .Select(x => x.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangThanhToanTronGoi = donHangThanhToans
                    .Where(x => x.SoLanThanhToanTrenDonHang == 1 && x.PhanTramThanhToan >= 80) // Thanh toán >= 80%
                    .Select(x => x.IdKhachHang).Distinct().Count();

                model.PhanTramThanhToanNhieuLan = model.SoLuongKhachHangMoi == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang / (decimal)model.SoLuongKhachHangMoi) * 100, 2);
                model.PhanTramUpSellChung = model.SoLuongKhachHangMoi == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangUpSell / (decimal)model.SoLuongKhachHangMoi), 2);
                model.PhanTramKhachHang_HocThuSangHocChinh = model.SoLuongKhachHangHocThu == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangHocThuSangHocChinh / (decimal)model.SoLuongKhachHangHocThu) * 100, 2);
                #endregion

                #region Sản phẩm
                var _donHangs = donHangs
                    .Join(sanPhamRepo,
                        dh => dh.DonHang.IdSanPham,
                        sp => sp.IdSanPham,
                        (dh, sp) => new
                        {
                            DonHang = dh.DonHang,
                            SanPham = sp,
                        })
                    .Join(loaiSanPhamRepo,
                        dh => dh.SanPham.IdLoaiSanPham,
                        lsp => lsp.IdLoaiSanPham,
                        (dh, lsp) => new
                        {
                            DonHang = dh.DonHang,
                            SanPham = dh.SanPham,
                            LoaiSanPham = lsp,
                        }).ToList();
                model.SoLuongSanPham_TiengAnh = _donHangs.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Count();
                model.SoLuongSanPham_TiengDuc = _donHangs.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Count();
                model.SanPham_BanChay = _donHangs
                  .GroupBy(x => x.SanPham.IdSanPham)
                  .Select(gr => new
                  {
                      Key = gr.Key,
                      SanPham = gr.FirstOrDefault().SanPham,
                      SoLuongDonHang = gr.Count(),
                  })
                  .OrderByDescending(x => x.SoLuongDonHang)
                  .Select(x => new SanPhamDaBan_Dto
                  {
                      SanPham = x.SanPham,
                      SoLuongDaBan = x.SoLuongDonHang,
                  }).Take(3).ToList();
                model.SanPham_TopDoanhThu = thanhToans
                    .Where(x => x.Thang == thang)
                    .GroupBy(x => x.SanPham.IdSanPham)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        SanPham = gr.FirstOrDefault().SanPham,
                        SoTienDaDong = gr.Sum(x => x.ThanhToan.SoTienDaDong),
                    })
                    .OrderByDescending(x => x.SoTienDaDong)
                     .Select(x => new SanPhamDaBan_Dto
                     {
                         SanPham = x.SanPham,
                         TongDoanhThu = (long)x.SoTienDaDong,
                     }).Take(3).ToList();
                #endregion
            }
            else
            {
                #region Doanh thu
                model.DoanhThuMucTieu = (long)thongKeTongQuan.Where(x => x.Thang == thang).Sum(x => x.DoanhThuMucTieu);
                model.DoanhThuThucTe = (long)thongKeTongQuan.Where(x => x.Thang == thang).Sum(x => x.DoanhThuThucTe);
                model.DoanhThuTiengAnh = (long)thanhToans.Where(x => x.Thang == thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Sum(x => x.ThanhToan.SoTienDaDong);
                model.DoanhThuTiengDuc = (long)thanhToans.Where(x => x.Thang == thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Sum(x => x.ThanhToan.SoTienDaDong);
                model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
                model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
                #endregion

                #region Nhân viên kinh doanh
                var topDoanhThuNguoiDung = doanhThuNVKDs
                    .Where(x => x.Thang == thang)
                    .GroupBy(x => x.NguoiDung) // Nhóm theo NguoiDung
                    .Select(group => new DoanhThuNVKD_Dto
                    {
                        NguoiDung = group.Key, // Lấy thông tin người dùng
                        TongDoanhThu = (long)group.Sum(x => x.DoanhThu.DoanhThuThucTe) // Tính tổng doanh thu
                    })
                    .OrderByDescending(x => x.TongDoanhThu) // Sắp xếp giảm dần theo tổng doanh thu
                    .Take(3) // Lấy top 3
                    .ToList(); // Chuyển thành danh sách

                model.NVKD_TopDoanhThu = topDoanhThuNguoiDung;
                model.NVKD_KhongCoDoanhThu = null;
                #endregion

                #region Khách hàng & Đơn hàng
                model.SoLuongDonHang = donHangs.Where(x => x.Thang == thang).Count();
                model.SoLuongThanhToan = thanhToans.Where(x => x.Thang == thang).Count();
                model.SoLuongKhachHangMoi = khachHangs.Where(x => x.Thang == thang).Count();
                model.SoLuongKhachHangUpSell = khachHangs
                    .Join(donHangs.Where(x => x.Thang <= thang).ToList(), // Chỉ lấy đơn hàng trước tháng so sánh
                        kh => kh.KhachHang.IdKhachHang,
                        dh => dh.DonHang.IdKhachHang,
                        (kh, dh) => new
                        {
                            KhachHang = kh.KhachHang,
                            DonHang = dh.DonHang,
                        })
                    .GroupBy(x => x.KhachHang)
                    .Select(gr => new
                    {
                        KhachHang = gr.Key,
                        DonHangs = gr.Select(x => x.DonHang).ToList()
                    })
                    .Where(x => x.DonHangs.Count > 1) // Số lượng khách có nhiều hơn 1 đơn hàng
                    .Count();

                var thanhToanHocThus = thanhToans.Where(x => x.ThanhToan.PhanTramDaDong < 10 && x.Thang == thang).ToList();
                model.SoLuongKhachHangHocThu = thanhToanHocThus.Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangHocChinh = thanhToans.Where(x => x.ThanhToan.PhanTramDaDong > 10).Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangHocThuSangHocChinh = thanhToanHocThus
                    .Select(x => x.ThanhToan.IdDonHang).Distinct()
                    .Join(donHangs,
                        tt => tt,
                        dh => dh.DonHang.IdDonHang,
                        (tt, dh) => new
                        {
                            DonHang = dh.DonHang,
                        })
                    .Join(thanhToans.Where(x => x.Thang == thang).ToList(), // Chỉ lấy thanh toán trong tháng
                        dh => dh.DonHang.IdDonHang,
                        tt => tt.ThanhToan.IdDonHang,
                        (dh, tt) => new
                        {
                            IdDonHang = dh.DonHang,
                            ThanhToan = tt.ThanhToan,
                        })
                    .GroupBy(x => x.IdDonHang)
                    .Where(gr => gr.Count() > 1 && gr.Sum(x => x.ThanhToan.PhanTramDaDong) >= 10) // Nhiều hơn 1 lần thanh toán và tổng % đã đóng >= 10
                    .Count();
                model.SoLuongKhachHangChuaDangKyHoc = model.SoLuongKhachHangMoi - thanhToans
                    .Where(x => x.Thang == thang)
                    .Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangChuaDangKyHoc = model.SoLuongKhachHangChuaDangKyHoc < 0 ? 0 : model.SoLuongKhachHangChuaDangKyHoc;

                tbKhachHang_LoaiKhachHang loaiKhachHang_DangHoc = db.tbKhachHang_LoaiKhachHang.FirstOrDefault(x => x.TenLoaiKhachHang == "Đang học") ?? new tbKhachHang_LoaiKhachHang();
                tbKhachHang_LoaiKhachHang loaiKhachHang_DaHocXong = db.tbKhachHang_LoaiKhachHang.FirstOrDefault(x => x.TenLoaiKhachHang == "Đã học xong") ?? new tbKhachHang_LoaiKhachHang();
                model.SoLuongKhachHangDangHoc = khachHangs.Where(x => x.Thang < thang && x.KhachHang.IdLoaiKhachHang == loaiKhachHang_DangHoc.IdLoaiKhachHang).Count(); // Khách đang học từ trước tới giờ
                model.SoLuongKhachHangDaHocXong = khachHangs.Where(x => x.Thang == thang && x.KhachHang.IdLoaiKhachHang == loaiKhachHang_DaHocXong.IdLoaiKhachHang).Count();

                var donHangThanhToans = thanhToans
                    .Where(x => x.Thang == thang)
                    .GroupBy(x => x.ThanhToan.IdDonHang)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        IdKhachHang = gr.FirstOrDefault().ThanhToan.IdKhachHang,
                        PhanTramThanhToan = gr.Count() == 0 ? 0 : (gr.Sum(x => x.ThanhToan.PhanTramDaDong) / gr.Count()), // Nếu chỉ thanh toán trọn gói thì chỉ có 1 bản ghi
                        SoLanThanhToanTrenDonHang = gr.Count(),
                    }).ToList();
                model.SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang = donHangThanhToans
                    .Where(x => x.SoLanThanhToanTrenDonHang > 1)
                    .Select(x => x.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangThanhToanTronGoi = donHangThanhToans
                    .Where(x => x.SoLanThanhToanTrenDonHang == 1 && x.PhanTramThanhToan >= 80) // Thanh toán >= 80%
                    .Select(x => x.IdKhachHang).Distinct().Count();

                model.PhanTramThanhToanNhieuLan = model.SoLuongKhachHangMoi == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang / (decimal)model.SoLuongKhachHangMoi) * 100, 2);
                model.PhanTramUpSellChung = model.SoLuongKhachHangMoi == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangUpSell / (decimal)model.SoLuongKhachHangMoi), 2);
                model.PhanTramKhachHang_HocThuSangHocChinh = model.SoLuongKhachHangHocThu == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangHocThuSangHocChinh / (decimal)model.SoLuongKhachHangHocThu) * 100, 2);
                #endregion

                #region Sản phẩm
                var _donHangs = donHangs.Where(x => x.Thang == thang)
                    .Join(sanPhamRepo,
                        dh => dh.DonHang.IdSanPham,
                        sp => sp.IdSanPham,
                        (dh, sp) => new
                        {
                            DonHang = dh.DonHang,
                            SanPham = sp,
                        })
                    .Join(loaiSanPhamRepo,
                        dh => dh.SanPham.IdLoaiSanPham,
                        lsp => lsp.IdLoaiSanPham,
                        (dh, lsp) => new
                        {
                            DonHang = dh.DonHang,
                            SanPham = dh.SanPham,
                            LoaiSanPham = lsp,
                        }).ToList();
                model.SoLuongSanPham_TiengAnh = _donHangs.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Count();
                model.SoLuongSanPham_TiengDuc = _donHangs.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Count();
                model.SanPham_BanChay = _donHangs
                    .GroupBy(x => x.SanPham.IdSanPham)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        SanPham = gr.FirstOrDefault().SanPham,
                        SoLuongDonHang = gr.Count(),
                    })
                    .OrderByDescending(x => x.SoLuongDonHang)
                    .Select(x => new SanPhamDaBan_Dto
                    {
                        SanPham = x.SanPham,
                        SoLuongDaBan = x.SoLuongDonHang,
                    }).Take(3).ToList();
                model.SanPham_TopDoanhThu = thanhToans
                    .Where(x => x.Thang == thang)
                    .GroupBy(x => x.SanPham.IdSanPham)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        SanPham = gr.FirstOrDefault().SanPham,
                        SoTienDaDong = gr.Sum(x => x.ThanhToan.SoTienDaDong),
                    })
                    .OrderByDescending(x => x.SoTienDaDong)
                     .Select(x => new SanPhamDaBan_Dto
                     {
                         SanPham = x.SanPham,
                         TongDoanhThu = (long)x.SoTienDaDong,
                     }).Take(3).ToList();
                #endregion
            };

            // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
            string viewAsString = Public.Handle.RenderViewToString(
                this,
                $"~/Views/Admin/QuanLyDoanhThu/quanlydoanhthu-thongke/thongke-tongquan/thongke.cshtml",
                model);
            return Json(new
            {
                ThongKeTongQuan = thongKeTongQuan,
                view = viewAsString
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Chi tiết
        [HttpPost]
        public JsonResult layDanhMucThongKe(string loaiThongKe)
        {
            if (loaiThongKe == "NVKD")
            {
                List<Tree<tbCoCauToChuc>> coCauToChucs_Tree = organizationController.get_CoCauToChucs_Tree(idCoCau: Guid.Empty, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
                return Json(coCauToChucs_Tree, JsonRequestBehavior.AllowGet);
            };
            List<Tree<tbSanPham_LoaiSanPham>> loaiSanPham = db.tbSanPham_LoaiSanPham
                .Where(x => x.TrangThai != 0)
                .Select(x => new Tree<tbSanPham_LoaiSanPham>
                {
                    root = x,
                }).ToList();
            return Json(loaiSanPham, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult chonDanhMucThongTin(LocThongTinChiTietDto locThongTin)
        {
            ViewBag.locThongTin = locThongTin;
            if (locThongTin.LoaiThongKe == "NVKD")
            {
                ViewBag.DanhSachDoanhThuTheoNVKD = layDanhSachDoanhThuTheoNVKD(locThongTin: locThongTin);
                return PartialView($"{VIEW_PATH}/quanlydoanhthu-thongke/thongke-chitiet/thongke-danhsachnvkd.cshtml");
            };
            ViewBag.DanhSachDoanhThuTheoSanPham = layDanhSachDoanhThuTheoSanPham(locThongTin: locThongTin);
            return PartialView($"{VIEW_PATH}/quanlydoanhthu-thongke/thongke-chitiet/thongke-danhsachsanpham.cshtml");
        }
        [HttpPost]
        public JsonResult thongKeChiTietTheoNVKD(ThongKeChiTietTheoThongTinDto locThongTin)
        {
            int thang = 0; int.TryParse(locThongTin.ThoiGian.Substring(0, 2), out thang);
            int nam = 0; int.TryParse(locThongTin.ThoiGian.Substring(3, 4), out nam);
            var idThongTin = locThongTin.IdThongTin;

            #region
            /**
             * 1. Lấy mục tiêu/tiến độ doanh thu từng tháng
             * 2. Lấy các chỉ số chi tiết theo thời gian
             */
            var sanPhamRepo = db.tbSanPhams.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            var donHangRepo = db.tbKhachHang_DonHang.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            var loaiSanPhamRepo = db.tbSanPham_LoaiSanPham.Where(x => x.TrangThai != 0);

            var layThongTinThongKeTheoNVKD_Output_Dto = new LayThongTinThongKeTheoNVKD_Output_Dto();

            layThongTinThongKeTheoNVKD_Output_Dto.NhanVienKinhDoanh = db.tbNguoiDungs.FirstOrDefault(x => x.IdNguoiDung == idThongTin);

            var doanhThus = db.tbNguoiDung_DoanhThu
              .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                    && x.NgayLenMucTieu.Substring(3, 4) == nam.ToString()
                    && x.IdNguoiTao == layThongTinThongKeTheoNVKD_Output_Dto.NhanVienKinhDoanh.IdNguoiDung)
              .ToList() // Tải dữ liệu về trước
              .Select(x => new
              {
                  Thang = int.Parse(x.NgayLenMucTieu.Substring(0, 2)), // Chuyển đổi sang số
                  DoanhThu = x,
              })
              .ToList();

            var khachHangs = db.tbKhachHangs
               .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                        && x.IdNguoiTao == layThongTinThongKeTheoNVKD_Output_Dto.NhanVienKinhDoanh.IdNguoiDung
                        && x.NgayTao.Value.Year == nam)
               .Select(x => new
               {
                   Thang = x.NgayTao.Value.Month,
                   KhachHang = x
               }).ToList();

            var donHangs = donHangRepo
               .Where(x => x.IdNguoiTao == locThongTin.IdThongTin
                        && x.IdNguoiTao == layThongTinThongKeTheoNVKD_Output_Dto.NhanVienKinhDoanh.IdNguoiDung
                        && x.NgayTao.Value.Year == nam)
               .ToList() // Tải dữ liệu về trước
               .Select(x => new
               {
                   Thang = x.NgayTao.Value.Month, // Chuyển đổi sang số
                   DonHang = x,
               })
              .ToList();

            var thanhToans = db.tbKhachHang_DonHang_ThanhToan
               .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                        && x.IdNguoiTao == layThongTinThongKeTheoNVKD_Output_Dto.NhanVienKinhDoanh.IdNguoiDung
                        && x.NgayTao.Value.Year == nam)
               .Join(sanPhamRepo,
                       tt => tt.IdSanPham,
                       sp => sp.IdSanPham,
                       (main, sp) => new
                       {
                           ThanhToan = main,
                           SanPham = sp,
                       })
               .Join(loaiSanPhamRepo,
                       sp => sp.SanPham.IdLoaiSanPham,
                       lsp => lsp.IdLoaiSanPham,
                       (main, lsp) => new
                       {
                           ThanhToan = main.ThanhToan,
                           SanPham = main.SanPham,
                           LoaiSanPham = lsp,
                       })
               .Select(x => new
               {
                   Thang = x.ThanhToan.NgayTao.Value.Month,
                   ThanhToan = x.ThanhToan,
                   SanPham = x.SanPham,
                   LoaiSanPham = x.LoaiSanPham,
               })
               .ToList();
            #endregion

            // Khởi tạo danh sách 12 tháng với giá trị mặc định là 0
            var thongKeTongQuan = Enumerable.Range(1, 12).ToList()
                .Select(_thang => new
                {
                    Thang = _thang,
                    DoanhThuMucTieu = doanhThus?.Where(x => x.Thang == _thang)?.Sum(x => x.DoanhThu.DoanhThuMucTieu),
                    DoanhThuThucTe = doanhThus?.Where(x => x.Thang == _thang)?.Sum(x => x.DoanhThu.DoanhThuThucTe),

                    SoLuongKhachHang = donHangs?.Where(x => x.Thang == _thang)?.Select(x => x.DonHang.IdKhachHang).Count(),
                    SoLuongDonHang = donHangs?.Where(x => x.Thang == _thang)?.Count(),
                    SoLuongThanhToan = thanhToans?.Where(x => x.Thang == _thang)?.Count(),
                    SoLuongSanPham_TiengAnh = thanhToans?
                    .Where(x => x.Thang == _thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh")?
                    .Select(x => x.ThanhToan.IdDonHang).Distinct().Count(),
                    SoLuongSanPham_TiengDuc = thanhToans?
                    .Where(x => x.Thang == _thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức")?
                    .Select(x => x.ThanhToan.IdDonHang).Distinct().Count(),
                })
                .ToList();

            var model = new ThongKeChiTietOutput_Dto()
            {
                Thang = thang,
                Nam = nam,
                IdThongTin = locThongTin.IdThongTin,
            };

            if (thang == 0) // Cả năm
            {
                /**
                 * 1. Doanh thu
                 * 2. Tiến độ
                 * 3. Danh sách khách hàng
                 * 4. Số lượng sản phẩm tiếng Đức
                 * 5. Số lượng sản phẩm tiếng Anh
                 * 6. Số lượng khách hàng học thử
                 * 7. Số lượng khách hàng học chính
                 * 8. Số lượng đơn hàng upsell
                 * 9. Tỉ lệ upsell
                 * 10. Tỉ lệ học thử sang học chính
                 */
                #region Doanh thu
                model.DoanhThuMucTieu = (long)thongKeTongQuan.Sum(x => x.DoanhThuMucTieu);
                model.DoanhThuThucTe = (long)thongKeTongQuan.Sum(x => x.DoanhThuThucTe);
                model.DoanhThuTiengAnh = (long)thanhToans.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Sum(x => x.ThanhToan.SoTienDaDong);
                model.DoanhThuTiengDuc = (long)thanhToans.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Sum(x => x.ThanhToan.SoTienDaDong);
                model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
                #endregion

                #region Khách hàng & Đơn hàng
                model.SoLuongDonHang = donHangs.Count;
                model.SoLuongThanhToan = thanhToans.Count;
                model.SoLuongKhachHangMoi = khachHangs.Count;
                model.SoLuongKhachHangUpSell = khachHangs
                    .Join(donHangs,
                        kh => kh.KhachHang.IdKhachHang,
                        dh => dh.DonHang.IdKhachHang,
                        (kh, dh) => new
                        {
                            KhachHang = kh.KhachHang,
                            DonHang = dh.DonHang,
                        })
                    .GroupBy(x => x.KhachHang)
                    .Select(gr => new
                    {
                        KhachHang = gr.Key,
                        DonHangs = gr.Select(x => x.DonHang).ToList()
                    })
                    .Where(x => x.DonHangs.Count > 1) // Số lượng khách có nhiều hơn 1 đơn hàng
                    .Count();

                var thanhToanHocThus = thanhToans.Where(x => x.ThanhToan.PhanTramDaDong < 10).ToList();
                model.SoLuongKhachHangHocThu = thanhToanHocThus.Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangHocChinh = thanhToans.Where(x => x.ThanhToan.PhanTramDaDong > 10).Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangHocThuSangHocChinh = thanhToanHocThus
                    .Select(x => x.ThanhToan.IdDonHang).Distinct()
                    .Join(donHangs,
                        tt => tt,
                        dh => dh.DonHang.IdDonHang,
                        (tt, dh) => new
                        {
                            DonHang = dh.DonHang,
                        })
                    .Join(thanhToans,
                        dh => dh.DonHang.IdDonHang,
                        tt => tt.ThanhToan.IdDonHang,
                        (dh, tt) => new
                        {
                            IdDonHang = dh.DonHang,
                            ThanhToan = tt.ThanhToan,
                        })
                    .GroupBy(x => x.IdDonHang)
                    .Where(gr => gr.Count() > 1 && gr.Sum(x => x.ThanhToan.PhanTramDaDong) >= 10) // Nhiều hơn 1 lần thanh toán và tổng % đã đóng >= 10
                    .Count();
                model.SoLuongKhachHangChuaDangKyHoc = model.SoLuongKhachHangMoi - thanhToans
                    .Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangChuaDangKyHoc = model.SoLuongKhachHangChuaDangKyHoc < 0 ? 0 : model.SoLuongKhachHangChuaDangKyHoc;

                tbKhachHang_LoaiKhachHang loaiKhachHang_DangHoc = db.tbKhachHang_LoaiKhachHang.FirstOrDefault(x => x.TenLoaiKhachHang == "Đang học") ?? new tbKhachHang_LoaiKhachHang();
                tbKhachHang_LoaiKhachHang loaiKhachHang_DaHocXong = db.tbKhachHang_LoaiKhachHang.FirstOrDefault(x => x.TenLoaiKhachHang == "Đã học xong") ?? new tbKhachHang_LoaiKhachHang();

                var donHangThanhToans = thanhToans
                    .GroupBy(x => x.ThanhToan.IdDonHang)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        IdKhachHang = gr.FirstOrDefault().ThanhToan.IdKhachHang,
                        PhanTramThanhToan = gr.Count() == 0 ? 0 : (gr.Sum(x => x.ThanhToan.PhanTramDaDong) / gr.Count()), // Nếu chỉ thanh toán trọn gói thì chỉ có 1 bản ghi
                        SoLanThanhToanTrenDonHang = gr.Count(),
                    }).ToList();
                model.SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang = donHangThanhToans
                    .Where(x => x.SoLanThanhToanTrenDonHang > 1)
                    .Select(x => x.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangThanhToanTronGoi = donHangThanhToans
                    .Where(x => x.SoLanThanhToanTrenDonHang == 1 && x.PhanTramThanhToan >= 80) // Thanh toán >= 80%
                    .Select(x => x.IdKhachHang).Distinct().Count();

                model.PhanTramThanhToanNhieuLan = model.SoLuongKhachHangMoi == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang / (decimal)model.SoLuongKhachHangMoi) * 100, 2);
                model.PhanTramUpSellChung = model.SoLuongKhachHangMoi == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangUpSell / (decimal)model.SoLuongKhachHangMoi), 2);
                model.PhanTramKhachHang_HocThuSangHocChinh = model.SoLuongKhachHangHocThu == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangHocThuSangHocChinh / (decimal)model.SoLuongKhachHangHocThu) * 100, 2);
                #endregion

                #region Sản phẩm
                var _donHangs = donHangs
                    .Join(sanPhamRepo,
                        dh => dh.DonHang.IdSanPham,
                        sp => sp.IdSanPham,
                        (dh, sp) => new
                        {
                            DonHang = dh.DonHang,
                            SanPham = sp,
                        })
                    .Join(loaiSanPhamRepo,
                        dh => dh.SanPham.IdLoaiSanPham,
                        lsp => lsp.IdLoaiSanPham,
                        (dh, lsp) => new
                        {
                            DonHang = dh.DonHang,
                            SanPham = dh.SanPham,
                            LoaiSanPham = lsp,
                        }).ToList();
                model.SoLuongSanPham_TiengAnh = _donHangs.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Count();
                model.SoLuongSanPham_TiengDuc = _donHangs.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Count();
                model.SanPham_BanChay = _donHangs
                  .GroupBy(x => x.SanPham.IdSanPham)
                  .Select(gr => new
                  {
                      Key = gr.Key,
                      SanPham = gr.FirstOrDefault().SanPham,
                      SoLuongDonHang = gr.Count(),
                  })
                  .OrderByDescending(x => x.SoLuongDonHang)
                  .Select(x => new SanPhamDaBan_Dto
                  {
                      SanPham = x.SanPham,
                      SoLuongDaBan = x.SoLuongDonHang,
                  }).Take(3).ToList();
                model.SanPham_TopDoanhThu = thanhToans
                    .Where(x => x.Thang == thang)
                    .GroupBy(x => x.SanPham.IdSanPham)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        SanPham = gr.FirstOrDefault().SanPham,
                        SoTienDaDong = gr.Sum(x => x.ThanhToan.SoTienDaDong),
                    })
                    .OrderByDescending(x => x.SoTienDaDong)
                     .Select(x => new SanPhamDaBan_Dto
                     {
                         SanPham = x.SanPham,
                         TongDoanhThu = (long)x.SoTienDaDong,
                     }).Take(3).ToList();
                #endregion
            }
            else
            {
                #region Doanh thu
                model.DoanhThuMucTieu = (long)thongKeTongQuan.Where(x => x.Thang == thang).Sum(x => x.DoanhThuMucTieu);
                model.DoanhThuThucTe = (long)thongKeTongQuan.Where(x => x.Thang == thang).Sum(x => x.DoanhThuThucTe);
                model.DoanhThuTiengAnh = (long)thanhToans.Where(x => x.Thang == thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Sum(x => x.ThanhToan.SoTienDaDong);
                model.DoanhThuTiengDuc = (long)thanhToans.Where(x => x.Thang == thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Sum(x => x.ThanhToan.SoTienDaDong);
                model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
                model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
                #endregion

                #region Khách hàng & Đơn hàng
                model.SoLuongDonHang = donHangs.Where(x => x.Thang == thang).Count();
                model.SoLuongThanhToan = thanhToans.Where(x => x.Thang == thang).Count();
                model.SoLuongKhachHangMoi = khachHangs.Where(x => x.Thang == thang).Count();
                model.SoLuongKhachHangUpSell = khachHangs
                    .Join(donHangs.Where(x => x.Thang <= thang).ToList(), // Chỉ lấy đơn hàng trước tháng so sánh
                        kh => kh.KhachHang.IdKhachHang,
                        dh => dh.DonHang.IdKhachHang,
                        (kh, dh) => new
                        {
                            KhachHang = kh.KhachHang,
                            DonHang = dh.DonHang,
                        })
                    .GroupBy(x => x.KhachHang)
                    .Select(gr => new
                    {
                        KhachHang = gr.Key,
                        DonHangs = gr.Select(x => x.DonHang).ToList()
                    })
                    .Where(x => x.DonHangs.Count > 1) // Số lượng khách có nhiều hơn 1 đơn hàng
                    .Count();

                var thanhToanHocThus = thanhToans.Where(x => x.ThanhToan.PhanTramDaDong < 10 && x.Thang == thang).ToList();
                model.SoLuongKhachHangHocThu = thanhToanHocThus.Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangHocChinh = thanhToans.Where(x => x.ThanhToan.PhanTramDaDong > 10).Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangHocThuSangHocChinh = thanhToanHocThus
                    .Select(x => x.ThanhToan.IdDonHang).Distinct()
                    .Join(donHangs,
                        tt => tt,
                        dh => dh.DonHang.IdDonHang,
                        (tt, dh) => new
                        {
                            DonHang = dh.DonHang,
                        })
                    .Join(thanhToans.Where(x => x.Thang == thang).ToList(), // Chỉ lấy thanh toán trong tháng
                        dh => dh.DonHang.IdDonHang,
                        tt => tt.ThanhToan.IdDonHang,
                        (dh, tt) => new
                        {
                            IdDonHang = dh.DonHang,
                            ThanhToan = tt.ThanhToan,
                        })
                    .GroupBy(x => x.IdDonHang)
                    .Where(gr => gr.Count() > 1 && gr.Sum(x => x.ThanhToan.PhanTramDaDong) >= 10) // Nhiều hơn 1 lần thanh toán và tổng % đã đóng >= 10
                    .Count();
                model.SoLuongKhachHangChuaDangKyHoc = model.SoLuongKhachHangMoi - thanhToans
                    .Where(x => x.Thang == thang)
                    .Select(x => x.ThanhToan.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangChuaDangKyHoc = model.SoLuongKhachHangChuaDangKyHoc < 0 ? 0 : model.SoLuongKhachHangChuaDangKyHoc;

                tbKhachHang_LoaiKhachHang loaiKhachHang_DangHoc = db.tbKhachHang_LoaiKhachHang.FirstOrDefault(x => x.TenLoaiKhachHang == "Đang học") ?? new tbKhachHang_LoaiKhachHang();
                tbKhachHang_LoaiKhachHang loaiKhachHang_DaHocXong = db.tbKhachHang_LoaiKhachHang.FirstOrDefault(x => x.TenLoaiKhachHang == "Đã học xong") ?? new tbKhachHang_LoaiKhachHang();

                var donHangThanhToans = thanhToans
                    .Where(x => x.Thang == thang)
                    .GroupBy(x => x.ThanhToan.IdDonHang)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        IdKhachHang = gr.FirstOrDefault().ThanhToan.IdKhachHang,
                        PhanTramThanhToan = gr.Count() == 0 ? 0 : (gr.Sum(x => x.ThanhToan.PhanTramDaDong) / gr.Count()), // Nếu chỉ thanh toán trọn gói thì chỉ có 1 bản ghi
                        SoLanThanhToanTrenDonHang = gr.Count(),
                    }).ToList();
                model.SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang = donHangThanhToans
                    .Where(x => x.SoLanThanhToanTrenDonHang > 1)
                    .Select(x => x.IdKhachHang).Distinct().Count();
                model.SoLuongKhachHangThanhToanTronGoi = donHangThanhToans
                    .Where(x => x.SoLanThanhToanTrenDonHang == 1 && x.PhanTramThanhToan >= 80) // Thanh toán >= 80%
                    .Select(x => x.IdKhachHang).Distinct().Count();

                model.PhanTramThanhToanNhieuLan = model.SoLuongKhachHangMoi == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangThanhToanNhieuLanTrenMotDonHang / (decimal)model.SoLuongKhachHangMoi) * 100, 2);
                model.PhanTramUpSellChung = model.SoLuongKhachHangMoi == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangUpSell / (decimal)model.SoLuongKhachHangMoi), 2);
                model.PhanTramKhachHang_HocThuSangHocChinh = model.SoLuongKhachHangHocThu == 0 ? 0 : Math.Round(((decimal)model.SoLuongKhachHangHocThuSangHocChinh / (decimal)model.SoLuongKhachHangHocThu) * 100, 2);
                #endregion

                #region Sản phẩm
                var _donHangs = donHangs.Where(x => x.Thang == thang)
                    .Join(sanPhamRepo,
                        dh => dh.DonHang.IdSanPham,
                        sp => sp.IdSanPham,
                        (dh, sp) => new
                        {
                            DonHang = dh.DonHang,
                            SanPham = sp,
                        })
                    .Join(loaiSanPhamRepo,
                        dh => dh.SanPham.IdLoaiSanPham,
                        lsp => lsp.IdLoaiSanPham,
                        (dh, lsp) => new
                        {
                            DonHang = dh.DonHang,
                            SanPham = dh.SanPham,
                            LoaiSanPham = lsp,
                        }).ToList();
                model.SoLuongSanPham_TiengAnh = _donHangs.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Count();
                model.SoLuongSanPham_TiengDuc = _donHangs.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Count();
                model.SanPham_BanChay = _donHangs
                    .GroupBy(x => x.SanPham.IdSanPham)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        SanPham = gr.FirstOrDefault().SanPham,
                        SoLuongDonHang = gr.Count(),
                    })
                    .OrderByDescending(x => x.SoLuongDonHang)
                    .Select(x => new SanPhamDaBan_Dto
                    {
                        SanPham = x.SanPham,
                        SoLuongDaBan = x.SoLuongDonHang,
                    }).Take(3).ToList();
                model.SanPham_TopDoanhThu = thanhToans
                    .Where(x => x.Thang == thang)
                    .GroupBy(x => x.SanPham.IdSanPham)
                    .Select(gr => new
                    {
                        Key = gr.Key,
                        SanPham = gr.FirstOrDefault().SanPham,
                        SoTienDaDong = gr.Sum(x => x.ThanhToan.SoTienDaDong),
                    })
                    .OrderByDescending(x => x.SoTienDaDong)
                     .Select(x => new SanPhamDaBan_Dto
                     {
                         SanPham = x.SanPham,
                         TongDoanhThu = (long)x.SoTienDaDong,
                     }).Take(3).ToList();
                #endregion
            }
            // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
            string viewAsString = Public.Handle.RenderViewToString(
                this,
                $"~/Views/Admin/QuanLyDoanhThu/quanlydoanhthu-thongke/thongke-chitiet/thongke.cshtml",
                model);
            return Json(new
            {
                ThongKeTongQuan = thongKeTongQuan,
                view = viewAsString
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region So sánh

        #endregion


        #endregion

        #region Private Methods
        private IEnumerable<DanhMucThongKeTheoNVKD> layDanhSachDoanhThuTheoNVKD(LocThongTinChiTietDto locThongTin)
        {
            if (string.IsNullOrEmpty(locThongTin.ThoiGian))
                locThongTin.ThoiGian = DateTime.Now.ToString("MM/yyyy");
            int thang = 0; int.TryParse(locThongTin.ThoiGian.Substring(0, 2), out thang);
            int nam = 0; int.TryParse(locThongTin.ThoiGian.Substring(3, 4), out nam);

            var chucVuRepo = db.default_tbChucVu.Where(x => x.TrangThai != 0);
            var NVKDRepo = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .Join(chucVuRepo,
                    nd => nd.IdChucVu,
                    cv => cv.IdChucVu,
                    (nd, cv) => new
                    {
                        NguoiDung = nd,
                        ChucVu = cv,
                    })
                .Where(x => x.ChucVu.MaChucVu == "NVKD")
                .Select(x => x.NguoiDung);

            var doanhThuNVKDs = db.tbNguoiDung_DoanhThu
               .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                       && x.NgayLenMucTieu.Substring(3, 4) == nam.ToString())
               .ToList() // Tải dữ liệu về trước
               .Join(NVKDRepo,
                       dt => dt.IdNguoiTao,
                       nd => nd.IdNguoiDung,
                       (dt, nd) => new
                       {
                           DoanhThu = dt,
                           NguoiDung = nd,
                       })
               .Select(x => new
               {
                   Thang = int.Parse(x.DoanhThu.NgayLenMucTieu.Substring(0, 2)), // Chuyển đổi sang số
                   DoanhThu = x.DoanhThu,
                   NguoiDung = x.NguoiDung
               })
               .ToList();

            var rs = new List<DanhMucThongKeTheoNVKD>();
            if (thang == 0) // Cả năm
            {
                rs = doanhThuNVKDs
                   .GroupBy(x => x.NguoiDung.IdNguoiDung)
                   .Select(gr => new DanhMucThongKeTheoNVKD
                   {
                       NguoiDung = gr.FirstOrDefault()?.NguoiDung,
                       DoanhThu = new tbNguoiDung_DoanhThu
                       {
                           DoanhThuMucTieu = gr.Sum(x => x.DoanhThu.DoanhThuMucTieu),
                           DoanhThuThucTe = gr.Sum(x => x.DoanhThu.DoanhThuThucTe),
                           PhanTramHoanThien = gr.Count() == 0 ? 0 : gr.Sum(x => x.DoanhThu.PhanTramHoanThien) / gr.Count(),
                       }
                   })
                   .OrderByDescending(x => x.DoanhThu.DoanhThuThucTe).ToList();
            }
            else
            {
                rs = doanhThuNVKDs
                   .Where(x => x.Thang == thang)
                   .GroupBy(x => x.NguoiDung.IdNguoiDung)
                   .Select(gr => new DanhMucThongKeTheoNVKD
                   {
                       NguoiDung = gr.FirstOrDefault()?.NguoiDung,
                       DoanhThu = new tbNguoiDung_DoanhThu
                       {
                           DoanhThuMucTieu = gr.Sum(x => x.DoanhThu.DoanhThuMucTieu),
                           DoanhThuThucTe = gr.Sum(x => x.DoanhThu.DoanhThuThucTe),
                           PhanTramHoanThien = gr.Count() == 0 ? 0 : gr.Sum(x => x.DoanhThu.PhanTramHoanThien) / gr.Count(),
                       }
                   })
                   .OrderByDescending(x => x.DoanhThu.DoanhThuThucTe).ToList();
            };

            //if (thang == 0 || locThongTin.LoaiThongKe == "theonam") // Cả năm
            //{
            //    #region Doanh thu
            //    model.DoanhThuMucTieu = (long)thongKeTongQuan.Sum(x => x.DoanhThuMucTieu);
            //    model.DoanhThuThucTe = (long)thongKeTongQuan.Sum(x => x.DoanhThuThucTe);
            //    model.DoanhThuTiengAnh = (long)thanhToans.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Sum(x => x.ThanhToan.SoTienDaDong);
            //    model.DoanhThuTiengDuc = (long)thanhToans.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Sum(x => x.ThanhToan.SoTienDaDong);
            //    model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
            //    #endregion
            //}
            //else
            //{
            //    #region Doanh thu
            //    model.DoanhThuMucTieu = (long)thongKeTongQuan.Where(x => x.Thang == thang).Sum(x => x.DoanhThuMucTieu);
            //    model.DoanhThuThucTe = (long)thongKeTongQuan.Where(x => x.Thang == thang).Sum(x => x.DoanhThuThucTe);
            //    model.DoanhThuTiengAnh = (long)thanhToans.Where(x => x.Thang == thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Sum(x => x.ThanhToan.SoTienDaDong);
            //    model.DoanhThuTiengDuc = (long)thanhToans.Where(x => x.Thang == thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Sum(x => x.ThanhToan.SoTienDaDong);
            //    model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
            //    model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
            //    #endregion
            //};

            return rs;
        }
        private IEnumerable<DanhMucThongKeTheoSanPham> layDanhSachDoanhThuTheoSanPham(LocThongTinChiTietDto locThongTin)
        {
            if (string.IsNullOrEmpty(locThongTin.ThoiGian))
                locThongTin.ThoiGian = DateTime.Now.ToString("MM/yyyy");
            int thang = 0; int.TryParse(locThongTin.ThoiGian.Substring(0, 2), out thang);
            int nam = 0; int.TryParse(locThongTin.ThoiGian.Substring(3, 4), out nam);

            var chucVuRepo = db.default_tbChucVu.Where(x => x.TrangThai != 0);
            var NVKDRepo = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .Join(chucVuRepo,
                    nd => nd.IdChucVu,
                    cv => cv.IdChucVu,
                    (nd, cv) => new
                    {
                        NguoiDung = nd,
                        ChucVu = cv,
                    })
                .Where(x => x.ChucVu.MaChucVu == "NVKD")
                .Select(x => x.NguoiDung);

            var doanhThuNVKDs = db.tbNguoiDung_DoanhThu
               .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                       && x.NgayLenMucTieu.Substring(3, 4) == nam.ToString())
               .ToList() // Tải dữ liệu về trước
               .Join(NVKDRepo,
                       dt => dt.IdNguoiTao,
                       nd => nd.IdNguoiDung,
                       (dt, nd) => new
                       {
                           DoanhThu = dt,
                           NguoiDung = nd,
                       })
               .Select(x => new
               {
                   Thang = int.Parse(x.DoanhThu.NgayLenMucTieu.Substring(0, 2)), // Chuyển đổi sang số
                   DoanhThu = x.DoanhThu,
                   NguoiDung = x.NguoiDung
               })
               .ToList();

            var rs = new List<DanhMucThongKeTheoSanPham>();

            //thongTinThongKe = db.tbSanPhams
            //  .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
            //  && locThongTin.IdDanhMucChas.Contains(x.IdLoaiSanPham.Value))
            //  .Select(x => new ThongTinThongKeDto
            //  {
            //      IdThongTin = x.IdSanPham,
            //      TenThongTin = x.TenSanPham
            //  })
            //  .ToList();
            //if (thang == 0 || locThongTin.LoaiThongKe == "theonam") // Cả năm
            //{
            //    rs = doanhThuNVKDs
            //       .GroupBy(x => x.NguoiDung.IdNguoiDung)
            //       .Select(gr => new DanhMucThongKeTheoNVKD
            //       {
            //           NguoiDung = gr.FirstOrDefault()?.NguoiDung,
            //           DoanhThu = new tbNguoiDung_DoanhThu
            //           {
            //               DoanhThuMucTieu = gr.Sum(x => x.DoanhThu.DoanhThuMucTieu),
            //               DoanhThuThucTe = gr.Sum(x => x.DoanhThu.DoanhThuThucTe),
            //               PhanTramHoanThien = gr.Count() == 0 ? 0 : gr.Sum(x => x.DoanhThu.PhanTramHoanThien) / gr.Count(),
            //           }
            //       })
            //       .OrderByDescending(x => x.DoanhThu).ToList();
            //}
            //else
            //{
            //    rs = doanhThuNVKDs
            //       .Where(x => x.Thang == thang)
            //       .GroupBy(x => x.NguoiDung.IdNguoiDung)
            //       .Select(gr => new DanhMucThongKeTheoNVKD
            //       {
            //           NguoiDung = gr.FirstOrDefault()?.NguoiDung,
            //           DoanhThu = new tbNguoiDung_DoanhThu
            //           {
            //               DoanhThuMucTieu = gr.Sum(x => x.DoanhThu.DoanhThuMucTieu),
            //               DoanhThuThucTe = gr.Sum(x => x.DoanhThu.DoanhThuThucTe),
            //               PhanTramHoanThien = gr.Count() == 0 ? 0 : gr.Sum(x => x.DoanhThu.PhanTramHoanThien) / gr.Count(),
            //           }
            //       })
            //       .OrderByDescending(x => x.DoanhThu).ToList();
            //};

            //if (thang == 0 || locThongTin.LoaiThongKe == "theonam") // Cả năm
            //{
            //    #region Doanh thu
            //    model.DoanhThuMucTieu = (long)thongKeTongQuan.Sum(x => x.DoanhThuMucTieu);
            //    model.DoanhThuThucTe = (long)thongKeTongQuan.Sum(x => x.DoanhThuThucTe);
            //    model.DoanhThuTiengAnh = (long)thanhToans.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Sum(x => x.ThanhToan.SoTienDaDong);
            //    model.DoanhThuTiengDuc = (long)thanhToans.Where(x => x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Sum(x => x.ThanhToan.SoTienDaDong);
            //    model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
            //    #endregion
            //}
            //else
            //{
            //    #region Doanh thu
            //    model.DoanhThuMucTieu = (long)thongKeTongQuan.Where(x => x.Thang == thang).Sum(x => x.DoanhThuMucTieu);
            //    model.DoanhThuThucTe = (long)thongKeTongQuan.Where(x => x.Thang == thang).Sum(x => x.DoanhThuThucTe);
            //    model.DoanhThuTiengAnh = (long)thanhToans.Where(x => x.Thang == thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Anh").Sum(x => x.ThanhToan.SoTienDaDong);
            //    model.DoanhThuTiengDuc = (long)thanhToans.Where(x => x.Thang == thang && x.LoaiSanPham.TenLoaiSanPham == "Tiếng Đức").Sum(x => x.ThanhToan.SoTienDaDong);
            //    model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
            //    model.PhanTramHoanThanhMucTieu = model.DoanhThuMucTieu == 0 ? 0 : Math.Round(((decimal)model.DoanhThuThucTe / (decimal)model.DoanhThuMucTieu) * 100, 2);
            //    #endregion
            //};

            return rs;
        }
        #endregion
    }
}