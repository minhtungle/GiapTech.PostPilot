using ClosedXML.Excel;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using StorageLocation.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace StorageLocation.Controllers
{
    public class StorageLocationController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_DirectorySetting/StorageLocation";
        private List<tbViTriLuuTruExtend> EXCEL_VITRIs_UPLOAD
        {
            get
            {
                return Session["EXCEL_VITRIs_UPLOAD"] as List<tbViTriLuuTruExtend> ?? new List<tbViTriLuuTruExtend>();
            }
            set
            {
                Session["EXCEL_VITRIs_UPLOAD"] = value;
            }
        }
        private List<tbViTriLuuTruExtend> EXCEL_VITRIs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_VITRIs_DOWNLOAD"] as List<tbViTriLuuTruExtend> ?? new List<tbViTriLuuTruExtend>();
            }
            set
            {
                Session["EXCEL_VITRIs_DOWNLOAD"] = value;
            }
        }
        private List<tbDonViSuDung_PhongLuuTru> PHONGLUUTRUS
        {
            get
            {
                return Session["PHONGLUUTRUS"] as List<tbDonViSuDung_PhongLuuTru> ?? new List<tbDonViSuDung_PhongLuuTru>();
            }
            set
            {
                Session["PHONGLUUTRUS"] = value;
            }
        }
        private List<tbViTriLuuTru> VITRILUUTRUS
        {
            get
            {
                return Session["VITRILUUTRUS"] as List<tbViTriLuuTru> ?? new List<tbViTriLuuTru>();
            }
            set
            {
                Session["VITRILUUTRUS"] = value;
            }
        }
        private List<tbViTriLuuTru> VITRILUUTRUS_KHONGDAUMUC
        {
            get
            {
                return Session["VITRILUUTRUS_KHONGDAUMUC"] as List<tbViTriLuuTru> ?? new List<tbViTriLuuTru>();
            }
            set
            {
                Session["VITRILUUTRUS_KHONGDAUMUC"] = value;
            }
        }
        private List<Tree<tbViTriLuuTru>> VITRILUUTRUS_TREE
        {
            get
            {
                return Session["VITRILUUTRUS_TREE"] as List<Tree<tbViTriLuuTru>> ?? new List<Tree<tbViTriLuuTru>>();
            }
            set
            {
                Session["VITRILUUTRUS_TREE"] = value;
            }
        }
        private string HTMLVITRIs
        {
            get
            {
                return Session["HTMLVITRIs"] as string ?? string.Empty;
            }
            set
            {
                Session["HTMLVITRIs"] = value;
            }
        }
        #endregion
        public StorageLocationController() { }
        public ActionResult Index()
        {
            #region Phông lưu trữ
            List<tbDonViSuDung_PhongLuuTru> phongLuuTrus = db.tbDonViSuDung_PhongLuuTru.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1).ToList() ?? new List<tbDonViSuDung_PhongLuuTru>();
            #endregion
            PHONGLUUTRUS = phongLuuTrus;
            return View($"{VIEW_PATH}/storagelocation.cshtml");
        }
        [HttpGet]
        public ActionResult getList()
        {
            #region Lấy các danh sách
            #region Tạo cây vị trí
            List<tbViTriLuuTru> viTriLuuTrus = new List<tbViTriLuuTru>();
            List<tbViTriLuuTru> viTriLuuTrus_KhongDauMuc = new List<tbViTriLuuTru>();
            List<Tree<tbViTriLuuTru>> viTriLuuTrus_Tree = get_ViTriLuuTrus_Tree(idViTri: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            xuLy_TenViTriLuuTru(viTris_IN: viTriLuuTrus_Tree, viTris_OUT: viTriLuuTrus);
            xuLy_TenViTriLuuTru(viTris_IN: viTriLuuTrus_Tree, viTris_OUT: viTriLuuTrus_KhongDauMuc, kichHoat: false);
            string HtmlViTris(List<Tree<tbViTriLuuTru>> viTriLuuTrus_IN, string mucLuc)
            {
                string html = "";
                foreach (Tree<tbViTriLuuTru> _viTris in viTriLuuTrus_IN)
                {
                    tbViTriLuuTru viTri = _viTris.root;
                    int capDo = viTri.CapDo ?? 0;
                    int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                    string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, _viTris.SoThuTu);
                    //string tenDanhMuc_NEW = string.Format("{0} {1}", mucLuc_NEW, viTri.TenViTriLuuTru);
                    string tenDanhMuc_NEW = viTri.TenViTriLuuTru;
                    string khoangCach = String.Join("", Enumerable.Repeat("<span class=\"ms-4 me-4\"></span>", capDoCha).ToArray());
                    html +=
                        $"<tr id=\"{viTri.IdViTriLuuTru}\">" +
                        $" <td>" +
                        $"      {khoangCach}<input type=\"checkbox\" class=\"form-check-input ms-3 me-3\" data-id=\"{viTri.IdViTriLuuTru}\" data-idcha=\"{viTri.IdCha}\" data-capdo=\"{capDoCha}\"/> {tenDanhMuc_NEW}" +
                        $"  </td>" +
                        $"  <td class=\"text-center\">" +
                        // Thêm - Cập nhật - Xóa
                        $"      <div class=\"btn-group btn-group-sm\">" +
                        $"          <a href=\"#\" class=\"btn btn-sm btn-success\" title=\"Thêm mới\" onclick=\"sl.viTriLuuTru.displayModal_CRUD('create',{viTri.IdViTriLuuTru}, {viTri.IdViTriLuuTru})\"><i class=\"bi bi-plus-square\"></i></a>" +
                        $"          <a href=\"#\" class=\"btn btn-sm btn-primary\" title=\"Cập nhật\" onclick=\"sl.viTriLuuTru.displayModal_CRUD('update', {viTri.IdViTriLuuTru}, {viTri.IdCha})\"><i class=\"bi bi-pencil-square\"></i></a>" +
                        $"      </div>" +
                        $"      <div class=\"btn-group btn-group-sm dropdown\">" +
                        $"          <button type=\"button\" title=\"Xóa bỏ\" class=\"btn btn-danger dropdown-toggle me-1\" data-bs-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">" +
                        $"              Xóa bỏ" +
                        $"          </button>" +
                        $"          <div class=\"dropdown-menu\">" +
                        $"              <a class=\"dropdown-item\" href=\"#\" onclick=\"sl.viTriLuuTru.displayModal_Delete(false, {viTri.IdViTriLuuTru})\">" +
                        $"                  1. Chỉ xóa vị trí này" +
                        $"              </a>" +
                        $"              <a class=\"dropdown-item\" href=\"#\" onclick=\"sl.viTriLuuTru.displayModal_Delete(true, {viTri.IdViTriLuuTru})\">" +
                        $"                  2. Xóa vị trí này và vị trí con" +
                        $"              </a>" +
                        $"          </div>" +
                        $"      </div>" +
                        $"  </td>" +
                        "</tr>" + HtmlViTris(viTriLuuTrus_IN: _viTris.nodes, mucLuc: mucLuc_NEW);
                }
                return html;
            }
            #endregion
            #endregion
            VITRILUUTRUS = viTriLuuTrus;
            VITRILUUTRUS_KHONGDAUMUC = viTriLuuTrus_KhongDauMuc;
            VITRILUUTRUS_TREE = viTriLuuTrus_Tree;
            HTMLVITRIs = HtmlViTris(viTriLuuTrus_IN: viTriLuuTrus_Tree, mucLuc: "");
            ViewBag.HtmlViTris = HTMLVITRIs;
            return PartialView($"{VIEW_PATH}/storagelocation-getList.cshtml");
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
        #region Xử lý tree
        [HttpPost]
        public Tree<tbViTriLuuTru> get_ViTriLuuTrus_Tree(int idViTri = 0, int maDonViSuDung = 0)
        {
            // Tạo cây cơ cấu
            List<Tree<tbViTriLuuTru>> makeTree(int idCha)
            {
                // Tạo nhánh
                List<Tree<tbViTriLuuTru>> nodes = new List<Tree<tbViTriLuuTru>>();
                // Tìm con
                //List<tbViTriLuuTru> viTris = new List<tbViTriLuuTru>();
                //viTris = db.tbViTriLuuTrus.Where(x => x.MaDonViSuDung == maDonViSuDung && x.TrangThai == 1 && x.IdCha == idCha)
                //    .OrderByDescending(x => x.IdViTriLuuTru).ToList();
                List<tbViTriLuuTru> viTris = db.Database.SqlQuery<tbViTriLuuTru>(
                    $@"select * from tbViTriLuuTru 
                    where MaDonViSuDung = {maDonViSuDung} and TrangThai = 1 and IdCha = {idCha}
                    order by IdViTriLuuTru desc").ToList();
                for (int i = 0; i < viTris.Count; i++)
                {
                    tbViTriLuuTru viTri = viTris[i];
                    // 1 node gồm nhiều con hơn
                    Tree<tbViTriLuuTru> tree = new Tree<tbViTriLuuTru>();
                    tree.SoThuTu = (i + 1);
                    tree.root = viTri;
                    tree.nodes = makeTree(viTri.IdViTriLuuTru);
                    nodes.Add(tree);
                }
                return nodes;
            }
            Tree<tbViTriLuuTru> _tree = new Tree<tbViTriLuuTru>
            {
                nodes = makeTree(idViTri),
            };
            return _tree;
        }
        public void xuLy_TenViTriLuuTru(List<Tree<tbViTriLuuTru>> viTris_IN, List<tbViTriLuuTru> viTris_OUT, string mucLuc = "", string khoangCachDauDong = "", bool kichHoat = true)
        {
            foreach (Tree<tbViTriLuuTru> viTri_IN in viTris_IN)
            {
                string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, viTri_IN.SoThuTu);
                int capDo = viTri_IN.root.CapDo ?? 0;
                int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                string khoangCachDauDong_NEW = String.Join("", Enumerable.Repeat(khoangCachDauDong, capDoCha).ToArray());

                tbViTriLuuTru viTri_OUT = new tbViTriLuuTru
                {
                    IdViTriLuuTru = viTri_IN.root.IdViTriLuuTru,
                    TenViTriLuuTru = kichHoat ? string.Format("{0} {1} {2}", khoangCachDauDong_NEW, mucLuc_NEW, viTri_IN.root.TenViTriLuuTru) : viTri_IN.root.TenViTriLuuTru,
                    CapDo = viTri_IN.root.CapDo,
                    IdCha = viTri_IN.root.IdCha,
                    IdPhongLuuTru = viTri_IN.root.IdPhongLuuTru,
                    GhiChu = viTri_IN.root.GhiChu,
                    TrangThai = viTri_IN.root.TrangThai,
                    MaDonViSuDung = viTri_IN.root.MaDonViSuDung,
                    NgayTao = viTri_IN.root.NgayTao,
                    NguoiTao = viTri_IN.root.NguoiTao,
                    NgaySua = viTri_IN.root.NgaySua,
                    NguoiSua = viTri_IN.root.NguoiSua
                };
                viTris_OUT.Add(viTri_OUT);
                xuLy_TenViTriLuuTru(viTris_OUT: viTris_OUT, viTris_IN: viTri_IN.nodes, mucLuc: mucLuc_NEW, khoangCachDauDong: khoangCachDauDong);
            };
        }
        #endregion
        #region CRUD
        [HttpPost]
        public ActionResult displayModal_CRUD(string loai, int idViTri, int idViTriCha)
        {
            tbViTriLuuTru viTri = new tbViTriLuuTru();
            tbViTriLuuTru viTriCha = new tbViTriLuuTru();
            List<tbDonViSuDung_PhongLuuTru> phongLuuTrus = new List<tbDonViSuDung_PhongLuuTru>();
            phongLuuTrus = db.tbDonViSuDung_PhongLuuTru.Where(k => k.TrangThai != 0 && k.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();

            if (idViTriCha == 0)
            {
                viTriCha.IdViTriLuuTru = idViTriCha;
                viTriCha.CapDo = 0;
            }
            else
            {
                viTriCha = db.tbViTriLuuTrus.Find(idViTriCha);
            }

            if (loai != "create")
            {
                viTri = db.tbViTriLuuTrus.Find(idViTri);
            };

            ViewBag.loai = loai;
            ViewBag.viTri = viTri;
            ViewBag.viTriCha = viTriCha;
            ViewBag.phongLuuTrus = phongLuuTrus;
            return PartialView($"{VIEW_PATH}/storagelocation-crud.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_Delete(bool deleteChilds = false, int idViTri = 0)
        {
            #region Tạo cây vị trí - thay thế
            var _viTriLuuTrus = get_ViTriLuuTrus_Tree(idViTri: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung);
            string HtmlViTris_THAYTHE(List<Tree<tbViTriLuuTru>> viTriLuuTrus, string mucLuc)
            {
                string html = "";
                foreach (Tree<tbViTriLuuTru> _viTri in viTriLuuTrus)
                {
                    tbViTriLuuTru viTri = _viTri.root;
                    int capDo = viTri.CapDo ?? 0;
                    int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                    string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, _viTri.SoThuTu);
                    string tenDanhMuc_NEW = string.Format("{0} {1}", mucLuc_NEW, viTri.TenViTriLuuTru);
                    string khoangCach = String.Join("", Enumerable.Repeat("<span class=\"ms-4 me-4\"></span>", capDoCha).ToArray());
                    if (_viTri.root.IdViTriLuuTru == idViTri)
                    {
                        html +=
                            $"<tr id=\"{viTri.IdViTriLuuTru}\">" +
                            $"  <td class=\"text-decoration-line-through\">" +
                            $"      {khoangCach}<span class=\"ms-4 me-4\"></span>&ensp;{tenDanhMuc_NEW}" + // Không cho chọn checkbox
                            $"  </td>" +
                            "</tr>";
                        if (!deleteChilds) html += HtmlViTris_THAYTHE(viTriLuuTrus: _viTri.nodes, mucLuc: mucLuc_NEW);
                    }
                    else
                    {
                        html +=
                            $"<tr id=\"{viTri.IdViTriLuuTru}\">" +
                            $"  <td>" +
                            $"      {khoangCach}<input type=\"checkbox\" class=\"form-check-input ms-3 me-3 checkRow-vitriluutru-thaythe-getList\" data-id=\"{viTri.IdViTriLuuTru}\" data-idcha=\"{viTri.IdCha}\" data-capdo\"{capDo}\"/> {tenDanhMuc_NEW}" +
                            $"  </td>" +
                            "</tr>" + HtmlViTris_THAYTHE(viTriLuuTrus: _viTri.nodes, mucLuc: mucLuc_NEW);
                    };
                };
                return html;
            };
            #endregion
            ViewBag.HtmlViTris_THAYTHE = HtmlViTris_THAYTHE(viTriLuuTrus: _viTriLuuTrus.nodes, mucLuc: "");
            ViewBag.deleteChilds = deleteChilds;
            ViewBag.idViTri = idViTri;
            return PartialView($"{VIEW_PATH}/storagelocation-delete.cshtml");
        }
        [HttpPost]
        public ActionResult timKiem(string noiDung = "")
        {
            int idViTri = 0;
            #region Tạo cây vị trí - thay thế
            string HtmlViTris_THAYTHE(List<Tree<tbViTriLuuTru>> viTriLuuTrus, string mucLuc)
            {
                string html = "";
                foreach (Tree<tbViTriLuuTru> _viTri in viTriLuuTrus)
                {
                    tbViTriLuuTru viTri = _viTri.root;
                    int capDo = viTri.CapDo ?? 0;
                    int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                    string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, _viTri.SoThuTu);
                    string tenDanhMuc_NEW = string.Format("{0} {1}", mucLuc_NEW, viTri.TenViTriLuuTru);
                    string khoangCach = String.Join("", Enumerable.Repeat("<span class=\"ms-4 me-4\"></span>", capDoCha).ToArray());
                    if (_viTri.root.IdViTriLuuTru == idViTri)
                    {
                        html +=
                            $"<tr id=\"{viTri.IdViTriLuuTru}\">" +
                            $"  <td class=\"text-decoration-line-through\">" +
                            $"      {khoangCach}<span class=\"ms-4 me-4\"></span>&ensp;{tenDanhMuc_NEW}" + // Không cho chọn checkbox
                            $"  </td>" +
                            "</tr>";
                        html += HtmlViTris_THAYTHE(viTriLuuTrus: _viTri.nodes, mucLuc: mucLuc_NEW);
                    };
                };
                return html;
            };
            #endregion
            ViewBag.HtmlViTris = HtmlViTris_THAYTHE(viTriLuuTrus: VITRILUUTRUS_TREE, mucLuc: "");
            return PartialView($"{VIEW_PATH}/storagelocation-getList.cshtml");
        }
        public bool kiemTra_ViTri(tbViTriLuuTru viTri)
        {
            // Kiểm tra có tồn tại bản ghi cùng cha mà trùng tên không
            tbViTriLuuTru viTri_OLD = db.tbViTriLuuTrus.FirstOrDefault(x =>
            x.TenViTriLuuTru == viTri.TenViTriLuuTru && x.IdViTriLuuTru != viTri.IdViTriLuuTru &&
            x.IdCha == viTri.IdCha &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (viTri_OLD == null) return false;
            return true;
        }
        [HttpPost]
        public ActionResult create_ViTri(string str_viTris)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbViTriLuuTru viTri_NEW = JsonConvert.DeserializeObject<tbViTriLuuTru>(str_viTris ?? "");
                    if (viTri_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_ViTri(viTri: viTri_NEW))
                        {
                            status = "warning";
                            mess = "Đã tồn tại bản ghi cùng nhóm";
                        }
                        else
                        {
                            // Tạo kiểu người dùng
                            tbViTriLuuTru viTri = new tbViTriLuuTru
                            {
                                TenViTriLuuTru = viTri_NEW.TenViTriLuuTru,
                                IdPhongLuuTru = viTri_NEW.IdPhongLuuTru,
                                IdCha = viTri_NEW.IdCha,
                                CapDo = viTri_NEW.CapDo + 1,
                                GhiChu = viTri_NEW.GhiChu,

                                TrangThai = 1,
                                NguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbViTriLuuTrus.Add(viTri);
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Vị trí lưu trữ",
                                ThaoTac = "Thêm mới",
                                NoiDungChiTiet = "Thêm mới vị trí lưu trữ",

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
        public ActionResult update_ViTri(string str_viTris)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbViTriLuuTru viTri_NEW = JsonConvert.DeserializeObject<tbViTriLuuTru>(str_viTris ?? "");
                    if (viTri_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_ViTri(viTri: viTri_NEW))
                        {
                            status = "warning";
                            mess = "Đã tồn tại bản ghi cùng nhóm";
                        }
                        else
                        {
                            tbViTriLuuTru viTri_OLD = db.tbViTriLuuTrus.Find(viTri_NEW.IdViTriLuuTru);
                            viTri_OLD.TenViTriLuuTru = viTri_NEW.TenViTriLuuTru;
                            viTri_OLD.IdPhongLuuTru = viTri_NEW.IdPhongLuuTru;
                            viTri_OLD.GhiChu = viTri_NEW.GhiChu;
                            viTri_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                            viTri_OLD.NgaySua = DateTime.Now;
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Vị trí lưu trữ",
                                ThaoTac = "Cập nhật",
                                NoiDungChiTiet = "Cập nhật vị trí lưu trữ",

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
        public JsonResult delete_ViTris(bool deleteChilds = false, int idViTri = 0, int idViTri_THAYTHE = 0)
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (deleteChilds)
                    {
                        void _deleteChilds(int idCha)
                        {
                            List<int> idViTris = new List<int>();
                            idViTris = db.tbViTriLuuTrus.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1 && x.IdCha == idCha).Select(x => x.IdViTriLuuTru).ToList();
                            if (idViTris.Count > 0)
                            {
                                string capNhatSQL = $@"
                                -- Xóa vị trí
                                update tbViTriLuuTru 
                                set 
                                    TrangThai = 0 ,NguoiSua = {per.NguoiDung.IdNguoiDung} ,NgaySua = '{DateTime.Now}' 
                                where 
                                    IdViTriLuuTru in({String.Join(",", idViTris)})

                                -- Cập nhật hồ sơ
                                update tbHoSo 
                                set 
                                    IdViTriLuuTru = {idViTri_THAYTHE}, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' 
                                where 
                                    IdViTriLuuTru in({String.Join(",", idViTris)}) and TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                                ";
                                db.Database.ExecuteSqlCommand(capNhatSQL);
                                foreach (int _idDanhMuc in idViTris) _deleteChilds(_idDanhMuc);
                            };
                        };
                        _deleteChilds(idViTri);
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Vị trí lưu trữ",
                            ThaoTac = "Xóa",
                            NoiDungChiTiet = "Xóa vị trí (chỉ xóa cha) và cập nhật vị trí thay thế cho các hồ sơ đang sử dụng",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                    }
                    else
                    {
                        #region Hàm hỗ trợ
                        string xuLyTenTrung(string tenBanGhi)
                        {
                            // Sử dụng regex để kiểm tra chuỗi có chứa "(a)" với a là một số lớn hơn 1 không
                            string pattern = @"\((\d+)\)$";
                            Regex regex = new Regex(pattern);
                            // Tìm vị trí cuối cùng của chuỗi khớp với mẫu regex
                            Match lastMatch = Regex.Match(tenBanGhi, pattern, RegexOptions.RightToLeft);
                            // Kiểm tra xem có khớp và có phải ở cuối chuỗi không
                            if (lastMatch.Success && lastMatch.Index == (tenBanGhi.Length - lastMatch.Length))
                            {
                                // Lấy giá trị số từ nhóm con trong match
                                string numberStr = lastMatch.Groups[1].Value;
                                // Chuyển đổi sang số nguyên
                                int number = int.Parse(numberStr);
                                // Tăng giá trị lên 1 đơn vị
                                number++;
                                tenBanGhi = tenBanGhi.Remove(lastMatch.Index) + "(" + number + ")";
                            }
                            else tenBanGhi += " (1)";
                            return tenBanGhi;
                        };
                        bool kiemTraTrungTen(tbViTriLuuTru viTri)
                        {
                            // Kiểm tra có tồn tại bản ghi cùng cha mà trùng tên không
                            tbViTriLuuTru viTri_OLD = db.tbViTriLuuTrus.FirstOrDefault(x =>
                            x.TenViTriLuuTru == viTri.TenViTriLuuTru &&
                            x.IdViTriLuuTru != viTri.IdViTriLuuTru && // Không tìm chính nó
                            x.IdViTriLuuTru != idViTri && // Không tìm bản ghi cha đang xóa
                            x.IdCha == viTri.IdCha &&
                            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                            if (viTri_OLD == null) return false;
                            return true;
                        }
                        #endregion

                        // Cập nhật idCha và cấp độ cho các bản ghi con
                        void _updateChilds(tbViTriLuuTru _viTriCha)
                        {
                            /** CHỈ CẦN KIỂM TRA TRÙNG TÊN TẠI CẤP CON ĐẦU TIÊN
                             *|----------------------------------------------------------------------------------|
                             *| B1: Cập nhật cấp độ và thông |B2: Kiểm tra trùng tên   |B3: Đổi tên              |
                             *| tin cơ bản                   |                         |                         |
                             *| ---------------------------------------------------------------------------------|
                             *| Record_0 <delete>            | ########################| ########################|                        
                             *|      Record_0.1 <check_exist>| Record_0.1 <exist>      | Record_0.1 (1) <changed>|
                             *|          Record_0.1.1        |     Record_0.1.1        |     Record_0.1.1        |
                             *|          Record_0.1.2        |     Record_0.1.2        |     Record_0.1.2        |
                             *|      Record_0.2 <check_exist>| Record_0.2 <not_exist>  | Record_0.2              |
                             *|          Record_0.2.1        |     Record_0.2.1        |     Record_0.2.1        |
                             *|      Record_0.3 <check_exist>| Record_0.3 <not_exist>  | Record_0.3              |
                             *| Record_0.1                   | Record_0.1 <exist>      | Record_0.1              |
                             *|      Record_0.1.1            |      Record_0.1.1       |      Record_0.1.1       |
                             *|      Record_0.1.2            |      Record_0.1.2       |      Record_0.1.2       |
                             *|----------------------------------------------------------------------------------|
                             */
                            List<tbViTriLuuTru> viTris = db.tbViTriLuuTrus.Where(
                                x => x.IdCha == _viTriCha.IdViTriLuuTru
                                && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1)
                                .OrderBy(x => x.IdViTriLuuTru).ToList();
                            // B1: Cập nhật cấp độ và thông tin cơ bản
                            foreach (tbViTriLuuTru viTri in viTris)
                            {
                                viTri.IdCha = _viTriCha.IdCha;
                                viTri.CapDo = _viTriCha.CapDo;
                                viTri.NguoiSua = per.NguoiDung.IdNguoiDung;
                                viTri.NgaySua = DateTime.Now;
                                _updateChilds(viTri); // Tiếp tục thực hiện với các bản ghi con
                            };
                            db.SaveChanges();
                            if (_viTriCha.IdViTriLuuTru == idViTri)
                            {
                                // B2: Kiểm tra trùng tên
                                foreach (tbViTriLuuTru viTri in viTris)
                                {
                                    // Thay tới khi nào tên không còn trùng nữa
                                    while (kiemTraTrungTen(viTri: viTri))
                                    {
                                        // B3: Đổi tên
                                        viTri.TenViTriLuuTru = xuLyTenTrung(tenBanGhi: viTri.TenViTriLuuTru);
                                    };
                                };
                            };
                        };
                        tbViTriLuuTru viTriCha = db.tbViTriLuuTrus.Find(idViTri);
                        _updateChilds(_viTriCha: viTriCha);
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Vị trí lưu trữ",
                            ThaoTac = "Xóa",
                            NoiDungChiTiet = "Xóa vị trí (xóa cha và tất cả con) và cập nhật vị trí thay thế cho các hồ sơ đang sử dụng",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                    }
                    // Xóa bản ghi cha
                    db.Database.ExecuteSqlCommand($@"
                    -- Xóa vị trí
                    update tbViTriLuuTru 
                    set 
                        TrangThai = 0 , NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                    where 
                        IdViTriLuuTru in({idViTri})

                    -- Cập nhật hồ sơ
                    update tbHoSo 
                    set 
                        IdViTriLuuTru = {idViTri_THAYTHE}, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' 
                    where 
                        IdViTriLuuTru in({idViTri}) and TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                    ");
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
        #endregion
        #region Excel
        public tbViTriLuuTruExtend kiemTra_Excel_ViTri(tbViTriLuuTruExtend viTri)
        {
            List<string> ketQuas = new List<string>();
            /**
             * Các thông tin cần kiểm tra
             * Vị trí lưu trữ
             * Phông lưu trữ
             */
            if (kiemTra_ViTri(viTri: viTri))
            {
                ketQuas.Add("Đã tồn tại bản ghi cùng nhóm");
                viTri.KiemTraExcel.TrangThai = 0;
            };
            if (viTri.TenViTriLuuTru == "" || viTri.PhongLuuTru.TenPhongLuuTru == "")
            {
                ketQuas.Add("Thiếu thông tin");
                viTri.KiemTraExcel.TrangThai = 0;
            };
            if (viTri.TenViTriLuuTru == viTri.ViTriCha.TenViTriLuuTru)
            {
                ketQuas.Add("Trùng tên vị trí cha");
                viTri.KiemTraExcel.TrangThai = 0;
            };
            viTri.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            if (viTri.KiemTraExcel.KetQua == "")
            {
                viTri.KiemTraExcel.TrangThai = 1;
                viTri.KiemTraExcel.KetQua = "Hợp lệ";
            };
            return viTri;
        }
        [HttpPost]
        public ActionResult upload_Excel_ViTri(HttpPostedFileBase[] files)
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
                        EXCEL_VITRIs_UPLOAD = new List<tbViTriLuuTruExtend>();
                        foreach (var sheet in workSheets)
                        {
                            // Lấy dữ liệu từ sheet hồ sơ
                            if (sheet.Name.Contains("ViTri"))
                            {
                                /**
                                 * Xóa bảng đang có vì nó chiếm vùng dữ liệu nhưng không đầy đủ
                                 * Bảng này chỉ chưa dữ liệu được tạo mặc định trong hàm download
                                 */
                                sheet.Tables.Remove(sheet.Name.Replace(" ", String.Empty));
                                var table = sheet.RangeUsed().AsTable(); // Tạo bảng mới trên vùng dữ liệu đầy đủ
                                foreach (var row in table.DataRange.Rows())
                                {
                                    if (!row.IsEmpty())
                                    {
                                        tbViTriLuuTruExtend viTri = new tbViTriLuuTruExtend();
                                        viTri.TenViTriLuuTru = row.Field("Tên vị trí").GetString();
                                        viTri.GhiChu = row.Field("Ghi chú").GetString();
                                        string tenViTriCha = row.Field("Tên vị trí cha").GetString();
                                        string tenPhongLuuTru = row.Field("Tên phông lưu trữ").GetString();
                                        viTri.PhongLuuTru = PHONGLUUTRUS.FirstOrDefault(x => x.TenPhongLuuTru == tenPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                                        viTri.IdPhongLuuTru = viTri.PhongLuuTru.IdPhongLuuTru;
                                        #region Tìm cha
                                        //viTri.ViTriCha = VITRILUUTRUS.FirstOrDefault(x => x.TenViTriLuuTru == tenViTriCha) ?? new tbViTriLuuTru();
                                        viTri.ViTriCha = new tbViTriLuuTru
                                        {
                                            IdViTriLuuTru = 0,
                                            TenViTriLuuTru = tenViTriCha
                                        };
                                        viTri.IdCha = viTri.ViTriCha.IdViTriLuuTru;

                                        #endregion
                                        EXCEL_VITRIs_UPLOAD.Add(viTri);
                                    };
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
        public ActionResult download_Excel_ViTri()
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Tạo excel
            using (var workBook = new XLWorkbook())
            {
                #region Tạo sheet vị trí
                DataTable tbViTri = new DataTable();
                tbViTri.Columns.Add("Tên vị trí", typeof(string)); // 1
                tbViTri.Columns.Add("Tên vị trí cha", typeof(string)); // 2
                tbViTri.Columns.Add("Tên phông lưu trữ", typeof(string)); // 3
                tbViTri.Columns.Add("Ghi chú", typeof(string)); // 4
                #region Thêm dữ liệu
                foreach (tbViTriLuuTruExtend viTri in EXCEL_VITRIs_DOWNLOAD)
                {
                    tbViTri.Rows.Add(
                       viTri.TenViTriLuuTru,
                       viTri.ViTriCha.TenViTriLuuTru,
                       viTri.PhongLuuTru.TenPhongLuuTru,
                       viTri.GhiChu);
                };
                #endregion
                #endregion
                #region Tạo sheet danh sách
                DataTable tbDanhSach = new DataTable();
                tbDanhSach.Columns.Add("TenViTruLuuTru", typeof(string));
                tbDanhSach.Columns.Add("TenPhongLuuTru", typeof(string));
                #region Thêm dữ liệu
                int VITRILUUTRUS_KHONGDAUMUC_Count = VITRILUUTRUS_KHONGDAUMUC.Count;
                int PHONGLUUTRUS_Count = PHONGLUUTRUS.Count;
                for (int i = 0; i < VITRILUUTRUS_KHONGDAUMUC_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][0] = VITRILUUTRUS_KHONGDAUMUC[i].TenViTriLuuTru;
                };
                for (int i = 0; i < PHONGLUUTRUS_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][1] = PHONGLUUTRUS[i].TenPhongLuuTru;
                };
                #endregion
                #endregion
                #region Tạo file excel
                workBook.Worksheets.Add(tbViTri, "ViTri");
                workBook.Worksheets.Add(tbDanhSach, "DanhSach");
                tbViTri.TableName = ""; tbDanhSach.TableName = "";
                for (int i = 1; i <= tbViTri.Rows.Count + 1; i++)
                {
                    // List
                    //workBook.Worksheets.First().Cell(i, 2).CreateDataValidation().List($"=OFFSET(DanhSach!$A$2,0,0,COUNTA(DanhSach!$A:$A),1)");
                    workBook.Worksheets.First().Cell(i, 3).CreateDataValidation().List($"=OFFSET(DanhSach!$B$2,0,0,COUNTA(DanhSach!$B:$B),1)");
                }
                #endregion
                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                workBook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode("VITRI.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/StorageLocation/Index");
        }
        public ActionResult save_Excel_ViTri()
        {
            string status = "";
            string mess = "";
            List<tbViTriLuuTruExtend> viTri_HopLes = new List<tbViTriLuuTruExtend>();
            List<tbViTriLuuTruExtend> viTri_KhongHopLes = new List<tbViTriLuuTruExtend>();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (EXCEL_VITRIs_DOWNLOAD.Count == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        #region Kiểm tra excel
                        foreach (tbViTriLuuTruExtend viTri_NEW in EXCEL_VITRIs_DOWNLOAD)
                        {
                            tbViTriLuuTruExtend viTri_KhongHopLe = kiemTra_Excel_ViTri(viTri_NEW);
                            if (viTri_KhongHopLe.KiemTraExcel.TrangThai == 0)
                            {
                                viTri_KhongHopLes.Add(viTri_KhongHopLe);
                            }
                            else
                            {
                                // Kiểm tra tên biểu mẫu này đã được thêm ở bản ghi trước đó chưa
                                if (viTri_HopLes.Any(x => x.TenViTriLuuTru == viTri_NEW.TenViTriLuuTru))
                                {
                                    viTri_NEW.KiemTraExcel.TrangThai = 0;
                                    viTri_NEW.KiemTraExcel.KetQua = "Trùng tên vị trí";
                                    viTri_KhongHopLes.Add(viTri_NEW);
                                }
                                else
                                {
                                    viTri_HopLes.Add(viTri_NEW);
                                };
                            };
                        };
                        #endregion
                        if (viTri_KhongHopLes.Count == 0)
                        { // Thêm bản ghi thành công và không tồn tại bản ghi không hợp lệ
                            #region Sắp xếp lại danh sách theo nhóm cha con để thêm theo thứ tự
                            /**
                             * LOGIC:
                             * (vòng lặp 1) Duyệt danh sách hợp lệ.
                             * =>   | B1: (vòng lặp 2) Tìm cha của phần tử này.
                             *          | B1: Đệ quy (2) để tìm ra cha lớn nhất của phẩn tử đó và lưu vào csdl.
                             *          | B2: Tìm con của phần tử cha.
                                        | B3: (vòng lặp 3) Duyệt danh sách con.
                             *          => | B1: Lưu vào csdl.
                             *             | B2: Xóa con trong danh sách hợp lệ.
                             *             | B3: Đệ quy (3) và lưu vào csdl (Lưu tất cả con của từng phần tử).
                             *      | B2: Xóa cha trong danh sách hợp lệ.
                             * (Tiếp tục đến hết danh sách hợp lệ).
                             */
                            List<tbViTriLuuTruExtend> viTris_DaThem = new List<tbViTriLuuTruExtend>();
                            void timCon(tbViTriLuuTruExtend viTri_Cha)
                            {
                                // Tìm con của cha này để lưu
                                List<tbViTriLuuTruExtend> viTris_Con = viTri_HopLes.Where(x => x.ViTriCha.TenViTriLuuTru == viTri_Cha.TenViTriLuuTru).ToList() ?? new List<tbViTriLuuTruExtend>();
                                foreach (tbViTriLuuTruExtend viTri_Con in viTris_Con)
                                {
                                    viTri_Con.IdCha = viTri_Cha.IdViTriLuuTru;
                                    viTri_Con.CapDo = viTri_Cha.CapDo + 1;
                                    viTri_Con.TrangThai = 1;
                                    viTri_Con.NguoiTao = per.NguoiDung.IdNguoiDung;
                                    viTri_Con.NgayTao = DateTime.Now;
                                    viTri_Con.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;

                                    tbViTriLuuTru viTri_NEW = new tbViTriLuuTru
                                    {
                                        TenViTriLuuTru = viTri_Con.TenViTriLuuTru,
                                        IdPhongLuuTru = viTri_Con.IdPhongLuuTru,

                                        IdCha = viTri_Con.IdCha,
                                        CapDo = viTri_Con.CapDo,
                                        TrangThai = viTri_Con.TrangThai,
                                        NguoiTao = viTri_Con.NguoiTao,
                                        NgayTao = viTri_Con.NgayTao,
                                        MaDonViSuDung = viTri_Con.MaDonViSuDung,
                                    };
                                    db.tbViTriLuuTrus.Add(viTri_NEW);
                                    db.SaveChanges();
                                    viTri_Con.IdViTriLuuTru = viTri_NEW.IdViTriLuuTru;
                                    viTris_DaThem.Add(viTri_Con); // Xóa ở danh sách
                                    // Tìm con trong danh sách
                                    timCon(viTri_Cha: viTri_Con);
                                };
                            };
                            void timCha(tbViTriLuuTruExtend viTri_Con)
                            {
                                // Tìm cha trong danh sách
                                tbViTriLuuTruExtend viTri_Cha_DS = viTri_HopLes.FirstOrDefault(x => x.TenViTriLuuTru == viTri_Con.ViTriCha.TenViTriLuuTru);
                                if (viTri_Cha_DS == null)
                                {
                                    int idCha = 0;
                                    int capDo = 1;
                                    // Tìm cha trong csdl
                                    tbViTriLuuTru viTri_Cha_CSDL = db.tbViTriLuuTrus.FirstOrDefault(x => x.TenViTriLuuTru == viTri_Con.ViTriCha.TenViTriLuuTru);
                                    if (viTri_Cha_CSDL != null)
                                    {
                                        capDo = viTri_Cha_CSDL.CapDo.Value + 1;
                                        idCha = viTri_Cha_CSDL.IdViTriLuuTru;
                                    };
                                    viTri_Con.IdCha = idCha;
                                    viTri_Con.CapDo = capDo;
                                    viTri_Con.TrangThai = 1;
                                    viTri_Con.NguoiTao = per.NguoiDung.IdNguoiDung;
                                    viTri_Con.NgayTao = DateTime.Now;
                                    viTri_Con.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;
                                    tbViTriLuuTru viTri_NEW = new tbViTriLuuTru
                                    {
                                        TenViTriLuuTru = viTri_Con.TenViTriLuuTru,
                                        IdPhongLuuTru = viTri_Con.IdPhongLuuTru,

                                        IdCha = viTri_Con.IdCha,
                                        CapDo = viTri_Con.CapDo,
                                        TrangThai = viTri_Con.TrangThai,
                                        NguoiTao = viTri_Con.NguoiTao,
                                        NgayTao = viTri_Con.NgayTao,
                                        MaDonViSuDung = viTri_Con.MaDonViSuDung,
                                    };
                                    db.tbViTriLuuTrus.Add(viTri_NEW);
                                    db.SaveChanges();
                                    viTri_Con.IdViTriLuuTru = viTri_NEW.IdViTriLuuTru;
                                    viTris_DaThem.Add(viTri_Con); // Xóa ở danh sách
                                    // Tìm con trong danh sách
                                    timCon(viTri_Cha: viTri_Con);
                                }
                                else
                                {
                                    timCha(viTri_Con: viTri_Cha_DS);
                                };
                            };
                            foreach (tbViTriLuuTruExtend viTri in viTri_HopLes)
                            {
                                if (!viTris_DaThem.Any(x => x.TenViTriLuuTru == viTri.TenViTriLuuTru))
                                {
                                    timCha(viTri_Con: viTri);
                                };
                            };
                            #endregion
                            #region Thông báo
                            status = "success";
                            mess = "Thêm mới bản ghi thành công";
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Vị trí lưu trữ",
                                ThaoTac = "Thêm mới",
                                NoiDungChiTiet = "Thêm mới bằng tệp",

                                NgayTao = DateTime.Now,
                                IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            });
                            db.SaveChanges();
                            scope.Commit();
                            #endregion
                        }
                        else
                        { // Khi thêm thành công, thay thế EXCEL_VITRIs_UPLOAD bằng viTri_KhongHopLes
                            status = "error-1";
                            mess = "Thêm mới bản ghi không thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                            // Trả lại danh sách bản ghi không hợp lệ
                            EXCEL_VITRIs_UPLOAD = new List<tbViTriLuuTruExtend>();
                            EXCEL_VITRIs_UPLOAD.AddRange(viTri_KhongHopLes);
                            EXCEL_VITRIs_UPLOAD.AddRange(viTri_HopLes);
                        };
                    };
                }
                catch (Exception ex)
                {
                    status = "error-0";
                    mess = ex.Message;
                    scope.Rollback();
                };
            };
            return Json(new
            {
                viTri_KhongHopLes,
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public void get_ViTris_download()
        {
            string loaiTaiXuong = Request.Form["loaiTaiXuong"];
            string str_viTris = Request.Form["str_viTris"];
            EXCEL_VITRIs_DOWNLOAD = new List<tbViTriLuuTruExtend>();
            EXCEL_VITRIs_DOWNLOAD.Add(new tbViTriLuuTruExtend
            {
                TenViTriLuuTru = "Nhập thông tin ...",
                ViTriCha = VITRILUUTRUS_KHONGDAUMUC.FirstOrDefault() ?? new tbViTriLuuTru(),
                PhongLuuTru = PHONGLUUTRUS.FirstOrDefault() ?? new tbDonViSuDung_PhongLuuTru(),
            });
            if (loaiTaiXuong == "data")
            {
                EXCEL_VITRIs_DOWNLOAD = JsonConvert.DeserializeObject<List<tbViTriLuuTruExtend>>(str_viTris ?? "") ?? new List<tbViTriLuuTruExtend>();
            };
        }
        [HttpPost]
        public ActionResult getList_Excel_ViTri(string loai)
        {
            if (loai == "reload") EXCEL_VITRIs_UPLOAD = new List<tbViTriLuuTruExtend>();
            return PartialView($"{VIEW_PATH}/storagelocation-excel.vitri/excel.vitri-getList.cshtml");
        }
        #endregion
    }
}