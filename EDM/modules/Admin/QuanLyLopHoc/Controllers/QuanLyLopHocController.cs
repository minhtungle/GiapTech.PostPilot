using Applications.QuanLyLopHoc.Dtos;
using Applications.QuanLyLopHoc.Models;
using EDM_DB;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using QuanLyKhachHang.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace QuanLyLopHoc.Controllers
{
    public class QuanLyLopHocController : RouteConfigController
    {
        // GET: QuanLyLopHoc
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyLopHoc";
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
        private List<tbSanPham> SANPHAMs
        {
            get
            {
                return Session["SANPHAMs"] as List<tbSanPham> ?? new List<tbSanPham>();
            }
            set
            {
                Session["SANPHAMs"] = value;
            }
        }
        private List<tbKhachHang_DonHang_TrangThaiHoc> DONHANG_TRANGTHAIHOCs
        {
            get
            {
                return Session["DONHANG_TRANGTHAIHOCs"] as List<tbKhachHang_DonHang_TrangThaiHoc> ?? new List<tbKhachHang_DonHang_TrangThaiHoc>();
            }
            set
            {
                Session["DONHANG_TRANGTHAIHOCs"] = value;
            }
        }
        private List<tbLopHoc_TrangThaiHoc> LOPHOC_TRANGTHAIHOCs
        {
            get
            {
                return Session["LOPHOC_TRANGTHAIHOCs"] as List<tbLopHoc_TrangThaiHoc> ?? new List<tbLopHoc_TrangThaiHoc>();
            }
            set
            {
                Session["LOPHOC_TRANGTHAIHOCs"] = value;
            }
        }
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
        private List<tbSanPham_LoaiSanPham_TrinhDo> TRINHDOs
        {
            get
            {
                return Session["TRINHDOs"] as List<tbSanPham_LoaiSanPham_TrinhDo> ?? new List<tbSanPham_LoaiSanPham_TrinhDo>();
            }
            set
            {
                Session["TRINHDOs"] = value;
            }
        }
        private List<tbKhachHang> KHACHHANGs
        {
            get
            {
                return Session["KHACHHANGs"] as List<tbKhachHang> ?? new List<tbKhachHang>();
            }
            set
            {
                Session["KHACHHANGs"] = value;
            }
        }
        private List<tbKhachHang_LoaiKhachHang> LOAIKHACHHANGs
        {
            get
            {
                return Session["LOAIKHACHHANGs"] as List<tbKhachHang_LoaiKhachHang> ?? new List<tbKhachHang_LoaiKhachHang>();
            }
            set
            {
                Session["LOAIKHACHHANGs"] = value;
            }
        }
        private List<tbGoiChamSoc> GOICHAMSOCs
        {
            get
            {
                return Session["GOICHAMSOCs"] as List<tbGoiChamSoc> ?? new List<tbGoiChamSoc>();
            }
            set
            {
                Session["GOICHAMSOCs"] = value;
            }
        }
        private List<tbDonViTien> DONVITIENs
        {
            get
            {
                return Session["DONVITIENs"] as List<tbDonViTien> ?? new List<tbDonViTien>();
            }
            set
            {
                Session["DONVITIENs"] = value;
            }
        }
        private List<tbPhuongThucThanhToan> PHUONGTHUCTHANHTOANs
        {
            get
            {
                return Session["PHUONGTHUCTHANHTOANs"] as List<tbPhuongThucThanhToan> ?? new List<tbPhuongThucThanhToan>();
            }
            set
            {
                Session["PHUONGTHUCTHANHTOANs"] = value;
            }
        }
        private List<tbNguoiDung> GIAOVIENs
        {
            get
            {
                return Session["GIAOVIENs"] as List<tbNguoiDung> ?? new List<tbNguoiDung>();
            }
            set
            {
                Session["GIAOVIENs"] = value;
            }
        }
        private List<tbDonHangExtend> DONHANGs
        {
            get
            {
                return Session["DONHANGs"] as List<tbDonHangExtend> ?? new List<tbDonHangExtend>();
            }
            set
            {
                Session["DONHANGs"] = value;
            }
        }
        public QuanLyLopHocController()
        {

        }
        #endregion

        public ActionResult Index()
        {
            #region Lấy các danh sách

            #region Thao tác
            var kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            var thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyLopHoc").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #region Loại sản phẩm
            var loaiSanPhams = db.tbSanPham_LoaiSanPham
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .OrderBy(x => x.Stt)
                .ToList() ?? new List<tbSanPham_LoaiSanPham>();
            #endregion

            #region Đơn hàng, lớp học - trạng thái học
            var donHangTrangThaiHocRepo = db.tbKhachHang_DonHang_TrangThaiHoc.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList() ?? new List<tbKhachHang_DonHang_TrangThaiHoc>();
            var lopHocTrangThaiHocRepo = db.tbLopHoc_TrangThaiHoc.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList() ?? new List<tbLopHoc_TrangThaiHoc>();
            #endregion

            #region Sản phẩm
            var sanPhams = db.tbSanPhams
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .OrderBy(x => x.Stt)
                .ToList() ?? new List<tbSanPham>();
            #endregion

            #region Trình độ
            var trinhDos = db.tbSanPham_LoaiSanPham_TrinhDo
                .Where(x => x.TrangThai != 0)
                .OrderBy(x => x.IdLoaiSanPham)
                .ThenBy(x => x.CapDo)
                .ToList() ?? new List<tbSanPham_LoaiSanPham_TrinhDo>();
            #endregion

            #region Giáo viên
            var giaoViens = db.tbNguoiDungs
                .Join(db.default_tbChucVu,
                    gv => gv.IdChucVu,
                    cv => cv.IdChucVu,
                    (gv, cv) => new
                    {
                        GiaoVien = gv,
                        ChucVu = cv
                    })
                .Where(x => x.GiaoVien.TrangThai != 0 && x.GiaoVien.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                    && x.ChucVu.MaChucVu == "GV")
                .Select(x => x.GiaoVien)
                .ToList() ?? new List<tbNguoiDung>();
            #endregion

            #region Phương thức thanh toán
            var phuongThucThanhToans = db.Database.SqlQuery<tbPhuongThucThanhToan>($@"
            select * from tbPhuongThucThanhToan
            where TrangThai != 0
            order by Stt").ToList() ?? new List<tbPhuongThucThanhToan>();
            #endregion

            #region Khách hàng
            var khachHangs = db.tbKhachHangs
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .OrderBy(x => x.TenKhachHang).ToList() ?? new List<tbKhachHang>();
            #endregion

            #region Loại khách hàng
            var loaiKhachHangs = db.tbKhachHang_LoaiKhachHang
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                && x.Stt != 4 && x.Stt != 5)
                .OrderBy(x => x.Stt)
                .ToList() ?? new List<tbKhachHang_LoaiKhachHang>();
            #endregion

            #region Gói chăm sóc
            var goiChamSocs = db.Database.SqlQuery<tbGoiChamSoc>($@"
            select * from tbGoiChamSoc
            where TrangThai != 0
            order by Stt").ToList() ?? new List<tbGoiChamSoc>();
            #endregion

            #region Đơn vị tiền
            var donViTiens = db.tbDonViTiens.Where(x => x.TrangThai != 0).ToList();
            #endregion

            #endregion

            THAOTACs = thaoTacs;
            SANPHAMs = sanPhams;
            LOAISANPHAMs = loaiSanPhams;
            LOPHOC_TRANGTHAIHOCs = lopHocTrangThaiHocRepo;
            GIAOVIENs = giaoViens;
            TRINHDOs = trinhDos;
            KHACHHANGs = khachHangs;
            LOAIKHACHHANGs = loaiKhachHangs;
            GOICHAMSOCs = goiChamSocs;
            DONVITIENs = donViTiens;
            PHUONGTHUCTHANHTOANs = phuongThucThanhToans;

            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;

            return View($"{VIEW_PATH}/quanlylophoc.cshtml");
        }
        [HttpPost]
        public ActionResult getList_ChoXepLop(LocThongTin_ChoXepLop_Dto locThongTin)
        {
            //var locThongTin = JsonConvert.DeserializeObject<LocThongTin_ChoXepLop_Dto>(Request.Form["locThongTin"]);
            var trangThaiDonHangRepo = DONHANG_TRANGTHAIHOCs
                .Where(x => x.TenTrangThai.ToLower().Contains("chờ xếp lớp"))
                .Select(x => x.IdTrangThai).ToList();
            locThongTin.IdTrangThaiHoc = trangThaiDonHangRepo;
            List<tbDonHangExtend> donHangs = getDonHangs(loai: "all", locThongTin: locThongTin).ToList(); // Lấy đơn hàng chờ xếp lớp
            return PartialView($"{VIEW_PATH}/test/quanlylophoc-lophoc/lophoc-getList/choxeplop/choxeplop-getList.cshtml", donHangs);
        }
        [HttpPost]
        public ActionResult getList_DaXepLop(LocThongTin_DaXepLop_Dto locThongTin)
        {
            //var locThongTin = JsonConvert.DeserializeObject<LocThongTin_DaXepLop_Dto>(Request.Form["locThongTin"]);
            List<tbLopHocExtend> lopHocs = get_LopHocs(loai: "all", locThongTin: locThongTin);
            return PartialView($"{VIEW_PATH}/test/quanlylophoc-lophoc/lophoc-getList/daxeplop/daxeplop-getList.cshtml", lopHocs);
        }

        #region Lấy các danh sách dữ liệu
        [HttpGet]
        public List<tbNguoiDung> get_NguoiDungs()
        {
            List<tbNguoiDung> nguoiDungs = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .OrderByDescending(x => x.NgayTao).ToList();
            return nguoiDungs;
        }
        public List<tbLopHocExtend> get_LopHocs(string loai = "all", List<Guid> idLopHocs = null, LocThongTin_DaXepLop_Dto locThongTin = null)
        {
            var lopHocRepo = db.tbLopHocs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();
            var sanPhamRepo = db.tbSanPhams.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();

            // 1. Lọc dữ liệu lớp học ban đầu
            var filteredLopHocs = lopHocRepo
                .Where(lh =>
                    (loai != "single" || idLopHocs.Contains(lh.IdLopHoc)) &&
                    (locThongTin == null || (
                        (string.IsNullOrEmpty(locThongTin.ThoiGian) || (lh.NgayTao.HasValue && lh.NgayTao.Value.ToString("MM/yyyy") == locThongTin.ThoiGian)) &&
                        (string.IsNullOrEmpty(locThongTin.TenLopHoc) || lh.TenLopHoc.Contains(locThongTin.TenLopHoc)) &&
                        (locThongTin.IdGiaoVien == null || lh.IdGiaoVien.Split(',').Any(x => x.Trim() == locThongTin.IdGiaoVien.ToString())) &&
                        (locThongTin.IdKhachHang == null || lh.IdKhachHang.Split(',').Any(x => x.Trim() == locThongTin.IdKhachHang.ToString()))
                    ))
                )
                .OrderByDescending(lh => lh.NgayTao)
                .ThenBy(lh => lh.NgaySua)
                .ToList(); // Đây là kết quả trung gian (var a)

            // 2. Từ kết quả đã lọc, tiếp tục xử lý dữ liệu khác
            var lopHocs = filteredLopHocs.Select(lh =>
            {
                var rs = new tbLopHocExtend
                {
                    LopHoc = lh,
                    GiaoViens = getGiaoViens(str_idGiaoViens: lh.IdGiaoVien),
                    KhachHangs = getKhachHangs(str_idKhachHangs: lh.IdKhachHang),
                    BuoiHocs = getBuoiHocs(idLopHoc: lh.IdLopHoc),
                    TaiLieus = getTaiLieus(idLopHoc: lh.IdLopHoc),
                };
                if (lh.IdDonHang != null)
                {
                    var idDonHangs = lh.IdDonHang.Split(',') // Tách chuỗi thành danh sách GUID
                                                           .Where(x => !string.IsNullOrWhiteSpace(x))
                                                           .Select(x => Guid.Parse(x.Trim())).ToList();
                    rs.DonHangs = getDonHangs(loai: "single", idDonHangs: idDonHangs);
                }
                ;

                return rs;
            }).ToList();

            return lopHocs;
        }
        public List<tbLopHoc_BuoiHocExtend> getBuoiHocs(Guid idLopHoc)
        {
            List<tbLopHoc_BuoiHocExtend> lopHoc_BuoiHocs = db.tbLopHoc_BuoiHoc
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                && x.IdLopHoc == idLopHoc)
                .OrderBy(x => x.ThuTuBuoiHoc)
                .Select(x => new tbLopHoc_BuoiHocExtend
                {
                    BuoiHoc = x,
                })
                .ToList();
            return lopHoc_BuoiHocs;
        }
        public List<tbLopHoc_TaiLieu> getTaiLieus(Guid idLopHoc)
        {
            List<tbLopHoc_TaiLieu> lopHoc_TaiLieus = db.tbLopHoc_TaiLieu
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                && x.IdLopHoc == idLopHoc)
                .OrderBy(x => x.Stt).ToList() ?? new List<tbLopHoc_TaiLieu>();
            return lopHoc_TaiLieus;
        }
        public List<tbLopHoc_BuoiHoc_HinhAnh> getHinhAnhs(Guid idBuoiHoc)
        {
            List<tbLopHoc_BuoiHoc_HinhAnh> lopHoc_BuoiHoc_HinhAnhs = db.tbLopHoc_BuoiHoc_HinhAnh
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                && x.IdLopHoc_BuoiHoc == idBuoiHoc)
                .OrderByDescending(x => x.Stt)
                .ToList() ?? new List<tbLopHoc_BuoiHoc_HinhAnh>();
            return lopHoc_BuoiHoc_HinhAnhs;
        }
        public List<tbDonHangExtend> getDonHangs(string loai = "all", List<Guid> idDonHangs = null, LocThongTin_ChoXepLop_Dto locThongTin = null)
        {
            var donHangRepo = db.tbKhachHang_DonHang.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();
            var sanPhamRepo = db.tbSanPhams.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();
            var trinhDoRepo = db.tbSanPham_LoaiSanPham_TrinhDo.Where(x => x.TrangThai != 0).ToList();
            var khachHangRepo = db.tbKhachHangs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();
            var nguoiDungRepo = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();

            var donHangs = (from dh in donHangRepo
                            join sp in sanPhamRepo on dh.IdSanPham equals sp.IdSanPham
                            join tddv in trinhDoRepo on dh.IdTrinhDoDauVao equals tddv.IdTrinhDo
                            join tddr in trinhDoRepo on dh.IdTrinhDoDauRa equals tddr.IdTrinhDo
                            join kh in khachHangRepo on dh.IdKhachHang equals kh.IdKhachHang
                            join nt in nguoiDungRepo on dh.IdNguoiTao equals nt.IdNguoiDung

                            where (loai != "single" || idDonHangs.Contains(dh.IdDonHang)) &&
                                  (locThongTin == null || (
                                  ((locThongTin.IdTrangThaiHoc.Count == 0) || (dh.IdTrangThaiHoc.HasValue && locThongTin.IdTrangThaiHoc.Contains(dh.IdTrangThaiHoc.Value))) &&
                                  (locThongTin.IdLoaiSanPham == null || sp.IdLoaiSanPham == locThongTin.IdLoaiSanPham) &&
                                  (locThongTin.IdSanPham == null || sp.IdSanPham == locThongTin.IdSanPham) &&
                                  (string.IsNullOrEmpty(locThongTin.ThoiGian) || (dh.NgayTao.HasValue && dh.NgayTao.Value.ToString("MM/yyyy") == locThongTin.ThoiGian)) &&
                                  (locThongTin.IdKhachHang == null || dh.IdKhachHang == locThongTin.IdKhachHang) &&
                                  (locThongTin.IdTrinhDoDauVao == null || dh.IdTrinhDoDauVao == locThongTin.IdTrinhDoDauVao) &&
                                  (locThongTin.IdTrinhDoDauRa == null || dh.IdTrinhDoDauRa == locThongTin.IdTrinhDoDauRa)
                                  ))
                            orderby dh.Stt descending

                            select new tbDonHangExtend
                            {
                                DonHang = dh,

                                SanPham = sp,
                                TrinhDoDauVao = tddv,
                                TrinhDoDauRa = tddr,
                                KhachHang = kh,
                                ThongTinNguoiTao = nt,
                            }).ToList();

            return donHangs;
        }
        public List<tbKhachHangExtend> get_KhachHangs(
               string loai = "all", List<Guid> idKhachHangs = null,
             Applications.QuanLyKhachHang.Dtos.LocThongTinDto locThongTin = null)
        {
            // Chỉ hiển thị bản ghi có quyền truy cập
            var khachHangRepo = db.tbKhachHangs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();
            var nguoiDungRepo = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();

            var khachHangs = (from kh in khachHangRepo
                              join nd in nguoiDungRepo on kh.IdNguoiTao equals nd.IdNguoiDung
                              where (loai != "single" || idKhachHangs.Contains(kh.IdKhachHang)) &&
                                    (locThongTin == null || (
                                        (locThongTin.NgayTao == null || (kh.NgayTao.HasValue && kh.NgayTao.Value.ToString("MM/yyyy") == locThongTin.NgayTao)) &&
                                        (locThongTin.IdCoCauToChucs == null || locThongTin.IdCoCauToChucs.Contains(nd.IdCoCauToChuc.Value)) &&
                                        (string.IsNullOrEmpty(locThongTin.TenKhachHang) || kh.TenKhachHang.ToLower().Contains(locThongTin.TenKhachHang.ToLower())) &&
                                        (string.IsNullOrEmpty(locThongTin.TenNhanVien) || nd.TenNguoiDung.ToLower().Contains(locThongTin.TenNhanVien.ToLower())) &&
                                        (string.IsNullOrEmpty(locThongTin.Email) || kh.Email.ToLower().Contains(locThongTin.Email.ToLower())) &&
                                        (locThongTin.IdLoaiKhachHang == null || locThongTin.IdLoaiKhachHang == Guid.Empty || kh.IdLoaiKhachHang == locThongTin.IdLoaiKhachHang) &&
                                        (locThongTin.IdGoiChamSoc == null || locThongTin.IdGoiChamSoc == Guid.Empty || kh.IdGoiChamSoc == locThongTin.IdGoiChamSoc)
                                    ))
                              orderby kh.NgayTao descending, kh.NgaySua descending
                              select new tbKhachHangExtend
                              {
                                  IdKhachHang = kh.IdKhachHang,
                                  IdLoaiKhachHang = kh.IdLoaiKhachHang,
                                  IdGoiChamSoc = kh.IdGoiChamSoc,
                                  IdQuocGiaSinhSong = kh.IdQuocGiaSinhSong,
                                  IdPhuongThucThanhToan = kh.IdPhuongThucThanhToan,
                                  TenKhachHang = kh.TenKhachHang,
                                  Email = kh.Email,
                                  LienKet = kh.LienKet,
                                  LienKetSale = kh.LienKetSale,
                                  SoDienThoai = kh.SoDienThoai,
                                  DoTuoi = kh.DoTuoi,
                                  NgheNghiep = kh.NgheNghiep,
                                  NguonKhachHang = kh.NguonKhachHang,
                                  DiaChi = kh.DiaChi,
                                  QuyenTruyCap = kh.QuyenTruyCap,
                                  GhiChu = kh.GhiChu,
                                  TrangThai = kh.TrangThai,
                                  MaDonViSuDung = kh.MaDonViSuDung,
                                  IdNguoiTao = kh.IdNguoiTao,
                                  IdNguoiSua = kh.IdNguoiSua,

                                  NgayTao = kh.NgayTao,
                                  NgaySua = kh.NgaySua,
                                  LoaiKhachHang = LOAIKHACHHANGs.FirstOrDefault(x => x.IdLoaiKhachHang == kh.IdLoaiKhachHang) ?? new tbKhachHang_LoaiKhachHang(),
                                  GoiChamSoc = GOICHAMSOCs.FirstOrDefault(x => x.IdGoiChamSoc == kh.IdGoiChamSoc) ?? new tbGoiChamSoc(),
                                  PhuongThucThanhToan = PHUONGTHUCTHANHTOANs.FirstOrDefault(x => x.IdPhuongThucThanhToan == kh.IdPhuongThucThanhToan) ?? new tbPhuongThucThanhToan(),
                                  ThongTinNguoiTao = nd,
                                  DonViSuDung = per.DonViSuDung,
                              }).ToList();


            return khachHangs;
        }
        public List<tbKhachHang_DonHangExtend> get_KhachHang_DonHangs(tbKhachHangExtend khachHang)
        {
            List<tbKhachHang_DonHangExtend> khachHang_DonHangs = db.Database.SqlQuery<tbKhachHang_DonHangExtend>($@"
            select * 
            from tbKhachHang_DonHang 
            where IdKhachHang = '{khachHang.IdKhachHang}' and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
            order by ThuTuDonHang asc").ToList() ?? new List<tbKhachHang_DonHangExtend>();

            foreach (tbKhachHang_DonHangExtend khachHang_DonHang in khachHang_DonHangs)
            {
                khachHang_DonHang.SanPham = db.tbSanPhams.FirstOrDefault(x => x.IdSanPham == khachHang_DonHang.IdSanPham) ?? new tbSanPham();
            }
            ;
            return khachHang_DonHangs;
        }
        public List<tbKhachHang_DonHang_ThanhToan> get_KhachHang_DonHang_ThanhToans(tbKhachHang_DonHangExtend khachHang_DonHang)
        {
            List<tbKhachHang_DonHang_ThanhToan> khachHang_DonHang_ThanhToans = new List<tbKhachHang_DonHang_ThanhToan>();
            khachHang_DonHang_ThanhToans = db.Database.SqlQuery<tbKhachHang_DonHang_ThanhToan>($@"select * 
                from tbKhachHang_DonHang_ThanhToan
                where IdDonHang = '{khachHang_DonHang.IdDonHang}' and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                order by ThuTuThanhToan asc
                ").ToList() ?? new List<tbKhachHang_DonHang_ThanhToan>();
            return khachHang_DonHang_ThanhToans;
        }
        public ActionResult lopHocThamGia(Guid idGiaoVien)
        {
            /**
             * 0: Xóa, 
             * 1: Khởi tạo - Chưa bắt đầu, 
             * 2: Đang học, 
             * 3: Tạm dừng
             * 4: Kết thúc
             */
            List<tbLopHocExtend> lopHocThamGias = get_LopHocs(loai: "all", locThongTin: new LocThongTin_DaXepLop_Dto
            {
                IdGiaoVien = idGiaoVien,
            });
            return PartialView($"{VIEW_PATH}/giaovien/lophocthamgia/lophocthamgia-getList.cshtml", lopHocThamGias);
        }
        public ActionResult buoiHocSapToi(Guid idGiaoVien)
        {
            var ngayHomNay = DateTime.Today;
            var ngayHomQua = ngayHomNay.AddDays(-1);

            // Lấy dữ liệu ban đầu
            var lopHocRepo = db.tbLopHocs
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .ToList();

            var buoiHocRepo = db.tbLopHoc_BuoiHoc
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .ToList();

            // 1️⃣ Buổi học từ hôm qua đến hôm nay
            var buoiHocHomQuaDenHomNay = buoiHocRepo
                .Where(bh =>
                    bh.IdGiaoVien.Contains(idGiaoVien.ToString()) &&
                    bh.ThoiGianBatDau.HasValue &&
                    bh.ThoiGianBatDau.Value.Date >= ngayHomQua &&
                    bh.ThoiGianBatDau.Value.Date <= ngayHomNay)
                .Join(lopHocRepo,
                    buoiHoc => buoiHoc.IdLopHoc,
                    lopHoc => lopHoc.IdLopHoc,
                    (buoiHoc, lopHoc) => new BuoiHocSapToiOutput_Dto
                    {
                        BuoiHoc = buoiHoc,
                        LopHoc = lopHoc,
                        SoNgayConLai = (int)(buoiHoc.ThoiGianBatDau.Value.Date - ngayHomNay).TotalDays
                    });

            // 2️⃣ Buổi học sau hôm nay (lấy 7 buổi sắp tới)
            var buoiHocSauHomNay = buoiHocRepo
                .Where(bh =>
                    bh.IdGiaoVien.Contains(idGiaoVien.ToString()) &&
                    bh.ThoiGianBatDau.HasValue &&
                    bh.ThoiGianBatDau.Value.Date > ngayHomNay)
                .Join(lopHocRepo,
                    buoiHoc => buoiHoc.IdLopHoc,
                    lopHoc => lopHoc.IdLopHoc,
                    (buoiHoc, lopHoc) => new BuoiHocSapToiOutput_Dto
                    {
                        BuoiHoc = buoiHoc,
                        LopHoc = lopHoc,
                        SoNgayConLai = (int)(buoiHoc.ThoiGianBatDau.Value.Date - ngayHomNay).TotalDays
                    })
                .OrderBy(dto => dto.BuoiHoc.ThoiGianBatDau)
                .Take(7);

            // 3️⃣ Gộp hai kết quả và sắp xếp
            var buoiHocSapTois = buoiHocHomQuaDenHomNay
                .Union(buoiHocSauHomNay)
                .OrderBy(x => x.SoNgayConLai)
                .ThenBy(x => x.BuoiHoc.ThoiGianBatDau)
                .ToList();


            return PartialView($"{VIEW_PATH}/giaovien/buoihocsaptoi/buoihocsaptoi-getList.cshtml", buoiHocSapTois);
        }
        #endregion

        #region Lớp học
        [HttpPost]
        public ActionResult displayModal_KhachHang_XemChiTiet(Guid idKhachHang)
        {
            #region Thông tin chung
            tbKhachHangExtend khachHang = new tbKhachHangExtend();
            khachHang = get_KhachHangs(loai: "single", idKhachHangs: new List<Guid> { idKhachHang }).FirstOrDefault();
            #endregion

            #region Đơn hàng
            List<tbKhachHang_DonHangExtend> khachHang_DonHangs = get_KhachHang_DonHangs(khachHang: khachHang);
            khachHang.DonHangs = khachHang_DonHangs;
            #endregion

            #region Thanh toán
            List<tbKhachHang_DonHang_ThanhToan> khachHang_DonHang_ThanhToans = new List<tbKhachHang_DonHang_ThanhToan>();
            foreach (tbKhachHang_DonHangExtend khachHang_DonHang in khachHang.DonHangs)
            {
                khachHang_DonHang.ThanhToans = get_KhachHang_DonHang_ThanhToans(khachHang_DonHang: khachHang_DonHang);
            }
            ;
            #endregion

            ViewBag.khachHang = khachHang;
            return PartialView($"{VIEW_PATH}/khachhang/khachhang-xemchitiet.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_CRUD_LopHoc(string loai, Guid idLopHoc)
        {
            #region Thông tin chung
            tbLopHocExtend lopHoc = new tbLopHocExtend();
            if (loai != "create" && idLopHoc != Guid.Empty)
            {
                lopHoc = get_LopHocs(loai: "single", idLopHocs: new List<Guid> { idLopHoc }).FirstOrDefault();
            }
            ;
            #endregion

            var model = new DisplayModal_XemLichHoc_Output_Dto
            {
                Loai = loai,
                LopHoc = lopHoc,
            };
            return PartialView($"{VIEW_PATH}/test/quanlylophoc-lophoc/lophoc-crud.cshtml", model);
        }

        [HttpPost]
        public tbLopHocExtend kiemTra_LopHoc(tbLopHocExtend lopHoc)
        {
            List<string> ketQuas = new List<string>();
            // Kiểm tra còn sản phẩm khác có trùng mã không
            tbLopHoc lopHoc_OLD = db.tbLopHocs.FirstOrDefault(x =>
            //x.MaLopHoc == lopHoc.MaLopHoc && 
            x.TenLopHoc == lopHoc.LopHoc.TenLopHoc &&
            x.IdLopHoc != lopHoc.LopHoc.IdLopHoc &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (lopHoc_OLD != null)
            {
                ketQuas.Add("Lớp học đã tồn tại");
                lopHoc.KiemTraExcel.TrangThai = 0;
            }
            ;
            lopHoc.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            return lopHoc;
        }
        [HttpPost]
        public ActionResult create_LopHoc()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbLopHocExtend lopHoc_NEW = JsonConvert.DeserializeObject<tbLopHocExtend>(Request.Form["lopHoc"]);
                    if (lopHoc_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_LopHoc(lopHoc: lopHoc_NEW).KiemTraExcel.TrangThai == 0)
                        {
                            status = "warning";
                            mess = lopHoc_NEW.KiemTraExcel.KetQua;
                        }
                        else
                        {
                            List<Guid> idKhachHangs = new List<Guid>();
                            List<Guid> idGiaoViens = new List<Guid>();
                            List<Guid> idDonHangs = new List<Guid>();
                            // Thêm mới
                            tbLopHoc lopHoc = new tbLopHoc
                            {
                                IdLopHoc = Guid.NewGuid(),

                                TenLopHoc = lopHoc_NEW.LopHoc.TenLopHoc,
                                NenTangHoc = lopHoc_NEW.LopHoc.NenTangHoc,
                                SoBuoi = 0,

                                GhiChu = lopHoc_NEW.LopHoc.GhiChu,
                                TrangThaiLopHoc = 1,
                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbLopHocs.Add(lopHoc);

                            #region Tạo thư mục ảnh cho lớp học
                            var duongDanHinhAnh = duongDanAnhBuoiHoc(idLopHoc: lopHoc.IdLopHoc, idBuoiHoc: Guid.Empty);
                            if (System.IO.Directory.Exists(duongDanHinhAnh.DuongDanThuMuc_LOPHOC_SERVER))
                                System.IO.Directory.Delete(duongDanHinhAnh.DuongDanThuMuc_LOPHOC_SERVER, true);
                            System.IO.Directory.CreateDirectory(duongDanHinhAnh.DuongDanThuMuc_LOPHOC_SERVER);
                            #endregion

                            foreach (tbLopHoc_TaiLieu taiLieu_NEW in lopHoc_NEW.TaiLieus)
                            {
                                tbLopHoc_TaiLieu taiLieu = new tbLopHoc_TaiLieu
                                {
                                    IdTaiLieu = Guid.NewGuid(),
                                    IdLopHoc = lopHoc.IdLopHoc,

                                    TenTaiLieu = taiLieu_NEW.TenTaiLieu,
                                    DuongDanTaiLieu = taiLieu_NEW.DuongDanTaiLieu,

                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbLopHoc_TaiLieu.Add(taiLieu);
                            }
                            ;

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
        public ActionResult update_LopHoc()
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbLopHocExtend lopHoc_NEW = JsonConvert.DeserializeObject<tbLopHocExtend>(Request.Form["lopHoc"]);
                    // Kiểm tra có trùng bản ghi nào khác không
                    if (kiemTra_LopHoc(lopHoc: lopHoc_NEW).KiemTraExcel.TrangThai == 0)
                    {
                        status = "warning";
                        mess = lopHoc_NEW.KiemTraExcel.KetQua;
                    }
                    else
                    {
                        tbLopHoc lopHoc_OLD = db.tbLopHocs.Find(lopHoc_NEW.LopHoc.IdLopHoc);

                        lopHoc_OLD.TenLopHoc = lopHoc_NEW.LopHoc.TenLopHoc;
                        lopHoc_OLD.NenTangHoc = lopHoc_NEW.LopHoc.NenTangHoc;
                        lopHoc_OLD.SoBuoi = lopHoc_NEW.BuoiHocs.Count;

                        //lopHoc_OLD.TrangThaiLopHoc = 1,
                        lopHoc_OLD.GhiChu = lopHoc_NEW.LopHoc.GhiChu;
                        lopHoc_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        lopHoc_OLD.NgaySua = DateTime.Now;

                        // Xóa tài liệu
                        db.Database.ExecuteSqlCommand($@"update tbLopHoc_TaiLieu 
                        set TrangThai = 0 
                        where IdLopHoc = '{lopHoc_NEW.LopHoc.IdLopHoc}' and TrangThai = 1 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'");
                        foreach (tbLopHoc_TaiLieu taiLieu_NEW in lopHoc_NEW.TaiLieus)
                        {
                            // Kiểm tra tồn tại
                            tbLopHoc_TaiLieu taiLieu_OLD = db.tbLopHoc_TaiLieu.Find(taiLieu_NEW.IdTaiLieu);
                            if (taiLieu_OLD == null)
                            {
                                tbLopHoc_TaiLieu taiLieu = new tbLopHoc_TaiLieu
                                {
                                    IdTaiLieu = Guid.NewGuid(),
                                    IdLopHoc = lopHoc_NEW.LopHoc.IdLopHoc,

                                    TenTaiLieu = taiLieu_NEW.TenTaiLieu,
                                    DuongDanTaiLieu = taiLieu_NEW.DuongDanTaiLieu,

                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbLopHoc_TaiLieu.Add(taiLieu);
                            }
                            else
                            {
                                taiLieu_OLD.TenTaiLieu = taiLieu_NEW.TenTaiLieu;
                                taiLieu_OLD.DuongDanTaiLieu = taiLieu_NEW.DuongDanTaiLieu;

                                taiLieu_OLD.TrangThai = 1;
                                taiLieu_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                taiLieu_OLD.NgaySua = DateTime.Now;
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
        public JsonResult delete_LopHocs()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idLopHocs = Request.Form["str_idLopHocs"];
                    List<Guid> idLopHocs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idLopHocs"]);
                    foreach (Guid idLopHoc in idLopHocs)
                    {
                        tbLopHoc lopHoc = db.tbLopHocs.Find(idLopHoc);

                        // Xóa bản ghi trong db
                        lopHoc.TrangThai = 0;
                        lopHoc.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        lopHoc.NgaySua = DateTime.Now;

                        db.Database.ExecuteSqlCommand($@"
                        -- Buổi học
                        update tbLopHoc_BuoiHoc
                        set TrangThai = 0, IdNguoiSua = '{per.NguoiDung.IdNguoiDung}', NgaySua = '{DateTime.Now}'
                        where IdLopHoc = '{idLopHoc}' and TrangThai = 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'

                        -- Tài liệu
                        update tbLopHoc_BuoiHoc_HinhAnh
                        set TrangThai = 0, IdNguoiSua = '{per.NguoiDung.IdNguoiDung}', NgaySua = '{DateTime.Now}'
                        where IdLopHoc = '{idLopHoc}' and TrangThai = 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'

                        -- Hình ảnh
                        update tbLopHoc_TaiLieu
                        set TrangThai = 0, IdNguoiSua = '{per.NguoiDung.IdNguoiDung}', NgaySua = '{DateTime.Now}'
                        where IdLopHoc = '{idLopHoc}' and TrangThai = 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                        ");

                        #region Tạo thư mục ảnh cho lớp học
                        var duongDanHinhAnh = duongDanAnhBuoiHoc(idLopHoc: lopHoc.IdLopHoc, idBuoiHoc: Guid.Empty);
                        if (System.IO.Directory.Exists(duongDanHinhAnh.DuongDanThuMuc_LOPHOC_SERVER))
                            System.IO.Directory.Delete(duongDanHinhAnh.DuongDanThuMuc_LOPHOC_SERVER, true);
                        #endregion

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
        [HttpPost]
        public ActionResult guiMail(Guid idLopHoc)
        {
            string status = "success";
            string mess = "Gửi mail thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    #region Thông tin chung
                    tbLopHocExtend lopHoc = new tbLopHocExtend();
                    lopHoc = get_LopHocs(loai: "single", idLopHocs: new List<Guid> { idLopHoc }).FirstOrDefault();
                    #endregion

                    if (lopHoc.BuoiHocs.Count == 0 || lopHoc.BuoiHocs == null)
                    {
                        status = "warning";
                        mess = "Lớp học này hiện chưa được xếp lịch, vui lòng xếp lịch trước khi thao tác";
                    }
                    else
                    {
                        #region Gửi mail
                        string mail()
                        {
                            var model = lopHoc;
                            // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                            string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/mail/quanlylophoc-mail.cshtml", model);
                            // Trả về chuỗi đã được tạo ra từ view
                            return viewAsString;
                        }
                        string tieuDeMail = "[📣 VIETGEN] - THÔNG TIN LỚP HỌC";
                        string mailBody = mail();
                        // Gửi mail
                        foreach (tbKhachHang khachHang in lopHoc.KhachHangs)
                        {
                            Public.Handle.SendEmail(sendTo: khachHang.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                        }
                        ;
                        #endregion
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
        #endregion

        #region Lịch học & Buổi học
        [HttpPost]
        public ActionResult displayModal_XemLichHoc(string loai, Guid idLopHoc)
        {
            #region Thông tin chung
            tbLopHocExtend lopHoc = new tbLopHocExtend();
            if (loai != "create" && idLopHoc != Guid.Empty)
            {
                lopHoc = get_LopHocs(loai: "single", idLopHocs: new List<Guid> { idLopHoc }).FirstOrDefault();
            }
            ;
            #endregion

            var model = new DisplayModal_XemLichHoc_Output_Dto
            {
                Loai = loai,
                LopHoc = lopHoc,
            };
            return PartialView($"{VIEW_PATH}/test/quanlylophoc-lichhoc/lichhoc-xemlichhoc.cshtml", model);
        }
        [HttpGet]
        public ActionResult displayModal_TaoLichHoc(Guid idLopHoc)
        {
            #region Đơn hàng
            var trangThaiDonHangRepo = DONHANG_TRANGTHAIHOCs
             .Where(x => x.TenTrangThai.ToLower().Contains("chờ xếp lớp"))
             .Select(x => x.IdTrangThai).ToList();
            var locThongTin = new LocThongTin_ChoXepLop_Dto
            {
                IdTrangThaiHoc = trangThaiDonHangRepo
            };
            List<tbDonHangExtend> donHangs = getDonHangs(loai: "all", locThongTin: locThongTin).ToList();
            #endregion

            var model = new DisplayModal_TaoLichHoc_Output_Dto
            {
                IdLopHoc = idLopHoc,
                DonHangs = donHangs
            };
            return PartialView($"{VIEW_PATH}/test/quanlylophoc-lichhoc/lichhoc-taolichhoc.cshtml", model);
        }
        [HttpGet]
        public ActionResult layDonHangTheoYeuCau(string loaiDonHang, Guid idLopHoc)
        {
            if (loaiDonHang == "dangsudung")
            {
                var lopHoc = db.tbLopHocs.Find(idLopHoc);
                var idDonHangs = lopHoc.IdDonHang.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => Guid.Parse(x.Trim()))
                                                   .ToList();
                var donHangs = getDonHangs(loai: "single", idDonHangs: idDonHangs).ToList();

                var model = new DisplayModal_TaoLichHoc_Output_Dto
                {
                    IdLopHoc = idLopHoc,
                    DonHangs = donHangs
                };
                return PartialView($"{VIEW_PATH}/test/quanlylophoc-lichhoc/lichhoc-taolichhoc-.cshtml", donHangs);
            }
            else
            {
                var donHangs = getDonHangs(loai: "all").Where(x => x.DonHang.TrangThai == 1).ToList();

                var model = new DisplayModal_TaoLichHoc_Output_Dto
                {
                    IdLopHoc = idLopHoc,
                    DonHangs = donHangs
                };
                return PartialView($"{VIEW_PATH}/test/quanlylophoc-lichhoc/lichhoc-taolichhoc.cshtml", donHangs);
            }
        }
        [HttpPost]
        public ActionResult displayModal_DiemDanhBuoiHoc(Guid idBuoiHoc)
        {
            string loai = "diemdanh";
            var buoiHoc = db.tbLopHoc_BuoiHoc
                .Where(x => x.IdLopHoc_BuoiHoc == idBuoiHoc)
                .ToList()
                .Select(x => new tbLopHoc_BuoiHocExtend
                {
                    BuoiHoc = x
                })
                .FirstOrDefault();
            buoiHoc.HinhAnhBuoiHocs = getHinhAnhs(idBuoiHoc: buoiHoc.BuoiHoc.IdLopHoc_BuoiHoc);

            var lopHoc = db.tbLopHocs
                .Where(x => x.IdLopHoc == idBuoiHoc)
                .ToList()
                .Select(x => new tbLopHocExtend
                {
                    LopHoc = x,
                    BuoiHocs = new List<tbLopHoc_BuoiHocExtend> {
                       buoiHoc
                    },
                })?
               .FirstOrDefault();

            #region Giáo viên
            lopHoc.GiaoViens = getGiaoViens(str_idGiaoViens: lopHoc.LopHoc.IdGiaoVien);
            #endregion

            #region Khách hàng
            lopHoc.KhachHangs = getKhachHangs(str_idKhachHangs: lopHoc.LopHoc.IdKhachHang);
            #endregion

            #region Tài liệu

            #endregion

            ViewBag.lopHoc = lopHoc;
            ViewBag.buoiHoc = buoiHoc;
            ViewBag.loai = "diemdanh";
            return PartialView($"{VIEW_PATH}/quanlylophoc-buoihoc-crud.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_XemBuoiHoc(string loai, Guid idBuoiHoc)
        {
            var x = db.tbLopHoc_BuoiHoc.Find(idBuoiHoc);

            var buoiHoc = new tbLopHoc_BuoiHocExtend
            {
                BuoiHoc = x,
                GiaoViens = getGiaoViens(str_idGiaoViens: x.IdGiaoVien),
                KhachHangs = getKhachHangs(str_idKhachHangs: x.IdKhachHang),
                HinhAnhBuoiHocs = getHinhAnhs(idBuoiHoc: x.IdLopHoc_BuoiHoc)
            };

            var model = new DisplayModal_XemBuoiHoc_Output_Dto
            {
                Loai = loai,
                BuoiHoc = buoiHoc,
            };
            return PartialView($"{VIEW_PATH}/test/quanlylophoc-buoihoc/buoihoc-crud.cshtml", model);
        }
        [HttpPost]
        public ActionResult diemDanh()
        {
            string status = "success";
            string mess = "Điểm danh thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var buoiHoc_NEW = JsonConvert.DeserializeObject<tbLopHoc_BuoiHocExtend>(Request.Form["buoiHoc"]);
                    tbLopHoc_BuoiHoc buoiHoc_OLD = db.tbLopHoc_BuoiHoc.Find(buoiHoc_NEW.BuoiHoc.IdLopHoc_BuoiHoc);
                    if (buoiHoc_OLD == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        var ngayHomNay = DateTime.Now.Date;
                        var ngayHomQua = ngayHomNay.AddDays(-1);

                        // Kiểm tra hạn buôi học
                        if (buoiHoc_OLD.ThoiGianBatDau.Value.Date != ngayHomQua && buoiHoc_OLD.ThoiGianBatDau.Value.Date != ngayHomNay)
                        {
                            status = "warning";
                            mess = "Buổi học nằm ngoài thời hạn điểm danh";
                        }
                        else
                        {
                            if (buoiHoc_OLD.DiemDanh != 0)
                            {
                                status = "warning";
                                mess = "Buổi học đã được điểm danh trước đó";
                            }
                            else
                            {
                                var lopHoc_OLD = db.tbLopHocs.Find(buoiHoc_OLD.IdLopHoc);

                                #region Tạo thư mục ảnh cho lớp học
                                var duongDanHinhAnh = duongDanAnhBuoiHoc(idLopHoc: lopHoc_OLD.IdLopHoc, idBuoiHoc: Guid.Empty);
                                if (!System.IO.Directory.Exists(duongDanHinhAnh.DuongDanThuMuc_LOPHOC_SERVER))
                                    System.IO.Directory.CreateDirectory(duongDanHinhAnh.DuongDanThuMuc_LOPHOC_SERVER);
                                #endregion

                                /** Trạng thái lớp học
                                 * 0: Xóa, 
                                 * 1: Khởi tạo - Chưa bắt đầu, 
                                 * 2: Đang học, 
                                 * 3: Tạm dừng
                                 * 4: Kết thúc
                                 */

                                lopHoc_OLD.TrangThaiLopHoc = 2; // Đã điểm danh

                                buoiHoc_OLD.DiemDanh = buoiHoc_NEW.BuoiHoc.DiemDanh;

                                buoiHoc_OLD.GhiChu = buoiHoc_NEW.BuoiHoc.GhiChu;
                                buoiHoc_OLD.TrangThai = 1;
                                buoiHoc_OLD.IdNguoiTao = per.NguoiDung.IdNguoiDung;
                                buoiHoc_OLD.NgayTao = DateTime.Now;
                                buoiHoc_OLD.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;

                                // Kiểm tra buổi học cuối để cập nhật cho học viên
                                if (lopHoc_OLD.SoBuoi == buoiHoc_OLD.ThuTuBuoiHoc)
                                {
                                    List<Guid> idKhachHangs = lopHoc_OLD.IdKhachHang.Remove(0, 1).Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                                    List<Guid> idDonHangs = lopHoc_OLD.IdDonHang.Remove(0, 1).Split(',').Select(x => Guid.Parse(x.Trim())).ToList();

                                    #region Thay đổi trạng thái khách hàng & đơn hàng
                                    db.Database.ExecuteSqlCommand(
                                        sql_ThayDoiTrangThaiKhachHangVaDonHang(idKhachHangs: idKhachHangs,
                                        idDonHangs: idDonHangs, lopHoc: lopHoc_OLD, loaiKhachHang: "Đã học xong"));
                                    #endregion
                                }
                                ;
                                // Xóa hết ảnh cũ
                                db.Database.ExecuteSqlCommand($@"update tbLopHoc_BuoiHoc_HinhAnh set TrangThai = 0
                                where IdLopHoc_BuoiHoc = '{buoiHoc_OLD.IdLopHoc_BuoiHoc}'");

                                #region Tạo thư mục ảnh cho buổi học
                                duongDanHinhAnh = duongDanAnhBuoiHoc(idLopHoc: lopHoc_OLD.IdLopHoc, idBuoiHoc: buoiHoc_OLD.IdLopHoc_BuoiHoc);
                                if (System.IO.Directory.Exists(duongDanHinhAnh.DuongDanThuMuc_BUOIHOC_SERVER))
                                    System.IO.Directory.Delete(duongDanHinhAnh.DuongDanThuMuc_BUOIHOC_SERVER, true);
                                System.IO.Directory.CreateDirectory(duongDanHinhAnh.DuongDanThuMuc_BUOIHOC_SERVER);
                                #endregion

                                foreach (tbLopHoc_BuoiHoc_HinhAnh hinhAnhBuoiHoc_NEW in buoiHoc_NEW.HinhAnhBuoiHocs)
                                {
                                    tbLopHoc_BuoiHoc_HinhAnh hinhAnhBuoiHoc_OLD = db.tbLopHoc_BuoiHoc_HinhAnh.Find(hinhAnhBuoiHoc_NEW.IdHinhAnh);

                                    if (hinhAnhBuoiHoc_OLD != null)
                                    {
                                        hinhAnhBuoiHoc_OLD.TrangThai = 1;
                                        hinhAnhBuoiHoc_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                        hinhAnhBuoiHoc_OLD.NgaySua = DateTime.Now;
                                        hinhAnhBuoiHoc_OLD.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;
                                    }
                                    else
                                    {
                                        tbLopHoc_BuoiHoc_HinhAnh hinhAnhBuoiHoc = new tbLopHoc_BuoiHoc_HinhAnh
                                        {
                                            IdHinhAnh = Guid.NewGuid(),
                                            IdLopHoc_BuoiHoc = buoiHoc_OLD.IdLopHoc_BuoiHoc,
                                            IdLopHoc = buoiHoc_OLD.IdLopHoc,

                                            TenHinhAnh = hinhAnhBuoiHoc_NEW.TenHinhAnh,

                                            TrangThai = 1,
                                            IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                            NgayTao = DateTime.Now,
                                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                        };

                                        #region Tạo và lưu hình ảnh
                                        duongDanHinhAnh = duongDanAnhBuoiHoc(idLopHoc: lopHoc_OLD.IdLopHoc, idBuoiHoc: buoiHoc_OLD.IdLopHoc_BuoiHoc, tenTep: hinhAnhBuoiHoc_NEW.TenHinhAnh);
                                        luuHinhAnhVaoThuMuc(hinhAnhBuoiHoc: hinhAnhBuoiHoc_NEW, duongDanHinhAnh: duongDanHinhAnh);
                                        #endregion

                                        hinhAnhBuoiHoc.DuongDanHinhAnh = duongDanHinhAnh.DuongDan_HINHANH.DuongDanTep_BANDAU;
                                        db.tbLopHoc_BuoiHoc_HinhAnh.Add(hinhAnhBuoiHoc);
                                    }
                                    ;
                                }
                                ;

                                db.SaveChanges();
                                scope.Commit();
                            }
                            ;
                        }
                        ;
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
        public ActionResult taoBuoiHoc()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var idLopHoc = Guid.Parse(Request.Form["idLopHoc"]);
                    var buoiHocs_NEW = JsonConvert.DeserializeObject<List<tbLopHoc_BuoiHocExtend>>(Request.Form["buoiHocs"]);
                    tbLopHoc lopHoc = db.tbLopHocs.Find(idLopHoc);
                    if (lopHoc == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        /**
                         * B1. Lấy buổi học mới và loại bỏ buổi quá khứ
                         * B2. Lấy buổi học cũ
                         * B3. Gộp lại và sắp xếp thứ tự với buổi mới
                         * B4. Cập nhật
                         */
                        var ngayHomNay = DateTime.Now.Date;

                        // B1
                        buoiHocs_NEW = buoiHocs_NEW
                            .Where(x => x.BuoiHoc.ThoiGianBatDau.Value.Date >= ngayHomNay)
                            .ToList(); // Bỏ buổi quá khứ - Không thể thêm buổi vào quá khứ
                        var buoiHocs_OLD = db.tbLopHoc_BuoiHoc
                            .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                            && x.IdLopHoc == idLopHoc) // Tìm buổi đã có
                            .Select(x => new tbLopHoc_BuoiHocExtend
                            {
                                BuoiHoc = x
                            })
                            .ToList();

                        lopHoc.SoBuoi = buoiHocs_NEW.Count + buoiHocs_OLD.Count; // Cập nhật số buổi
                        // B2 + B3
                        var buoiHocs_MIX = buoiHocs_OLD
                            .Union(buoiHocs_NEW) // Kết hợp với buổi mới
                            .OrderBy(x => x.BuoiHoc.ThoiGianBatDau) // Sắp xếp buổi theo thời gian
                            .Select((x, index) =>
                            {
                                x.BuoiHoc.ThuTuBuoiHoc = index + 1; // Đánh lại thứ tự buổi
                                return x;
                            })
                            .Where(x => x.BuoiHoc.ThoiGianBatDau.Value.Date >= ngayHomNay) // Bỏ buổi quá khứ - Không thay đổi buổi đã có trong quá khứ
                            .ToList();
                        // B4
                        List<Guid> idKhachHangs = new List<Guid>();
                        List<Guid> idGiaoViens = new List<Guid>();
                        List<Guid> idDonHangs = new List<Guid>();

                        foreach (var buoiHoc_MIX in buoiHocs_MIX)
                        {
                            #region Thêm các thông tin của buổi học (gv, hv, dh) vào thông tin chung của lớp
                            List<Guid> idKhachHangs_BuoiHoc = buoiHoc_MIX.BuoiHoc.IdKhachHang.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => Guid.Parse(x.Trim()))
                                                   .ToList();
                            List<Guid> idGiaoViens_BuoiHoc = buoiHoc_MIX.BuoiHoc.IdGiaoVien.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => Guid.Parse(x.Trim()))
                                                   .ToList();
                            List<Guid> idDonHangs_BuoiHoc = buoiHoc_MIX.BuoiHoc.IdDonHang.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => Guid.Parse(x.Trim()))
                                                   .ToList();

                            idKhachHangs.AddRange(idKhachHangs_BuoiHoc);
                            idGiaoViens.AddRange(idGiaoViens_BuoiHoc);
                            idDonHangs.AddRange(idDonHangs_BuoiHoc);
                            #endregion

                            if (buoiHoc_MIX.BuoiHoc.IdLopHoc_BuoiHoc == Guid.Empty) // Chưa có thì tạo mới
                            {
                                tbLopHoc_BuoiHoc buoiHoc_NEW = new tbLopHoc_BuoiHoc
                                {
                                    IdLopHoc_BuoiHoc = Guid.NewGuid(),
                                    IdLopHoc = buoiHoc_MIX.BuoiHoc.IdLopHoc,

                                    IdKhachHang = buoiHoc_MIX.BuoiHoc.IdKhachHang,
                                    IdGiaoVien = buoiHoc_MIX.BuoiHoc.IdGiaoVien,
                                    IdDonHang = buoiHoc_MIX.BuoiHoc.IdDonHang,

                                    LuongTheoBuoi = buoiHoc_MIX.BuoiHoc.LuongTheoBuoi,
                                    ThuTuBuoiHoc = buoiHoc_MIX.BuoiHoc.ThuTuBuoiHoc,
                                    ThoiLuong = buoiHoc_MIX.BuoiHoc.ThoiLuong,
                                    ThoiGianBatDau = buoiHoc_MIX.BuoiHoc.ThoiGianBatDau,
                                    DiemDanh = buoiHoc_MIX.BuoiHoc.DiemDanh,

                                    GhiChu = buoiHoc_MIX.BuoiHoc.GhiChu,
                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbLopHoc_BuoiHoc.Add(buoiHoc_NEW);
                            }
                            else // Có rồi thì chỉ cập nhật thứ tự
                            {
                                var buoiHoc_OLD = db.tbLopHoc_BuoiHoc.Find(buoiHoc_MIX.BuoiHoc.IdLopHoc_BuoiHoc);
                                buoiHoc_OLD.ThuTuBuoiHoc = buoiHoc_MIX.BuoiHoc.ThuTuBuoiHoc;
                                buoiHoc_OLD.NgaySua = DateTime.Now;
                                buoiHoc_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                            }
                            ;
                        }
                        ;

                        #region Cập nhật lại thông tin của buổi học (gv, hv, dh) vào thông tin chung của lớp
                        idKhachHangs = idKhachHangs.Distinct().ToList();
                        idGiaoViens = idGiaoViens.Distinct().ToList();
                        idDonHangs = idDonHangs.Distinct().ToList();

                        lopHoc.IdKhachHang = "," + string.Join(",", idKhachHangs.Distinct());
                        lopHoc.IdGiaoVien = "," + string.Join(",", idGiaoViens.Distinct());
                        lopHoc.IdDonHang = "," + string.Join(",", idDonHangs.Distinct());
                        #endregion

                        #region Thay đổi trạng thái khách hàng & đơn hàng
                        db.Database.ExecuteSqlCommand(
                                          sql_ThayDoiTrangThaiKhachHangVaDonHang(idKhachHangs: idKhachHangs,
                                          idDonHangs: idDonHangs, lopHoc: lopHoc, loaiKhachHang: "Đang học"));
                        #endregion

                        db.SaveChanges();
                        scope.Commit();
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
        public ActionResult capNhatBuoiHoc()
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var idLopHoc = Guid.Parse(Request.Form["idLopHoc"]);
                    var buoiHocs_NEW = JsonConvert.DeserializeObject<List<tbLopHoc_BuoiHocExtend>>(Request.Form["buoiHocs"]);
                    tbLopHoc lopHoc = db.tbLopHocs.Find(idLopHoc);
                    if (lopHoc == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        List<Guid> idKhachHangs = new List<Guid>();
                        List<Guid> idGiaoViens = new List<Guid>();
                        List<Guid> idDonHangs = new List<Guid>();

                        /**
                         * B1. Cho phép sửa mọi buổi học trừ ảnh
                         */
                        var ngayHomNay = DateTime.Now.Date;

                        // B1
                        buoiHocs_NEW = buoiHocs_NEW
                            .Where(x => x.BuoiHoc.ThoiGianBatDau.Value.Date >= ngayHomNay)
                            .ToList(); // Bỏ buổi quá khứ - Không thể thêm buổi vào quá khứ
                        // B2
                        var buoiHocs_MIX = db.tbLopHoc_BuoiHoc
                            .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                            && x.IdLopHoc == idLopHoc) // Tìm buổi đã có
                            .Select(x => new tbLopHoc_BuoiHocExtend
                            {
                                BuoiHoc = x
                            })
                            .ToList() // Ép kiểu tránh lỗi
                            .Union(buoiHocs_NEW) // Kết hợp với buổi mới
                            .OrderBy(x => x.BuoiHoc.ThoiGianBatDau) // Sắp xếp buổi theo thời gian
                            .Select((x, index) =>
                            {
                                x.BuoiHoc.ThuTuBuoiHoc = index + 1; // Đánh lại thứ tự buổi
                                return x;
                            })
                            .Where(x => x.BuoiHoc.ThoiGianBatDau.Value.Date >= ngayHomNay) // Bỏ buổi quá khứ - Không thay đổi buổi đã có trong quá khứ
                            .ToList();
                        // B3
                        foreach (var buoiHoc_MIX in buoiHocs_MIX)
                        {
                            #region Thêm các thông tin của buổi học (gv, hv, dh) vào thông tin chung của lớp
                            List<Guid> idKhachHangs_BuoiHoc = buoiHoc_MIX.BuoiHoc.IdKhachHang.Split(',') // Tách chuỗi thành danh sách GUID
                                                    .Where(x => !string.IsNullOrWhiteSpace(x))
                                                    .Select(x => Guid.Parse(x.Trim()))
                                                    .ToList();
                            List<Guid> idGiaoViens_BuoiHoc = buoiHoc_MIX.BuoiHoc.IdGiaoVien.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => Guid.Parse(x.Trim()))
                                                   .ToList();
                            List<Guid> idDonHangs_BuoiHoc = buoiHoc_MIX.BuoiHoc.IdDonHang.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => Guid.Parse(x.Trim()))
                                                   .ToList();
                            #endregion

                            if (buoiHoc_MIX.BuoiHoc.IdLopHoc_BuoiHoc == Guid.Empty) // Chưa có thì tạo mới
                            {
                                tbLopHoc_BuoiHoc buoiHoc_NEW = new tbLopHoc_BuoiHoc
                                {
                                    IdLopHoc_BuoiHoc = Guid.NewGuid(),
                                    IdLopHoc = buoiHoc_MIX.BuoiHoc.IdLopHoc,

                                    IdKhachHang = buoiHoc_MIX.BuoiHoc.IdKhachHang,
                                    IdGiaoVien = buoiHoc_MIX.BuoiHoc.IdGiaoVien,
                                    IdDonHang = buoiHoc_MIX.BuoiHoc.IdDonHang,

                                    LuongTheoBuoi = buoiHoc_MIX.BuoiHoc.LuongTheoBuoi,
                                    ThuTuBuoiHoc = buoiHoc_MIX.BuoiHoc.ThuTuBuoiHoc,
                                    ThoiLuong = buoiHoc_MIX.BuoiHoc.ThoiLuong,
                                    ThoiGianBatDau = buoiHoc_MIX.BuoiHoc.ThoiGianBatDau,
                                    DiemDanh = buoiHoc_MIX.BuoiHoc.DiemDanh,

                                    GhiChu = buoiHoc_MIX.BuoiHoc.GhiChu,
                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbLopHoc_BuoiHoc.Add(buoiHoc_NEW);
                            }
                            else // Có rồi thì chỉ cập nhật thứ tự
                            {
                                var buoiHoc_OLD = db.tbLopHoc_BuoiHoc.Find(buoiHoc_MIX.BuoiHoc.IdLopHoc_BuoiHoc);
                                buoiHoc_OLD.ThuTuBuoiHoc = buoiHoc_MIX.BuoiHoc.ThuTuBuoiHoc;
                            }
                            ;
                        }
                        ;

                        #region Cập nhật lại thông tin của buổi học (gv, hv, dh) vào thông tin chung của lớp
                        idKhachHangs = idKhachHangs.Distinct().ToList();
                        idGiaoViens = idGiaoViens.Distinct().ToList();
                        idDonHangs = idDonHangs.Distinct().ToList();

                        lopHoc.IdKhachHang = "," + string.Join(",", idKhachHangs.Distinct());
                        lopHoc.IdGiaoVien = "," + string.Join(",", idGiaoViens.Distinct());
                        lopHoc.IdDonHang = "," + string.Join(",", idDonHangs.Distinct());
                        #endregion

                        #region Thay đổi trạng thái khách hàng & đơn hàng
                        db.Database.ExecuteSqlCommand(
                                          sql_ThayDoiTrangThaiKhachHangVaDonHang(idKhachHangs: idKhachHangs,
                                          idDonHangs: idDonHangs, lopHoc: lopHoc, loaiKhachHang: "Đang học"));
                        #endregion

                        db.SaveChanges();
                        scope.Commit();
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
        public ActionResult xoaBuoiHoc()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    var idLopHoc = Guid.Parse(Request.Form["idLopHoc"]);
                    var idBuoiHocs = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["idBuoiHocs"]);
                    tbLopHoc lopHoc = db.tbLopHocs.Find(idLopHoc);
                    if (lopHoc == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        /**
                         * B1; Láy buổi học cần xóa (không lấy buổi đã qua) - Xóa
                         * B2; Lấy buổi học còn lại, đánh lại thứ tự và loại bỏ buổi đã qua
                         * B3; Cập nhật thứ tự
                         * B4: Cập nhật thông tin giáo viên, đơn hàng, số buổi cho lớp học
                         */
                        var ngayHomNay = DateTime.Now.Date;

                        #region B1
                        var buoiHocs_CanXoa = db.tbLopHoc_BuoiHoc
                            .ToList()
                            .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                                    && idBuoiHocs.Contains(x.IdLopHoc_BuoiHoc)
                                    && x.ThoiGianBatDau.Value.Date >= ngayHomNay) // Không lấy buổi đã qua
                            .ToList();

                        foreach (var buoiHoc in buoiHocs_CanXoa)
                        {
                            buoiHoc.TrangThai = 0;
                            buoiHoc.NgaySua = DateTime.Now;
                            buoiHoc.IdNguoiSua = per.NguoiDung.IdNguoiDung;

                            #region Xóa thư mục ảnh cho buổi học
                            var duongDanHinhAnh = duongDanAnhBuoiHoc(idLopHoc: idLopHoc, idBuoiHoc: buoiHoc.IdLopHoc_BuoiHoc);
                            if (System.IO.Directory.Exists(duongDanHinhAnh.DuongDanThuMuc_BUOIHOC_SERVER))
                                System.IO.Directory.Delete(duongDanHinhAnh.DuongDanThuMuc_BUOIHOC_SERVER, true);
                            #endregion
                        }
                        ;
                        db.SaveChanges();
                        #endregion

                        #region B2
                        var buoiHocs_ConLai = db.tbLopHoc_BuoiHoc
                            .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung
                                    && x.IdLopHoc == idLopHoc
                                    && x.TrangThai != 0) // Lấy buổi còn lại
                            .OrderBy(x => x.ThoiGianBatDau) // Sắp xếp theo thời gian bắt đầu
                            .ToList();

                        lopHoc.SoBuoi = buoiHocs_ConLai.Count; // Cập nhật số buổi

                        var _buoiHocs_ConLai = buoiHocs_ConLai
                        .Select((x, index) =>
                         {
                             x.ThuTuBuoiHoc = index + 1;
                             return x;
                         })
                        .Where(x => x.ThoiGianBatDau.Value.Date >= ngayHomNay) // Không lấy buổi đã qua
                        .ToList();

                        List<Guid> idKhachHangs = new List<Guid>();
                        List<Guid> idGiaoViens = new List<Guid>();
                        List<Guid> idDonHangs = new List<Guid>();

                        foreach (var buoiHoc in _buoiHocs_ConLai)
                        {
                            #region Thêm các thông tin của buổi học (gv, hv, dh) vào thông tin chung của lớp
                            List<Guid> idKhachHangs_BuoiHoc = buoiHoc.IdKhachHang.Split(',') // Tách chuỗi thành danh sách GUID
                                                           .Where(x => !string.IsNullOrWhiteSpace(x))
                                                           .Select(x => Guid.Parse(x.Trim()))
                                                           .ToList();
                            List<Guid> idGiaoViens_BuoiHoc = buoiHoc.IdGiaoVien.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => Guid.Parse(x.Trim()))
                                                   .ToList();
                            List<Guid> idDonHangs_BuoiHoc = buoiHoc.IdDonHang.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => Guid.Parse(x.Trim()))
                                                   .ToList();


                            idKhachHangs.AddRange(idKhachHangs_BuoiHoc);
                            idGiaoViens.AddRange(idGiaoViens_BuoiHoc);
                            idDonHangs.AddRange(idDonHangs_BuoiHoc);
                            #endregion

                            var buoiHoc_OLD = db.tbLopHoc_BuoiHoc.Find(buoiHoc.IdLopHoc_BuoiHoc);
                            buoiHoc_OLD.ThuTuBuoiHoc = buoiHoc.ThuTuBuoiHoc;
                            buoiHoc.NgaySua = DateTime.Now;
                            buoiHoc.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        }
                        ;
                        #endregion

                        #region Cập nhật lại thông tin của buổi học (gv, hv, dh) vào thông tin chung của lớp
                        idKhachHangs = idKhachHangs.Distinct().ToList();
                        idGiaoViens = idGiaoViens.Distinct().ToList();
                        idDonHangs = idDonHangs.Distinct().ToList();

                        lopHoc.IdKhachHang = "," + string.Join(",", idKhachHangs.Distinct());
                        lopHoc.IdGiaoVien = "," + string.Join(",", idGiaoViens.Distinct());
                        lopHoc.IdDonHang = "," + string.Join(",", idDonHangs.Distinct());
                        #endregion

                        #region Thay đổi trạng thái khách hàng & đơn hàng
                        db.Database.ExecuteSqlCommand(
                                          sql_ThayDoiTrangThaiKhachHangVaDonHang(idKhachHangs: idKhachHangs,
                                          idDonHangs: idDonHangs, lopHoc: lopHoc, loaiKhachHang: "Đang học"));
                        #endregion

                        db.SaveChanges();
                        scope.Commit();
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
        #endregion

        #region Private Methods
        private List<tbNguoiDung> getGiaoViens(string str_idGiaoViens)
        {
            var giaoViens = new List<tbNguoiDung>();

            if (!string.IsNullOrEmpty(str_idGiaoViens))
            {
                var idGiaoViens = str_idGiaoViens.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => x.Trim());
                giaoViens = GIAOVIENs
                    .Where(x => idGiaoViens.Contains(x.IdNguoiDung.ToString())).ToList() ?? new List<tbNguoiDung>();
            }
            ;
            return giaoViens;
        }
        private List<tbKhachHang> getKhachHangs(string str_idKhachHangs)
        {
            var khachHangs = new List<tbKhachHang>();
            if (!string.IsNullOrEmpty(str_idKhachHangs))
            {
                var idKhachHangs = str_idKhachHangs.Split(',') // Tách chuỗi thành danh sách GUID
                                                   .Where(x => !string.IsNullOrWhiteSpace(x))
                                                   .Select(x => x.Trim());
                khachHangs = KHACHHANGs
                    .Where(x => idKhachHangs.Contains(x.IdKhachHang.ToString())).ToList() ?? new List<tbKhachHang>();
            }
            ;
            return khachHangs;
        }
        private string sql_ThayDoiTrangThaiKhachHangVaDonHang(List<Guid> idKhachHangs, List<Guid> idDonHangs, tbLopHoc lopHoc, string loaiKhachHang = "")
        {

            tbKhachHang_LoaiKhachHang loaiKhachHang_DangHoc = db.tbKhachHang_LoaiKhachHang
                .FirstOrDefault(x => x.TenLoaiKhachHang == loaiKhachHang) ?? new tbKhachHang_LoaiKhachHang();
            return $@"
                   update tbKhachHang set IdLoaiKhachHang = '{loaiKhachHang_DangHoc.IdLoaiKhachHang}' 
                   where IdKhachHang in ({string.Join(",", idKhachHangs.Select(x => string.Format("'{0}'", x)))})
                   and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'

                   -- Đơn hàng
                   update tbKhachHang_DonHang set TrangThaiLopHoc = {lopHoc.TrangThaiLopHoc} 
                   where IdDonHang in ({string.Join(",", idDonHangs.Select(x => string.Format("'{0}'", x)))})
                   and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                   ";
        }
        private DuongDanAnhBuoiHocDto duongDanAnhBuoiHoc(Guid idLopHoc, Guid idBuoiHoc, string tenTep = "NULL")
        {
            #region Lấy thư mục ảnh cho lớp học
            string duongDanThuMuc_LOPHOC = "";
            string duongDanThuMuc_LOPHOC_SERVER = "";
            if (idLopHoc != Guid.Empty)
            {
                duongDanThuMuc_LOPHOC = string.Format("/Assets/uploads/{0}/LOPHOC/{1}",
                    per.DonViSuDung.MaDonViSuDung, idLopHoc);
                duongDanThuMuc_LOPHOC_SERVER = Request.MapPath(duongDanThuMuc_LOPHOC);
            }
            ;
            #endregion

            #region Lấy thư mục ảnh cho buổi học
            string duongDanThuMuc_BUOIHOC = "";
            string duongDanThuMuc_BUOIHOC_SERVER = "";
            if (idBuoiHoc != Guid.Empty)
            {
                duongDanThuMuc_BUOIHOC = string.Format("{0}/{1}",
                   duongDanThuMuc_LOPHOC, idBuoiHoc);
                duongDanThuMuc_BUOIHOC_SERVER = Request.MapPath(duongDanThuMuc_BUOIHOC);
            }
            ;
            #endregion

            #region Lấy đường dẫn ảnh
            var layDuongDanTep = LayDuongDanTep(duongDanThuMucGoc: duongDanThuMuc_BUOIHOC, tenTep_BANDAU: tenTep);
            #endregion
            return new DuongDanAnhBuoiHocDto
            {
                DuongDanThuMuc_LOPHOC = duongDanThuMuc_LOPHOC,
                DuongDanThuMuc_LOPHOC_SERVER = duongDanThuMuc_LOPHOC_SERVER,
                DuongDanThuMuc_BUOIHOC = duongDanThuMuc_BUOIHOC,
                DuongDanThuMuc_BUOIHOC_SERVER = duongDanThuMuc_BUOIHOC_SERVER,
                DuongDan_HINHANH = layDuongDanTep,

            };
        }
        private void luuHinhAnhVaoThuMuc(tbLopHoc_BuoiHoc_HinhAnh hinhAnhBuoiHoc, DuongDanAnhBuoiHocDto duongDanHinhAnh)
        {
            // Loại bỏ tiền tố nếu tồn tại
            if (hinhAnhBuoiHoc.FileBase64.Contains(","))
                hinhAnhBuoiHoc.FileBase64 = hinhAnhBuoiHoc.FileBase64.Split(',')[1];
            var fileBytes = Convert.FromBase64String(hinhAnhBuoiHoc.FileBase64);
            // (Nếu có rồi thì xóa)
            if (System.IO.Directory.Exists(duongDanHinhAnh.DuongDan_HINHANH.DuongDanThuMuc_BANDAU_SERVER))
                System.IO.Directory.Delete(duongDanHinhAnh.DuongDan_HINHANH.DuongDanThuMuc_BANDAU_SERVER, true);
            System.IO.Directory.CreateDirectory(duongDanHinhAnh.DuongDan_HINHANH.DuongDanThuMuc_BANDAU_SERVER);

            if (System.IO.File.Exists(duongDanHinhAnh.DuongDan_HINHANH.DuongDanTep_BANDAU_SERVER))
                System.IO.File.Delete(duongDanHinhAnh.DuongDan_HINHANH.DuongDanTep_BANDAU_SERVER);
            System.IO.File.WriteAllBytes(duongDanHinhAnh.DuongDan_HINHANH.DuongDanTep_BANDAU_SERVER, fileBytes);
        }
        #endregion
    }
}