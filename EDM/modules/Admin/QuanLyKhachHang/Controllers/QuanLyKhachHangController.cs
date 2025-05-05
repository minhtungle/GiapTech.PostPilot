using Applications.QuanLyKhachHang.Dtos;
using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Organization.Controllers;
using Public.Controllers;
using Public.Models;
using QuanLyKhachHang.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserAccount.Models;

namespace QuanLyKhachHang.Controllers
{
    public class QuanLyKhachHangController : RouteConfigController
    {
        // GET: QuanLyKhachHang
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/QuanLyKhachHang";
        IsoDateTimeConverter DATETIMECONVERTER = new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" };
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
        private List<tbNguoiDungExtend> NGUOIDUNGs
        {
            get
            {
                return Session["NGUOIDUNGs"] as List<tbNguoiDungExtend> ?? new List<tbNguoiDungExtend>();
            }
            set
            {
                Session["NGUOIDUNGs"] = value;
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
        private List<tbKhachHangExtend> EXCEL_HOSOs_UPLOAD
        {
            get
            {
                return Session["EXCEL_HOSOs_UPLOAD"] as List<tbKhachHangExtend> ?? new List<tbKhachHangExtend>();
            }
            set
            {
                Session["EXCEL_HOSOs_UPLOAD"] = value;
            }
        }
        private List<tbKhachHangExtend> EXCEL_HOSOs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_HOSOs_DOWNLOAD"] as List<tbKhachHangExtend> ?? new List<tbKhachHangExtend>();
            }
            set
            {
                Session["EXCEL_HOSOs_DOWNLOAD"] = value;
            }
        }
        private OrganizationController organizationController = new OrganizationController();
        public QuanLyKhachHangController()
        {

        }
        #endregion
        public ActionResult Index()
        {
            #region Gửi mail
            // Gửi mail
            //Public.Handle.SendEmail(sendTo: "minhtungle.giap@gmail.com", subject: "TEST MAIL", body: "TEST MAIL", isHTML: false, donViSuDung: per.DonViSuDung);
            //if (nguoiDung_NEW.Email != nguoiDung_OLD.Email)
            //{
            //    Public.Handle.SendEmail(sendTo: nguoiDung_NEW.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
            //};
            #endregion

            #region Lấy các danh sách

            #region Người dùng
            List<tbNguoiDungExtend> nguoiDungs = db.Database.SqlQuery<tbNguoiDungExtend>($@"
            select * from tbNguoiDung where TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
            ").ToList() ?? new List<tbNguoiDungExtend>();
            foreach (tbNguoiDungExtend nguoiDung in nguoiDungs)
            {
                nguoiDung.CoCauToChuc = db.tbCoCauToChucs.Find(nguoiDung.IdCoCauToChuc) ?? new tbCoCauToChuc();
            };
            #endregion

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyKhachHang").ThaoTacs ?? new List<ThaoTac>();
            #endregion

            #region Sản phẩm
            List<tbSanPham> sanPhams = db.Database.SqlQuery<tbSanPham>($@"
            select * from tbSanPham
            where TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
            order by IdLoaiSanPham, Stt").ToList() ?? new List<tbSanPham>();
            #endregion

            #region Trình độ
            List<tbSanPham_LoaiSanPham_TrinhDo> trinhDos = db.Database.SqlQuery<tbSanPham_LoaiSanPham_TrinhDo>($@"
            select * from tbSanPham_LoaiSanPham_TrinhDo
            where TrangThai != 0
            order by IdLoaiSanPham, CapDo").ToList() ?? new List<tbSanPham_LoaiSanPham_TrinhDo>();
            #endregion

            #region Phương thức thanh toán
            List<tbPhuongThucThanhToan> phuongThucThanhToans = db.Database.SqlQuery<tbPhuongThucThanhToan>($@"
            select * from tbPhuongThucThanhToan
            where TrangThai != 0
            order by Stt").ToList() ?? new List<tbPhuongThucThanhToan>();
            #endregion

            #region Loại khách hàng
            List<tbKhachHang_LoaiKhachHang> loaiKhachHangs = db.Database.SqlQuery<tbKhachHang_LoaiKhachHang>($@"
            select * from tbKhachHang_LoaiKhachHang
            where TrangThai != 0 and Stt not in (4,5)
            order by Stt").ToList() ?? new List<tbKhachHang_LoaiKhachHang>();
            #endregion

            #region Gói chăm sóc
            List<tbGoiChamSoc> goiChamSocs = db.Database.SqlQuery<tbGoiChamSoc>($@"
            select * from tbGoiChamSoc
            where TrangThai != 0
            order by Stt").ToList() ?? new List<tbGoiChamSoc>();
            #endregion

            #region Đơn vị tiền
            List<tbDonViTien> donViTiens = db.tbDonViTiens
                .Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung)
                .OrderBy(x => x.Stt)
                .ToList();
            #endregion

            #endregion

            THAOTACs = thaoTacs;
            NGUOIDUNGs = nguoiDungs;
            SANPHAMs = sanPhams;
            TRINHDOs = trinhDos;
            LOAIKHACHHANGs = loaiKhachHangs;
            GOICHAMSOCs = goiChamSocs;
            DONVITIENs = donViTiens;
            PHUONGTHUCTHANHTOANs = phuongThucThanhToans;

            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;
            ViewBag.nguoiDungs = NGUOIDUNGs;
            return View($"{VIEW_PATH}/quanlykhachhang.cshtml");
        }

        #region Lấy cơ cấu tổ chức
        public JsonResult get_CoCauToChucs()
        {
            List<Tree<tbCoCauToChuc>> coCauToChucs_Tree = organizationController.get_CoCauToChucs_Tree(idCoCau: Guid.Empty, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            return Json(coCauToChucs_Tree, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public ActionResult getList(LocThongTinDto locThongTin)
        {
            List<tbKhachHangExtend> khachHangs = get_KhachHangs(loai: "all", locThongTin: locThongTin);
            return Json(new
            {
                data = khachHangs
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

        #region Lấy các danh sách dữ liệu
        public List<tbKhachHangExtend> get_KhachHangs(
            string loai = "all", List<Guid> idKhachHangs = null,
            LocThongTinDto locThongTin = null)
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
        public List<tbKhachHang_LichSuExtend> get_KhachHang_LichSus(tbKhachHangExtend khachHang)
        {
            List<tbKhachHang_LichSuExtend> khachHang_LichSus = db.Database.SqlQuery<tbKhachHang_LichSuExtend>($@"
                    select * 
                    from tbKhachHang_LichSu
                    where IdKhachHang = '{khachHang.IdKhachHang}' and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                    order by NgayTao desc
                    ").ToList() ?? new List<tbKhachHang_LichSuExtend>();
            foreach (tbKhachHang_LichSuExtend khachHang_LichSu in khachHang.LichSus)
            {
                khachHang_LichSu.ThongTinNguoiTao = db.tbNguoiDungs.FirstOrDefault(x =>
                x.IdNguoiDung == khachHang_LichSu.IdNguoiTao && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung) ?? new tbNguoiDung();
            };
            return khachHang_LichSus;
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
            };
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
        #endregion

        #region CRUD
        [HttpPost]
        public ActionResult themDonHang(List<tbKhachHang_DonHangExtend> khachHang_DonHangs,
            string loaiThemMoi = "donhang", // Thêm đơn hàng || thanh toán
            string loai = "" // Trạng thái create || update || read
            )
        {
            ViewBag.khachHang_DonHangs = khachHang_DonHangs;
            ViewBag.loaiThemMoi = loaiThemMoi;
            ViewBag.loai = loai;
            return PartialView($"{VIEW_PATH}/quanlykhachhang-donhang/donhang-themdonhang.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_CRUD(string loai, Guid idKhachHang)
        {
            #region Thông tin chung
            tbKhachHangExtend khachHang = new tbKhachHangExtend();

            if (loai != "create" && idKhachHang != Guid.Empty)
            {
                khachHang = get_KhachHangs(loai: "single", idKhachHangs: new List<Guid> { idKhachHang }).FirstOrDefault();
            };
            #endregion

            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "QuanLyKhachHang").ThaoTacs;
            #endregion

            #region Lịch sử thay đổi
            List<tbKhachHang_LichSuExtend> khachHang_LichSus = get_KhachHang_LichSus(khachHang: khachHang);
            khachHang.LichSus = khachHang_LichSus;
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
            };
            #endregion

            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;
            ViewBag.khachHang = khachHang;
            ViewBag.loai = loai;
            return PartialView($"{VIEW_PATH}/quanlykhachhang-crud.cshtml");
        }
        [HttpPost]
        public tbKhachHangExtend kiemTra_KhachHang(tbKhachHangExtend khachHang)
        {
            List<string> ketQuas = new List<string>();
            // Kiểm tra còn khách hàng khác có trùng mã không
            tbKhachHang khachHang_OLD = db.tbKhachHangs.FirstOrDefault(x =>
            x.TenKhachHang == khachHang.TenKhachHang && x.Email == khachHang.Email &&
            x.IdKhachHang != khachHang.IdKhachHang &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (khachHang_OLD != null)
            {
                ketQuas.Add("Khách hàng đã tồn tại");
                khachHang.KiemTraExcel.TrangThai = 0;
                khachHang.IdKhachHang = khachHang_OLD.IdKhachHang;
            };
            khachHang.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            return khachHang;
        }
        tbCapDo_DoanhThu chonCapDo_DoanhThu_TiepTheo(long doanhThuCanSoSanh, tbCapDo_DoanhThu capDo_DoanhThu_HienTai)
        {
            // Tìm cấp tiếp theo
            tbCapDo_DoanhThu capDo_DoanhThu_TiepTheo = db.tbCapDo_DoanhThu.FirstOrDefault(x =>
            x.CapDo == (capDo_DoanhThu_HienTai.CapDo + 1)
            && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (capDo_DoanhThu_TiepTheo == null) return capDo_DoanhThu_HienTai;
            if (doanhThuCanSoSanh < capDo_DoanhThu_TiepTheo.DoanhThuYeuCau) return capDo_DoanhThu_HienTai;
            return chonCapDo_DoanhThu_TiepTheo(doanhThuCanSoSanh: doanhThuCanSoSanh, capDo_DoanhThu_HienTai: capDo_DoanhThu_TiepTheo);
        }
        [HttpPost]
        public ActionResult create_KhachHang(string str_KhachHang)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            string loaiThemMoi = "daydu";
            tbKhachHangExtend khachHang_OLD = new tbKhachHangExtend();
            tbNguoiDung_DoanhThu nguoiDung_DoanhThu = new tbNguoiDung_DoanhThu();
            tbCoCauToChuc_DoanhThu coCauToChuc_DoanhThu = new tbCoCauToChuc_DoanhThu();
            List<tbNguoiDung_DoanhThu> nguoiDung_DoanhThus_NHOM = new List<tbNguoiDung_DoanhThu>();

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbKhachHangExtend khachHang_NEW = JsonConvert.DeserializeObject<tbKhachHangExtend>(str_KhachHang ?? "", DATETIMECONVERTER);
                    if (khachHang_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        khachHang_OLD = kiemTra_KhachHang(khachHang: khachHang_NEW);
                        if (khachHang_OLD.KiemTraExcel.TrangThai == 0)
                        {
                            status = "warning";
                            mess = khachHang_NEW.KiemTraExcel.KetQua;
                        }
                        else
                        {
                            #region Doanh thu
                            string ngayTao = DateTime.Now.ToString("MM/yyyy");
                            nguoiDung_DoanhThu = db.tbNguoiDung_DoanhThu.FirstOrDefault(x =>
                            x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
                            x.NgayLenMucTieu == ngayTao &&
                            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);

                            coCauToChuc_DoanhThu = db.tbCoCauToChuc_DoanhThu.FirstOrDefault(x =>
                            //x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
                            x.IdCoCauToChuc == Guid.Empty &&
                            x.NgayLenMucTieu == ngayTao &&
                            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                            #endregion

                            #region Khách hàng
                            // Gán quyền truy cập cho quản lý cơ cấu
                            string QuyenTruyCap = string.Format("{0},{1}", per.CoCauToChuc.IdQuanLy == null ? "" : per.CoCauToChuc.IdQuanLy, per.NguoiDung.IdNguoiDung);
                            // Thêm mới
                            tbKhachHang khachHang = new tbKhachHang
                            {
                                IdKhachHang = Guid.NewGuid(),
                                TenKhachHang = khachHang_NEW.TenKhachHang,
                                Email = khachHang_NEW.Email,
                                LienKet = khachHang_NEW.LienKet,
                                LienKetSale = khachHang_NEW.LienKetSale,
                                SoDienThoai = khachHang_NEW.SoDienThoai,
                                DoTuoi = khachHang_NEW.DoTuoi,
                                NgheNghiep = khachHang_NEW.NgheNghiep,
                                NguonKhachHang = khachHang_NEW.NguonKhachHang,
                                DiaChi = khachHang_NEW.DiaChi,

                                IdLoaiKhachHang = khachHang_NEW.IdLoaiKhachHang,
                                IdGoiChamSoc = khachHang_NEW.IdGoiChamSoc,
                                IdPhuongThucThanhToan = khachHang_NEW.IdPhuongThucThanhToan,
                                IdQuocGiaSinhSong = khachHang_NEW.IdQuocGiaSinhSong,

                                QuyenTruyCap = QuyenTruyCap,

                                GhiChu = khachHang_NEW.GhiChu,
                                TrangThai = khachHang_NEW.TrangThai, // Dùng cái này để phân biệt loại khách hàng
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbKhachHangs.Add(khachHang);
                            #endregion

                            #region Đơn hàng & Thanh toán & Doanh thu
                            List<tbKhachHang_DonHangExtend> donHangs_NEW = khachHang_NEW.DonHangs;
                            if (donHangs_NEW.Count == 0) loaiThemMoi = "thongtinchung";
                            foreach (tbKhachHang_DonHangExtend donHang_NEW in donHangs_NEW)
                            {
                                tbKhachHang_DonHang donHang = new tbKhachHang_DonHang
                                {
                                    IdDonHang = Guid.NewGuid(),
                                    IdKhachHang = khachHang.IdKhachHang,
                                    IdSanPham = donHang_NEW.IdSanPham,
                                    IdTrinhDoDauVao = donHang_NEW.IdTrinhDoDauVao,
                                    IdTrinhDoDauRa = donHang_NEW.IdTrinhDoDauRa,
                                    ThuTuDonHang = donHang_NEW.ThuTuDonHang,
                                    TongSoTien = donHang_NEW.TongSoTien,
                                    //TrangThaiLopHoc = null,

                                    GhiChu = donHang_NEW.GhiChu,
                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbKhachHang_DonHang.Add(donHang);

                                foreach (tbKhachHang_DonHang_ThanhToan thanhToan_NEW in donHang_NEW.ThanhToans)
                                {
                                    tbKhachHang_DonHang_ThanhToan thanhToan = new tbKhachHang_DonHang_ThanhToan
                                    {
                                        IdThanhToan = Guid.NewGuid(),
                                        IdDonHang = donHang.IdDonHang,
                                        IdSanPham = donHang.IdSanPham,
                                        IdKhachHang = khachHang.IdKhachHang,
                                        ThuTuThanhToan = thanhToan_NEW.ThuTuThanhToan,

                                        SoTienDaDong_ChuaQuyDoi = thanhToan_NEW.SoTienDaDong_ChuaQuyDoi,
                                        IdDonViTien = thanhToan_NEW.IdDonViTien,
                                        SoTienDaDong = thanhToan_NEW.SoTienDaDong,
                                        PhanTramDaDong = thanhToan_NEW.PhanTramDaDong,
                                        //GhiChu = donHang_NEW.GhiChu,
                                        TrangThai = 1,
                                        IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                        NgayTao = DateTime.Now,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                    };
                                    db.tbKhachHang_DonHang_ThanhToan.Add(thanhToan);

                                    #region Doanh thu
                                    if (nguoiDung_DoanhThu != null)
                                    {
                                        // Kiểm tra đã hoàn thiện mục tiêu chưa
                                        nguoiDung_DoanhThu.DoanhThuThucTe += thanhToan_NEW.SoTienDaDong;
                                        decimal? phanTramHoanThien = ((decimal)nguoiDung_DoanhThu.DoanhThuThucTe / (decimal)nguoiDung_DoanhThu.DoanhThuMucTieu) * 100;
                                        if (nguoiDung_DoanhThu.PhanTramHoanThien < 100 && phanTramHoanThien >= 100)
                                        {
                                            nguoiDung_DoanhThu.NgayDatMucTieu = DateTime.Now;
                                        };
                                        nguoiDung_DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                                        nguoiDung_DoanhThu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                        nguoiDung_DoanhThu.NgaySua = DateTime.Now;

                                        db.SaveChanges();
                                    };
                                    if (coCauToChuc_DoanhThu != null)
                                    {
                                        // Kiểm tra đã hoàn thiện mục tiêu chưa
                                        coCauToChuc_DoanhThu.DoanhThuThucTe += thanhToan_NEW.SoTienDaDong;
                                        decimal? phanTramHoanThien = ((decimal)coCauToChuc_DoanhThu.DoanhThuThucTe / (decimal)coCauToChuc_DoanhThu.DoanhThuMucTieu) * 100;
                                        if (coCauToChuc_DoanhThu.PhanTramHoanThien < 100 && phanTramHoanThien >= 100)
                                        {
                                            coCauToChuc_DoanhThu.NgayDatMucTieu = DateTime.Now;
                                        };
                                        coCauToChuc_DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                                        coCauToChuc_DoanhThu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                        coCauToChuc_DoanhThu.NgaySua = DateTime.Now;

                                        db.SaveChanges();
                                    };
                                    #endregion
                                };
                            };
                            #endregion

                            #region Kiểm tra tiến độ đội nhóm
                            if (nguoiDung_DoanhThu != null)
                            {
                                nguoiDung_DoanhThus_NHOM = db.tbNguoiDung_DoanhThu.Where(x =>
                                x.IdNguoiTao != per.NguoiDung.IdNguoiDung &&
                                x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                                x.DoanhThuThucTe >= nguoiDung_DoanhThu.DoanhThuThucTe && // Doanh thu cao hơn mình
                                x.NgaySua < nguoiDung_DoanhThu.NgaySua && // Nhập doanh thu sớm hơn mình
                                x.NgayLenMucTieu == nguoiDung_DoanhThu.NgayLenMucTieu).OrderBy(x => x.DoanhThuThucTe).ToList();
                            };
                            #endregion

                            #region Kiểm tra cấp độ doanh thu
                            long? TongDoanhThu = db.tbNguoiDung_DoanhThu.Where(x =>
                            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                            x.IdNguoiTao == per.NguoiDung.IdNguoiDung).Sum(x => x.DoanhThuThucTe) ?? 0;
                            tbNguoiDung nguoiDung = db.tbNguoiDungs.FirstOrDefault(x => x.IdNguoiDung == per.NguoiDung.IdNguoiDung);
                            nguoiDung.IdCapDo_DoanhThu = chonCapDo_DoanhThu_TiepTheo(doanhThuCanSoSanh: TongDoanhThu.Value, capDo_DoanhThu_HienTai: per.CapDo_DoanhThu).IdCapDo_DoanhThu;
                            #endregion

                            db.tbKhachHang_LichSu.Add(new tbKhachHang_LichSu
                            {
                                IdLichSu = Guid.NewGuid(),
                                IdKhachHang = khachHang.IdKhachHang,
                                NoiDung = "Khởi tạo khách hàng",
                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            });
                            db.SaveChanges();
                            scope.Commit();
                        }
                    };
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
                mess,
                khachHang_OLD,
                loaiThemMoi,
                nguoiDung_DoanhThu,
                nguoiDung_DoanhThus_NHOM
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult _update_KhachHang_(string str_khachhang)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";
            tbKhachHang khachHang_OLD = new tbKhachHang();
            tbNguoiDung_DoanhThu nguoiDung_DoanhThu = new tbNguoiDung_DoanhThu();
            tbCoCauToChuc_DoanhThu coCauToChuc_DoanhThu = new tbCoCauToChuc_DoanhThu();
            List<tbNguoiDung_DoanhThu> nguoiDung_DoanhThus_NHOM = new List<tbNguoiDung_DoanhThu>();

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string format = "dd/MM/yyyy";
                    IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    tbKhachHangExtend khachHang_NEW = JsonConvert.DeserializeObject<tbKhachHangExtend>(str_khachhang ?? "", dateTimeConverter);
                    // Kiểm tra Tên khách hàng có trùng bản ghi nào khác không
                    if (kiemTra_KhachHang(khachHang: khachHang_NEW).KiemTraExcel.TrangThai == 0)
                    {
                        status = "warning";
                        mess = khachHang_NEW.KiemTraExcel.KetQua;
                    }
                    else
                    {
                        #region Doanh thu
                        string ngayTao = DateTime.Now.ToString("MM/yyyy");
                        nguoiDung_DoanhThu = db.tbNguoiDung_DoanhThu.FirstOrDefault(x =>
                        x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
                        x.NgayLenMucTieu == ngayTao &&
                        x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);

                        coCauToChuc_DoanhThu = db.tbCoCauToChuc_DoanhThu.FirstOrDefault(x =>
                        //x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
                        x.IdCoCauToChuc == Guid.Empty &&
                        x.NgayLenMucTieu == ngayTao &&
                        x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                        #endregion

                        #region Khách hàng
                        khachHang_OLD = db.tbKhachHangs.Find(khachHang_NEW.IdKhachHang);

                        #region Lịch sử thay đổi thông tin
                        List<Tuple<string, object, object>> thayDois = Public.Handle.CompareSpecificFields(obj1: khachHang_OLD, obj2: khachHang_NEW,
                            fieldsToCompare: new List<string>(),
                            fieldsToExclude: new List<string> {
                                "QuyenTruyCap", "DuongDanFile", "TrangThai", "NguoiTao", "NguoiSua", "NgayTao", "NgaySua", "MaDonViSuDung"}
                            );
                        List<DuLieu_LichSu_ChiTiet> chiTiets = Public.Handle.ChiTietThayDoi(thayDois: thayDois);
                        db.tbKhachHang_LichSu.Add(new tbKhachHang_LichSu
                        {
                            IdLichSu = Guid.NewGuid(),
                            IdKhachHang = khachHang_OLD.IdKhachHang,
                            NoiDung = "Cập nhật thông tin khách hàng",
                            ChiTiet = JsonConvert.SerializeObject(chiTiets),
                            TrangThai = 1,
                            IdNguoiTao = per.NguoiDung.IdNguoiDung,
                            NgayTao = DateTime.Now,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        #endregion

                        khachHang_OLD.TenKhachHang = khachHang_NEW.TenKhachHang;
                        khachHang_OLD.Email = khachHang_NEW.Email;
                        khachHang_OLD.LienKet = khachHang_NEW.LienKet;
                        khachHang_OLD.LienKetSale = khachHang_NEW.LienKetSale;
                        khachHang_OLD.SoDienThoai = khachHang_NEW.SoDienThoai;
                        khachHang_OLD.DoTuoi = khachHang_NEW.DoTuoi;
                        khachHang_OLD.NgheNghiep = khachHang_NEW.NgheNghiep;
                        khachHang_OLD.NguonKhachHang = khachHang_NEW.NguonKhachHang;
                        khachHang_OLD.DiaChi = khachHang_NEW.DiaChi;

                        khachHang_OLD.IdLoaiKhachHang = khachHang_NEW.IdLoaiKhachHang;
                        khachHang_OLD.IdGoiChamSoc = khachHang_NEW.IdGoiChamSoc;
                        khachHang_OLD.IdPhuongThucThanhToan = khachHang_NEW.IdPhuongThucThanhToan;
                        khachHang_OLD.IdQuocGiaSinhSong = khachHang_NEW.IdQuocGiaSinhSong;

                        //khachHang_OLD.QuyenTruyCap = khachHang_NEW.QuyenTruyCap;

                        khachHang_OLD.GhiChu = khachHang_NEW.GhiChu;
                        khachHang_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        khachHang_OLD.NgaySua = DateTime.Now;
                        #endregion

                        #region Đơn hàng & Thanh toán
                        // Lấy danh sách id bản ghi không xóa
                        List<Guid> idDonHangs_NEW = new List<Guid>();
                        List<Guid> idThanhToans_NEW = new List<Guid>();

                        List<tbKhachHang_DonHangExtend> donHangs_NEW = khachHang_NEW.DonHangs;
                        foreach (tbKhachHang_DonHangExtend donHang_NEW in donHangs_NEW)
                        {
                            idDonHangs_NEW.Add(donHang_NEW.IdDonHang);
                            // Có rồi thì cập nhật, chưa có thì tạo mới
                            tbKhachHang_DonHang donHang_OLD = db.tbKhachHang_DonHang.FirstOrDefault(x => x.IdDonHang == donHang_NEW.IdDonHang);
                            if (donHang_OLD != null)
                            {
                                //donHang_OLD.IdKhachHang = donHang_NEW.IdKhachHang;
                                donHang_OLD.IdSanPham = donHang_NEW.IdSanPham;
                                donHang_OLD.IdTrinhDoDauVao = donHang_NEW.IdTrinhDoDauVao;
                                donHang_OLD.IdTrinhDoDauRa = donHang_NEW.IdTrinhDoDauRa;
                                donHang_OLD.ThuTuDonHang = donHang_NEW.ThuTuDonHang;
                                donHang_OLD.TongSoTien = donHang_NEW.TongSoTien;

                                donHang_OLD.GhiChu = donHang_NEW.GhiChu;
                                //donHang_OLD.TrangThai = 1;
                                donHang_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                donHang_OLD.NgaySua = DateTime.Now;
                                donHang_OLD.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;

                                List<tbKhachHang_DonHang_ThanhToan> thanhToans_NEW = donHang_NEW.ThanhToans;
                                themMoiThanhToans(donHang: donHang_OLD, thanhToans_NEW: thanhToans_NEW);
                            }
                            else
                            {
                                tbKhachHang_DonHang donHang = new tbKhachHang_DonHang
                                {
                                    IdDonHang = Guid.NewGuid(),
                                    IdKhachHang = khachHang_OLD.IdKhachHang,
                                    IdSanPham = donHang_NEW.IdSanPham,
                                    IdTrinhDoDauVao = donHang_NEW.IdTrinhDoDauVao,
                                    IdTrinhDoDauRa = donHang_NEW.IdTrinhDoDauRa,
                                    ThuTuDonHang = donHang_NEW.ThuTuDonHang,
                                    TongSoTien = donHang_NEW.TongSoTien,
                                    //TrangThaiLopHoc = 1,

                                    GhiChu = donHang_NEW.GhiChu,
                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbKhachHang_DonHang.Add(donHang);

                                List<tbKhachHang_DonHang_ThanhToan> thanhToans_NEW = donHang_NEW.ThanhToans;
                                themMoiThanhToans(donHang: donHang, thanhToans_NEW: thanhToans_NEW, kiemTraTonTai: false);
                            };

                            void themMoiThanhToans(tbKhachHang_DonHang donHang, List<tbKhachHang_DonHang_ThanhToan> thanhToans_NEW, bool kiemTraTonTai = true)
                            {
                                void themMoiThanhToan(tbKhachHang_DonHang_ThanhToan thanhToan_NEW)
                                {
                                    tbKhachHang_DonHang_ThanhToan thanhToan = new tbKhachHang_DonHang_ThanhToan
                                    {
                                        IdThanhToan = Guid.NewGuid(),
                                        IdDonHang = thanhToan_NEW.IdDonHang,
                                        IdSanPham = thanhToan_NEW.IdSanPham,
                                        IdKhachHang = thanhToan_NEW.IdKhachHang,
                                        ThuTuThanhToan = thanhToan_NEW.ThuTuThanhToan,

                                        SoTienDaDong_ChuaQuyDoi = thanhToan_NEW.SoTienDaDong_ChuaQuyDoi,
                                        IdDonViTien = thanhToan_NEW.IdDonViTien,
                                        SoTienDaDong = thanhToan_NEW.SoTienDaDong,
                                        PhanTramDaDong = thanhToan_NEW.PhanTramDaDong,

                                        TrangThai = 1,
                                        IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                        NgayTao = DateTime.Now,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                    };
                                    db.tbKhachHang_DonHang_ThanhToan.Add(thanhToan);
                                };

                                foreach (tbKhachHang_DonHang_ThanhToan thanhToan_NEW in thanhToans_NEW)
                                {
                                    /**
                                    * Tính chênh lệch sau khi cập nhật số tiền đã đóng (âm/dương)
                                    * Thêm doanh thu như bình thường
                                    */
                                    int soTienDaDong_OLD = 0; int soTienDaDong_NEW = thanhToan_NEW.SoTienDaDong.Value;
                                    int chenhLech = 0;

                                    idThanhToans_NEW.Add(thanhToan_NEW.IdThanhToan);
                                    thanhToan_NEW.IdDonHang = donHang.IdDonHang;
                                    thanhToan_NEW.IdSanPham = donHang.IdSanPham;

                                    if (!kiemTraTonTai) themMoiThanhToan(thanhToan_NEW: thanhToan_NEW); // Không kiểm tra thì thêm luôn
                                    else
                                    {
                                        tbKhachHang_DonHang_ThanhToan thanhToan_OLD = db.tbKhachHang_DonHang_ThanhToan.FirstOrDefault(x => x.IdThanhToan == thanhToan_NEW.IdThanhToan);
                                        if (thanhToan_OLD != null)
                                        {
                                            soTienDaDong_OLD = thanhToan_OLD.SoTienDaDong.Value; // Gán số liệu cũ

                                            //thanhToan_OLD.IdDonHang = thanhToan_NEW.IdDonHang;
                                            thanhToan_OLD.IdSanPham = thanhToan_NEW.IdSanPham;
                                            //thanhToan_OLD.IdKhachHang = thanhToan_NEW.IdKhachHang;
                                            thanhToan_OLD.ThuTuThanhToan = thanhToan_NEW.ThuTuThanhToan;

                                            thanhToan_OLD.SoTienDaDong_ChuaQuyDoi = thanhToan_NEW.SoTienDaDong_ChuaQuyDoi;
                                            thanhToan_OLD.IdDonViTien = thanhToan_NEW.IdDonViTien;
                                            thanhToan_OLD.SoTienDaDong = thanhToan_NEW.SoTienDaDong;
                                            thanhToan_OLD.PhanTramDaDong = thanhToan_NEW.PhanTramDaDong;

                                            thanhToan_OLD.GhiChu = donHang_NEW.GhiChu;
                                            //thanhToan_OLD.TrangThai = 1;
                                            thanhToan_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                            thanhToan_OLD.NgaySua = DateTime.Now;
                                            thanhToan_OLD.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;
                                        }
                                        else themMoiThanhToan(thanhToan_NEW: thanhToan_NEW);
                                    };

                                    chenhLech = soTienDaDong_NEW - soTienDaDong_OLD; // Tính khoảng chênh lệch

                                    #region Doanh thu (trừ lần thanh toán này đi để tính lại)
                                    if (nguoiDung_DoanhThu != null)
                                    {
                                        // Kiểm tra đã hoàn thiện mục tiêu chưa
                                        nguoiDung_DoanhThu.DoanhThuThucTe += chenhLech;
                                        decimal? phanTramHoanThien = ((decimal)nguoiDung_DoanhThu.DoanhThuThucTe / (decimal)nguoiDung_DoanhThu.DoanhThuMucTieu) * 100;
                                        if (nguoiDung_DoanhThu.PhanTramHoanThien < 100 && phanTramHoanThien >= 100)
                                        {
                                            nguoiDung_DoanhThu.NgayDatMucTieu = DateTime.Now;
                                        };
                                        nguoiDung_DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                                        nguoiDung_DoanhThu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                        nguoiDung_DoanhThu.NgaySua = DateTime.Now;

                                        db.SaveChanges();
                                    };
                                    if (coCauToChuc_DoanhThu != null)
                                    {
                                        // Kiểm tra đã hoàn thiện mục tiêu chưa
                                        coCauToChuc_DoanhThu.DoanhThuThucTe += chenhLech;
                                        decimal? phanTramHoanThien = ((decimal)coCauToChuc_DoanhThu.DoanhThuThucTe / (decimal)coCauToChuc_DoanhThu.DoanhThuMucTieu) * 100;
                                        if (coCauToChuc_DoanhThu.PhanTramHoanThien < 100 && phanTramHoanThien >= 100)
                                        {
                                            coCauToChuc_DoanhThu.NgayDatMucTieu = DateTime.Now;
                                        };
                                        coCauToChuc_DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                                        coCauToChuc_DoanhThu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                        coCauToChuc_DoanhThu.NgaySua = DateTime.Now;

                                        db.SaveChanges();
                                    };
                                    #endregion

                                };
                            }

                        };

                        #region Xóa bản ghi không sử dụng (trừ cả các lần thanh toán)
                        string str_idDonHangs_NEW = string.Join(",", idDonHangs_NEW.Distinct().Select(x => string.Format("'{0}'", x)));
                        string str_iThanhToans_NEW = string.Join(",", idThanhToans_NEW.Distinct().Select(x => string.Format("'{0}'", x)));
                        db.Database.ExecuteSqlCommand($@"
                        update tbKhachHang_DonHang 
                        set TrangThai = 0 
                        where IdKhachHang = '{khachHang_OLD.IdKhachHang}' and IdDonHang not in ({str_idDonHangs_NEW})

                        update tbKhachHang_DonHang_ThanhToan 
                        set TrangThai = 0 
                        where IdKhachHang = '{khachHang_OLD.IdKhachHang}' and IdThanhToan not in ({str_iThanhToans_NEW})
                        ");
                        #endregion

                        #endregion

                        #region Kiểm tra tiến độ đội nhóm
                        if (nguoiDung_DoanhThu != null)
                        {
                            nguoiDung_DoanhThus_NHOM = db.tbNguoiDung_DoanhThu.Where(x =>
                            x.IdNguoiTao != per.NguoiDung.IdNguoiDung &&
                            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                            x.DoanhThuThucTe >= nguoiDung_DoanhThu.DoanhThuThucTe && // Doanh thu cao hơn mình
                            x.NgaySua < nguoiDung_DoanhThu.NgaySua && // Nhập doanh thu sớm hơn mình
                            x.NgayLenMucTieu == nguoiDung_DoanhThu.NgayLenMucTieu).OrderBy(x => x.DoanhThuThucTe).ToList();
                        };
                        #endregion

                        #region Kiểm tra cấp độ doanh thu
                        long? TongDoanhThu = db.tbNguoiDung_DoanhThu.Where(x =>
                        x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                        x.IdNguoiTao == per.NguoiDung.IdNguoiDung).Sum(x => x.DoanhThuThucTe);
                        tbNguoiDung nguoiDung = db.tbNguoiDungs.FirstOrDefault(x => x.IdNguoiDung == per.NguoiDung.IdNguoiDung);
                        nguoiDung.IdCapDo_DoanhThu = chonCapDo_DoanhThu_TiepTheo(doanhThuCanSoSanh: TongDoanhThu.Value, capDo_DoanhThu_HienTai: per.CapDo_DoanhThu).IdCapDo_DoanhThu;
                        #endregion

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
        public ActionResult update_KhachHang_DonHang(string str_khachhang)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";
            string loaiThemMoi = "daydu";
            tbNguoiDung_DoanhThu nguoiDung_DoanhThu = new tbNguoiDung_DoanhThu();
            tbCoCauToChuc_DoanhThu coCauToChuc_DoanhThu = new tbCoCauToChuc_DoanhThu();
            List<tbNguoiDung_DoanhThu> nguoiDung_DoanhThus_NHOM = new List<tbNguoiDung_DoanhThu>();

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string format = "dd/MM/yyyy";
                    IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    tbKhachHangExtend khachHang_NEW = JsonConvert.DeserializeObject<tbKhachHangExtend>(str_khachhang ?? "", dateTimeConverter);
                    // Kiểm tra Tên khách hàng có trùng bản ghi nào khác không
                    if (kiemTra_KhachHang(khachHang: khachHang_NEW).KiemTraExcel.TrangThai == 0)
                    {
                        status = "warning";
                        mess = khachHang_NEW.KiemTraExcel.KetQua;
                    }
                    else
                    {
                        #region Doanh thu
                        string ngayTao = DateTime.Now.ToString("MM/yyyy");
                        nguoiDung_DoanhThu = db.tbNguoiDung_DoanhThu.FirstOrDefault(x =>
                        x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
                        x.NgayLenMucTieu == ngayTao &&
                        x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);

                        coCauToChuc_DoanhThu = db.tbCoCauToChuc_DoanhThu.FirstOrDefault(x =>
                        //x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
                        x.IdCoCauToChuc == Guid.Empty &&
                        x.NgayLenMucTieu == ngayTao &&
                        x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                        #endregion

                        #region Khách hàng
                        tbKhachHang khachHang_OLD = db.tbKhachHangs.Find(khachHang_NEW.IdKhachHang);

                        #region Lịch sử thay đổi thông tin
                        List<Tuple<string, object, object>> thayDois = Public.Handle.CompareSpecificFields(obj1: khachHang_OLD, obj2: khachHang_NEW,
                            fieldsToCompare: new List<string>(),
                            fieldsToExclude: new List<string> {
                                "QuyenTruyCap", "DuongDanFile", "TrangThai", "NguoiTao", "NguoiSua", "NgayTao", "NgaySua", "MaDonViSuDung"}
                            );
                        List<DuLieu_LichSu_ChiTiet> chiTiets = Public.Handle.ChiTietThayDoi(thayDois: thayDois);
                        db.tbKhachHang_LichSu.Add(new tbKhachHang_LichSu
                        {
                            IdLichSu = Guid.NewGuid(),
                            IdKhachHang = khachHang_OLD.IdKhachHang,
                            NoiDung = "Cập nhật thông tin khách hàng",
                            ChiTiet = JsonConvert.SerializeObject(chiTiets),
                            TrangThai = 1,
                            IdNguoiTao = per.NguoiDung.IdNguoiDung,
                            NgayTao = DateTime.Now,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        #endregion

                        khachHang_OLD.IdLoaiKhachHang = khachHang_NEW.IdLoaiKhachHang;
                        khachHang_OLD.LienKetSale = khachHang_NEW.LienKetSale;
                        khachHang_OLD.TrangThai = khachHang_NEW.TrangThai;
                        #endregion

                        #region Đơn hàng & Thanh toán & Doanh thu
                        // Lấy danh sách id bản ghi không xóa
                        List<Guid> idDonHangs_NEW = new List<Guid>();
                        List<Guid> idThanhToans_NEW = new List<Guid>();

                        List<tbKhachHang_DonHangExtend> donHangs_NEW = khachHang_NEW.DonHangs
                            //.Where(x => x.IdDonHang == Guid.Empty).ToList()
                            ;
                        foreach (tbKhachHang_DonHangExtend donHang_NEW in donHangs_NEW)
                        {
                            idDonHangs_NEW.Add(donHang_NEW.IdDonHang);
                            // Có rồi thì cập nhật, chưa có thì tạo mới
                            tbKhachHang_DonHang donHang_OLD = db.tbKhachHang_DonHang.FirstOrDefault(x => x.IdDonHang == donHang_NEW.IdDonHang);
                            if (donHang_OLD != null) // Không cho cập nhật
                            {
                                ////donHang_OLD.IdKhachHang = donHang_NEW.IdKhachHang;
                                //donHang_OLD.IdSanPham = donHang_NEW.IdSanPham;
                                //donHang_OLD.IdTrinhDoDauVao = donHang_NEW.IdTrinhDoDauVao;
                                //donHang_OLD.IdTrinhDoDauRa = donHang_NEW.IdTrinhDoDauRa;
                                //donHang_OLD.ThuTuDonHang = donHang_NEW.ThuTuDonHang;
                                //donHang_OLD.TongSoTien = donHang_NEW.TongSoTien;

                                //donHang_OLD.GhiChu = donHang_NEW.GhiChu;
                                ////donHang_OLD.TrangThai = 1;
                                //donHang_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                //donHang_OLD.NgaySua = DateTime.Now;
                                //donHang_OLD.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;

                                // Chỉ lấy thanh toán mới
                                List<tbKhachHang_DonHang_ThanhToan> thanhToans_NEW = donHang_NEW.ThanhToans
                                    .Where(x => x.IdThanhToan == Guid.Empty).ToList();
                                themMoiThanhToans(donHang: donHang_OLD, thanhToans_NEW: thanhToans_NEW);
                            }
                            else
                            {
                                tbKhachHang_DonHang donHang = new tbKhachHang_DonHang
                                {
                                    IdDonHang = Guid.NewGuid(),
                                    IdKhachHang = khachHang_OLD.IdKhachHang,
                                    IdSanPham = donHang_NEW.IdSanPham,
                                    IdTrinhDoDauVao = donHang_NEW.IdTrinhDoDauVao,
                                    IdTrinhDoDauRa = donHang_NEW.IdTrinhDoDauRa,
                                    ThuTuDonHang = donHang_NEW.ThuTuDonHang,
                                    TongSoTien = donHang_NEW.TongSoTien,
                                    //TrangThaiLopHoc = 1,

                                    GhiChu = donHang_NEW.GhiChu,
                                    TrangThai = 1,
                                    IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbKhachHang_DonHang.Add(donHang);

                                // Chỉ lấy thanh toán mới
                                List<tbKhachHang_DonHang_ThanhToan> thanhToans_NEW = donHang_NEW.ThanhToans
                                    .Where(x => x.IdThanhToan == Guid.Empty).ToList();
                                themMoiThanhToans(donHang: donHang, thanhToans_NEW: thanhToans_NEW, kiemTraTonTai: false);
                            };

                            void themMoiThanhToans(tbKhachHang_DonHang donHang, List<tbKhachHang_DonHang_ThanhToan> thanhToans_NEW, bool kiemTraTonTai = true)
                            {
                                void themMoiThanhToan(tbKhachHang_DonHang_ThanhToan thanhToan_NEW)
                                {
                                    tbKhachHang_DonHang_ThanhToan thanhToan = new tbKhachHang_DonHang_ThanhToan
                                    {
                                        IdThanhToan = Guid.NewGuid(),
                                        IdDonHang = thanhToan_NEW.IdDonHang,
                                        IdSanPham = thanhToan_NEW.IdSanPham,
                                        IdKhachHang = thanhToan_NEW.IdKhachHang,
                                        ThuTuThanhToan = thanhToan_NEW.ThuTuThanhToan,

                                        SoTienDaDong_ChuaQuyDoi = thanhToan_NEW.SoTienDaDong_ChuaQuyDoi,
                                        IdDonViTien = thanhToan_NEW.IdDonViTien,
                                        SoTienDaDong = thanhToan_NEW.SoTienDaDong,
                                        PhanTramDaDong = thanhToan_NEW.PhanTramDaDong,

                                        TrangThai = 1,
                                        IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                        NgayTao = DateTime.Now,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                    };
                                    db.tbKhachHang_DonHang_ThanhToan.Add(thanhToan);
                                };

                                foreach (tbKhachHang_DonHang_ThanhToan thanhToan_NEW in thanhToans_NEW)
                                {
                                    idThanhToans_NEW.Add(thanhToan_NEW.IdThanhToan);
                                    thanhToan_NEW.IdDonHang = donHang.IdDonHang;
                                    thanhToan_NEW.IdSanPham = donHang.IdSanPham;
                                    if (!kiemTraTonTai) themMoiThanhToan(thanhToan_NEW: thanhToan_NEW); // Không kiểm tra thì thêm luôn
                                    else
                                    {
                                        tbKhachHang_DonHang_ThanhToan thanhToan_OLD = db.tbKhachHang_DonHang_ThanhToan.FirstOrDefault(x => x.IdThanhToan == thanhToan_NEW.IdThanhToan);
                                        if (thanhToan_OLD != null) // Không cho cập nhật
                                        {
                                            ////thanhToan_OLD.IdDonHang = thanhToan_NEW.IdDonHang;
                                            //thanhToan_OLD.IdSanPham = thanhToan_NEW.IdSanPham;
                                            ////thanhToan_OLD.IdKhachHang = thanhToan_NEW.IdKhachHang;
                                            //thanhToan_OLD.ThuTuThanhToan = thanhToan_NEW.ThuTuThanhToan;

                                            //thanhToan_OLD.SoTienDaDong = thanhToan_NEW.SoTienDaDong;
                                            //thanhToan_OLD.PhanTramDaDong = thanhToan_NEW.PhanTramDaDong;

                                            //thanhToan_OLD.GhiChu = donHang_NEW.GhiChu;
                                            ////thanhToan_OLD.TrangThai = 1;
                                            //thanhToan_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                            //thanhToan_OLD.NgaySua = DateTime.Now;
                                            //thanhToan_OLD.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;
                                        }
                                        else themMoiThanhToan(thanhToan_NEW: thanhToan_NEW);
                                    };

                                    #region Doanh thu
                                    if (nguoiDung_DoanhThu != null)
                                    {
                                        // Kiểm tra đã hoàn thiện mục tiêu chưa
                                        nguoiDung_DoanhThu.DoanhThuThucTe += thanhToan_NEW.SoTienDaDong;
                                        decimal? phanTramHoanThien = ((decimal)nguoiDung_DoanhThu.DoanhThuThucTe / (decimal)nguoiDung_DoanhThu.DoanhThuMucTieu) * 100;
                                        if (nguoiDung_DoanhThu.PhanTramHoanThien < 100 && phanTramHoanThien >= 100)
                                        {
                                            nguoiDung_DoanhThu.NgayDatMucTieu = DateTime.Now;
                                        };
                                        nguoiDung_DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                                        nguoiDung_DoanhThu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                        nguoiDung_DoanhThu.NgaySua = DateTime.Now;

                                        db.SaveChanges();
                                    };
                                    if (coCauToChuc_DoanhThu != null)
                                    {
                                        // Kiểm tra đã hoàn thiện mục tiêu chưa
                                        coCauToChuc_DoanhThu.DoanhThuThucTe += thanhToan_NEW.SoTienDaDong;
                                        decimal? phanTramHoanThien = ((decimal)coCauToChuc_DoanhThu.DoanhThuThucTe / (decimal)coCauToChuc_DoanhThu.DoanhThuMucTieu) * 100;
                                        if (coCauToChuc_DoanhThu.PhanTramHoanThien < 100 && phanTramHoanThien >= 100)
                                        {
                                            coCauToChuc_DoanhThu.NgayDatMucTieu = DateTime.Now;
                                        };
                                        coCauToChuc_DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                                        coCauToChuc_DoanhThu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                        coCauToChuc_DoanhThu.NgaySua = DateTime.Now;

                                        db.SaveChanges();
                                    };
                                    #endregion
                                };
                            }

                        };

                        #endregion

                        #region Kiểm tra tiến độ đội nhóm
                        if (nguoiDung_DoanhThu != null)
                        {
                            nguoiDung_DoanhThus_NHOM = db.tbNguoiDung_DoanhThu.Where(x =>
                            x.IdNguoiTao != per.NguoiDung.IdNguoiDung &&
                            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                            x.DoanhThuThucTe >= nguoiDung_DoanhThu.DoanhThuThucTe && // Doanh thu cao hơn mình
                            x.NgaySua < nguoiDung_DoanhThu.NgaySua && // Nhập doanh thu sớm hơn mình
                            x.NgayLenMucTieu == nguoiDung_DoanhThu.NgayLenMucTieu).OrderBy(x => x.DoanhThuThucTe).ToList();
                        };
                        #endregion

                        #region Kiểm tra cấp độ doanh thu
                        long? TongDoanhThu = db.tbNguoiDung_DoanhThu.Where(x =>
                        x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung &&
                        x.IdNguoiTao == per.NguoiDung.IdNguoiDung).Sum(x => x.DoanhThuThucTe);
                        tbNguoiDung nguoiDung = db.tbNguoiDungs.FirstOrDefault(x => x.IdNguoiDung == per.NguoiDung.IdNguoiDung);
                        nguoiDung.IdCapDo_DoanhThu = chonCapDo_DoanhThu_TiepTheo(doanhThuCanSoSanh: TongDoanhThu.Value, capDo_DoanhThu_HienTai: per.CapDo_DoanhThu).IdCapDo_DoanhThu;
                        #endregion

                        db.tbKhachHang_LichSu.Add(new tbKhachHang_LichSu
                        {
                            IdLichSu = Guid.NewGuid(),
                            IdKhachHang = khachHang_OLD.IdKhachHang,
                            NoiDung = "Cập nhật đơn hàng",
                            TrangThai = 1,
                            IdNguoiTao = per.NguoiDung.IdNguoiDung,
                            NgayTao = DateTime.Now,
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
                mess,
                loaiThemMoi,
                nguoiDung_DoanhThu,
                nguoiDung_DoanhThus_NHOM
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult _delete_KhachHangs_()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            tbNguoiDung_DoanhThu nguoiDung_DoanhThu = new tbNguoiDung_DoanhThu();
            tbCoCauToChuc_DoanhThu coCauToChuc_DoanhThu = new tbCoCauToChuc_DoanhThu();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idKhachHangs = Request.Form["str_idKhachHangs"];
                    List<Guid> idKhachHangs = str_idKhachHangs.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();

                    foreach (Guid idKhachHang in idKhachHangs)
                    {
                        tbKhachHang khachHang = db.tbKhachHangs.Find(idKhachHang);

                        // Xóa bản ghi trong db
                        khachHang.TrangThai = 0;
                        khachHang.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                        khachHang.NgaySua = DateTime.Now;

                        #region Doanh thu
                        string ngayTao = DateTime.Now.ToString("MM/yyyy");
                        nguoiDung_DoanhThu = db.tbNguoiDung_DoanhThu.FirstOrDefault(x =>
                        x.IdNguoiTao == khachHang.IdNguoiTao &&
                        x.NgayLenMucTieu == ngayTao &&
                        x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);

                        coCauToChuc_DoanhThu = db.tbCoCauToChuc_DoanhThu.FirstOrDefault(x =>
                        //x.IdNguoiTao == per.NguoiDung.IdNguoiDung &&
                        x.IdCoCauToChuc == Guid.Empty &&
                        x.NgayLenMucTieu == ngayTao &&
                        x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                        #endregion

                        #region Xóa lịch sử khách hàng
                        db.Database.ExecuteSqlCommand($@"
                        update tbKhachHang_LichSu
                        set TrangThai = 0, IdNguoiSua = '{per.NguoiDung.IdNguoiDung}', NgaySua = '{DateTime.Now}'
                        where IdKhachHang = '{idKhachHang}'
                        ");
                        #endregion

                        #region Đơn hàng & thanh toán
                        List<tbKhachHang_DonHang> donHangs = db.tbKhachHang_DonHang.Where(x => x.IdKhachHang == idKhachHang).ToList() ?? new List<tbKhachHang_DonHang>();
                        foreach (tbKhachHang_DonHang donHang in donHangs)
                        {
                            donHang.TrangThai = 0;
                            donHang.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                            donHang.NgaySua = DateTime.Now;

                            List<tbKhachHang_DonHang_ThanhToan> thanhToans = db.tbKhachHang_DonHang_ThanhToan.Where(x => x.IdDonHang == donHang.IdDonHang).ToList() ?? new List<tbKhachHang_DonHang_ThanhToan>();
                            foreach (tbKhachHang_DonHang_ThanhToan thanhToan in thanhToans)
                            {
                                thanhToan.TrangThai = 0;
                                thanhToan.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                thanhToan.NgaySua = DateTime.Now;

                                #region Doanh thu
                                if (nguoiDung_DoanhThu != null)
                                {
                                    // Kiểm tra đã hoàn thiện mục tiêu chưa
                                    nguoiDung_DoanhThu.DoanhThuThucTe -= thanhToan.SoTienDaDong; // ngược lại lúc thêm
                                    decimal? phanTramHoanThien = ((decimal)nguoiDung_DoanhThu.DoanhThuThucTe / (decimal)nguoiDung_DoanhThu.DoanhThuMucTieu) * 100;
                                    if (nguoiDung_DoanhThu.PhanTramHoanThien >= 100 && phanTramHoanThien < 100) // ngược lại lúc thêm
                                    {
                                        nguoiDung_DoanhThu.NgayDatMucTieu = null;
                                    };
                                    nguoiDung_DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                                    nguoiDung_DoanhThu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                    nguoiDung_DoanhThu.NgaySua = DateTime.Now;

                                    db.SaveChanges();
                                };
                                if (coCauToChuc_DoanhThu != null)
                                {
                                    // Kiểm tra đã hoàn thiện mục tiêu chưa
                                    coCauToChuc_DoanhThu.DoanhThuThucTe -= thanhToan.SoTienDaDong; // ngược lại lúc thêm
                                    decimal? phanTramHoanThien = ((decimal)coCauToChuc_DoanhThu.DoanhThuThucTe / (decimal)coCauToChuc_DoanhThu.DoanhThuMucTieu) * 100;
                                    if (coCauToChuc_DoanhThu.PhanTramHoanThien >= 100 && phanTramHoanThien < 100) // ngược lại lúc thêm
                                    {
                                        nguoiDung_DoanhThu.NgayDatMucTieu = null;
                                    };
                                    coCauToChuc_DoanhThu.PhanTramHoanThien = Math.Round(phanTramHoanThien.Value, 2);

                                    coCauToChuc_DoanhThu.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                    coCauToChuc_DoanhThu.NgaySua = DateTime.Now;

                                    db.SaveChanges();
                                };
                                #endregion
                            };
                        };
                        #endregion
                    };
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

        [HttpGet]
        public JsonResult get_NguoiDungs(Guid idCoCauToChuc)
        {
            List<tbNguoiDung> nguoiDungs = db.tbNguoiDungs.Where(x => x.IdCoCauToChuc == idCoCauToChuc && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).OrderByDescending(x => x.NgayTao).ToList();
            return Json(new
            {
                data = nguoiDungs
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Methods
        private LocThongTinDto XuLyDuLieuLocThongTin(LocThongTinDto locThongTin)
        {
            if (string.IsNullOrEmpty(locThongTin.NgayTao)) locThongTin.NgayTao = DateTime.Now.ToString("MM/yyyy"); // mặc định tháng này

            return null;
        }
        #endregion
    }
}