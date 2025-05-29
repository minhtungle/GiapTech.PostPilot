using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Auth.Models;
using EDM_DB;
using Infrastructure.Enums;
using Newtonsoft.Json;
using Public.Models;
using ICacheManager = Infrastructure.Interfaces.ICacheManager;
using Infrastructure.Extentions;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin;

namespace Auth.Controllers
{
    public class AuthController : Controller
    {
        #region Biến public để in hoa
        private EDM_DBEntities db;
        private readonly string VIEW_PATH = "~/Views/Auth";
        private string MAXACTHUC
        {
            get
            {
                return Session["MAXACTHUC"] as string ?? string.Empty;
            }
            set
            {
                Session["MAXACTHUC"] = value;
            }
        }
        private tbNguoiDung NGUOIDUNG
        {
            get
            {
                return Session["NGUOIDUNG"] as tbNguoiDung ?? new tbNguoiDung();
            }
            set
            {
                Session["NGUOIDUNG"] = value;
            }
        }
        #endregion
        private readonly int _cacheTimeOut;
        public AuthController()
        {
            db = new EDM_DBEntities();
        }

        #region VIEW
        public void LayQuyen()
        {
            string currentDomain = Request.Url.Host.ToLower();
            Permission per = new Permission
            {
                DonViSuDung = layDonViSuDung(),
                Role = "USER"
            };
            Session["Permission"] = per; // Phải set như này thì từ sau mới sử dụng được session
        }
        public ActionResult Index()
        {
            LayQuyen();
            return View($"{VIEW_PATH}/auth.login.cshtml");
        }
        public ActionResult Login()
        {
            LayQuyen();
            return View($"{VIEW_PATH}/auth.login.cshtml");
        }
        public ActionResult Register()
        {
            LayQuyen();
            return View($"{VIEW_PATH}/auth.register.cshtml");
        }
        public ActionResult Forgot()
        {
            LayQuyen();
            return View($"{VIEW_PATH}/auth.forgot.cshtml");
        }
        public ActionResult Error()
        {
            LayQuyen();
            return View("~/Views/Error/Index.cshtml");
        }
        public ActionResult ChangePassword()
        {
            LayQuyen();
            return View("~/Views/Auth/auth.changepassword.cshtml");
        }
        #endregion VIEW

        [HttpPost]
        public ActionResult Login(AuthM loginM)
        {
            int status = 1;
            string mess = "";
            string action = "";
            tbNguoiDung nguoiDung = new tbNguoiDung();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string currentDomain = Request.Url.Host.ToLower();
                    //tbDonViSuDung donViSuDung = db.tbDonViSuDungs.FirstOrDefault(k => k.TenMien == currentDomain && k.TrangThai == 1) ?? new tbDonViSuDung();
                    tbDonViSuDung donViSuDung = layDonViSuDung();
                    string matKhau_MD5 = Public.Handle.HashToMD5(loginM.MatKhau);
                    nguoiDung = db.tbNguoiDungs.FirstOrDefault(x => x.TenDangNhap == loginM.TenDangNhap
                    && x.MatKhau == matKhau_MD5
                    && x.KichHoat == true
                    && x.TrangThai != 0
                    && x.MaDonViSuDung == donViSuDung.MaDonViSuDung) ?? new tbNguoiDung();
                    if (nguoiDung.IdNguoiDung != Guid.Empty)
                    {
                        if (nguoiDung.YeuCauDoiMatKhau == null || nguoiDung.YeuCauDoiMatKhau == true) // Lần dầu đăng nhập phải đổi mật khẩu
                        {
                            NGUOIDUNG = nguoiDung;
                            status = 1;
                            action = "/Auth/ChangePassword";
                        }
                        else
                        {
                            //string cacheKey = $"Permission_{nguoiDung.IdNguoiDung}";

                            // Cập nhât số lần đăng nhập
                            nguoiDung.SoLanDangNhap = nguoiDung.SoLanDangNhap ?? 0;
                            nguoiDung.SoLanDangNhap += 1;
                            nguoiDung.Online = true;
                            db.SaveChanges();
                            // Lưu thông tin người dùng
                            var per = new Permission
                            {
                                NguoiDung = nguoiDung,
                                ChucVu = db.default_tbChucVu.FirstOrDefault(x => x.IdChucVu == nguoiDung.IdChucVu) ?? new default_tbChucVu(),
                                DonViSuDung = donViSuDung,
                                KieuNguoiDung = db.tbKieuNguoiDungs.FirstOrDefault(x => x.IdKieuNguoiDung == nguoiDung.IdKieuNguoiDung) ?? new tbKieuNguoiDung(),
                                CoCauToChuc = db.tbCoCauToChucs.FirstOrDefault(x => x.IdCoCauToChuc == nguoiDung.IdCoCauToChuc) ?? new tbCoCauToChuc()
                            };
                            Session["Permission"] = per; // Phải set như này thì từ sau mới sử dụng được session
                                                         //_cacheManager.Set(cacheKey, per, _cacheTimeOut); // Không cần session

                            #region Lưu claim
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, nguoiDung.IdNguoiDung.ToString()),
                                new Claim(ClaimTypes.Name, nguoiDung.TenNguoiDung ?? ""),
                                new Claim("MaDonViSuDung", nguoiDung.MaDonViSuDung.ToString()),

                                new Claim("NguoiDung", JsonConvert.SerializeObject(per.NguoiDung)),
                                new Claim("DonViSuDung", JsonConvert.SerializeObject(per.DonViSuDung)),
                                new Claim("ChucVu", JsonConvert.SerializeObject(per.ChucVu)),
                                new Claim("KieuNguoiDung", JsonConvert.SerializeObject(per.KieuNguoiDung)),
                                new Claim("CoCauToChuc", JsonConvert.SerializeObject(per.CoCauToChuc))
                            };

                            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                            var authManager = Request.GetOwinContext().Authentication;

                            authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                            authManager.SignIn(new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddHours(8)
                            }, identity);

