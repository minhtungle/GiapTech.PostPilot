using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace UserType.Controllers
{
    public class UserTypeController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemSetting/UserType";
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
        #endregion
        public ActionResult Index()
        {
            CHUCNANGs = db.default_tbChucNang.OrderBy(x => x.TrangThai != 0).OrderBy(x => x.SoThuTu).ToList();
            return View($"{VIEW_PATH}/usertype.cshtml");
        }
        [HttpGet]
        public JsonResult getList()
        {
            List<tbKieuNguoiDung> kieuNguoiDungs = get_KieuNguoiDungs(loai: "all");
            return Json(new
            {
                data = kieuNguoiDungs
            }, JsonRequestBehavior.AllowGet);
        }
        #region CRUD
        [HttpPost]
        public ActionResult displayModal_CRUD(string loai, Guid idKieuNguoiDung)
        {
            tbKieuNguoiDung kieuNguoiDung = new tbKieuNguoiDung();
            // Tìm cấp độ thấp nhất của cây
            string sql_max = $@"select isnull(max(CapDo),0) from default_tbChucNang where TrangThai = 1";
            int gioiHan = db.Database.SqlQuery<int>(sql_max).FirstOrDefault() + 1;
            if (loai != "create" && idKieuNguoiDung != Guid.Empty)
            {
                kieuNguoiDung = db.tbKieuNguoiDungs.Find(idKieuNguoiDung);
            };

            string taoTree(Guid idCha, int capDo)
            {
                string html = default(string);
                string khoangCach = String.Join("", Enumerable.Repeat("<td></td>", (int)capDo).ToArray());
                List<default_tbChucNang> _chucNangs = CHUCNANGs.Where(x => x.IdCha == idCha && x.TrangThai == 1).ToList();
                foreach (default_tbChucNang _chucNang in _chucNangs)
                {
                    #region Tìm các thao tác của chức năng
                    List<default_tbChucNang_ThaoTac> thaoTacs = db.default_tbChucNang_ThaoTac.Where(x => x.TrangThai == 1 && x.IdChucNang == _chucNang.IdChucNang).ToList();
                    #endregion
                    html += "<tr>" +
                        khoangCach +
                    $"<td class=\"text-center w-5\">" +
                    $"  <input type=\"checkbox\" class=\"form-check-input checkbox-chucnang\" data-machucnang=\"{_chucNang.MaChucNang}\" data-id=\"{_chucNang.IdChucNang}\" data-idcha=\"{_chucNang.IdCha}\" data-capdo=\"{_chucNang.CapDo}\"/></td>" +
                    $"<td colspan=\"{gioiHan - capDo}\">{_chucNang.TenChucNang}</td>" + // colspan theo cấp độ
                    "<td>";
                    foreach (default_tbChucNang_ThaoTac thaoTac in thaoTacs)
                    {
                        html += $"<div class=\"form-group\">" +
                            $"<input type=\"checkbox\" class=\"form-check-input checkbox-thaotac\" data-mathaotac=\"{thaoTac.MaThaoTac}\" data-idthaotac=\"{thaoTac.IdThaoTac}\"/>" +
                            $"<span>&ensp;{thaoTac.TenThaoTac}</span>" +
                            $"</div>";
                    };
                    html += "</td>" +
                        "</tr>" + taoTree((Guid)_chucNang.IdChucNang, capDo + 1);
                };
                return html;
            };
            ViewBag.HtmlChucNangs = taoTree(idCha: Guid.Empty, 0);
            ViewBag.gioiHan = gioiHan;
            ViewBag.loai = loai;
            ViewBag.kieuNguoiDung = kieuNguoiDung;
            return PartialView($"{VIEW_PATH}/usertype-crud.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_Delete()
        {
            List<tbKieuNguoiDung> kieuNguoiDungs = get_KieuNguoiDungs(loai: "all");
            string str_idKieuNguoiDungs_XOA = Request.Form["str_idKieuNguoiDungs_XOA"];
            ViewBag.kieuNguoiDungs = kieuNguoiDungs;
            ViewBag.idKieuNguoiDungs_XOA = JsonConvert.DeserializeObject<List<Guid>>(str_idKieuNguoiDungs_XOA);
            return PartialView($"{VIEW_PATH}/usertype-delete.cshtml");
        }
        public List<tbKieuNguoiDung> get_KieuNguoiDungs(string loai, string str_idKieuNguoiDungs = "")
        {
            List<tbKieuNguoiDung> kieuNguoiDungs = new List<tbKieuNguoiDung>();
            if (loai == "all")
            {
                kieuNguoiDungs = db.tbKieuNguoiDungs.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai != 0)
                    .OrderByDescending(x => x.NgayTao).ToList();
            }
            else
            {
                if (str_idKieuNguoiDungs != "")
                {
                    string getSql = $@"select * from tbKieuNguoiDung 
                    where MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}' and TrangThai != 0 and IdKieuNguoiDung in ({str_idKieuNguoiDungs}) 
                    order by NgayTao desc";
                    kieuNguoiDungs = db.Database.SqlQuery<tbKieuNguoiDung>(getSql).ToList();
                }
            }
            return kieuNguoiDungs;
        }
        public bool kiemTra_KieuNguoiDung(tbKieuNguoiDung kieuNguoiDung)
        {
            // Kiểm tra còn hồ sơ khác có trùng mã không
            tbKieuNguoiDung kieuNguoiDung_OLD = db.tbKieuNguoiDungs.FirstOrDefault(x => x.TenKieuNguoiDung == kieuNguoiDung.TenKieuNguoiDung && x.IdKieuNguoiDung != kieuNguoiDung.IdKieuNguoiDung && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (kieuNguoiDung_OLD == null) return false;
            return true;
        }
        [HttpPost]
        public ActionResult create_KieuNguoiDung(string str_kieuNguoiDung)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbKieuNguoiDung kieuNguoiDung_NEW = JsonConvert.DeserializeObject<tbKieuNguoiDung>(str_kieuNguoiDung ?? "");
                    if (kieuNguoiDung_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_KieuNguoiDung(kieuNguoiDung: kieuNguoiDung_NEW))
                        {
                            status = "datontai";
                            mess = "Kiểu người dùng đã tồn tại";
                        }
                        else
                        {
                            // Tạo kiểu người dùng
                            tbKieuNguoiDung kieuNguoiDung = new tbKieuNguoiDung
                            {
                                IdKieuNguoiDung = Guid.NewGuid(),
                                TenKieuNguoiDung = kieuNguoiDung_NEW.TenKieuNguoiDung,
                                IdChucNang = kieuNguoiDung_NEW.IdChucNang,
                                GhiChu = kieuNguoiDung_NEW.GhiChu,
                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbKieuNguoiDungs.Add(kieuNguoiDung);
                           
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
        public ActionResult update_KieuNguoiDung(string str_kieuNguoiDung)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbKieuNguoiDung kieuNguoiDung_NEW = JsonConvert.DeserializeObject<tbKieuNguoiDung>(str_kieuNguoiDung ?? "");
                    if (kieuNguoiDung_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_KieuNguoiDung(kieuNguoiDung: kieuNguoiDung_NEW))
                        {
                            status = "datontai";
                            mess = "Kiểu người dùng đã tồn tại";
                        }
                        else
                        {
                            // Kiểu người dùng
                            tbKieuNguoiDung kieuNguoiDung_OLD = db.tbKieuNguoiDungs.Find(kieuNguoiDung_NEW.IdKieuNguoiDung);
                            kieuNguoiDung_OLD.TenKieuNguoiDung = kieuNguoiDung_NEW.TenKieuNguoiDung;
                            kieuNguoiDung_OLD.IdChucNang = kieuNguoiDung_NEW.IdChucNang;
                            kieuNguoiDung_OLD.GhiChu = kieuNguoiDung_NEW.GhiChu;
                            kieuNguoiDung_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                            kieuNguoiDung_OLD.NgaySua = DateTime.Now;
                            if (kieuNguoiDung_OLD.IdKieuNguoiDung == per.NguoiDung.IdKieuNguoiDung)
                            {
                                status = "logout";
                                mess = "[Kiểu người dùng đang sử dụng]";
                            }
                           
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
        public JsonResult delete_KieuNguoiDungs()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    List<Guid> idKieuNguoiDungs_XOA = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["str_idKieuNguoiDungs_XOA"]);
                    Guid idKieuNguoiDung_THAYTHE = Guid.Parse(Request.Form["idKieuNguoiDung_THAYTHE"]);
                    if (idKieuNguoiDungs_XOA.Count > 0)
                    {
                        string str_idKieuNguoiDungs_XOA = string.Join(",", idKieuNguoiDungs_XOA.Select(x => string.Format("'{0}'", x)));
                        string capNhatSQL = $@"
                        -- Xóa kiểu người dùng
                        update tbKieuNguoiDung 
                        set 
                            TrangThai = 0 , IdNguoiSua = '{per.NguoiDung.IdNguoiDung}' , NgaySua = '{DateTime.Now}' 
                        where 
                            MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}' and IdKieuNguoiDung in ({str_idKieuNguoiDungs_XOA})
                        -- Cập nhật người dùng
                        update tbNguoiDung
                        set
                            IdKieuNguoiDung = '{idKieuNguoiDung_THAYTHE}', IdNguoiSua = '{per.NguoiDung.IdNguoiDung}' , NgaySua = '{DateTime.Now}'
                        where
                            IdKieuNguoiDung in ({str_idKieuNguoiDungs_XOA}) and TrangThai <> 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                        ";
                        db.Database.ExecuteSqlCommand(capNhatSQL);
                        //db.Database.ExecuteSqlCommand(capNhatSQL,
                        //    new SqlParameter("@str_idKieuNguoiDungs_XOA", string.Format("{0}", str_idKieuNguoiDungs_XOA)),
                        //    new SqlParameter("@idKieuNguoiDung_THAYTHE", string.Format("{0}", idKieuNguoiDung_THAYTHE))
                        //    );
                        if (idKieuNguoiDungs_XOA.Contains(per.NguoiDung.IdKieuNguoiDung.Value))
                        {
                            status = "logout";
                            mess = "[Kiểu người dùng đang sử dụng]";
                        }
                        
                        db.SaveChanges();
                        scope.Commit();
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
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}