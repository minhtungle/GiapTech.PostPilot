using ClosedXML.Excel;
using DocumentDirectory.Models;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DocumentDirectory.Controllers
{
    public class DocumentDirectoryController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_DirectorySetting/DocumentDirectory";
        private List<tbDanhMucHoSoExtend> EXCEL_DANHMUCs_UPLOAD
        {
            get
            {
                return Session["EXCEL_DANHMUCs_UPLOAD"] as List<tbDanhMucHoSoExtend> ?? new List<tbDanhMucHoSoExtend>();
            }
            set
            {
                Session["EXCEL_DANHMUCs_UPLOAD"] = value;
            }
        }
        private List<tbDanhMucHoSoExtend> EXCEL_DANHMUCs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_DANHMUCs_DOWNLOAD"] as List<tbDanhMucHoSoExtend> ?? new List<tbDanhMucHoSoExtend>();
            }
            set
            {
                Session["EXCEL_DANHMUCs_DOWNLOAD"] = value;
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
        private List<tbDanhMucHoSo> DANHMUCHOSOS
        {
            get
            {
                return Session["DANHMUCHOSOS"] as List<tbDanhMucHoSo> ?? new List<tbDanhMucHoSo>();
            }
            set
            {
                Session["DANHMUCHOSOS"] = value;
            }
        }
        private List<tbDanhMucHoSo> DANHMUCHOSOS_KHONGDAUMUC
        {
            get
            {
                return Session["DANHMUCHOSOS_KHONGDAUMUC"] as List<tbDanhMucHoSo> ?? new List<tbDanhMucHoSo>();
            }
            set
            {
                Session["DANHMUCHOSOS_KHONGDAUMUC"] = value;
            }
        }
        private List<Tree<tbDanhMucHoSo>> DANHMUCHOSOS_TREE
        {
            get
            {
                return Session["DANHMUCHOSOS_TREE"] as List<Tree<tbDanhMucHoSo>> ?? new List<Tree<tbDanhMucHoSo>>();
            }
            set
            {
                Session["DANHMUCHOSOS_TREE"] = value;
            }
        }
        private string HTMLDANHMUCs
        {
            get
            {
                return Session["HTMLDANHMUCs"] as string ?? string.Empty;
            }
            set
            {
                Session["HTMLDANHMUCs"] = value;
            }
        }
        #endregion
        public DocumentDirectoryController() { }
        public ActionResult Index()
        {
            #region Phông lưu trữ
            List<tbDonViSuDung_PhongLuuTru> phongLuuTrus = db.tbDonViSuDung_PhongLuuTru.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1).ToList() ?? new List<tbDonViSuDung_PhongLuuTru>();
            #endregion
            PHONGLUUTRUS = phongLuuTrus;
            return View($"{VIEW_PATH}/documentdirectory.cshtml");
        }
        [HttpGet]
        public ActionResult getList()
        {
            #region Lấy các danh sách
            #region Tạo cây danh mục
            List<tbDanhMucHoSo> danhMucHoSos = new List<tbDanhMucHoSo>();
            List<tbDanhMucHoSo> danhMucHoSos_KhongDauMuc = new List<tbDanhMucHoSo>();
            List<Tree<tbDanhMucHoSo>> danhMucHoSos_Tree = get_DanhMucHoSos_Tree(idDanhMuc: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            xuLy_TenDanhMucHoSo(danhMucs_IN: danhMucHoSos_Tree, danhMucs_OUT: danhMucHoSos);
            xuLy_TenDanhMucHoSo(danhMucs_IN: danhMucHoSos_Tree, danhMucs_OUT: danhMucHoSos_KhongDauMuc, kichHoat: false);
            string HtmlDanhMucs(List<Tree<tbDanhMucHoSo>> danhMucHoSos_IN, string mucLuc)
            {
                string html = "";
                foreach (Tree<tbDanhMucHoSo> _danhMucs in danhMucHoSos_IN)
                {
                    tbDanhMucHoSo danhMuc = _danhMucs.root;
                    int capDo = danhMuc.CapDo ?? 0;
                    int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                    string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, _danhMucs.SoThuTu);
                    string tenDanhMuc_NEW = string.Format("{0} {1}", mucLuc_NEW, danhMuc.TenDanhMucHoSo);
                    string khoangCach = String.Join("", Enumerable.Repeat("<span class=\"ms-4 me-4\"></span>", capDoCha).ToArray());
                    html +=
                        $"<tr id=\"{danhMuc.IdDanhMucHoSo}\">" +
                         $" <td>" +
                        $"      {khoangCach}<input type=\"checkbox\" class=\"form-check-input ms-3 me-3\" data-id=\"{danhMuc.IdDanhMucHoSo}\" data-idcha=\"{danhMuc.IdCha}\" data-capdo=\"{capDoCha}\"/> {tenDanhMuc_NEW}" +
                        $"  </td>" +
                        $"  <td class=\"text-center\">" +
                        // Thêm - Cập nhật - Xóa
                        $"      <div class=\"btn-group btn-group-sm\">" +
                        $"          <a href=\"#\" class=\"btn btn-sm btn-success\" title=\"Thêm mới\" onclick=\"dd.danhMucHoSo.displayModal_CRUD('create',{danhMuc.IdDanhMucHoSo}, {danhMuc.IdDanhMucHoSo})\"><i class=\"bi bi-plus-square\"></i></a>" +
                        $"          <a href=\"#\" class=\"btn btn-sm btn-primary\" title=\"Cập nhật\" onclick=\"dd.danhMucHoSo.displayModal_CRUD('update', {danhMuc.IdDanhMucHoSo}, {danhMuc.IdCha})\"><i class=\"bi bi-pencil-square\"></i></a>" +
                        $"      </div>" +
                        $"      <div class=\"btn-group btn-group-sm dropdown\">" +
                        $"          <button type=\"button\" title=\"Xóa bỏ\" class=\"btn btn-danger dropdown-toggle me-1\" data-bs-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">" +
                        $"              Xóa bỏ" +
                        $"          </button>" +
                        $"          <div class=\"dropdown-menu\">" +
                        $"              <a class=\"dropdown-item\" href=\"#\" onclick=\"dd.danhMucHoSo.displayModal_Delete(false, {danhMuc.IdDanhMucHoSo})\">" +
                        $"                  1. Chỉ xóa danh mục này" +
                        $"              </a>" +
                        $"              <a class=\"dropdown-item\" href=\"#\" onclick=\"dd.danhMucHoSo.displayModal_Delete(true, {danhMuc.IdDanhMucHoSo})\">" +
                        $"                  2. Xóa danh mục này và danh mục con" +
                        $"              </a>" +
                        $"          </div>" +
                        $"      </div>" +
                        $"  </td>" +
                        "</tr>" + HtmlDanhMucs(danhMucHoSos_IN: _danhMucs.nodes, mucLuc: mucLuc_NEW);
                }
                return html;
            };
            #endregion
            #endregion
            DANHMUCHOSOS = danhMucHoSos;
            DANHMUCHOSOS_KHONGDAUMUC = danhMucHoSos_KhongDauMuc;
            DANHMUCHOSOS_TREE = danhMucHoSos_Tree;
            HTMLDANHMUCs = HtmlDanhMucs(danhMucHoSos_IN: danhMucHoSos_Tree, mucLuc: "");
            ViewBag.HtmlDanhMucs = HTMLDANHMUCs;
            return PartialView($"{VIEW_PATH}/documentdirectory-getList.cshtml");
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
        public Tree<tbDanhMucHoSo> get_DanhMucHoSos_Tree(int idDanhMuc = 0, int maDonViSuDung = 0)
        {
            List<Tree<tbDanhMucHoSo>> makeTree(int idCha)
            {
                // Tạo nhánh
                List<Tree<tbDanhMucHoSo>> nodes = new List<Tree<tbDanhMucHoSo>>();
                // Tìm con
                //List<tbDanhMucHoSo> danhMucs = new List<tbDanhMucHoSo>();
                //danhMucs = db.tbDanhMucHoSoes.Where(x => x.MaDonViSuDung == maDonViSuDung && x.TrangThai == 1 && x.IdCha == idCha)
                //    .OrderByDescending(x => x.IdDanhMucHoSo).ToList();
                List<tbDanhMucHoSo> danhMucs = db.Database.SqlQuery<tbDanhMucHoSo>(
                    $@"select * from tbDanhMucHoSo 
                    where MaDonViSuDung = {maDonViSuDung} and TrangThai = 1 and IdCha = {idCha}
                    order by IdDanhMucHoSo desc").ToList();
                for (int i = 0; i < danhMucs.Count; i++)
                {
                    tbDanhMucHoSo danhMuc = danhMucs[i];
                    // 1 node gồm nhiều con hơn
                    Tree<tbDanhMucHoSo> tree = new Tree<tbDanhMucHoSo>();
                    tree.SoThuTu = (i + 1);
                    tree.root = danhMuc;
                    tree.nodes = makeTree(danhMuc.IdDanhMucHoSo);
                    nodes.Add(tree);
                }
                return nodes;
            }
            Tree<tbDanhMucHoSo> _tree = new Tree<tbDanhMucHoSo>
            {
                nodes = makeTree(idDanhMuc),
            };
            return _tree;
        }
        public void xuLy_TenDanhMucHoSo(List<Tree<tbDanhMucHoSo>> danhMucs_IN, List<tbDanhMucHoSo> danhMucs_OUT, string mucLuc = "", string khoangCachDauDong = "", bool kichHoat = true)
        {
            foreach (Tree<tbDanhMucHoSo> danhMuc_IN in danhMucs_IN)
            {
                string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, danhMuc_IN.SoThuTu);
                int capDo = danhMuc_IN.root.CapDo ?? 0;
                int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                string khoangCachDauDong_NEW = String.Join("", Enumerable.Repeat(khoangCachDauDong, capDoCha).ToArray());

                tbDanhMucHoSo danhMuc_OUT = new tbDanhMucHoSo
                {
                    IdDanhMucHoSo = danhMuc_IN.root.IdDanhMucHoSo,
                    TenDanhMucHoSo = kichHoat ? string.Format("{0} {1} {2}", khoangCachDauDong_NEW, mucLuc_NEW, danhMuc_IN.root.TenDanhMucHoSo) : danhMuc_IN.root.TenDanhMucHoSo,
                    CapDo = danhMuc_IN.root.CapDo,
                    IdCha = danhMuc_IN.root.IdCha,
                    IdPhongLuuTru = danhMuc_IN.root.IdPhongLuuTru,
                    GhiChu = danhMuc_IN.root.GhiChu,
                    TrangThai = danhMuc_IN.root.TrangThai,
                    MaDonViSuDung = danhMuc_IN.root.MaDonViSuDung,
                    NgayTao = danhMuc_IN.root.NgayTao,
                    NguoiTao = danhMuc_IN.root.NguoiTao,
                    NgaySua = danhMuc_IN.root.NgaySua,
                    NguoiSua = danhMuc_IN.root.NguoiSua
                };
                danhMucs_OUT.Add(danhMuc_OUT);
                xuLy_TenDanhMucHoSo(danhMucs_OUT: danhMucs_OUT, danhMucs_IN: danhMuc_IN.nodes, mucLuc: mucLuc_NEW, khoangCachDauDong: khoangCachDauDong);
            };
        }
        #endregion
        #region CRUD
        [HttpPost]
        public ActionResult displayModal_CRUD(string loai, int idDanhMuc, int idDanhMucCha)
        {
            tbDanhMucHoSo danhMuc = new tbDanhMucHoSo();
            tbDanhMucHoSo danhMucCha = new tbDanhMucHoSo();
            List<tbDonViSuDung_PhongLuuTru> phongLuuTrus = new List<tbDonViSuDung_PhongLuuTru>();
            phongLuuTrus = db.tbDonViSuDung_PhongLuuTru.Where(k => k.TrangThai != 0 && k.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();

            if (idDanhMucCha == 0)
            {
                danhMucCha.IdDanhMucHoSo = idDanhMucCha;
                danhMucCha.CapDo = 0;
            }
            else
            {
                danhMucCha = db.tbDanhMucHoSoes.Find(idDanhMucCha);
            }

            if (loai != "create")
            {
                danhMuc = db.tbDanhMucHoSoes.Find(idDanhMuc);
            };

            ViewBag.loai = loai;
            ViewBag.danhMuc = danhMuc;
            ViewBag.danhMucCha = danhMucCha;
            ViewBag.phongLuuTrus = phongLuuTrus;
            return PartialView($"{VIEW_PATH}/documentdirectory-crud.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_Delete(bool deleteChilds = false, int idDanhMuc = 0)
        {
            #region Tạo cây danh mục - thay thế
            var _danhMucHoSos = get_DanhMucHoSos_Tree(idDanhMuc: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung);
            string taoTree(List<Tree<tbDanhMucHoSo>> danhMucHoSos, string mucLuc)
            {
                string html = "";
                foreach (Tree<tbDanhMucHoSo> _danhMuc in danhMucHoSos)
                {
                    tbDanhMucHoSo danhMuc = _danhMuc.root;
                    int capDo = danhMuc.CapDo ?? 0;
                    int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                    string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, _danhMuc.SoThuTu);
                    string tenDanhMuc_NEW = string.Format("{0} {1}", mucLuc_NEW, danhMuc.TenDanhMucHoSo);
                    string khoangCach = String.Join("", Enumerable.Repeat("<span class=\"ms-4 me-4\"></span>", capDoCha).ToArray());
                    if (_danhMuc.root.IdDanhMucHoSo == idDanhMuc)
                    {
                        html +=
                            $"<tr id=\"{danhMuc.IdDanhMucHoSo}\">" +
                            $"  <td class=\"text-decoration-line-through\">" +
                            $"      {khoangCach}<span class=\"ms-4 me-4\"></span>&ensp;{tenDanhMuc_NEW}" + // Không cho chọn checkbox
                            $"  </td>" +
                            "</tr>";
                        if (!deleteChilds) html += taoTree(danhMucHoSos: _danhMuc.nodes, mucLuc: mucLuc_NEW);
                    }
                    else
                    {
                        html +=
                            $"<tr id=\"{danhMuc.IdDanhMucHoSo}\">" +
                            $"  <td>" +
                            $"      {khoangCach}<input type=\"checkbox\" class=\"form-check-input ms-3 me-3 checkRow-danhmuchoso-thaythe-getList\" data-id=\"{danhMuc.IdDanhMucHoSo}\" data-idcha=\"{danhMuc.IdCha}\" data-capdo\"{capDo}   \"/> {tenDanhMuc_NEW}" +
                            $"  </td>" +
                            "</tr>" + taoTree(danhMucHoSos: _danhMuc.nodes, mucLuc: mucLuc_NEW);
                    };
                };
                return html;
            };
            #endregion
            ViewBag.HtmlDanhMucs_THAYTHE = taoTree(danhMucHoSos: _danhMucHoSos.nodes, mucLuc: "");
            ViewBag.deleteChilds = deleteChilds;
            ViewBag.idDanhMuc = idDanhMuc;
            return PartialView($"{VIEW_PATH}/documentdirectory-delete.cshtml");
        }
        public bool kiemTra_DanhMuc(tbDanhMucHoSo danhMuc)
        {
            // Kiểm tra có tồn tại bản ghi cùng cha mà trùng tên không
            tbDanhMucHoSo danhMuc_OLD = db.tbDanhMucHoSoes.FirstOrDefault(x =>
            x.TenDanhMucHoSo == danhMuc.TenDanhMucHoSo && x.IdDanhMucHoSo != danhMuc.IdDanhMucHoSo &&
            x.IdCha == danhMuc.IdCha &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (danhMuc_OLD == null) return false;
            return true;
        }
        [HttpPost]
        public ActionResult create_DanhMuc(string str_danhMuc)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbDanhMucHoSo danhMuc_NEW = JsonConvert.DeserializeObject<tbDanhMucHoSo>(str_danhMuc ?? "");
                    if (danhMuc_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_DanhMuc(danhMuc: danhMuc_NEW))
                        {
                            status = "warning";
                            mess = "Đã tồn tại bản ghi cùng nhóm";
                        }
                        else
                        {
                            // Tạo kiểu người dùng
                            tbDanhMucHoSo danhMuc = new tbDanhMucHoSo
                            {
                                TenDanhMucHoSo = danhMuc_NEW.TenDanhMucHoSo,
                                IdPhongLuuTru = danhMuc_NEW.IdPhongLuuTru,
                                IdCha = danhMuc_NEW.IdCha,
                                CapDo = danhMuc_NEW.CapDo + 1,
                                GhiChu = danhMuc_NEW.GhiChu,
                                TrangThai = 1,
                                NguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbDanhMucHoSoes.Add(danhMuc);
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Danh mục hồ sơ",
                                ThaoTac = "Thêm mới",
                                NoiDungChiTiet = "Thêm mới danh mục hồ sơ",

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
        public ActionResult update_DanhMuc(string str_danhMuc)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbDanhMucHoSo danhMuc_NEW = JsonConvert.DeserializeObject<tbDanhMucHoSo>(str_danhMuc ?? "");
                    if (danhMuc_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_DanhMuc(danhMuc: danhMuc_NEW))
                        {
                            status = "warning";
                            mess = "Đã tồn tại bản ghi cùng nhóm";
                        }
                        else
                        {
                            tbDanhMucHoSo danhMuc_OLD = db.tbDanhMucHoSoes.Find(danhMuc_NEW.IdDanhMucHoSo);
                            danhMuc_OLD.TenDanhMucHoSo = danhMuc_NEW.TenDanhMucHoSo;
                            danhMuc_OLD.IdPhongLuuTru = danhMuc_NEW.IdPhongLuuTru;
                            danhMuc_OLD.GhiChu = danhMuc_NEW.GhiChu;
                            danhMuc_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                            danhMuc_OLD.NgaySua = DateTime.Now;
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Danh mục hồ sơ",
                                ThaoTac = "Cập nhật",
                                NoiDungChiTiet = "Cập nhật danh mục hồ sơ",

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
        public JsonResult delete_DanhMucs(bool deleteChilds = false, int idDanhMuc = 0, int idDanhMuc_THAYTHE = 0)
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (deleteChilds)
                    { // Xóa con
                        void _deleteChilds(int idCha)
                        {
                            List<int> idDanhMucs = new List<int>();
                            idDanhMucs = db.tbDanhMucHoSoes.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1 && x.IdCha == idCha).Select(x => x.IdDanhMucHoSo).ToList();
                            if (idDanhMucs.Count > 0)
                            {
                                string capNhatSQL = $@"
                                -- Xóa danh mục
                                update tbDanhMucHoSo 
                                set 
                                    TrangThai = 0 ,NguoiSua = {per.NguoiDung.IdNguoiDung} ,NgaySua = '{DateTime.Now}' 
                                where 
                                    IdDanhMucHoSo in({String.Join(",", idDanhMucs)})

                                -- Cập nhật hồ sơ
                                update tbHoSo 
                                set 
                                    IdDanhMucHoSo = {idDanhMuc_THAYTHE}, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' 
                                where 
                                    IdDanhMucHoSo in({String.Join(",", idDanhMucs)}) and TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                                ";
                                db.Database.ExecuteSqlCommand(capNhatSQL);
                                foreach (int _idDanhMuc in idDanhMucs) _deleteChilds(_idDanhMuc);
                            };
                        };
                        _deleteChilds(idDanhMuc);

                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Danh mục hồ sơ",
                            ThaoTac = "Xóa",
                            NoiDungChiTiet = "Xóa danh mục hồ sơ (chỉ xóa cha) và cập nhật danh mục thay thế cho hồ sơ đang sử dụng",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung,
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
                        bool kiemTraTrungTen(tbDanhMucHoSo danhMuc)
                        {
                            // Kiểm tra có tồn tại bản ghi cùng cha mà trùng tên không
                            tbDanhMucHoSo danhMuc_OLD = db.tbDanhMucHoSoes.FirstOrDefault(x =>
                            x.TenDanhMucHoSo == danhMuc.TenDanhMucHoSo &&
                            x.IdDanhMucHoSo != danhMuc.IdDanhMucHoSo && // Không tìm chính nó
                            x.IdDanhMucHoSo != idDanhMuc && // Không tìm bản ghi cha đang xóa
                            x.IdCha == danhMuc.IdCha &&
                            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                            if (danhMuc_OLD == null) return false;
                            return true;
                        }
                        #endregion
                      
                        // Cập nhật idCha và cấp độ cho các bản ghi con
                        void _updateChilds(tbDanhMucHoSo _danhMucCha)
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
                            List<tbDanhMucHoSo> danhMucs = db.tbDanhMucHoSoes.Where(
                                x => x.IdCha == _danhMucCha.IdDanhMucHoSo
                                && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1)
                                .OrderBy(x => x.IdDanhMucHoSo).ToList();
                            // B1: Cập nhật cấp độ và thông tin cơ bản
                            foreach (tbDanhMucHoSo danhMuc in danhMucs)
                            {
                                danhMuc.IdCha = _danhMucCha.IdCha;
                                danhMuc.CapDo = _danhMucCha.CapDo;
                                danhMuc.NguoiSua = per.NguoiDung.IdNguoiDung;
                                danhMuc.NgaySua = DateTime.Now;
                                _updateChilds(danhMuc); // Tiếp tục thực hiện với các bản ghi con
                            };
                            db.SaveChanges();
                            if (_danhMucCha.IdDanhMucHoSo == idDanhMuc)
                            {
                                // B2: Kiểm tra trùng tên
                                foreach (tbDanhMucHoSo danhMuc in danhMucs)
                                {
                                    // Thay tới khi nào tên không còn trùng nữa
                                    while (kiemTraTrungTen(danhMuc: danhMuc))
                                    {
                                        // B3: Đổi tên
                                        danhMuc.TenDanhMucHoSo = xuLyTenTrung(tenBanGhi: danhMuc.TenDanhMucHoSo);
                                    };
                                };
                            };
                        };
                        tbDanhMucHoSo danhMucCha = db.tbDanhMucHoSoes.Find(idDanhMuc);
                        _updateChilds(_danhMucCha: danhMucCha);
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Danh mục hồ sơ",
                            ThaoTac = "Xóa",
                            NoiDungChiTiet = "Xóa danh mục hồ sơ (xóa cha và tất cả con) và cập nhật danh mục thay thế cho hồ sơ đang sử dụng",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung,
                        });
                    }
                    // Xóa bản ghi cha
                    db.Database.ExecuteSqlCommand($@"
                    -- Xóa vị trí
                    update tbDanhMucHoSo 
                    set 
                        TrangThai = 0 , NguoiSua = {per.NguoiDung.IdNguoiDung} , NgaySua = '{DateTime.Now}' 
                    where 
                        IdDanhMucHoSo in({idDanhMuc})

                    -- Cập nhật hồ sơ
                    update tbHoSo 
                    set 
                        IdDanhMucHoSo = {idDanhMuc_THAYTHE}, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' 
                    where 
                        IdDanhMucHoSo in({idDanhMuc}) and TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
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
        public tbDanhMucHoSoExtend kiemTra_Excel_DanhMuc(tbDanhMucHoSoExtend danhMuc)
        {
            List<string> ketQuas = new List<string>();
            /**
             * Các thông tin cần kiểm tra
             * Tên danh mục
             * Tên phông
             */
            if (kiemTra_DanhMuc(danhMuc: danhMuc))
            {
                ketQuas.Add("Đã tồn tại bản ghi cùng nhóm");
                danhMuc.KiemTraExcel.TrangThai = 0;
            };
            if (danhMuc.TenDanhMucHoSo == "" || danhMuc.PhongLuuTru.TenPhongLuuTru == "")
            {
                ketQuas.Add("Thiếu thông tin");
                danhMuc.KiemTraExcel.TrangThai = 0;
            };
            if (danhMuc.TenDanhMucHoSo == danhMuc.DanhMucCha.TenDanhMucHoSo)
            {
                ketQuas.Add("Trùng tên danh mục cha");
                danhMuc.KiemTraExcel.TrangThai = 0;
            };
            danhMuc.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            if (danhMuc.KiemTraExcel.KetQua == "")
            {
                danhMuc.KiemTraExcel.TrangThai = 1;
                danhMuc.KiemTraExcel.KetQua = "Hợp lệ";
            };
            return danhMuc;
        }
        [HttpPost]
        public ActionResult upload_Excel_DanhMuc(HttpPostedFileBase[] files)
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
                        EXCEL_DANHMUCs_UPLOAD = new List<tbDanhMucHoSoExtend>();
                        foreach (var sheet in workSheets)
                        {
                            // Lấy dữ liệu từ sheet hồ sơ
                            if (sheet.Name.Contains("DanhMuc"))
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
                                        tbDanhMucHoSoExtend danhMuc = new tbDanhMucHoSoExtend();
                                        danhMuc.TenDanhMucHoSo = row.Field("Tên danh mục").GetString();
                                        danhMuc.GhiChu = row.Field("Ghi chú").GetString();
                                        string tenDanhMucCha = row.Field("Tên danh mục cha").GetString();
                                        string tenPhongLuuTru = row.Field("Tên phông lưu trữ").GetString();
                                        danhMuc.PhongLuuTru = PHONGLUUTRUS.FirstOrDefault(x => x.TenPhongLuuTru == tenPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                                        danhMuc.IdPhongLuuTru = danhMuc.PhongLuuTru.IdPhongLuuTru;
                                        #region Tìm cha
                                        danhMuc.DanhMucCha = new tbDanhMucHoSo
                                        {
                                            IdDanhMucHoSo = 0,
                                            TenDanhMucHoSo = tenDanhMucCha
                                        };
                                        danhMuc.IdCha = danhMuc.DanhMucCha.IdDanhMucHoSo;
                                        #endregion
                                        EXCEL_DANHMUCs_UPLOAD.Add(danhMuc);
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
        public ActionResult download_Excel_DanhMuc()
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Tạo excel
            using (var workBook = new XLWorkbook())
            {
                #region Tạo sheet vị trí
                DataTable tbDanhMuc = new DataTable();
                tbDanhMuc.Columns.Add("Tên danh mục", typeof(string)); // 1
                tbDanhMuc.Columns.Add("Tên danh mục cha", typeof(string)); // 2
                tbDanhMuc.Columns.Add("Tên phông lưu trữ", typeof(string)); // 3
                tbDanhMuc.Columns.Add("Ghi chú", typeof(string)); // 4
                #region Thêm dữ liệu
                foreach (tbDanhMucHoSoExtend danhMuc in EXCEL_DANHMUCs_DOWNLOAD)
                {
                    tbDanhMuc.Rows.Add(
                       danhMuc.TenDanhMucHoSo,
                       danhMuc.DanhMucCha.TenDanhMucHoSo,
                       danhMuc.PhongLuuTru.TenPhongLuuTru,
                       danhMuc.GhiChu);
                };
                #endregion
                #endregion
                #region Tạo sheet danh sách
                DataTable tbDanhSach = new DataTable();
                tbDanhSach.Columns.Add("TenDanhMucHoSo", typeof(string));
                tbDanhSach.Columns.Add("TenPhongLuuTru", typeof(string));
                #region Thêm dữ liệu
                int DANHMUCHOSOS_KHONGDAUMUC_Count = DANHMUCHOSOS_KHONGDAUMUC.Count;
                int PHONGLUUTRUS_Count = PHONGLUUTRUS.Count;
                for (int i = 0; i < DANHMUCHOSOS_KHONGDAUMUC_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][0] = DANHMUCHOSOS_KHONGDAUMUC[i].TenDanhMucHoSo;
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
                workBook.Worksheets.Add(tbDanhMuc, "DanhMuc");
                workBook.Worksheets.Add(tbDanhSach, "DanhSach");
                tbDanhMuc.TableName = ""; tbDanhSach.TableName = "";
                for (int i = 1; i <= tbDanhMuc.Rows.Count + 1; i++)
                {
                    // List
                    //workBook.Worksheets.First().Cell(i, 2).CreateDataValidation().List($"=OFFSET(DanhSach!$A$2,0,0,COUNTA(DanhSach!$A:$A),1)");
                    workBook.Worksheets.First().Cell(i, 3).CreateDataValidation().List($"=OFFSET(DanhSach!$B$2,0,0,COUNTA(DanhSach!$B:$B),1)");
                };
                #endregion
                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                workBook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode("DANHMUC.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/DocumentDirectory/Index");
        }
        public ActionResult save_Excel_DanhMuc()
        {
            string status = "";
            string mess = "";
            List<tbDanhMucHoSoExtend> danhMuc_HopLes = new List<tbDanhMucHoSoExtend>();
            List<tbDanhMucHoSoExtend> danhMuc_KhongHopLes = new List<tbDanhMucHoSoExtend>();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (EXCEL_DANHMUCs_DOWNLOAD.Count == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        #region Kiểm tra excel
                        foreach (tbDanhMucHoSoExtend danhMuc_NEW in EXCEL_DANHMUCs_DOWNLOAD)
                        {
                            tbDanhMucHoSoExtend danhMuc_KhongHopLe = kiemTra_Excel_DanhMuc(danhMuc_NEW);
                            if (danhMuc_KhongHopLe.KiemTraExcel.TrangThai == 0)
                            {
                                danhMuc_KhongHopLes.Add(danhMuc_KhongHopLe);
                            }
                            else
                            {
                                // Kiểm tra tên biểu mẫu này đã được thêm ở bản ghi trước đó chưa
                                if (danhMuc_HopLes.Any(x => x.TenDanhMucHoSo == danhMuc_NEW.TenDanhMucHoSo))
                                {
                                    danhMuc_NEW.KiemTraExcel.TrangThai = 0;
                                    danhMuc_NEW.KiemTraExcel.KetQua = "Trùng tên danh mục";
                                    danhMuc_KhongHopLes.Add(danhMuc_NEW);
                                }
                                else
                                {
                                    danhMuc_HopLes.Add(danhMuc_NEW);
                                };
                            };
                        };
                        #endregion
                        if (danhMuc_KhongHopLes.Count == 0)
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
                            List<tbDanhMucHoSoExtend> danhMucs_DaThem = new List<tbDanhMucHoSoExtend>();
                            void timCon(tbDanhMucHoSoExtend danhMuc_Cha)
                            {
                                // Tìm con của cha này để lưu
                                List<tbDanhMucHoSoExtend> danhMucs_Con = danhMuc_HopLes.Where(x => x.DanhMucCha.TenDanhMucHoSo == danhMuc_Cha.TenDanhMucHoSo).ToList() ?? new List<tbDanhMucHoSoExtend>();
                                foreach (tbDanhMucHoSoExtend danhMuc_Con in danhMucs_Con)
                                {
                                    danhMuc_Con.IdCha = danhMuc_Cha.IdDanhMucHoSo;
                                    danhMuc_Con.CapDo = danhMuc_Cha.CapDo + 1;
                                    danhMuc_Con.TrangThai = 1;
                                    danhMuc_Con.NguoiTao = per.NguoiDung.IdNguoiDung;
                                    danhMuc_Con.NgayTao = DateTime.Now;
                                    danhMuc_Con.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;

                                    tbDanhMucHoSo danhMuc_NEW = new tbDanhMucHoSo
                                    {
                                        TenDanhMucHoSo = danhMuc_Con.TenDanhMucHoSo,
                                        IdPhongLuuTru = danhMuc_Con.IdPhongLuuTru,

                                        IdCha = danhMuc_Con.IdCha,
                                        CapDo = danhMuc_Con.CapDo,
                                        TrangThai = danhMuc_Con.TrangThai,
                                        NguoiTao = danhMuc_Con.NguoiTao,
                                        NgayTao = danhMuc_Con.NgayTao,
                                        MaDonViSuDung = danhMuc_Con.MaDonViSuDung,
                                    };
                                    db.tbDanhMucHoSoes.Add(danhMuc_NEW);
                                    db.SaveChanges();
                                    danhMuc_Con.IdDanhMucHoSo = danhMuc_NEW.IdDanhMucHoSo;
                                    danhMucs_DaThem.Add(danhMuc_Con); // Xóa ở danh sách
                                    // Tìm con trong danh sách
                                    timCon(danhMuc_Cha: danhMuc_Con);
                                };
                            };
                            void timCha(tbDanhMucHoSoExtend danhMuc_Con)
                            {
                                // Tìm cha trong danh sách
                                tbDanhMucHoSoExtend danhMuc_Cha_DS = danhMuc_HopLes.FirstOrDefault(x => x.TenDanhMucHoSo == danhMuc_Con.DanhMucCha.TenDanhMucHoSo);
                                if (danhMuc_Cha_DS == null)
                                {
                                    int idCha = 0;
                                    int capDo = 1;
                                    // Tìm cha trong csdl
                                    tbDanhMucHoSo danhMuc_Cha_CSDL = db.tbDanhMucHoSoes.FirstOrDefault(x => x.TenDanhMucHoSo == danhMuc_Con.DanhMucCha.TenDanhMucHoSo);
                                    if (danhMuc_Cha_CSDL != null)
                                    {
                                        capDo = danhMuc_Cha_CSDL.CapDo.Value + 1;
                                        idCha = danhMuc_Cha_CSDL.IdDanhMucHoSo;
                                    };
                                    danhMuc_Con.IdCha = idCha;
                                    danhMuc_Con.CapDo = capDo;
                                    danhMuc_Con.TrangThai = 1;
                                    danhMuc_Con.NguoiTao = per.NguoiDung.IdNguoiDung;
                                    danhMuc_Con.NgayTao = DateTime.Now;
                                    danhMuc_Con.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;
                                    tbDanhMucHoSo danhMuc_NEW = new tbDanhMucHoSo
                                    {
                                        TenDanhMucHoSo = danhMuc_Con.TenDanhMucHoSo,
                                        IdPhongLuuTru = danhMuc_Con.IdPhongLuuTru,

                                        IdCha = danhMuc_Con.IdCha,
                                        CapDo = danhMuc_Con.CapDo,
                                        TrangThai = danhMuc_Con.TrangThai,
                                        NguoiTao = danhMuc_Con.NguoiTao,
                                        NgayTao = danhMuc_Con.NgayTao,
                                        MaDonViSuDung = danhMuc_Con.MaDonViSuDung,
                                    };
                                    db.tbDanhMucHoSoes.Add(danhMuc_NEW);
                                    db.SaveChanges();
                                    danhMuc_Con.IdDanhMucHoSo = danhMuc_NEW.IdDanhMucHoSo;
                                    danhMucs_DaThem.Add(danhMuc_Con); // Xóa ở danh sách
                                    // Tìm con trong danh sách
                                    timCon(danhMuc_Cha: danhMuc_Con);
                                }
                                else
                                {
                                    timCha(danhMuc_Con: danhMuc_Cha_DS);
                                };
                            };
                            foreach (tbDanhMucHoSoExtend danhMuc in danhMuc_HopLes)
                            {
                                if (!danhMucs_DaThem.Any(x => x.TenDanhMucHoSo == danhMuc.TenDanhMucHoSo))
                                {
                                    timCha(danhMuc_Con: danhMuc);
                                };
                            };
                            #endregion
                            #region Thông báo
                            status = "success";
                            mess = "Thêm mới bản ghi thành công";
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Danh mục hồ sơ",
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
                        { // Khi thêm thành công, thay thế EXCEL_DANHMUCs_UPLOAD bằng danhMuc_KhongHopLes
                            status = "error-1";
                            mess = "Thêm mới bản ghi không thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                            // Trả lại danh sách bản ghi không hợp lệ
                            EXCEL_DANHMUCs_UPLOAD = new List<tbDanhMucHoSoExtend>();
                            EXCEL_DANHMUCs_UPLOAD.AddRange(danhMuc_KhongHopLes);
                            EXCEL_DANHMUCs_UPLOAD.AddRange(danhMuc_HopLes);
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
                danhMuc_KhongHopLes,
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public void get_DanhMucs_download()
        {
            string loaiTaiXuong = Request.Form["loaiTaiXuong"];
            string str_danhMucs = Request.Form["str_danhMucs"];
            EXCEL_DANHMUCs_DOWNLOAD = new List<tbDanhMucHoSoExtend>();
            EXCEL_DANHMUCs_DOWNLOAD.Add(new tbDanhMucHoSoExtend
            {
                TenDanhMucHoSo = "Nhập thông tin ...",
                DanhMucCha = DANHMUCHOSOS_KHONGDAUMUC.FirstOrDefault() ?? new tbDanhMucHoSo(),
                PhongLuuTru = PHONGLUUTRUS.FirstOrDefault() ?? new tbDonViSuDung_PhongLuuTru(),
            });
            if (loaiTaiXuong == "data")
            {
                EXCEL_DANHMUCs_DOWNLOAD = JsonConvert.DeserializeObject<List<tbDanhMucHoSoExtend>>(str_danhMucs ?? "") ?? new List<tbDanhMucHoSoExtend>();
            };
        }
        [HttpPost]
        public ActionResult getList_Excel_DanhMuc(string loai)
        {
            if (loai == "reload") EXCEL_DANHMUCs_UPLOAD = new List<tbDanhMucHoSoExtend>();
            return PartialView($"{VIEW_PATH}/documentdirectory-excel.danhmuc/excel.danhmuc-getList.cshtml");
        }
        #endregion
    }
}