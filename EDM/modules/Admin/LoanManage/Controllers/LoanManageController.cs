using DocumentFormation.Models;
using EDM_DB;
using LoanManage.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Public.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;

namespace LoanManage.Controllers
{
    public class LoanManageController : StaticArgController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/LoanManage";
        private List<tbPhieuMuonExtend> PHIEUMUONs
        {
            get
            {
                return Session["PHIEUMUONs"] as List<tbPhieuMuonExtend> ?? new List<tbPhieuMuonExtend>();
            }
            set
            {
                Session["PHIEUMUONs"] = value;
            }
        }
        IsoDateTimeConverter DATETIMECONVERTER = new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" };
        #endregion
        public ActionResult Index()
        {
            /**
           * Chuyển từ RouteConfigController sang để tránh việc gọi hàm trong controller này từ vai trò người dùng không vượt qua được kiểm tra quyền
           * */
            if (per.NguoiDung.IdNguoiDung == 0)
            {
                return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Auth", action = "Login" }));
            }
            // Tạo folder phiếu mượn văn bản
            string folderPath = $"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/PHIEUMUON";
            string folderPath_SERVER = Request.MapPath(folderPath);
            if (!System.IO.Directory.Exists(folderPath_SERVER))
            {
                System.IO.Directory.CreateDirectory(folderPath_SERVER);
            }
            return View($"{VIEW_PATH}/loanmanage.cshtml");
        }
        [HttpGet]
        public ActionResult getList()
        {
            List<tbPhieuMuonExtend> phieuMuons = get_PhieuMuons(loai: "all");
            return Json(new
            {
                data = phieuMuons
            }, JsonRequestBehavior.AllowGet);
        }
        public bool kiemTraThoiHanPhieuMuon(tbPhieuMuonExtend phieuMuon)
        {
            if (DateTime.Now >= phieuMuon.NgayHenTra.Value.AddDays(1))
            {
                return false;
            };
            return true;
        }
        #region CRUD
        [HttpPost]
        public ActionResult displayModal_Update_PhieuMuon(int idPhieuMuon)
        {
            // Lấy tên miền
            Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
            string hostName = uri.GetLeftPart(UriPartial.Authority);
            ViewBag.hostName = hostName;
            // Lấy thông tin phiếu
            tbPhieuMuonExtend phieuMuon = get_PhieuMuons("single", idPhieuMuons: idPhieuMuon.ToString()).FirstOrDefault();
            ViewBag.phieuMuon = phieuMuon;
            return PartialView($"{VIEW_PATH}/loanmanage-phieumuon.update.cshtml");
        }
        public List<tbPhieuMuonExtend> get_PhieuMuons(string loai, string idPhieuMuons = "")
        {
            List<tbPhieuMuonExtend> phieuMuons = new List<tbPhieuMuonExtend>();
            if (loai == "all")
            {
                string get_PhieuMuonsSQL = $@"select * from tbPhieuMuon where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai <> 0 order by NgayTao desc";
                phieuMuons = db.Database.SqlQuery<tbPhieuMuonExtend>(get_PhieuMuonsSQL).ToList() ?? new List<tbPhieuMuonExtend>();
            }
            else
            {
                if (idPhieuMuons != "")
                {
                    string get_PhieuMuonsSQL = $@"select * from tbPhieuMuon where TrangThai <> 0 and IdPhieuMuon in ({idPhieuMuons}) order by NgayTao desc";
                    phieuMuons = db.Database.SqlQuery<tbPhieuMuonExtend>(get_PhieuMuonsSQL).ToList() ?? new List<tbPhieuMuonExtend>();
                };
            };
            foreach (tbPhieuMuonExtend phieuMuon in phieuMuons)
            {
                // Lấy hình thức mượn
                phieuMuon.HinhThucMuon = db.default_tbHinhThucMuon.FirstOrDefault(x => x.IdHinhThucMuon == phieuMuon.IdHinhThucMuon && x.TrangThai == 1) ?? new default_tbHinhThucMuon();
                // Lấy danh sách văn bản
                phieuMuon.PhieuMuon_VanBans = get_PhieuMuon_VanBans(phieuMuon: phieuMuon);
                // Đổi trạng thái các phiếu hết hạn
                if (DateTime.Now >= phieuMuon.NgayHenTra.Value.AddDays(1) && phieuMuon.TrangThai != 4)
                {
                    hetHanPhieuMuon(phieuMuon: phieuMuon);
                };
            };
            return phieuMuons;
        }
        public void hetHanPhieuMuon(tbPhieuMuonExtend phieuMuon)
        {
            db.Database.ExecuteSqlCommand($@"
            update tbPhieuMuon 
                set TrangThai = 4 , NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
            where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdPhieuMuon = {phieuMuon.IdPhieuMuon}
            update tbPhieuMuon_VanBan 
                set TrangThai = 4 , NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
            where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdPhieuMuon = {phieuMuon.IdPhieuMuon}
            ");
            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
            {
                TenModule = "Quản lý đăng ký mượn",
                ThaoTac = "Hủy duyệt",
                NoiDungChiTiet = "Hết hạn phiếu mượn",

                NgayTao = DateTime.Now,
                IdNguoiDung = per.NguoiDung.IdNguoiDung,
                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
            });
            #region Xóa thư mục PHIEUMUON
            foreach (tbPhieuMuon_VanBanExtend phieuMuon_VanBan in phieuMuon.PhieuMuon_VanBans)
            {
                tbHoSo hoSo = phieuMuon_VanBan.HoSo;
                tbHoSo_VanBan vanBan = phieuMuon_VanBan.VanBan;
                string tenVanBan_BANDAU = string.Format("{0}{1}", vanBan.TenVanBan, vanBan.Loai);
                var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU, maXacThuc: phieuMuon.MaXacThuc);
                // Tạo thư mục chứa văn bản bóc tách của phiếu mượn
                if (System.IO.Directory.Exists(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER))
                {
                    System.IO.Directory.Delete(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER, true);
                    // Xóa văn bản đã cắt
                    //if (System.IO.File.Exists(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER)) {
                    //    System.IO.File.Delete(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER);
                    //};
                };
            };
            #endregion
            db.SaveChanges();
        }
        public List<tbPhieuMuon_VanBanExtend> get_PhieuMuon_VanBans(tbPhieuMuonExtend phieuMuon)
        {
            string get_PhieuMuon_VanBansSQL = $@"select * from tbPhieuMuon_VanBan where TrangThai <> 0 and IdPhieuMuon = {phieuMuon.IdPhieuMuon}";
            List<tbPhieuMuon_VanBanExtend> phieuMuon_VanBans = db.Database.SqlQuery<tbPhieuMuon_VanBanExtend>(get_PhieuMuon_VanBansSQL).ToList() ?? new List<tbPhieuMuon_VanBanExtend>();
            foreach (tbPhieuMuon_VanBanExtend vanBan in phieuMuon_VanBans)
            {
                // Tìm văn bản
                string get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai = 1 and IdVanBan in ({vanBan.IdVanBan})";
                vanBan.VanBan = db.Database.SqlQuery<tbHoSo_VanBanExtend>(get_VanBanSQL).FirstOrDefault() ?? new tbHoSo_VanBanExtend();
                vanBan.HoSo = db.tbHoSoes.Find(vanBan.VanBan.IdHoSo);
                // Nếu đã duyệt thì lấy đường dẫn văn bản ở thư mục file cắt
                //if (phieuMuon.TrangThai == 2) {
                //}
                vanBan.VanBan.DuongDan = vanBan.DuongDanFile_DaXuLy;
                if (phieuMuon.TrangThai == 3 || phieuMuon.TrangThai == 4)
                {
                    #region Lấy đường dẫn văn bản
                    string tenVanBan_BANDAU = string.Format("{0}{1}", vanBan.VanBan.TenVanBan, vanBan.VanBan.Loai);
                    var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: vanBan.HoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU);
                    vanBan.VanBan.DuongDan = duongDanVanBan.duongDanVanBan_BANDAU;
                    if (duongDanVanBan.loaiVanBan.Contains("xls") || duongDanVanBan.loaiVanBan.Contains("doc"))
                    {
                        vanBan.VanBan.DuongDan = duongDanVanBan.duongDanVanBan_CHUYENDOI;
                    };
                    #endregion
                };
            };
            return phieuMuon_VanBans;
        }
        public ActionResult xemPDF(int idPhieuMuon = 0, int idVanBan = 0)
        {
            tbPhieuMuonExtend phieuMuon = get_PhieuMuons(loai: "single", idPhieuMuons: idPhieuMuon.ToString()).FirstOrDefault();
            if (phieuMuon == null) return View();
            // Trả view xem pdf
            ViewBag.DuongDanFile = phieuMuon.PhieuMuon_VanBans.FirstOrDefault(x => x.IdVanBan == idVanBan).VanBan.DuongDan;
            // Chỉ khi đăng ký mượn và không phải mượn đọc thì mới cho tải
            bool quyenTaiXuong = false;
            if (phieuMuon.HinhThucMuon.IdHinhThucMuon == 2 || phieuMuon.HinhThucMuon.IdHinhThucMuon == 3)
            {
                quyenTaiXuong = true;
            };
            ViewBag.QuyenTaiXuong = quyenTaiXuong;
            ViewBag.phieuMuon = phieuMuon;
            //return View($"{VIEW_PATH}/loanmanage-pdf_viewer.cshtml");
            return View("~/Views/_Shared/_lib/pdf_viewer.cshtml");
        }
        [HttpPost]
        public ActionResult create_PhieuMuon(string str_phieuMuon)
        {
            string status = "success";
            string mess = "Yêu cầu đăng ký mượn của bạn đã được gửi đi, kết quả duyệt phiếu sẽ được gửi vào [mail] bạn cung cấp";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbPhieuMuonExtend phieuMuon_NEW = JsonConvert.DeserializeObject<tbPhieuMuonExtend>(str_phieuMuon ?? "", DATETIMECONVERTER);
                    if (phieuMuon_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        #region Tạo phiếu mượn, phiếu mượn văn bản và lịch sử
                        // Tạo phiếu mượn
                        tbPhieuMuon phieuMuon = new tbPhieuMuon
                        {
                            NguoiMuon_HoTen = phieuMuon_NEW.NguoiMuon_HoTen,
                            NguoiMuon_CCCD = phieuMuon_NEW.NguoiMuon_CCCD,
                            NguoiMuon_SoDienThoai = phieuMuon_NEW.NguoiMuon_SoDienThoai,
                            NguoiMuon_DonViCongTac = phieuMuon_NEW.NguoiMuon_DonViCongTac,
                            NguoiMuon_Email = phieuMuon_NEW.NguoiMuon_Email,
                            NguoiMuon_LyDoMuon = phieuMuon_NEW.NguoiMuon_LyDoMuon,
                            IdHinhThucMuon = phieuMuon_NEW.IdHinhThucMuon,
                            NgayYeuCau = phieuMuon_NEW.NgayYeuCau,
                            NgayHenTra = phieuMuon_NEW.NgayHenTra,
                            MaXacThuc = Public.Handle.RandomString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 6),

                            TrangThai = 1,
                            NguoiTao = per.NguoiDung.IdNguoiDung,
                            NgayTao = DateTime.Now,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        };
                        db.tbPhieuMuons.Add(phieuMuon);
                        db.SaveChanges();
                        phieuMuon.SoPhieu = string.Format("PM.{0}.{1}", DateTime.Now.Year, phieuMuon.IdPhieuMuon);
                        phieuMuon_NEW.SoPhieu = phieuMuon.SoPhieu; // Gán số phiếu
                        // Tạo phiếu mượn văn bản
                        foreach (tbPhieuMuon_VanBanExtend phieuMuon_VanBan_NEW in phieuMuon_NEW.PhieuMuon_VanBans)
                        {
                            tbPhieuMuon_VanBan phieuMuon_VanBan = new tbPhieuMuon_VanBan
                            {
                                IdPhieuMuon = phieuMuon.IdPhieuMuon,
                                IdVanBan = phieuMuon_VanBan_NEW.IdVanBan,
                                TuTrang = phieuMuon_VanBan_NEW.TuTrang,
                                DenTrang = phieuMuon_VanBan_NEW.DenTrang,
                                DuongDanFile_DaXuLy = null,

                                TrangThai = 1,
                                NguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            #region Tạo thư mục PHIEUMUON
                            tbHoSo_VanBan vanBan = db.tbHoSo_VanBan.Find(phieuMuon_VanBan_NEW.IdVanBan);
                            tbHoSo hoSo = db.tbHoSoes.Find(vanBan.IdHoSo);
                            string tenVanBan_BANDAU = string.Format("{0}{1}", vanBan.TenVanBan, vanBan.Loai);
                            var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU, maXacThuc: phieuMuon.MaXacThuc);
                            // Tạo thư mục chứa văn bản bóc tách của phiếu mượn
                            if (Directory.Exists(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER))
                            {
                                Directory.Delete(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER, true);
                            };
                            Directory.CreateDirectory(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER);
                            if (duongDanVanBan.loaiVanBan.Contains("xls") || duongDanVanBan.loaiVanBan.Contains("doc"))
                            {
                                // Lấy đường dẫn tới thư mục văn bản đã chuyển đổi thành PDF
                                int startPage = phieuMuon_VanBan.TuTrang ?? 0;
                                int endPage = phieuMuon_VanBan.DenTrang ?? 1;
                                bool ketQua = Public.Handle.CutPDF(inputPdfPath: duongDanVanBan.duongDanVanBan_CHUYENDOI_SERVER, outputPdfPath: duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER, startPage: startPage, endPage: endPage);
                                if (ketQua)
                                {
                                    phieuMuon_VanBan.DuongDanFile_DaXuLy = duongDanVanBan.duongDanVanBan_PHIEUMUON;
                                };
                            }
                            else if (duongDanVanBan.loaiVanBan.Contains("pdf"))
                            {
                                int startPage = phieuMuon_VanBan.TuTrang ?? 0;
                                int endPage = phieuMuon_VanBan.DenTrang ?? 1;
                                bool ketQua = Public.Handle.CutPDF(inputPdfPath: duongDanVanBan.duongDanVanBan_BANDAU_SERVER, outputPdfPath: duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER, startPage: startPage, endPage: endPage);
                                if (ketQua)
                                {
                                    phieuMuon_VanBan.DuongDanFile_DaXuLy = duongDanVanBan.duongDanVanBan_PHIEUMUON;
                                };
                            }
                            else
                            { // Không phải office thì không cần cắt
                                System.IO.File.Copy(duongDanVanBan.duongDanVanBan_BANDAU_SERVER, duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER, true);
                                phieuMuon_VanBan.DuongDanFile_DaXuLy = duongDanVanBan.duongDanVanBan_PHIEUMUON;
                            };
                            #endregion
                            db.tbPhieuMuon_VanBan.Add(phieuMuon_VanBan);
                        };
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Quản lý đăng ký mượn",
                            ThaoTac = "Thêm mới",
                            NoiDungChiTiet = "Thêm mới phiếu mượn",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        #endregion
                        #region Gửi mail
                        /*string tieuDeMail = "[📣 EDM] - XÁC NHẬN PHIẾU ĐĂNG KÝ MƯỢN 😍";
                        void guiMail(tbPhieuMuonExtend phieuMuon_IN) {
                            Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
                            string baseUrl = uri.GetLeftPart(UriPartial.Authority);
                            var model = new PhieuMuonMailM {
                                PhieuMuon = phieuMuon_IN,
                                DonViSuDung = per.DonViSuDung
                            };
                            // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                            string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/loanmanage-mail.xacnhanphieu.cshtml", model);
                            Public.Handle.SendEmail(sendTo: phieuMuon_IN.NguoiMuon_Email, subject: tieuDeMail, body: viewAsString, isHTML: true, donViSuDung: per.DonViSuDung);
                        };
                        guiMail(phieuMuon_NEW);*/
                        #endregion
                        db.SaveChanges();
                        #region Tìm danh sách quản trị viên có quyền duyệt phiếu và gửi mail thông báo
                        // Lấy người dùng có quyền duyệt phiếu mượn
                        string sql_EmailNguoiDung_CoQuyenDuyet = $@"
                            select nguoiDung.Email
                            from tbNguoiDung nguoiDung
                            join tbKieuNguoiDung kieuNguoiDung on nguoiDung.IdKieuNguoiDung = kieuNguoiDung.idKieuNguoiDung
                            where 
                                nguoiDung.TrangThai != 0 and nguoiDung.MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                                and kieuNguoiDung.IdChucNang like '%LoanManage%'
                            ";
                        List<string> emailNguoiDung_CoQuyenDuyets = db.Database.SqlQuery<string>(sql_EmailNguoiDung_CoQuyenDuyet).
                            Distinct().ToList();

                        #region Gửi mail
                        void guiMail(string email)
                        {
                            Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
                            string baseUrl = uri.GetLeftPart(UriPartial.Authority);
                            string tieuDeMail = "[📣 EDM] - NGƯỜI DÙNG ĐĂNG KÝ MƯỢN";
                            var model = new ThongBaoCoPhieuMuonMailM
                            {
                                //NguoiDung = nguoiDung,
                                PhieuMuon = phieuMuon_NEW,
                                GhiChu = baseUrl,
                                DonViSuDung = per.DonViSuDung
                            };
                            // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                            string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/loanmanage-mail.thongbaocophieumuon.cshtml", model);
                            Public.Handle.SendEmail(sendTo: email, subject: tieuDeMail, body: viewAsString, isHTML: true, donViSuDung: per.DonViSuDung);
                        };
                        foreach (string emailNguoiDung_CoQuyenDuyet in emailNguoiDung_CoQuyenDuyets)
                        {
                            guiMail(emailNguoiDung_CoQuyenDuyet);
                        };
                        #endregion
                        #endregion
                        scope.Commit();
                    }
                }
                catch (Exception ex)
                {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            };
            return Json(new
            {
                status,
                mess,
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Quy trình xử lý phiếu
        [HttpPost]
        public ActionResult displayModal_ChungThuc(int idPhieuMuon, int idVanBan, string loai)
        {
            // Lấy thông tin phiếu
            tbPhieuMuonExtend phieuMuon = get_PhieuMuons("single", idPhieuMuons: idPhieuMuon.ToString()).FirstOrDefault();
            // Lấy văn bản
            tbPhieuMuon_VanBanExtend phieuMuon_VanBan = phieuMuon.PhieuMuon_VanBans.FirstOrDefault(x => x.IdVanBan == idVanBan) ?? new tbPhieuMuon_VanBanExtend();
            // Lấy phông lưu trữ
            tbDonViSuDung_PhongLuuTru phongLuuTru = db.tbDonViSuDung_PhongLuuTru.Find(phieuMuon_VanBan.HoSo.IdPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();

            ViewBag.phieuMuon = phieuMuon;
            ViewBag.phieuMuon_VanBan = phieuMuon_VanBan;
            ViewBag.phongLuuTru = phongLuuTru;
            ViewBag.loai = loai;
            return PartialView($"{VIEW_PATH}/loanmanage-phieumuon.mauchungthuc.cshtml");
        }
        [HttpPost]
        public JsonResult luuMauChungThuc()
        {
            string[] base64_MAUCHUNGTHUCs = JsonConvert.DeserializeObject<string[]>(Request.Form["base64_MAUCHUNGTHUCs"]);
            int idPhieuMuon = int.Parse(Request.Form["idPhieuMuon"]);
            int idVanBan = int.Parse(Request.Form["idVanBan"]);
            tbHoSo_VanBan vanBan = db.tbHoSo_VanBan.Find(idVanBan);
            tbHoSo hoSo = db.tbHoSoes.Find(vanBan.IdHoSo);
            tbPhieuMuon phieuMuon = db.tbPhieuMuons.Find(idPhieuMuon);

            string tenVanBan_BANDAU = string.Format("{0}{1}", vanBan.TenVanBan, vanBan.Loai);
            var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU, maXacThuc: phieuMuon.MaXacThuc);
            if (!Directory.Exists(duongDanVanBan.duongDanThuMuc_MAUCHUNGTHUC_SERVER))
            {
                Directory.CreateDirectory(duongDanVanBan.duongDanThuMuc_MAUCHUNGTHUC_SERVER);
            };
            for (int i = 0; i < base64_MAUCHUNGTHUCs.Length; i++)
            {
                string _base64 = base64_MAUCHUNGTHUCs[i].Split(',')[1].Replace("\"", "");
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(_base64)))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {
                        decimal width = bm2.Width;
                        decimal height = bm2.Height;
                        decimal ratio = width / height;
                        int _height = 340; // Đặt mặc định chiều cao
                        int _width = (int)(_height * ratio);
                        Bitmap bitmap = new Bitmap(bm2, new Size((int)width, (int)height));
                        bitmap.SetResolution(1000, 1000);
                        bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                        string duongDanThuMuc_MAUCHUNGTHUC_SERVER = string.Format("{0}/mau-chung-thuc-{1}.png", duongDanVanBan.duongDanThuMuc_MAUCHUNGTHUC_SERVER, i);
                        bitmap.Save(duongDanThuMuc_MAUCHUNGTHUC_SERVER);
                    };
                };
            };
            return Json(new
            {
                duongDanVanBan
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult xacNhanChungThuc()
        {
            try
            {
                byte[] base64_VANBAN_CHUNGTHUC = Convert.FromBase64String(Request.Form["base64_VANBAN_CHUNGTHUC"].Split(',')[1].Replace("\"", ""));
                var duongDanVanBan = JsonConvert.DeserializeObject<Public.Models.DuongDanVanBan>(Request.Form["duongDanVanBan"]);
                int viTriTrangChungThuc = int.Parse(Request.Form["viTriTrangChungThuc"]);
                // Ghi đè văn bản đã chứng thực lên văn bản mượn
                //System.IO.File.WriteAllBytes(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER, base64_VANBAN_CHUNGTHUC);
                System.IO.File.WriteAllBytes(duongDanVanBan.duongDanVanBan_TRANGCHUNGTHUC_SERVER, base64_VANBAN_CHUNGTHUC);
                // Tạo đường dẫn tạm cho văn bản sau cắt ghép sau đó ta sẽ ghi đè lên văn bản cũ
                string duongDanVanBan_PHIEUMUON_TAMTHOI_SERVER = string.Format("{0}/van-ban-tam-thoi.pdf", duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER);
                bool ketQua = Public.Handle.ReplacePdf(
                    inputPdfPath: duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER,
                    //outputPdfPath: duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER,
                    outputPdfPath: duongDanVanBan_PHIEUMUON_TAMTHOI_SERVER,
                    replacePdfPath: duongDanVanBan.duongDanVanBan_TRANGCHUNGTHUC_SERVER,
                    replacePageNumber: viTriTrangChungThuc);
                if (ketQua)
                {
                    // Thay thế tệp gốc bằng tệp tạm thời
                    System.IO.File.Copy(duongDanVanBan_PHIEUMUON_TAMTHOI_SERVER, duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER, true);
                    // Xóa tệp tạm thời (nếu bạn muốn giữ tệp tam thời, hãy bỏ qua bước này)
                    System.IO.File.Delete(duongDanVanBan_PHIEUMUON_TAMTHOI_SERVER);
                    return Json(new
                    {
                        status = "success",
                        mess = "Chứng thực thành công"
                    }, JsonRequestBehavior.AllowGet);
                };
                return Json(new
                {
                    status = "error",
                    mess = "Chứng thực không thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new
                {
                    status = "error",
                    mess = "Chứng thực không thành công"
                }, JsonRequestBehavior.AllowGet);
            };
        }
        [HttpPost]
        public ActionResult update_PhieuMuons()
        {
            string status = "success";
            string mess = "";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idPhieuMuons = Request.Form["str_idPhieuMuons"];
                    string trangThaiCapNhat = Request.Form["trangThaiCapNhat"];
                    string lyDoHuyDuyet = Request.Form["lyDoHuyDuyet"] != "" ? Request.Form["lyDoHuyDuyet"] : "Phiếu của bạn không được chấp thuận bởi quản trị viên";
                    string tieuDeMail = "";
                    int trangThai = 0;
                    if (str_idPhieuMuons != "")
                    {
                        #region Gửi mail
                        void guiMail(tbPhieuMuonExtend phieuMuon)
                        {
                            Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
                            string baseUrl = uri.GetLeftPart(UriPartial.Authority);
                            string code = Public.Handle.EncodeTo64($@"{phieuMuon.MaXacThuc}-{phieuMuon.IdPhieuMuon}");
                            string duongDanKhaiThac = string.Format("{0}/KhaiThacHoSo?code={1}", baseUrl, code);
                            var model = new PhieuMuonMailM
                            {
                                TrangThai = trangThaiCapNhat,
                                PhieuMuon = phieuMuon,
                                DuongDanKhaiThac = duongDanKhaiThac,
                                GhiChu = lyDoHuyDuyet,
                                DonViSuDung = per.DonViSuDung
                            };
                            // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                            string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/loanmanage-mail.capnhatphieu.cshtml", model);
                            Public.Handle.SendEmail(sendTo: phieuMuon.NguoiMuon_Email, subject: tieuDeMail, body: viewAsString, isHTML: true, donViSuDung: per.DonViSuDung);
                        };
                        #endregion
                        List<int> idPhieuMuons = str_idPhieuMuons.Split(',').Select(Int32.Parse).ToList();
                        foreach (int idPhieuMuon in idPhieuMuons)
                        {
                            tbPhieuMuonExtend phieuMuon = get_PhieuMuons("single", idPhieuMuons: idPhieuMuon.ToString()).FirstOrDefault() ?? new tbPhieuMuonExtend();
                            if (trangThaiCapNhat == "duyet")
                            {
                                if (phieuMuon.TrangThai != 2 && phieuMuon.TrangThai != 3 & phieuMuon.TrangThai != 4)
                                {
                                    mess = "Duyệt phiếu thành công";
                                    tieuDeMail = "[📣 EDM] - ĐĂNG KÝ MƯỢN THÀNH CÔNG 😍";
                                    trangThai = 2;
                                    db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                                    {
                                        TenModule = "Quản lý đăng ký mượn",
                                        ThaoTac = "Duyệt",
                                        NoiDungChiTiet = "Duyệt phiếu mượn",

                                        NgayTao = DateTime.Now,
                                        IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                    });
                                    #region Tạo thư mục PHIEUMUON - Do đã tạo thư mục từ lúc tạo mới phiếu mượn nên không cần nữa
                                    //foreach (tbPhieuMuon_VanBanExtend phieuMuon_VanBan in phieuMuon.PhieuMuon_VanBans) {
                                    //    /**
                                    //     * Mô tả sơ đồ tệp
                                    //     * Ghi chú: 
                                    //     *  - ext: loại tệp (doc|docx|xls|xlsx|png|jpg|jpeg|mp4) không bao gồm pdf
                                    //     * ---------------------------------------------
                                    //     * (1) |(2)   |(3)      |(4)         |(5)
                                    //     * HoSo|
                                    //     *     |VanBan|VanBan.ext
                                    //     *            |CHUYENDOI|[.docx]     |VanBan.pdf
                                    //     *                      |[.xlsx]     |VanBan.pdf
                                    //     *                      ...
                                    //     *            |PHIEUMUON|(Mã phiếu 1)|VanBan.pdf
                                    //     *                      |(Mã phiếu 2)|VanBan.pdf
                                    //     *                      ...
                                    //     */
                                    //    tbHoSo hoSo = phieuMuon_VanBan.HoSo;
                                    //    tbHoSo_VanBan vanBan = phieuMuon_VanBan.VanBan;
                                    //    string tenVanBan_BANDAU = string.Format("{0}{1}", vanBan.TenVanBan, vanBan.Loai);
                                    //    var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU, maXacThuc: phieuMuon.MaXacThuc);
                                    //    #region K sử dụng vì trong thư mục còn chứa thư mục chứng thực
                                    //    //// Tạo thư mục chứa văn bản bóc tách của phiếu mượn
                                    //    //if (Directory.Exists(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER)) {
                                    //    //    Directory.Delete(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER, true);
                                    //    //};
                                    //    #endregion
                                    //    //Directory.CreateDirectory(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER);
                                    //    if (!Directory.Exists(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER)) {
                                    //        Directory.CreateDirectory(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER);
                                    //    };
                                    //    // Xóa văn bản đã cắt
                                    //    if (System.IO.File.Exists(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER)) {
                                    //        System.IO.File.Delete(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER);
                                    //    };
                                    //    // Tìm văn bản
                                    //    tbPhieuMuon_VanBan vanBan_NEW = db.tbPhieuMuon_VanBan.Find(phieuMuon_VanBan.IdPhieuMuonVanBan) ?? new tbPhieuMuon_VanBan();
                                    //    if (duongDanVanBan.loaiVanBan.Contains("xls") || duongDanVanBan.loaiVanBan.Contains("doc")) {
                                    //        // Lấy đường dẫn tới thư mục văn bản đã chuyển đổi thành PDF
                                    //        int startPage = phieuMuon_VanBan.TuTrang ?? 0;
                                    //        int endPage = phieuMuon_VanBan.DenTrang ?? 1;
                                    //        bool ketQua = Public.Handle.CutPDF(inputPdfPath: duongDanVanBan.duongDanVanBan_CHUYENDOI_SERVER, outputPdfPath: duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER, startPage: startPage, endPage: endPage);
                                    //        if (ketQua) {
                                    //            vanBan_NEW.DuongDanFile_DaXuLy = duongDanVanBan.duongDanVanBan_PHIEUMUON;
                                    //        };
                                    //    } else if (duongDanVanBan.loaiVanBan.Contains("pdf")) {
                                    //        int startPage = phieuMuon_VanBan.TuTrang ?? 0;
                                    //        int endPage = phieuMuon_VanBan.DenTrang ?? 1;
                                    //        bool ketQua = Public.Handle.CutPDF(inputPdfPath: duongDanVanBan.duongDanVanBan_BANDAU_SERVER, outputPdfPath: duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER, startPage: startPage, endPage: endPage);
                                    //        if (ketQua) {
                                    //            vanBan_NEW.DuongDanFile_DaXuLy = duongDanVanBan.duongDanVanBan_PHIEUMUON;
                                    //        };
                                    //    } else { // Không phải office thì không cần cắt
                                    //        System.IO.File.Copy(duongDanVanBan.duongDanVanBan_BANDAU_SERVER, duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER, true);
                                    //        vanBan_NEW.DuongDanFile_DaXuLy = duongDanVanBan.duongDanVanBan_PHIEUMUON;
                                    //    };
                                    //};
                                    #endregion
                                    db.SaveChanges();
                                }
                                else
                                {
                                    mess = "Tồn tại phiếu không đủ điều kiện duyệt";
                                    status = "warning";
                                    trangThai = phieuMuon.TrangThai.Value;
                                };
                            }
                            else if (trangThaiCapNhat == "huyduyet")
                            {
                                mess = "Đã hủy duyệt phiếu";
                                tieuDeMail = "[📣 EDM] - ĐĂNG KÝ MƯỢN KHÔNG THÀNH CÔNG 😓";
                                trangThai = 3;
                                db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                                {
                                    TenModule = "Quản lý đăng ký mượn",
                                    ThaoTac = "Hủy duyệt",
                                    NoiDungChiTiet = "Hủy duyệt phiếu mượn",

                                    NgayTao = DateTime.Now,
                                    IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                });
                                #region Xóa thư mục PHIEUMUON
                                foreach (tbPhieuMuon_VanBanExtend phieuMuon_VanBan in phieuMuon.PhieuMuon_VanBans)
                                {
                                    tbHoSo hoSo = phieuMuon_VanBan.HoSo;
                                    tbHoSo_VanBan vanBan = phieuMuon_VanBan.VanBan;
                                    string tenVanBan_BANDAU = string.Format("{0}{1}", vanBan.TenVanBan, vanBan.Loai);
                                    var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU, maXacThuc: phieuMuon.MaXacThuc);
                                    // Tạo thư mục chứa văn bản bóc tách của phiếu mượn
                                    if (System.IO.Directory.Exists(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER))
                                    {
                                        System.IO.Directory.Delete(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER, true);
                                        // Xóa văn bản đã cắt
                                        //if (System.IO.File.Exists(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER)) {
                                        //    System.IO.File.Delete(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER);
                                        //};
                                    };
                                };
                                #endregion
                            }
                            else if (trangThaiCapNhat == "xoa")
                            {
                                mess = "Xóa bản ghi thành công";
                                db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                                {
                                    TenModule = "Quản lý đăng ký mượn",
                                    ThaoTac = "Xóa",
                                    NoiDungChiTiet = "Xóa phiếu mượn",

                                    NgayTao = DateTime.Now,
                                    IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                });
                                #region Xóa thư mục PHIEUMUON
                                foreach (tbPhieuMuon_VanBanExtend phieuMuon_VanBan in phieuMuon.PhieuMuon_VanBans)
                                {
                                    tbHoSo hoSo = phieuMuon_VanBan.HoSo;
                                    tbHoSo_VanBan vanBan = phieuMuon_VanBan.VanBan;
                                    string tenVanBan_BANDAU = string.Format("{0}{1}", vanBan.TenVanBan, vanBan.Loai);
                                    var duongDanVanBan = LayDuongDanVanBan(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU, maXacThuc: phieuMuon.MaXacThuc);
                                    // Tạo thư mục chứa văn bản bóc tách của phiếu mượn
                                    if (System.IO.Directory.Exists(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER))
                                    {
                                        System.IO.Directory.Delete(duongDanVanBan.duongDanThuMuc_PHIEUMUON_SERVER, true);
                                        // Xóa văn bản đã cắt
                                        //if (System.IO.File.Exists(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER)) {
                                        //    System.IO.File.Delete(duongDanVanBan.duongDanVanBan_PHIEUMUON_SERVER);
                                        //};
                                    };
                                };
                                #endregion
                            }
                            else
                            {
                                mess = "Không đúng thao tác";
                                status = "error";
                                return Json(new
                                {
                                    status,
                                    mess
                                }, JsonRequestBehavior.AllowGet);
                            }
                            // Gửi mail (Nếu đang là (hủy || hết hạn) và tiến hành xóa thì không cần gửi mail)
                            if ((phieuMuon.TrangThai == 3 || phieuMuon.TrangThai == 4) && trangThai == 0) { }
                            else guiMail(phieuMuon);
                            // Thay đổi trạng thái
                            db.Database.ExecuteSqlCommand($@"
                            update tbPhieuMuon 
                                set TrangThai = {trangThai} , NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                            where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdPhieuMuon = {idPhieuMuon}
                            update tbPhieuMuon_VanBan 
                                set TrangThai = {trangThai} , NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                            where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdPhieuMuon = {idPhieuMuon}
                            ");
                        }
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
        #endregion
    }
}