using ClosedXML.Excel;
using EDM_DB;
using Newtonsoft.Json;
using Organization.Models;
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

namespace Organization.Controllers
{
    public class OrganizationController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemSetting/Organization";
        private List<tbCoCauToChucExtend> EXCEL_COCAUs_UPLOAD
        {
            get
            {
                return Session["EXCEL_COCAUs_UPLOAD"] as List<tbCoCauToChucExtend> ?? new List<tbCoCauToChucExtend>();
            }
            set
            {
                Session["EXCEL_COCAUs_UPLOAD"] = value;
            }
        }
        private List<tbCoCauToChucExtend> EXCEL_COCAUs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_COCAUs_DOWNLOAD"] as List<tbCoCauToChucExtend> ?? new List<tbCoCauToChucExtend>();
            }
            set
            {
                Session["EXCEL_COCAUs_DOWNLOAD"] = value;
            }
        }
        private List<tbCoCauToChuc> COCAUTOCHUCS
        {
            get
            {
                return Session["COCAUTOCHUCS"] as List<tbCoCauToChuc> ?? new List<tbCoCauToChuc>();
            }
            set
            {
                Session["COCAUTOCHUCS"] = value;
            }
        }
        private List<tbCoCauToChuc> COCAUTOCHUCS_KHONGMUCLUC
        {
            get
            {
                return Session["COCAUTOCHUCS_KHONGMUCLUC"] as List<tbCoCauToChuc> ?? new List<tbCoCauToChuc>();
            }
            set
            {
                Session["COCAUTOCHUCS_KHONGMUCLUC"] = value;
            }
        }
        private List<Tree<tbCoCauToChuc>> COCAUTOCHUCS_TREE
        {
            get
            {
                return Session["COCAUTOCHUCS_TREE"] as List<Tree<tbCoCauToChuc>> ?? new List<Tree<tbCoCauToChuc>>();
            }
            set
            {
                Session["COCAUTOCHUCS_TREE"] = value;
            }
        }
        private string HTMLCOCAUS
        {
            get
            {
                return Session["HTMLCOCAUS"] as string ?? string.Empty;
            }
            set
            {
                Session["HTMLCOCAUS"] = value;
            }
        }
        #endregion
        public OrganizationController() { }
        public ActionResult Index()
        {
            return View($"{VIEW_PATH}/organization.cshtml");
        }
        [HttpGet]
        public ActionResult getList()
        {
            #region Lấy các danh sách
            #region Tạo cây cơ cấu
            List<tbCoCauToChuc> coCauToChucs = new List<tbCoCauToChuc>();
            List<tbCoCauToChuc> coCauToChucs_KhongMucLuc = new List<tbCoCauToChuc>();
            List<Tree<tbCoCauToChuc>> coCauToChucs_Tree = get_CoCauToChucs_Tree(idCoCau: Guid.Empty, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            xuLy_TenCoCauToChuc(coCaus_IN: coCauToChucs_Tree, coCaus_OUT: coCauToChucs);
            xuLy_TenCoCauToChuc(coCaus_IN: coCauToChucs_Tree, coCaus_OUT: coCauToChucs_KhongMucLuc, kichHoat: false);
            string HtmlCoCaus(List<Tree<tbCoCauToChuc>> coCauToChucs_IN, string mucLuc)
            {
                string html = "";
                foreach (Tree<tbCoCauToChuc> _coCau in coCauToChucs_IN)
                {
                    tbCoCauToChuc coCau = _coCau.root;
                    int capDo = coCau.CapDo ?? 0;
                    int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                    string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, _coCau.SoThuTu);
                    string tenCoCau_NEW = string.Format("{0} {1}", mucLuc_NEW, coCau.TenCoCauToChuc);
                    if (coCau.IdCoCauToChuc == per.CoCauToChuc.IdCoCauToChuc)
                    {
                        tenCoCau_NEW += " <span class=\"text-danger fst-italic\"> (Đang sử dụng)</span>";
                    }
                    string khoangCach = String.Join("", Enumerable.Repeat("<span class=\"ms-4 me-4\"></span>", capDoCha).ToArray());
                    html +=
                        $"<tr id=\"{coCau.IdCoCauToChuc}\">" +
                         $" <td>" +
                        $"      {khoangCach}<input type=\"checkbox\" class=\"form-check-input ms-3 me-3\" data-id=\"{coCau.IdCoCauToChuc}\" data-idcha=\"{coCau.IdCha}\" data-capdo=\"{capDoCha}\"/> {tenCoCau_NEW}" +
                        $"  </td>" +
                        $"  <td class=\"text-center\">" +
                        // Thêm - Cập nhật - Xóa
                        $"      <div class=\"btn-group btn-group-sm\">" +
                        $"          <a href=\"#\" class=\"btn btn-sm btn-success\" title=\"Thêm mới\" onclick=\"org.coCau.displayModal_CRUD('create','{coCau.IdCoCauToChuc}', '{coCau.IdCoCauToChuc}')\"><i class=\"bi bi-plus-circle-fill\"></i></a>" +
                        $"          <a href=\"#\" class=\"btn btn-sm btn-primary\" title=\"Cập nhật\" onclick=\"org.coCau.displayModal_CRUD('update', '{coCau.IdCoCauToChuc}', '{coCau.IdCha}')\"><i class=\"bi bi-pencil-fill\"></i></a>" +
                        $"      </div>" +
                        $"      <div class=\"btn-group btn-group-sm dropdown\">" +
                        $"          <button type=\"button\" title=\"Xóa bỏ\" class=\"btn btn-danger dropdown-toggle me-1\" data-bs-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">" +
                        $"              Xóa bỏ" +
                        $"          </button>" +
                        $"          <div class=\"dropdown-menu\">" +
                        $"              <a class=\"dropdown-item\" onclick=\"org.coCau.displayModal_Delete(false, '{coCau.IdCoCauToChuc}')\">" +
                        $"                  1. Chỉ xóa cơ cấu này" +
                        $"              </a>" +
                        $"              <a class=\"dropdown-item\" onclick=\"org.coCau.displayModal_Delete(true, '{coCau.IdCoCauToChuc}')\">" +
                        $"                  2. Xóa cơ cấu này và cơ cấu con" +
                        $"              </a>" +
                        $"          </div>" +
                        $"      </div>" +
                        $"  </td>" +
                        "</tr>" + HtmlCoCaus(coCauToChucs_IN: _coCau.nodes, mucLuc: mucLuc_NEW);
                }
                return html;
            };
            #endregion
            #endregion
            COCAUTOCHUCS = coCauToChucs;
            COCAUTOCHUCS_KHONGMUCLUC = coCauToChucs_KhongMucLuc;
            COCAUTOCHUCS_TREE = coCauToChucs_Tree;
            HTMLCOCAUS = HtmlCoCaus(coCauToChucs_IN: coCauToChucs_Tree, mucLuc: "");
            ViewBag.HtmlCoCaus = HTMLCOCAUS;
            return PartialView($"{VIEW_PATH}/organization-getList.cshtml");
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
        #region Lấy danh sách
        [HttpGet]
        public List<tbNguoiDung> get_NguoiDungs()
        {
            List<tbNguoiDung> nguoiDungs = db.tbNguoiDungs.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).OrderByDescending(x => x.NgayTao).ToList();
            return nguoiDungs;
        }
        #endregion
        #region Xử lý tree
        [HttpPost]
        public Tree<tbCoCauToChuc> get_CoCauToChucs_Tree(Guid idCoCau, Guid maDonViSuDung)
        {
            List<Tree<tbCoCauToChuc>> makeTree(Guid idCha)
            {
                // Tạo nhánh
                List<Tree<tbCoCauToChuc>> nodes = new List<Tree<tbCoCauToChuc>>();
                // Tìm con
                //List<tbCoCauToChuc> _coCaus = new List<tbCoCauToChuc>();
                //_coCaus = db.tbCoCauToChucs.Where(x => x.MaDonViSuDung == maDonViSuDung && x.TrangThai == 1 && x.IdCha == idCha)
                //    .OrderByDescending(x => x.IdCoCauToChuc).ToList();
                List<tbCoCauToChuc> coCaus = db.Database.SqlQuery<tbCoCauToChuc>(
                    $@"select * from tbCoCauToChuc 
                    where MaDonViSuDung = '{maDonViSuDung}' and TrangThai != 0 and IdCha = '{idCha}'
                    order by NgayTao desc").ToList();
                for (int i = 0; i < coCaus.Count; i++)
                {
                    tbCoCauToChuc coCau = coCaus[i];
                    // 1 node gồm nhiều con hơn
                    Tree<tbCoCauToChuc> tree = new Tree<tbCoCauToChuc>();
                    tree.SoThuTu = (i + 1);
                    tree.root = coCau;
                    tree.nodes = makeTree(coCau.IdCoCauToChuc);
                    nodes.Add(tree);
                }
                return nodes;
            }
            Tree<tbCoCauToChuc> _tree = new Tree<tbCoCauToChuc>
            {
                nodes = makeTree(idCoCau),
            };
            return _tree;
        }
        public void xuLy_TenCoCauToChuc(List<Tree<tbCoCauToChuc>> coCaus_IN, List<tbCoCauToChuc> coCaus_OUT, string mucLuc = "", string khoangCachDauDong = "", bool kichHoat = true)
        {
            foreach (Tree<tbCoCauToChuc> coCau_IN in coCaus_IN)
            {
                string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, coCau_IN.SoThuTu);
                int capDo = coCau_IN.root.CapDo ?? 0;
                int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                string khoangCachDauDong_NEW = String.Join("", Enumerable.Repeat(khoangCachDauDong, capDoCha).ToArray());

                tbCoCauToChuc coCau_OUT = new tbCoCauToChuc
                {
                    IdCoCauToChuc = coCau_IN.root.IdCoCauToChuc,
                    TenCoCauToChuc = kichHoat ? string.Format("{0} {1} {2}", khoangCachDauDong_NEW, mucLuc_NEW, coCau_IN.root.TenCoCauToChuc) : coCau_IN.root.TenCoCauToChuc,
                    CapDo = coCau_IN.root.CapDo,
                    IdCha = coCau_IN.root.IdCha,
                    GhiChu = coCau_IN.root.GhiChu,
                    TrangThai = coCau_IN.root.TrangThai,
                    MaDonViSuDung = coCau_IN.root.MaDonViSuDung,
                    NgayTao = coCau_IN.root.NgayTao,
                    IdNguoiTao = coCau_IN.root.IdNguoiTao,
                    NgaySua = coCau_IN.root.NgaySua,
                    IdNguoiSua = coCau_IN.root.IdNguoiSua
                };
                coCaus_OUT.Add(coCau_OUT);
                xuLy_TenCoCauToChuc(coCaus_OUT: coCaus_OUT, coCaus_IN: coCau_IN.nodes, mucLuc: mucLuc_NEW, khoangCachDauDong: khoangCachDauDong, kichHoat: kichHoat);
            };
        }
        #endregion
        #region CRUD
        [HttpPost]
        public ActionResult displayModal_CRUD(string loai, Guid idCoCau, Guid idCoCauCha)
        {
            #region Thông tin chung
            tbCoCauToChuc coCau = new tbCoCauToChuc();
            tbCoCauToChuc coCauCha = new tbCoCauToChuc();
            if (idCoCauCha == Guid.Empty)
            {
                coCauCha.IdCoCauToChuc = idCoCauCha;
                coCauCha.CapDo = 0;
            }
            else
            {
                coCauCha = db.tbCoCauToChucs.Find(idCoCauCha);
            }

            if (loai != "create")
            {
                coCau = db.tbCoCauToChucs.Find(idCoCau);
            };
            #endregion

            #region Người dùng
            List<tbNguoiDung> nguoiDungs = get_NguoiDungs();
            #endregion

            ViewBag.loai = loai;
            ViewBag.coCau = coCau;
            ViewBag.coCauCha = coCauCha;
            ViewBag.nguoiDungs = nguoiDungs;
            return PartialView($"{VIEW_PATH}/organization-crud.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_Delete(Guid idCoCau, bool deleteChilds = false)
        {
            #region Tạo cây cơ cấu - thay thế
            var _coCaus = get_CoCauToChucs_Tree(idCoCau: Guid.Empty, maDonViSuDung: per.DonViSuDung.MaDonViSuDung);
            string taoTree(List<Tree<tbCoCauToChuc>> coCauToChucs, string mucLuc)
            {
                string html = "";
                foreach (Tree<tbCoCauToChuc> _coCau in coCauToChucs)
                {
                    tbCoCauToChuc coCau = _coCau.root;
                    int capDo = coCau.CapDo ?? 0;
                    int capDoCha = capDo >= 1 ? capDo - 1 : capDo;
                    string mucLuc_NEW = string.Format("{0}{1}.", mucLuc, _coCau.SoThuTu);
                    string tenCoCau_NEW = string.Format("{0} {1}", mucLuc_NEW, coCau.TenCoCauToChuc);
                    string khoangCach = String.Join("", Enumerable.Repeat("<span class=\"ms-4 me-4\"></span>", capDoCha).ToArray());
                    if (_coCau.root.IdCoCauToChuc == idCoCau)
                    {
                        html +=
                            $"<tr id=\"{coCau.IdCoCauToChuc}\">" +
                            $"  <td class=\"text-decoration-line-through\">" +
                            $"      {khoangCach}<span class=\"ms-4 me-4\"></span>&ensp;{tenCoCau_NEW}" + // Không cho chọn checkbox
                            $"  </td>" +
                            "</tr>";
                        if (!deleteChilds) html += taoTree(coCauToChucs: _coCau.nodes, mucLuc: mucLuc_NEW);
                    }
                    else
                    {
                        html +=
                            $"<tr id=\"{coCau.IdCoCauToChuc}\">" +
                            $"  <td>" +
                            $"      {khoangCach}<input type=\"checkbox\" class=\"form-check-input ms-3 me-3 checkRow-cocautochuc-thaythe-getList\" data-id=\"{coCau.IdCoCauToChuc}\" data-idcha=\"{coCau.IdCha}\" data-capdo\"{capDo}   \"/> {tenCoCau_NEW}" +
                            $"  </td>" +
                            "</tr>" + taoTree(coCauToChucs: _coCau.nodes, mucLuc: mucLuc_NEW);
                    };
                };
                return html;
            };
            #endregion
            ViewBag.HtmlCoCaus_THAYTHE = taoTree(coCauToChucs: _coCaus.nodes, mucLuc: "");
            ViewBag.deleteChilds = deleteChilds;
            ViewBag.idCoCau = idCoCau;
            return PartialView($"{VIEW_PATH}/organization-delete.cshtml");
        }
        public bool kiemTra_CoCau(tbCoCauToChuc coCau)
        {
            // Kiểm tra có tồn tại bản ghi cùng cha mà trùng tên không
            tbCoCauToChuc coCau_OLD = db.tbCoCauToChucs.FirstOrDefault(x =>
            x.TenCoCauToChuc == coCau.TenCoCauToChuc && x.IdCoCauToChuc != coCau.IdCoCauToChuc &&
            x.IdCha == coCau.IdCha &&
            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (coCau_OLD == null) return false;
            return true;
        }
        [HttpPost]
        public ActionResult create_CoCau(string str_coCau)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbCoCauToChuc coCau_NEW = JsonConvert.DeserializeObject<tbCoCauToChuc>(str_coCau ?? "");
                    if (coCau_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_CoCau(coCau_NEW))
                        {
                            status = "datontai";
                            mess = "Đã tồn tại bản ghi cùng nhóm";
                        }
                        else
                        {
                            // Tạo kiểu người dùng
                            tbCoCauToChuc coCau = new tbCoCauToChuc
                            {
                                IdCoCauToChuc = Guid.NewGuid(),
                                TenCoCauToChuc = coCau_NEW.TenCoCauToChuc,
                                IdCha = coCau_NEW.IdCha,
                                CapDo = coCau_NEW.CapDo + 1,
                                IdQuanLy = coCau_NEW.IdQuanLy,
                                GhiChu = coCau_NEW.GhiChu,
                                TrangThai = 1,
                                IdNguoiTao = per.NguoiDung.IdNguoiDung,
                                NgayTao = DateTime.Now,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            };
                            db.tbCoCauToChucs.Add(coCau);
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
        public ActionResult update_CoCau(string str_coCau)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbCoCauToChuc coCau_NEW = JsonConvert.DeserializeObject<tbCoCauToChuc>(str_coCau ?? "");
                    if (coCau_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_CoCau(coCau_NEW))
                        {
                            status = "datontai";
                            mess = "Đã tồn tại bản ghi cùng nhóm";
                        }
                        else
                        {
                            // Kiểu người dùng
                            tbCoCauToChuc coCau_OLD = db.tbCoCauToChucs.Find(coCau_NEW.IdCoCauToChuc);
                            coCau_OLD.TenCoCauToChuc = coCau_NEW.TenCoCauToChuc;
                            coCau_OLD.IdQuanLy = coCau_NEW.IdQuanLy;
                            coCau_OLD.GhiChu = coCau_NEW.GhiChu;
                            coCau_OLD.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                            coCau_OLD.NgaySua = DateTime.Now;
                            if (coCau_OLD.IdCoCauToChuc == per.CoCauToChuc.IdCoCauToChuc)
                            {
                                status = "logout";
                                mess = "[Cơ cấu đang sử dụng]";
                            };
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
        public JsonResult delete_CoCaus(Guid idCoCau, Guid idCoCau_THAYTHE, bool deleteChilds = false)
        {
            /**
             * THÊM:
             * - Kiểm tra với các bản ghi cùng cha rằng tên đã tồn tại chưa, nếu tồn tại thì thêm đuôi (số thứ tự) [1]
             * XÓA:
             * - Xóa bản ghi cha
             * - Thực hiện giống [1]
             */
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (deleteChilds)
                    { // Xóa con
                        void _deleteChilds(Guid idCha)
                        {
                            List<Guid> idCoCaus = new List<Guid>();
                            idCoCaus = db.tbCoCauToChucs.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1 && x.IdCha == idCha).Select(x => x.IdCoCauToChuc).ToList();
                            if (idCoCaus.Count > 0)
                            {
                                string _idCoCaus = String.Join(",", idCoCaus.Select(x => String.Format("'{0}'", x)).ToList());
                                string capNhatSQL = $@"
                                -- Xóa cơ cấu
                                update tbCoCauToChuc 
                                set 
                                    TrangThai = 0 ,IdNguoiSua = '{per.NguoiDung.IdNguoiDung}' ,NgaySua = '{DateTime.Now}' 
                                where 
                                    IdCoCauToChuc in({_idCoCaus})

                                -- Thay đổi IdCoCauToChuc ở Người dùng
                                update tbNguoiDung 
                                set 
                                    IdCoCauToChuc = '{idCoCau_THAYTHE}', IdNguoiSua = '{per.NguoiDung.IdNguoiDung}', NgaySua = '{DateTime.Now}' 
                                where 
                                    IdCoCauToChuc in({_idCoCaus}) and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                                ";
                                db.Database.ExecuteSqlCommand(capNhatSQL);
                                foreach (Guid _idCoCau in idCoCaus) _deleteChilds(_idCoCau);
                            };
                        };
                        _deleteChilds(idCoCau);
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
                        bool kiemTraTrungTen(tbCoCauToChuc coCau)
                        {
                            // Kiểm tra có tồn tại bản ghi cùng cha mà trùng tên không
                            tbCoCauToChuc coCau_OLD = db.tbCoCauToChucs.FirstOrDefault(x =>
                            x.TenCoCauToChuc == coCau.TenCoCauToChuc &&
                            x.IdCoCauToChuc != coCau.IdCoCauToChuc && // Không tìm chính nó
                            x.IdCoCauToChuc != idCoCau && // Không tìm bản ghi cha đang xóa
                            x.IdCha == coCau.IdCha &&
                            x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                            if (coCau_OLD == null) return false;
                            return true;
                        }
                        #endregion

                        // Cập nhật idCha và cấp độ cho các bản ghi con
                        void _updateChilds(tbCoCauToChuc _coCauCha)
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
                            List<tbCoCauToChuc> coCaus = db.tbCoCauToChucs.Where(
                                x => x.IdCha == _coCauCha.IdCoCauToChuc
                                && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai != 0)
                                .OrderBy(x => x.IdCoCauToChuc).ToList();
                            // B1: Cập nhật cấp độ và thông tin cơ bản
                            foreach (tbCoCauToChuc coCau in coCaus)
                            {
                                coCau.IdCha = _coCauCha.IdCha;
                                coCau.CapDo = _coCauCha.CapDo;
                                coCau.IdNguoiSua = per.NguoiDung.IdNguoiDung;
                                coCau.NgaySua = DateTime.Now;
                                _updateChilds(coCau); // Tiếp tục thực hiện với các bản ghi con
                            };
                            db.SaveChanges();
                            if (_coCauCha.IdCoCauToChuc == idCoCau)
                            {
                                // B2: Kiểm tra trùng tên
                                foreach (tbCoCauToChuc coCau in coCaus)
                                {
                                    // Thay tới khi nào tên không còn trùng nữa
                                    while (kiemTraTrungTen(coCau: coCau))
                                    {
                                        // B3: Đổi tên
                                        coCau.TenCoCauToChuc = xuLyTenTrung(tenBanGhi: coCau.TenCoCauToChuc);
                                    };
                                };
                            };
                        };
                        tbCoCauToChuc coCauCha = db.tbCoCauToChucs.Find(idCoCau);
                        _updateChilds(_coCauCha: coCauCha);
                    };
                    // Xóa bản ghi cha
                    db.Database.ExecuteSqlCommand($@"
                    -- Xóa cơ cấu
                    update tbCoCauToChuc 
                    set 
                        TrangThai = 0 , IdNguoiSua = '{per.NguoiDung.IdNguoiDung}' , NgaySua = '{DateTime.Now}' 
                    where 
                        IdCoCauToChuc in('{idCoCau}')

                    -- Thay đổi IdCoCauToChuc ở Người dùng
                    update tbNguoiDung 
                    set 
                        IdCoCauToChuc = '{idCoCau_THAYTHE}', IdNguoiSua = '{per.NguoiDung.IdNguoiDung}', NgaySua = '{DateTime.Now}' 
                    where 
                        IdCoCauToChuc in('{idCoCau}') and TrangThai != 0 and MaDonViSuDung = '{per.DonViSuDung.MaDonViSuDung}'
                    ");
                    if (idCoCau == per.CoCauToChuc.IdCoCauToChuc)
                    {
                        status = "logout";
                        mess = "[Cơ cấu đang sử dụng]";
                    }
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
        public tbCoCauToChucExtend kiemTra_Excel_CoCau(tbCoCauToChucExtend coCau)
        {
            List<string> ketQuas = new List<string>();
            /**
             * Các thông tin cần kiểm tra
             * Mã hồ sơ
             * Tiêu đề hồ sơ
             * Cơ cấu tổ chức
             * Danh mục hồ sơ
             * Phông lưu trữ
             * Chế độ sử dụng
             * Mục lục số
             * Số ký hiệu
             */
            if (kiemTra_CoCau(coCau: coCau))
            {
                ketQuas.Add("Đã tồn tại bản ghi cùng nhóm");
                coCau.KiemTraExcel.TrangThai = 0;
            };
            if (coCau.TenCoCauToChuc == "")
            {
                ketQuas.Add("Thiếu thông tin");
                coCau.KiemTraExcel.TrangThai = 0;
            };
            if (coCau.TenCoCauToChuc == coCau.CoCauCha.TenCoCauToChuc)
            {
                ketQuas.Add("Trùng tên cơ cấu cha");
                coCau.KiemTraExcel.TrangThai = 0;
            };
            coCau.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            if (coCau.KiemTraExcel.KetQua == "")
            {
                coCau.KiemTraExcel.TrangThai = 1;
                coCau.KiemTraExcel.KetQua = "Hợp lệ";
            };
            return coCau;
        }
        [HttpPost]
        public ActionResult upload_Excel_CoCau(HttpPostedFileBase[] files)
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
                        EXCEL_COCAUs_UPLOAD = new List<tbCoCauToChucExtend>();
                        foreach (var sheet in workSheets)
                        {
                            // Lấy dữ liệu từ sheet hồ sơ
                            if (sheet.Name.Contains("CoCau"))
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
                                        tbCoCauToChucExtend coCau = new tbCoCauToChucExtend();
                                        coCau.TenCoCauToChuc = row.Field("Tên cơ cấu").GetString();
                                        coCau.GhiChu = row.Field("Ghi chú").GetString();
                                        string tenCoCauCha = row.Field("Tên cơ cấu cha").GetString();
                                        #region Tìm cha
                                        coCau.CoCauCha = new tbCoCauToChuc
                                        {
                                            IdCoCauToChuc = Guid.Empty,
                                            TenCoCauToChuc = tenCoCauCha
                                        };
                                        coCau.IdCha = coCau.CoCauCha.IdCoCauToChuc;
                                        #endregion
                                        EXCEL_COCAUs_UPLOAD.Add(coCau);
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
        public ActionResult download_Excel_CoCau()
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Tạo excel
            using (var workBook = new XLWorkbook())
            {
                #region Tạo sheet cơ cấu
                DataTable tbCoCau = new DataTable();
                tbCoCau.Columns.Add("Tên cơ cấu", typeof(string)); // 1
                tbCoCau.Columns.Add("Tên cơ cấu cha", typeof(string)); // 2
                tbCoCau.Columns.Add("Ghi chú", typeof(string)); // 4
                #region Thêm dữ liệu
                foreach (tbCoCauToChucExtend coCau in EXCEL_COCAUs_DOWNLOAD)
                {
                    tbCoCau.Rows.Add(
                       coCau.TenCoCauToChuc,
                       coCau.CoCauCha.TenCoCauToChuc,
                       coCau.GhiChu);
                };
                #endregion
                #endregion
                #region Tạo sheet danh sách
                DataTable tbDanhSach = new DataTable();
                tbDanhSach.Columns.Add("TenCoCauToChuc", typeof(string));
                #region Thêm dữ liệu
                int COCAUTOCHUCS_KHONGMUCLUC_Count = COCAUTOCHUCS_KHONGMUCLUC.Count;
                for (int i = 0; i < COCAUTOCHUCS_KHONGMUCLUC_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][0] = COCAUTOCHUCS_KHONGMUCLUC[i].TenCoCauToChuc;
                };
                #endregion
                #endregion
                #region Tạo file excel
                workBook.Worksheets.Add(tbCoCau, "CoCau");
                workBook.Worksheets.Add(tbDanhSach, "DanhSach");
                tbCoCau.TableName = ""; tbDanhSach.TableName = "";
                for (int i = 1; i <= tbCoCau.Rows.Count + 1; i++)
                {
                    // List
                    //workBook.Worksheets.First().Cell(i, 2).CreateDataValidation().List($"=OFFSET(DanhSach!$A$2,0,0,COUNTA(DanhSach!$A:$A),1)");
                    //workBook.Worksheets.First().Cell(i, 3).CreateDataValidation().List($"=OFFSET(DanhSach!$B$2,0,0,COUNTA(DanhSach!$B:$B),1)");
                };
                #endregion
                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                workBook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode("COCAU.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/Organization/Index");
        }
        public ActionResult save_Excel_CoCau()
        {
            string status = "";
            string mess = "";
            List<tbCoCauToChucExtend> coCau_HopLes = new List<tbCoCauToChucExtend>();
            List<tbCoCauToChucExtend> coCau_KhongHopLes = new List<tbCoCauToChucExtend>();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (EXCEL_COCAUs_DOWNLOAD.Count == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        #region Kiểm tra excel
                        foreach (tbCoCauToChucExtend coCau_NEW in EXCEL_COCAUs_DOWNLOAD)
                        {
                            tbCoCauToChucExtend coCau_KhongHopLe = kiemTra_Excel_CoCau(coCau_NEW);
                            if (coCau_KhongHopLe.KiemTraExcel.TrangThai == 0)
                            {
                                coCau_KhongHopLes.Add(coCau_KhongHopLe);
                            }
                            else
                            {
                                // Kiểm tra tên biểu mẫu này đã được thêm ở bản ghi trước đó chưa
                                if (coCau_HopLes.Any(x => x.TenCoCauToChuc == coCau_NEW.TenCoCauToChuc))
                                {
                                    coCau_NEW.KiemTraExcel.TrangThai = 0;
                                    coCau_NEW.KiemTraExcel.KetQua = "Trùng tên cơ cấu";
                                    coCau_KhongHopLes.Add(coCau_NEW);
                                }
                                else
                                {
                                    coCau_HopLes.Add(coCau_NEW);
                                };
                            };
                        };
                        #endregion
                        if (coCau_KhongHopLes.Count == 0)
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
                            List<tbCoCauToChucExtend> coCaus_DaThem = new List<tbCoCauToChucExtend>();
                            void timCon(tbCoCauToChucExtend coCau_Cha)
                            {
                                // Tìm con của cha này để lưu
                                List<tbCoCauToChucExtend> coCaus_Con = coCau_HopLes.Where(x => x.CoCauCha.TenCoCauToChuc == coCau_Cha.TenCoCauToChuc).ToList() ?? new List<tbCoCauToChucExtend>();
                                foreach (tbCoCauToChucExtend coCau_Con in coCaus_Con)
                                {
                                    coCau_Con.IdCha = coCau_Cha.IdCoCauToChuc;
                                    coCau_Con.CapDo = coCau_Cha.CapDo + 1;
                                    coCau_Con.TrangThai = 1;
                                    coCau_Con.IdNguoiTao = per.NguoiDung.IdNguoiDung;
                                    coCau_Con.NgayTao = DateTime.Now;
                                    coCau_Con.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;

                                    tbCoCauToChuc coCau_NEW = new tbCoCauToChuc
                                    {
                                        IdCoCauToChuc = Guid.NewGuid(),
                                        TenCoCauToChuc = coCau_Con.TenCoCauToChuc,

                                        IdCha = coCau_Con.IdCha,
                                        CapDo = coCau_Con.CapDo,
                                        TrangThai = coCau_Con.TrangThai,
                                        IdNguoiTao = coCau_Con.IdNguoiTao,
                                        NgayTao = coCau_Con.NgayTao,
                                        MaDonViSuDung = coCau_Con.MaDonViSuDung,
                                    };
                                    db.tbCoCauToChucs.Add(coCau_NEW);
                                    db.SaveChanges();
                                    coCau_Con.IdCoCauToChuc = coCau_NEW.IdCoCauToChuc;
                                    coCaus_DaThem.Add(coCau_Con); // Xóa ở danh sách
                                    // Tìm con trong danh sách
                                    timCon(coCau_Cha: coCau_Con);
                                };
                            };
                            void timCha(tbCoCauToChucExtend coCau_Con)
                            {
                                // Tìm cha trong danh sách
                                tbCoCauToChucExtend coCau_Cha_DS = coCau_HopLes.FirstOrDefault(x => x.TenCoCauToChuc == coCau_Con.CoCauCha.TenCoCauToChuc);
                                if (coCau_Cha_DS == null)
                                {
                                    Guid idCha = Guid.Empty;
                                    int capDo = 1;
                                    // Tìm cha trong csdl
                                    tbCoCauToChuc coCau_Cha_CSDL = db.tbCoCauToChucs.FirstOrDefault(x => x.TenCoCauToChuc == coCau_Con.CoCauCha.TenCoCauToChuc);
                                    if (coCau_Cha_CSDL != null)
                                    {
                                        capDo = coCau_Cha_CSDL.CapDo.Value + 1;
                                        idCha = coCau_Cha_CSDL.IdCoCauToChuc;
                                    };
                                    coCau_Con.IdCha = idCha;
                                    coCau_Con.CapDo = capDo;
                                    coCau_Con.TrangThai = 1;
                                    coCau_Con.IdNguoiTao = per.NguoiDung.IdNguoiDung;
                                    coCau_Con.NgayTao = DateTime.Now;
                                    coCau_Con.MaDonViSuDung = per.DonViSuDung.MaDonViSuDung;
                                    tbCoCauToChuc coCau_NEW = new tbCoCauToChuc
                                    {
                                        IdCoCauToChuc = Guid.NewGuid(),
                                        TenCoCauToChuc = coCau_Con.TenCoCauToChuc,

                                        IdCha = coCau_Con.IdCha,
                                        CapDo = coCau_Con.CapDo,
                                        TrangThai = coCau_Con.TrangThai,
                                        IdNguoiTao = coCau_Con.IdNguoiTao,
                                        NgayTao = coCau_Con.NgayTao,
                                        MaDonViSuDung = coCau_Con.MaDonViSuDung,
                                    };
                                    db.tbCoCauToChucs.Add(coCau_NEW);
                                    db.SaveChanges();
                                    coCau_Con.IdCoCauToChuc = coCau_NEW.IdCoCauToChuc;
                                    coCaus_DaThem.Add(coCau_Con); // Xóa ở danh sách
                                    // Tìm con trong danh sách
                                    timCon(coCau_Cha: coCau_Con);
                                }
                                else
                                {
                                    timCha(coCau_Con: coCau_Cha_DS);
                                };
                            };
                            foreach (tbCoCauToChucExtend coCau in coCau_HopLes)
                            {
                                if (!coCaus_DaThem.Any(x => x.TenCoCauToChuc == coCau.TenCoCauToChuc))
                                {
                                    timCha(coCau_Con: coCau);
                                };
                            };
                            #endregion
                            #region Thông báo
                            status = "success";
                            mess = "Thêm mới bản ghi thành công";
                            db.SaveChanges();
                            scope.Commit();
                            #endregion
                        }
                        else
                        { // Khi thêm thành công, thay thế EXCEL_COCAUs_UPLOAD bằng coCau_KhongHopLes
                            status = "error-1";
                            mess = "Thêm mới bản ghi không thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                            // Trả lại danh sách bản ghi không hợp lệ
                            EXCEL_COCAUs_UPLOAD = new List<tbCoCauToChucExtend>();
                            EXCEL_COCAUs_UPLOAD.AddRange(coCau_KhongHopLes);
                            EXCEL_COCAUs_UPLOAD.AddRange(coCau_HopLes);
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
                coCau_KhongHopLes,
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public void get_CoCaus_download()
        {
            string loaiTaiXuong = Request.Form["loaiTaiXuong"];
            string str_coCaus = Request.Form["str_coCaus"];
            EXCEL_COCAUs_DOWNLOAD = new List<tbCoCauToChucExtend>();
            EXCEL_COCAUs_DOWNLOAD.Add(new tbCoCauToChucExtend
            {
                TenCoCauToChuc = "Nhập thông tin ...",
                CoCauCha = COCAUTOCHUCS_KHONGMUCLUC.FirstOrDefault() ?? new tbCoCauToChuc(),
            });
            if (loaiTaiXuong == "data")
            {
                EXCEL_COCAUs_DOWNLOAD = JsonConvert.DeserializeObject<List<tbCoCauToChucExtend>>(str_coCaus ?? "") ?? new List<tbCoCauToChucExtend>();
            };
        }
        [HttpPost]
        public ActionResult getList_Excel_CoCau(string loai)
        {
            if (loai == "reload") EXCEL_COCAUs_UPLOAD = new List<tbCoCauToChucExtend>();
            return PartialView($"{VIEW_PATH}/organization-excel.cocau/excel.cocau-getList.cshtml");
        }
        #endregion
    }
}