                            #endregion

                            #region Gửi mail
                            HttpBrowserCapabilitiesBase browser = HttpContext.Request.Browser;
                            ThongTinThietBi thongTinThietBi = new ThongTinThietBi
                            {
                                TenTrinhDuyet = browser.Browser,
                                PhienBan = browser.Version,
                                UserAgent = HttpContext.Request.UserAgent,
                            };
                            string mail()
                            {
                                var model = new ThongBaoThietBiDangNhapM
                                {
                                    NguoiDung = nguoiDung,
                                    ThongTinThietBi = thongTinThietBi,
                                    DonViSuDung = per.DonViSuDung
                                };
                                // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                                string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/auth-mail.thongbaodangnhap.cshtml", model);
                                // Trả về chuỗi đã được tạo ra từ view
                                return viewAsString;
                            }
                            string tieuDeMail = "[📣 PostPilot] - CẢNH BÁO THIẾT BỊ LẠ ĐĂNG NHẬP❗";
                            string mailBody = mail();

                            if (nguoiDung.ThongTinThietBi_TruyCap != null) // Kiểm tra thiết bị mới hay cũ
                            {
                                ThongTinThietBi thongTinThietBi_DaSuDung = JsonConvert.DeserializeObject<ThongTinThietBi>(nguoiDung.ThongTinThietBi_TruyCap);
                                List<Tuple<string, object, object>> thayDois = Public.Handle.CompareSpecificFields(obj1: thongTinThietBi_DaSuDung, obj2: thongTinThietBi,
                                       fieldsToCompare: new List<string>(),
                                       fieldsToExclude: new List<string> {
                                "QuyenTruyCap", "DuongDanFile", "TrangThai", "NguoiTao", "NguoiSua", "NgayTao", "NgaySua", "MaDonViSuDung"}
                                       );
                                if (thayDois.Count != 0)
                                {
                                    nguoiDung.ThongTinThietBi_TruyCap = JsonConvert.SerializeObject(thongTinThietBi); // Lưu thiết bị mới
                                    Public.Handle.SendEmail(sendTo: nguoiDung.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                                }
                                ;
                            }
                            else
                            {
                                nguoiDung.ThongTinThietBi_TruyCap = JsonConvert.SerializeObject(thongTinThietBi); // Lưu thiết bị mới
                                Public.Handle.SendEmail(sendTo: nguoiDung.Email, subject: tieuDeMail, body: mailBody, isHTML: true, donViSuDung: per.DonViSuDung);
                            }
                            ;
                            #endregion
                            // Gán vào danh sách người dùng đang hoạt động
                            //QuanLyNguoiDung_DangHoatDong.NguoiDung_DangHoatDong.Add(nguoiDung);

                            db.SaveChanges();
                            scope.Commit();

                            status = 1;
                            mess = $"Xin chào {nguoiDung.TenNguoiDung}, ngày hôm nay của bạn thế nào 🥰";
                            action = "/Home/Index";
                        }
                        ;
                    }
                    else
                    {
                        status = 0;
                        mess = "Thông tin đăng nhập chưa đúng, vui lòng kiểm tra lại";
                        action = "/#";
                    }
                }
                catch (Exception e)
                {
                    scope.Rollback();
                }
            }
            return Json(new
            {
                status = status,
                mess = mess,
                nguoiDung = nguoiDung,
                action = action
            });
        }
        public ActionResult Logout()
        {
            Permission per = Session["Permission"] as Permission;
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult Forgot(AuthM loginM)
        {
            string currentDomain = Request.Url.Host.ToLower();
            Permission per = new Permission
            {
                DonViSuDung = layDonViSuDung(),
            };
            int status = 0;
            string action = "";
            string mess = "";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbNguoiDung nguoiDung = db.tbNguoiDungs.FirstOrDefault(x => x.TenDangNhap == loginM.TenDangNhap && x.Email == loginM.Email && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                    if (nguoiDung != null)
                    {
                        mess = "Thông tin cung cấp chưa đúng, vui lòng kiểm tra lại";
                        if (loginM.MaXacThuc == MAXACTHUC)
                        {
                            // Kiểm tra độ bảo mật
                            var conditions = Public.Handle.CheckPassPattern(loginM.MatKhau);
                            // Kiểm tra từng điều kiện
                            foreach (var condition in conditions)
                            {
                                if (!condition.Value.status) return Json(new { status = "warning", mess = condition.Value.error });
                            }
                            ;

                            //if (nguoiDung_NEW.MatKhauMoi != nguoiDung_NEW.MatKhauMoi) return Json(new { mess = "Mật khẩu xác nhận chưa trùng khớp" });

                            string matKhau_MD5 = Public.Handle.HashToMD5(loginM.MatKhau);
                            nguoiDung.MatKhau = matKhau_MD5;

                            status = 1;
                            action = "/Auth/Login";
                            mess = "Đổi mật khẩu thành công";

                            db.SaveChanges();
                            scope.Commit();
                        }
                        ;
                    }
                    ;
                }
                catch (Exception ex)
                {
                    status = 0;
                    mess = ex.Message;
                    scope.Rollback();
                }
                ;
            }
            ;
            return Json(new
            {
                status,
                action,
                mess
            });
        }
        [HttpPost]
        public ActionResult LayMaXacThuc(AuthM loginM)
        {
            string maXacThuc = Public.Handle.RandomString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 6);
            string currentDomain = Request.Url.Host.ToLower();
            Permission per = new Permission
            {
                DonViSuDung = layDonViSuDung(),
            };
            tbNguoiDung nguoiDung = db.tbNguoiDungs.FirstOrDefault(x => x.TenDangNhap == loginM.TenDangNhap && x.Email == loginM.Email && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (nguoiDung != null)
            {
                NGUOIDUNG = nguoiDung;
                #region Gửi mail
                string tieuDeMail = "[📣 PostPilot] - LẤY LẠI THÔNG TIN TÀI KHOẢN 🔑";
                void guiMail()
                {
                    Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
                    string baseUrl = uri.GetLeftPart(UriPartial.Authority);
                    //string duongDanKhaiThac = string.Format("{0}/XacNhanDoiMauKhau?code={1}", baseUrl, nguoiDung.IdNguoiDung);
                    var model = new ForgotM
                    {
                        MaXacNhan = maXacThuc,
                        //DuongDanKhaiThac = duongDanKhaiThac,
                        DonViSuDung = per.DonViSuDung
                    };
                    // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                    string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/auth.forgot-mail.cshtml", model);
                    Public.Handle.SendEmail(sendTo: nguoiDung.Email, subject: tieuDeMail, body: viewAsString, isHTML: true, donViSuDung: per.DonViSuDung);
                }
                ;
                guiMail();
                #endregion
                return Json(new
                {
                    status = 1,
                    mess = "Mã xác thực đã được gửi vào [Email] bạn cung cấp, vui lòng kiểm tra và tiếp tục",
                    maXacThuc = maXacThuc,
                });
            }
            ;
            return Json(new
            {
                status = 0,
                mess = "Thông tin cung cấp chưa đúng, vui lòng kiểm tra lại",
            });
        }
        [HttpPost]
        public ActionResult DoiMatKhau(string matKhauMoi, string matKhauMoi_XacNhan)
        {
            string currentDomain = Request.Url.Host.ToLower();
            Permission per = new Permission
            {
                DonViSuDung = layDonViSuDung(),
            };
            int status = 0;
            string action = "";
            string mess = "";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra độ bảo mật
                    var conditions = Public.Handle.CheckPassPattern(matKhauMoi);
                    // Kiểm tra từng điều kiện
                    foreach (var condition in conditions)
                    {
                        if (!condition.Value.status) return Json(new { mess = condition.Value.error });
                    }
                    ;

                    if (matKhauMoi != matKhauMoi_XacNhan) return Json(new { mess = "Mật khẩu xác nhận chưa trùng khớp" });

                    tbNguoiDung nguoiDung = db.tbNguoiDungs.Find(NGUOIDUNG.IdNguoiDung);
                    if (nguoiDung != null)
                    {
                        string matKhau_MD5 = Public.Handle.HashToMD5(matKhauMoi);
                        nguoiDung.MatKhau = matKhau_MD5;
                        nguoiDung.YeuCauDoiMatKhau = false;

                        status = 1;
                        action = "/Auth/Login";
                        mess = "Đổi mật khẩu thành công";

                        db.SaveChanges();
                        scope.Commit();
                    }
                    ;
                }
                catch (Exception ex)
                {
                    status = 0;
                    mess = ex.Message;
                    scope.Rollback();
                }
                ;
            }
            ;
            return Json(new
            {
                status,
                action,
                mess
            });
        }
        [HttpPost]
        public ActionResult LoginAfterChangePass(AuthM loginM)
        {
            string maXacThuc = Public.Handle.RandomString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 6);
            string currentDomain = Request.Url.Host.ToLower();
            Permission per = new Permission
            {
                DonViSuDung = layDonViSuDung(),
            };
            tbNguoiDung nguoiDung = db.tbNguoiDungs.FirstOrDefault(x => x.TenDangNhap == loginM.TenDangNhap && x.Email == loginM.Email && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (nguoiDung != null)
            {
                #region Gửi mail
                string tieuDeMail = "[📣 PostPilot] - LẤY LẠI THÔNG TIN TÀI KHOẢN 🔑";
                void guiMail()
                {
                    Uri uri = new Uri(HttpContext.Request.Url.AbsoluteUri);
                    string baseUrl = uri.GetLeftPart(UriPartial.Authority);
                    //string duongDanKhaiThac = string.Format("{0}/XacNhanDoiMauKhau?code={1}", baseUrl, nguoiDung.IdNguoiDung);
                    var model = new ForgotM
                    {
                        MaXacNhan = maXacThuc,
                        //DuongDanKhaiThac = duongDanKhaiThac,
                        DonViSuDung = per.DonViSuDung
                    };
                    // Gọi phương thức RenderViewToString() để chuyển đổi view thành chuỗi
                    string viewAsString = Public.Handle.RenderViewToString(this, $"{VIEW_PATH}/auth.forgot-mail.cshtml", model);
                    Public.Handle.SendEmail(sendTo: nguoiDung.Email, subject: tieuDeMail, body: viewAsString, isHTML: true, donViSuDung: per.DonViSuDung);
                }
                ;
                guiMail();
                #endregion
                return Json(new
                {
                    status = 1,
                    mess = "Mã xác thực đã được gửi vào [Email] bạn cung cấp, vui lòng kiểm tra và tiếp tục",
                    maXacThuc = maXacThuc,
                });
            }
            ;
            return Json(new
            {
                status = 0,
                mess = "Thông tin cung cấp chưa đúng, vui lòng kiểm tra lại",
            });
        }
        public tbDonViSuDung layDonViSuDung()
        {
            string currentDomain = Request.Url.Host.ToLower();
            currentDomain = "vietstarter.com"; // Dùng để test
            var donViSuDung = db.Database.SqlQuery<tbDonViSuDung>($@"
            select * from tbDonViSuDung
                where TrangThai = 1
                AND RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(TenMien, 'https://', ''), 'http://', ''), 'www.', ''), '/','')) = '{currentDomain}'
            ").FirstOrDefault() ?? new tbDonViSuDung();
            return donViSuDung;
        }
    }
}