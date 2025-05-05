using Aspose.Cells;
using ClosedXML.Excel;
using DocumentDirectory.Controllers;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormation.Models;
using EDM_DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Organization.Controllers;
using Public.Controllers;
using Public.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using StorageLocation.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using UserAccount.Models;
using UserType.Models;

namespace DocumentFormation.Controllers
{
    public class DocumentFormationController : RouteConfigController
    {
        #region Biến public để in hoa
        private readonly string VIEW_PATH = "~/Views/Admin/_DocumentManage/DocumentFormation";
        IsoDateTimeConverter DATETIMECONVERTER = new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" };
        private List<tbHoSoExtend> EXCEL_HOSOs_UPLOAD
        {
            get
            {
                return Session["EXCEL_HOSOs_UPLOAD"] as List<tbHoSoExtend> ?? new List<tbHoSoExtend>();
            }
            set
            {
                Session["EXCEL_HOSOs_UPLOAD"] = value;
            }
        }
        private List<tbHoSoExtend> EXCEL_HOSOs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_HOSOs_DOWNLOAD"] as List<tbHoSoExtend> ?? new List<tbHoSoExtend>();
            }
            set
            {
                Session["EXCEL_HOSOs_DOWNLOAD"] = value;
            }
        }
        private List<tbHoSoExtend> EXCEL_TEPVANBANs_UPLOAD
        {
            get
            {
                return Session["EXCEL_TEPVANBANs_UPLOAD"] as List<tbHoSoExtend> ?? new List<tbHoSoExtend>();
            }
            set
            {
                Session["EXCEL_TEPVANBANs_UPLOAD"] = value;
            }
        }
        private List<tbHoSoExtend> EXCEL_TEPVANBANs_DOWNLOAD
        {
            get
            {
                return Session["EXCEL_TEPVANBANs_DOWNLOAD"] as List<tbHoSoExtend> ?? new List<tbHoSoExtend>();
            }
            set
            {
                Session["EXCEL_TEPVANBANs_DOWNLOAD"] = value;
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
        private List<default_tbCheDoSuDung> CHEDOSUDUNGS
        {
            get
            {
                return Session["CHEDOSUDUNGS"] as List<default_tbCheDoSuDung> ?? new List<default_tbCheDoSuDung>();
            }
            set
            {
                Session["CHEDOSUDUNGS"] = value;
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
        private List<tbNguoiDungExtend> NGUOIDUNGS
        {
            get
            {
                return Session["NGUOIDUNGS"] as List<tbNguoiDungExtend> ?? new List<tbNguoiDungExtend>();
            }
            set
            {
                Session["NGUOIDUNGS"] = value;
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
        private static StorageLocationController storageLocationController = new StorageLocationController();
        private static DocumentDirectoryController documentDirectoryController = new DocumentDirectoryController();
        private static OrganizationController organizationController = new OrganizationController();
        public DocumentFormationController()
        {
        }
        #endregion
        public ActionResult Index()
        {
            tbHoSo hoSo = new tbHoSo();
            #region Xóa tệp sao lưu để giải phóng bộ nhớ
            string duongDanGoc = string.Format("/Assets/uploads/{0}/SAOLUU/{1}", per.DonViSuDung.MaDonViSuDung, per.NguoiDung.IdNguoiDung);
            string duongDanGoc_SERVER = Request.MapPath(duongDanGoc);
            if (Directory.Exists(duongDanGoc_SERVER)) Directory.Delete(duongDanGoc_SERVER, true);
            #endregion

            #region Lấy các danh sách
            #region Phông lưu trữ & Chế độ sử dụng
            List<tbDonViSuDung_PhongLuuTru> phongLuuTrus = db.tbDonViSuDung_PhongLuuTru.Where(x => x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung && x.TrangThai == 1).ToList() ?? new List<tbDonViSuDung_PhongLuuTru>();
            List<default_tbCheDoSuDung> cheDoSuDungs = db.default_tbCheDoSuDung.Where(x => x.TrangThai == 1).ToList() ?? new List<default_tbCheDoSuDung>();
            #endregion
            #region Vị trí lưu trữ
            List<tbViTriLuuTru> viTriLuuTrus = new List<tbViTriLuuTru>();
            List<Tree<tbViTriLuuTru>> viTriLuuTrus_Tree = storageLocationController.get_ViTriLuuTrus_Tree(idViTri: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            storageLocationController.xuLy_TenViTriLuuTru(viTris_IN: viTriLuuTrus_Tree, viTris_OUT: viTriLuuTrus, khoangCachDauDong: "");
            #endregion
            #region Danh mục hồ sơ
            List<tbDanhMucHoSo> danhMucHoSos = new List<tbDanhMucHoSo>();
            List<Tree<tbDanhMucHoSo>> danhMucHoSos_Tree = documentDirectoryController.get_DanhMucHoSos_Tree(idDanhMuc: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            documentDirectoryController.xuLy_TenDanhMucHoSo(danhMucs_IN: danhMucHoSos_Tree, danhMucs_OUT: danhMucHoSos, khoangCachDauDong: "");
            #endregion
            #region Cơ cấu tổ chức
            List<tbCoCauToChuc> coCauToChucs = new List<tbCoCauToChuc>();
            List<Tree<tbCoCauToChuc>> coCauToChucs_Tree = organizationController.get_CoCauToChucs_Tree(idCoCau: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            organizationController.xuLy_TenCoCauToChuc(coCaus_IN: coCauToChucs_Tree, coCaus_OUT: coCauToChucs, khoangCachDauDong: "");
            #endregion
            #region Người dùng
            List<tbNguoiDungExtend> nguoiDungs = db.Database.SqlQuery<tbNguoiDungExtend>($@"
            select * from tbNguoiDung where TrangThai != 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
            ").ToList() ?? new List<tbNguoiDungExtend>();
            foreach (tbNguoiDungExtend nguoiDung in nguoiDungs)
            {
                nguoiDung.CoCauToChuc = db.tbCoCauToChucs.Find(nguoiDung.IdCoCauToChuc) ?? new tbCoCauToChuc();
            };
            #endregion
            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "DocumentFormation").ThaoTacs;
            #endregion
            #endregion
            NGUOIDUNGS = nguoiDungs;
            PHONGLUUTRUS = phongLuuTrus;
            CHEDOSUDUNGS = cheDoSuDungs;
            VITRILUUTRUS = viTriLuuTrus; VITRILUUTRUS_TREE = viTriLuuTrus_Tree;
            DANHMUCHOSOS = danhMucHoSos; DANHMUCHOSOS_TREE = danhMucHoSos_Tree;
            COCAUTOCHUCS = coCauToChucs; COCAUTOCHUCS_TREE = coCauToChucs_Tree;

            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;
            ViewBag.thaoTacs = thaoTacs;
            ViewBag.nguoiDungs = NGUOIDUNGS;
            ViewBag.phongLuuTrus = PHONGLUUTRUS;
            ViewBag.cheDoSuDungs = CHEDOSUDUNGS;
            ViewBag.viTriLuuTrus = VITRILUUTRUS; ViewBag.VITRILUUTRUS_TREE = VITRILUUTRUS_TREE;
            ViewBag.danhMucHoSos = DANHMUCHOSOS; ViewBag.DANHMUCHOSOS_TREE = DANHMUCHOSOS_TREE;
            ViewBag.coCauToChucs = COCAUTOCHUCS; ViewBag.COCAUTOCHUCS_TREE = COCAUTOCHUCS_TREE;
            return View($"{VIEW_PATH}/documentformation.cshtml");
        }
        [HttpGet]
        public ActionResult getList(string str_idViTriLuuTrus = "", string str_idDanhMucHoSos = "")
        {
            List<tbHoSoExtend> hoSos = get_HoSos(loai: "all", str_idViTriLuuTrus: str_idViTriLuuTrus, str_idDanhMucHoSos: str_idDanhMucHoSos);
            return Json(new
            {
                data = hoSos
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
        #region Lấy vị trí lưu trữ của hồ sơ
        public JsonResult get_ViTriLuuTrus()
        {
            List<Tree<tbViTriLuuTru>> viTriLuuTrus_Tree = get_ViTriLuuTrus_Tree(idViTri: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            return Json(viTriLuuTrus_Tree, JsonRequestBehavior.AllowGet);
        }
        public Tree<tbViTriLuuTru> get_ViTriLuuTrus_Tree(int idViTri = 0, int maDonViSuDung = 0)
        {
            // Tạo cây cơ cấu
            List<Tree<tbViTriLuuTru>> makeTree(int idCha)
            {
                // Tạo nhánh
                List<Tree<tbViTriLuuTru>> nodes = new List<Tree<tbViTriLuuTru>>();
                // Tìm con
                List<tbViTriLuuTru> viTris = db.Database.SqlQuery<tbViTriLuuTru>(
                    $@"select * from tbViTriLuuTru 
                    where MaDonViSuDung = {maDonViSuDung} and TrangThai = 1 and IdCha = {idCha}
                    order by IdViTriLuuTru desc").ToList();
                for (int i = 0; i < viTris.Count; i++)
                {
                    tbViTriLuuTru viTri = viTris[i];
                    // Lấy số lượng hồ sơ thuộc vị trí lưu trữ
                    List<tbHoSoExtend> hoSos = get_HoSos(loai: "all", layThongTinPhu: false, str_idViTriLuuTrus: viTri.IdViTriLuuTru.ToString());
                    // Tạo thông tin
                    // 1 node gồm nhiều con hơn
                    HoSoThuocTree<tbViTriLuuTru> tree = new HoSoThuocTree<tbViTriLuuTru>();
                    tree.SoThuTu = (i + 1);
                    tree.root = viTri;
                    tree.hoSos = hoSos;
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
        #endregion
        #region Lấy danh mục của hồ sơ
        public JsonResult get_DanhMucHoSos()
        {
            List<Tree<tbDanhMucHoSo>> danhMucHoSos_Tree = get_DanhMucHoSos_Tree(idDanhMuc: 0, maDonViSuDung: per.DonViSuDung.MaDonViSuDung).nodes;
            return Json(danhMucHoSos_Tree, JsonRequestBehavior.AllowGet);
        }
        public Tree<tbDanhMucHoSo> get_DanhMucHoSos_Tree(int idDanhMuc = 0, int maDonViSuDung = 0)
        {
            // Tạo cây cơ cấu
            List<Tree<tbDanhMucHoSo>> makeTree(int idCha)
            {
                // Tạo nhánh
                List<Tree<tbDanhMucHoSo>> nodes = new List<Tree<tbDanhMucHoSo>>();
                // Tìm con
                List<tbDanhMucHoSo> danhMucs = db.Database.SqlQuery<tbDanhMucHoSo>(
                    $@"select * from tbDanhMucHoSo 
                    where MaDonViSuDung = {maDonViSuDung} and TrangThai = 1 and IdCha = {idCha}
                    order by IdDanhMucHoSo desc").ToList();
                for (int i = 0; i < danhMucs.Count; i++)
                {
                    tbDanhMucHoSo danhMuc = danhMucs[i];
                    // Lấy số lượng hồ sơ thuộc vị trí lưu trữ
                    List<tbHoSoExtend> hoSos = get_HoSos(loai: "all", layThongTinPhu: false, str_idDanhMucHoSos: danhMuc.IdDanhMucHoSo.ToString());
                    // Tạo thông tin
                    // 1 node gồm nhiều con hơn
                    HoSoThuocTree<tbDanhMucHoSo> tree = new HoSoThuocTree<tbDanhMucHoSo>();
                    tree.SoThuTu = (i + 1);
                    tree.root = danhMuc;
                    tree.hoSos = hoSos;
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
        #endregion
        #region CRUD

        [HttpPost]
        public ActionResult displayModal_CRUD(string loai, int idHoSo)
        {
            #region Thao tác
            List<ChucNangs> kieuNguoiDung_IdChucNang = JsonConvert.DeserializeObject<List<ChucNangs>>(per.KieuNguoiDung.IdChucNang);
            List<ThaoTac> thaoTacs = kieuNguoiDung_IdChucNang.FirstOrDefault(x => x.ChucNang.MaChucNang == "DocumentFormation").ThaoTacs;
            #endregion
            #region Thông tin chung hồ sơ
            tbHoSo hoSo = new tbHoSo();
            // Gán quyền truy cập cho mọi tài khoản trong cùng cơ cấu
            var nguoiDungs_CungCoCau = NGUOIDUNGS.Where(x => x.IdCoCauToChuc == per.CoCauToChuc.IdCoCauToChuc).Select(x => x.IdNguoiDung).ToList();
            if (nguoiDungs_CungCoCau != null)
            {
                hoSo.QuyenTruyCap = string.Join(",", nguoiDungs_CungCoCau);
            };
            if (loai != "create" && idHoSo != 0)
            {
                hoSo = db.tbHoSoes.Find(idHoSo);
            };
            #endregion
            #region Lịch sử hồ sơ
            List<tbHoSo_LichSuExtend> hoSo_LichSus = db.Database.SqlQuery<tbHoSo_LichSuExtend>($@"
            select * 
            from tbHoSo_LichSu
            where IdHoSo = {idHoSo} and TrangThai != 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
            order by NgayTao desc
            ").ToList();
            foreach (tbHoSo_LichSuExtend hoSo_LichSu in hoSo_LichSus)
            {
                hoSo_LichSu.ThongTinNguoiTao = db.tbNguoiDungs.FirstOrDefault(x =>
                x.IdNguoiDung == hoSo_LichSu.NguoiTao && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung) ?? new tbNguoiDung();
            };
            #endregion

            ViewBag.kieuNguoiDung_IdChucNang = kieuNguoiDung_IdChucNang;
            ViewBag.hoSo = hoSo;
            ViewBag.hoSo_LichSus = hoSo_LichSus;
            ViewBag.loai = loai;
            return PartialView($"{VIEW_PATH}/documentformation-crud.cshtml");
        }
        [HttpGet]
        public ActionResult displayModal_ThietLapQuyen()
        {
            return PartialView($"{VIEW_PATH}/documentformation-thietlapquyen/thietlapquyen.cshtml");
        }
        [HttpPost]
        public ActionResult displayModal_LichSu(int idHoSo)
        {
            List<tbHoSo_LichSuExtend> hoSo_LichSus = db.Database.SqlQuery<tbHoSo_LichSuExtend>($@"
            select * 
            from tbHoSo_LichSu
            where IdHoSo = {idHoSo} and TrangThai != 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
            order by NgayTao desc
            ").ToList();
            foreach (tbHoSo_LichSuExtend hoSo_LichSu in hoSo_LichSus)
            {
                hoSo_LichSu.ThongTinNguoiTao = db.tbNguoiDungs.FirstOrDefault(x =>
                x.IdNguoiDung == hoSo_LichSu.NguoiTao && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung) ?? new tbNguoiDung();
            };
            ViewBag.hoSo_LichSus = hoSo_LichSus;
            return PartialView($"{VIEW_PATH}/documentformation-lichsu/lichsu.cshtml");
        }
        public List<tbHoSoExtend> get_HoSos(string loai, string idHoSos = "", bool layThongTinPhu = true,
            string str_idViTriLuuTrus = "", string str_idDanhMucHoSos = "")
        {
            List<tbHoSoExtend> hoSos = new List<tbHoSoExtend>();
            // Chỉ hiển thị bản ghi có quyền truy cập
            if (loai == "all")
            {
                hoSos = db.Database.SqlQuery<tbHoSoExtend>($@"
                select * from tbHoSo 
                where 
                    MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai in (1,2)
                    -- Đang cho hiển thị hết nên khóa
                    -- and exists (select 1 from string_split(QuyenTruyCap, ',') where value = '{per.NguoiDung.IdNguoiDung}')
                    -- Tìm theo vị trí lứu trữ
                    {(str_idViTriLuuTrus != "" ? $"and IdViTriLuuTru in ({str_idViTriLuuTrus})" : "")}
                    -- Tìm theo danh mục hồ sơ
                    {(str_idDanhMucHoSos != "" ? $"and IdDanhMucHoSo in ({str_idDanhMucHoSos})" : "")}
                order by NgayTao desc").ToList();
            }
            else
            {
                if (idHoSos != "")
                {
                    string getSql = $@"select * from tbHoSo where MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} 
                    and TrangThai in (1,2) and IdHoSo in ({idHoSos}) order by NgayTao desc";
                    hoSos = db.Database.SqlQuery<tbHoSoExtend>(getSql).ToList();
                }
            };
            if (layThongTinPhu)
            {
                foreach (tbHoSoExtend hoSo in hoSos)
                {
                    hoSo.VanBans = get_VanBans(hoSo: hoSo, loai: "all");
                    hoSo.ViTriLuuTru = db.tbViTriLuuTrus.Find(hoSo.IdViTriLuuTru) ?? new tbViTriLuuTru();
                    hoSo.PhongLuuTru = db.tbDonViSuDung_PhongLuuTru.Find(hoSo.IdPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                    // Lấy danh mục từ danh sách danh mục đã xử lý tên
                    //hoSo.DanhMucHoSo = db.tbDanhMucHoSoes.Find(hoSo.IdDanhMucHoSo) ?? new tbDanhMucHoSo();
                    hoSo.DanhMucHoSo = DANHMUCHOSOS.Where(x => x.IdDanhMucHoSo == hoSo.IdDanhMucHoSo).FirstOrDefault() ?? new tbDanhMucHoSo();
                    hoSo.CheDoSuDung = db.default_tbCheDoSuDung.Find(hoSo.IdCheDoSuDung) ?? new default_tbCheDoSuDung();
                    hoSo.ThongTinNguoiTao = db.tbNguoiDungs.Find(hoSo.NguoiTao) ?? new tbNguoiDung();
                    hoSo.DonViSuDung = db.tbDonViSuDungs.Find(hoSo.MaDonViSuDung) ?? new tbDonViSuDung();
                };
            };
            return hoSos;
        }
        public List<tbHoSo_VanBanExtend> get_VanBans(tbHoSoExtend hoSo, string loai, string idVanBans = "")
        {
            // Thông tin văn bản của hồ sơ
            string get_VanBanSQL = "";
            if (loai == "all")
            {
                get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdHoSo = {hoSo.IdHoSo}";
            }
            else if (idVanBans != "")
            {
                get_VanBanSQL = $@"select * from tbHoSo_VanBan where TrangThai <> 0 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdVanBan in ({idVanBans})";
            };
            List<tbHoSo_VanBanExtend> vanBans = db.Database.SqlQuery<tbHoSo_VanBanExtend>(get_VanBanSQL).ToList() ?? new List<tbHoSo_VanBanExtend>();
            // Lấy thông tin biểu mẫu của văn bản
            foreach (tbHoSo_VanBanExtend vanBan in vanBans)
            {
                string get_BieuMauSQL = $@"select * from tbBieuMau where TrangThai = 1 and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and IdBieuMau = {vanBan.IdBieuMau}";
                vanBan.BieuMau = db.Database.SqlQuery<tbBieuMauExtend>(get_BieuMauSQL).FirstOrDefault() ?? new tbBieuMauExtend();
                #region Lấy đường dẫn văn bản
                string tenVanBan_BANDAU = string.Format("{0}{1}", Path.GetFileName(vanBan.TenVanBan), vanBan.Loai);
                var duongDanVanBan = LayDuongDanTep(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU);

                vanBan.DuongDan = duongDanVanBan.duongDanVanBan_BANDAU;
                if (vanBan.Loai.Contains("xls") || vanBan.Loai.Contains("doc"))
                {
                    vanBan.DuongDan = duongDanVanBan.duongDanVanBan_CHUYENDOI;
                };
                #endregion
            };
            return vanBans;
        }
        [HttpPost]
        public tbHoSoExtend kiemTra_MaHoSo(tbHoSoExtend hoSo)
        {
            List<string> ketQuas = new List<string>();
            // Kiểm tra mã hồ sơ có đúng định dạng
            if (hoSo.MaHoSo != "")
            {
                // Kiểm tra không chứa ký tự đặc biệt ở đầu và cuối
                string pattern = @"^[^\W_](.*[^\W_])?$";
                Regex regex = new Regex(pattern);
                if (!regex.IsMatch(hoSo.MaHoSo))
                {
                    ketQuas.Add("Mã hồ sơ không được bắt đầu và kết thúc bằng ký tự đặc biệt");
                    hoSo.KiemTraExcel.TrangThai = 0;
                };
            };
            // Kiểm tra còn hồ sơ khác có trùng mã không
            tbHoSo hoSo_OLD = db.tbHoSoes.FirstOrDefault(x => x.MaHoSo == hoSo.MaHoSo && x.IdHoSo != hoSo.IdHoSo && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            if (hoSo_OLD != null)
            {
                ketQuas.Add("Mã hồ sơ đã tồn tại");
                hoSo.KiemTraExcel.TrangThai = 0;
            };
            hoSo.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            return hoSo;
        }
        public List<HoSo_LichSu_ChiTiet> ChiTietThayDoi_HoSo(List<Tuple<string, object, object>> thayDois)
        {
            List<HoSo_LichSu_ChiTiet> chiTiets = new List<HoSo_LichSu_ChiTiet>();
            foreach (var thayDoi in thayDois)
            {
                chiTiets.Add(new HoSo_LichSu_ChiTiet
                {
                    TenTruongDuLieu = thayDoi.Item1,
                    GiaTri_Cu = thayDoi.Item2 != null ? thayDoi.Item2.ToString(): "",
                    GiaTri_Moi = thayDoi.Item3 != null ? thayDoi.Item3.ToString() : "",
                });
            }
            return chiTiets;
        }
        [HttpPost]
        public ActionResult create_HoSo(string str_hoSo)
        {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    tbHoSoExtend hoSo_NEW = JsonConvert.DeserializeObject<tbHoSoExtend>(str_hoSo ?? "", DATETIMECONVERTER);
                    if (hoSo_NEW == null)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        if (kiemTra_MaHoSo(hoSo: hoSo_NEW).KiemTraExcel.TrangThai == 0)
                        {
                            status = "warning";
                            mess = hoSo_NEW.KiemTraExcel.KetQua;
                        }
                        else
                        {
                            // Tạo folder lưu văn bản
                            string duongDanThuMuc_HOSO = string.Format("/Assets/uploads/{0}/HOSO/{1}", per.DonViSuDung.MaDonViSuDung, Public.Handle.ConvertToUnSign(s: hoSo_NEW.MaHoSo, khoangCach: "_"));
                            string duongDanThuMuc_HOSO_SERVER = Request.MapPath(duongDanThuMuc_HOSO);
                            if (!System.IO.Directory.Exists(duongDanThuMuc_HOSO_SERVER))
                            {
                                // Tạo hồ sơ
                                tbHoSo hoSo = new tbHoSo
                                {
                                    TieuDeHoSo = hoSo_NEW.TieuDeHoSo,
                                    QuyenTruyCap = hoSo_NEW.QuyenTruyCap,
                                    DuongDanFile = duongDanThuMuc_HOSO,

                                    IdViTriLuuTru = hoSo_NEW.ViTriLuuTru.IdViTriLuuTru,
                                    IdPhongLuuTru = hoSo_NEW.PhongLuuTru.IdPhongLuuTru,
                                    IdDanhMucHoSo = hoSo_NEW.DanhMucHoSo.IdDanhMucHoSo,
                                    IdCheDoSuDung = hoSo_NEW.CheDoSuDung.IdCheDoSuDung,

                                    MucLucSo_NamHinhThanh = hoSo_NEW.MucLucSo_NamHinhThanh,
                                    So_KyHieu = hoSo_NEW.So_KyHieu,
                                    MaHoSo = hoSo_NEW.MaHoSo,
                                    ThoiHanBaoQuan = hoSo_NEW.ThoiHanBaoQuan,
                                    ThoiGianBatDau = hoSo_NEW.ThoiGianBatDau,
                                    ThoiGianKetThuc = hoSo_NEW.ThoiGianKetThuc,
                                    TinhTrangVatLy = hoSo_NEW.TinhTrangVatLy,
                                    NgonNgu = hoSo_NEW.NgonNgu,
                                    TuKhoa = hoSo_NEW.TuKhoa,
                                    KyHieuThongTin = hoSo_NEW.KyHieuThongTin,
                                    TongSoVanBan = hoSo_NEW.TongSoVanBan,
                                    SoLuongTo = hoSo_NEW.SoLuongTo,
                                    SoLuongTrang = hoSo_NEW.SoLuongTrang,
                                    GhiChu = hoSo_NEW.GhiChu,
                                    TrangThai = 1,
                                    NguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                };
                                db.tbHoSoes.Add(hoSo);
                                db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                                {
                                    TenModule = "Tạo lập hồ sơ",
                                    ThaoTac = "Thêm mới",
                                    NoiDungChiTiet = "Thêm mới hồ sơ",

                                    NgayTao = DateTime.Now,
                                    IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                });
                                db.SaveChanges();
                                db.tbHoSo_LichSu.Add(new tbHoSo_LichSu
                                {
                                    IdHoSo = hoSo.IdHoSo,
                                    NoiDung = "Khởi tạo hồ sơ",
                                    TrangThai = 1,
                                    NguoiTao = per.NguoiDung.IdNguoiDung,
                                    NgayTao = DateTime.Now,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                });
                                db.SaveChanges();
                                System.IO.Directory.CreateDirectory(duongDanThuMuc_HOSO_SERVER); // Tạo folder cuối cùng để tránh lỗi khi tạo bản ghi
                                scope.Commit();
                            };
                        };
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
        [HttpPost]
        public ActionResult update_HoSo(string str_hoSo)
        {
            string status = "success";
            string mess = "Cập nhật bản ghi thành công";

            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string format = "dd/MM/yyyy";
                    IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    tbHoSoExtend hoSo_NEW = JsonConvert.DeserializeObject<tbHoSoExtend>(str_hoSo ?? "", dateTimeConverter);
                    // Kiểm tra Mã hồ sơ có trùng bản ghi nào khác không
                    if (kiemTra_MaHoSo(hoSo: hoSo_NEW).KiemTraExcel.TrangThai == 0)
                    {
                        status = "warning";
                        mess = hoSo_NEW.KiemTraExcel.KetQua;
                    }
                    else
                    {
                        // Hồ sơ
                        tbHoSo hoSo_OLD = db.tbHoSoes.Find(hoSo_NEW.IdHoSo);
                        #region Tạo folder lưu văn bản
                        if (hoSo_NEW.MaHoSo != hoSo_OLD.MaHoSo)
                        {
                            string duongDanThuMuc_HOSO_NEW = string.Format("{0}/{1}", $"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/HOSO/", Public.Handle.ConvertToUnSign(s: hoSo_NEW.MaHoSo, khoangCach: "_"));
                            string duongDanThuMuc_HOSO_NEW_SERVER = Request.MapPath(duongDanThuMuc_HOSO_NEW);
                            string duongDanThuMuc_HOSO_OLD_SERVER = Request.MapPath(hoSo_OLD.DuongDanFile);
                            Directory.Move(duongDanThuMuc_HOSO_OLD_SERVER, duongDanThuMuc_HOSO_NEW_SERVER);
                            hoSo_OLD.DuongDanFile = duongDanThuMuc_HOSO_NEW;
                        };
                        #endregion

                        #region Lịch sử thay đổi thông tin hồ sơ
                        List<Tuple<string, object, object>> thayDois = Public.Handle.CompareSpecificFields(obj1: hoSo_OLD, obj2: hoSo_NEW,
                            fieldsToCompare: new List<string>(),
                            fieldsToExclude: new List<string> {
                                "QuyenTruyCap", "DuongDanFile", "TrangThai", "NguoiTao", "NguoiSua", "NgayTao", "NgaySua", "MaDonViSuDung"}
                            );
                        List<HoSo_LichSu_ChiTiet> chiTiets = ChiTietThayDoi_HoSo(thayDois: thayDois);
                        db.tbHoSo_LichSu.Add(new tbHoSo_LichSu
                        {
                            IdHoSo = hoSo_OLD.IdHoSo,
                            NoiDung = "Cập nhật thông tin hồ sơ",
                            ChiTiet = JsonConvert.SerializeObject(chiTiets),
                            TrangThai = 1,
                            NguoiTao = per.NguoiDung.IdNguoiDung,
                            NgayTao = DateTime.Now,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        #endregion

                        hoSo_OLD.TieuDeHoSo = hoSo_NEW.TieuDeHoSo;
                        hoSo_OLD.QuyenTruyCap = hoSo_NEW.QuyenTruyCap;

                        hoSo_OLD.IdViTriLuuTru = hoSo_NEW.ViTriLuuTru.IdViTriLuuTru;
                        hoSo_OLD.IdPhongLuuTru = hoSo_NEW.PhongLuuTru.IdPhongLuuTru;
                        hoSo_OLD.IdDanhMucHoSo = hoSo_NEW.DanhMucHoSo.IdDanhMucHoSo;
                        hoSo_OLD.IdCheDoSuDung = hoSo_NEW.CheDoSuDung.IdCheDoSuDung;

                        hoSo_OLD.MucLucSo_NamHinhThanh = hoSo_NEW.MucLucSo_NamHinhThanh;
                        hoSo_OLD.So_KyHieu = hoSo_NEW.So_KyHieu;
                        hoSo_OLD.MaHoSo = hoSo_NEW.MaHoSo;
                        hoSo_OLD.ThoiHanBaoQuan = hoSo_NEW.ThoiHanBaoQuan;
                        hoSo_OLD.ThoiGianBatDau = hoSo_NEW.ThoiGianBatDau;
                        hoSo_OLD.ThoiGianKetThuc = hoSo_NEW.ThoiGianKetThuc;
                        hoSo_OLD.TinhTrangVatLy = hoSo_NEW.TinhTrangVatLy;
                        hoSo_OLD.NgonNgu = hoSo_NEW.NgonNgu;
                        hoSo_OLD.TuKhoa = hoSo_NEW.TuKhoa;
                        hoSo_OLD.KyHieuThongTin = hoSo_NEW.KyHieuThongTin;
                        hoSo_OLD.TongSoVanBan = hoSo_NEW.TongSoVanBan;
                        hoSo_OLD.SoLuongTo = hoSo_NEW.SoLuongTo;
                        hoSo_OLD.SoLuongTrang = hoSo_NEW.SoLuongTrang;
                        hoSo_OLD.GhiChu = hoSo_NEW.GhiChu;
                        hoSo_OLD.NguoiSua = per.NguoiDung.IdNguoiDung;
                        hoSo_OLD.NgaySua = DateTime.Now;
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Tạo lập hồ sơ",
                            ThaoTac = "Cập nhật",
                            NoiDungChiTiet = "Cập nhật hồ sơ",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
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
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult delete_HoSos()
        {
            string status = "success";
            string mess = "Xóa bản ghi thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idHoSos = Request.Form["str_idHoSos"];
                    List<int> idHoSos = str_idHoSos.Split(',').Select(Int32.Parse).ToList();
                    foreach (int idHoSo in idHoSos)
                    {
                        tbHoSo hoSo = db.tbHoSoes.Find(idHoSo);
                        #region Xóa thư mục
                        string duongDanThuMuc_HOSO_SERVER = Request.MapPath(hoSo.DuongDanFile);
                        if (System.IO.Directory.Exists(duongDanThuMuc_HOSO_SERVER)) // Kiểm tra cho chắc
                        {
                            System.IO.Directory.Delete(duongDanThuMuc_HOSO_SERVER, true);
                        };
                        #endregion
                        // Xóa bản ghi trong db
                        hoSo.TrangThai = 0;
                        hoSo.NguoiSua = per.NguoiDung.IdNguoiDung;
                        hoSo.NgaySua = DateTime.Now;
                        // Xóa các bảng thành viên
                        List<tbHoSo_VanBan> vanBans = db.tbHoSo_VanBan.Where(x => x.IdHoSo == idHoSo).ToList();
                        foreach (tbHoSo_VanBan vanBan in vanBans)
                        {
                            vanBan.TrangThai = 0;
                            vanBan.NguoiSua = per.NguoiDung.IdNguoiDung;
                            vanBan.NgaySua = DateTime.Now;
                            db.Database.ExecuteSqlCommand($@"
                            -- Xóa dữ liệu số
                            update tbHoSo_VanBan_DuLieuSo set TrangThai = 0, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' where IdVanBan = {vanBan.IdVanBan} and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                            -- Xóa phiếu mượn văn bản
                            update tbPhieuMuon_VanBan set TrangThai = 0, NguoiSua = {per.NguoiDung.IdNguoiDung}, NgaySua = '{DateTime.Now}' where IdVanBan = {vanBan.IdVanBan} and MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung}
                            ");
                        };
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                        {
                            TenModule = "Tạo lập hồ sơ",
                            ThaoTac = "Xóa",
                            NoiDungChiTiet = "Xóa hồ sơ và toàn bộ văn bản thuộc hồ sơ",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        #region Xóa lịch sử hồ sơ

                        #endregion
                        db.SaveChanges();
                    };
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
        public JsonResult nopLuu()
        {
            string status = "success";
            string mess = "Nộp lưu hồ sơ thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idHoSos = Request.Form["str_idHoSos"];
                    List<int> idHoSos = str_idHoSos.Split(',').Select(Int32.Parse).ToList();
                    void _nopLuu(tbHoSo hoSo)
                    {
                        hoSo.TrangThai = 2;
                        hoSo.NguoiSua = per.NguoiDung.IdNguoiDung;
                        hoSo.NgaySua = DateTime.Now;
                    }
                    //int soVanBan_ChuaGanBieuMau = 0;
                    foreach (int idHoSo in idHoSos)
                    {
                        tbHoSo hoSo = db.tbHoSoes.Find(idHoSo);
                        if (hoSo.TrangThai == 1)
                        {
                            // Kiểm tra toàn bộ văn bản của hồ sơ đã gán biểu mẫu chưa
                            List<tbHoSo_VanBan> vanBans = db.tbHoSo_VanBan.Where(x => x.IdHoSo == hoSo.IdHoSo).ToList() ?? new List<tbHoSo_VanBan>();
                            //List<tbHoSo_VanBan> vanBans_ChuaGanBieuMau = vanBans.Where(x => x.IdBieuMau == 0).ToList() ?? new List<tbHoSo_VanBan>();
                            //if (idHoSos.Count == 1) {
                            if (vanBans.Count == 0)
                            {
                                status = "warning";
                                mess = "Tồn tại hồ sơ chưa có văn bản, vui lòng bổ sung văn bản trước khi thực hiện nộp lưu";
                                return Json(new
                                {
                                    status,
                                    mess
                                }, JsonRequestBehavior.AllowGet);
                            };
                            _nopLuu(hoSo); // Xóa nếu mở các đoạn còn lại
                                           //if (vanBans_ChuaGanBieuMau.Count == 0) _nopLuu(hoSo);
                                           //else {
                                           //    status = "warning";
                                           //    mess = "Tồn tại văn bản chưa được gán biểu mẫu";
                                           //}
                                           //} else {
                                           //    if (vanBans_ChuaGanBieuMau.Count == 0)
                                           //        _nopLuu(hoSo);
                                           //    else soVanBan_ChuaGanBieuMau++;
                                           //};
                        };
                    };
                    //if (soVanBan_ChuaGanBieuMau == idHoSos.Count) {
                    //    status = "warning";
                    //    mess = "Các hồ sơ trên chưa được gán biểu mẫu, vui lòng bổ sung trước khi thực hiện nộp lưu";
                    //};
                    db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                    {
                        TenModule = "Quản lý hồ sơ",
                        ThaoTac = "Nộp lưu",
                        NoiDungChiTiet = "Nộp lưu hồ sơ",

                        NgayTao = DateTime.Now,
                        IdNguoiDung = per.NguoiDung.IdNguoiDung,
                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                    });
                    db.SaveChanges();
                    scope.Commit();
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
        [HttpPost]
        public JsonResult huyNopLuu()
        {
            string status = "success";
            string mess = "Hủy nộp lưu hồ sơ thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idHoSos = Request.Form["str_idHoSos"];
                    List<int> idHoSos = str_idHoSos.Split(',').Select(Int32.Parse).ToList();
                    void _nopLuu(tbHoSo hoSo)
                    {
                        hoSo.TrangThai = 1;
                        hoSo.NguoiSua = per.NguoiDung.IdNguoiDung;
                        hoSo.NgaySua = DateTime.Now;
                    }
                    //int soVanBan_ChuaGanBieuMau = 0;
                    foreach (int idHoSo in idHoSos)
                    {
                        tbHoSo hoSo = db.tbHoSoes.Find(idHoSo);
                        if (hoSo.TrangThai == 2) _nopLuu(hoSo); // Xóa nếu mở các đoạn còn lại
                        //if (vanBans_ChuaGanBieuMau.Count == 0) _nopLuu(hoSo);
                        //else {
                        //    status = "warning";
                        //    mess = "Tồn tại văn bản chưa được gán biểu mẫu";
                        //}
                        //} else {
                        //    if (vanBans_ChuaGanBieuMau.Count == 0)
                        //        _nopLuu(hoSo);
                        //    else soVanBan_ChuaGanBieuMau++;
                        //};
                    };
                    //if (soVanBan_ChuaGanBieuMau == idHoSos.Count) {
                    //    status = "warning";
                    //    mess = "Các hồ sơ trên chưa được gán biểu mẫu, vui lòng bổ sung trước khi thực hiện nộp lưu";
                    //};
                    db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                    {
                        TenModule = "Quản lý hồ sơ",
                        ThaoTac = "Hủy nộp lưu",
                        NoiDungChiTiet = "Hủy nộp lưu hồ sơ",

                        NgayTao = DateTime.Now,
                        IdNguoiDung = per.NguoiDung.IdNguoiDung,
                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                    });
                    db.SaveChanges();
                    scope.Commit();
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
        [HttpPost]
        public JsonResult thietLapQuyenHangLoat()
        {
            string status = "success";
            string mess = "Thiết lập quyền truy cập hồ sơ thành công";
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    string str_idHoSos = Request.Form["str_idHoSos"];
                    string quyenTruyCap = Request.Form["quyenTruyCap"];
                    List<int> idHoSos = str_idHoSos.Split(',').Select(Int32.Parse).ToList();
                    //int soVanBan_ChuaGanBieuMau = 0;
                    foreach (int idHoSo in idHoSos)
                    {
                        tbHoSo hoSo = db.tbHoSoes.Find(idHoSo);
                        hoSo.QuyenTruyCap = quyenTruyCap;
                    };

                    db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                    {
                        TenModule = "Tạo lập hồ sơ",
                        ThaoTac = "Thiết lập quyền hàng loạt",
                        NoiDungChiTiet = "Thiết lập quyền hàng loạt",

                        NgayTao = DateTime.Now,
                        IdNguoiDung = per.NguoiDung.IdNguoiDung,
                        MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                    });
                    db.SaveChanges();
                    scope.Commit();
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

        [HttpGet]
        public JsonResult get_NguoiDungs(int idCoCauToChuc = 0)
        {
            List<tbNguoiDung> nguoiDungs = db.tbNguoiDungs.Where(x => x.IdCoCauToChuc == idCoCauToChuc && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).OrderByDescending(x => x.NgayTao).ToList();
            return Json(new
            {
                data = nguoiDungs
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Excel - hồ sơ
        public tbHoSoExtend kiemTra_Excel_HoSo(tbHoSoExtend hoSo)
        {
            List<string> ketQuas = new List<string>();
            /**
             * Các thông tin cần kiểm tra
             * Mã hồ sơ
             * Tiêu đề hồ sơ
             * Vị trí lưu trữ
             * Danh mục hồ sơ
             * Phông lưu trữ
             * Chế độ sử dụng
             * Mục lục số
             * Số ký hiệu
             */
            if (kiemTra_MaHoSo(hoSo: hoSo).KiemTraExcel.TrangThai == 0)
            {
                ketQuas.Add(hoSo.KiemTraExcel.KetQua);
                hoSo.KiemTraExcel.TrangThai = 0;
            };
            if (hoSo.MaHoSo == "" || hoSo.TieuDeHoSo == "" ||
                hoSo.ViTriLuuTru.IdViTriLuuTru == 0 || hoSo.DanhMucHoSo.IdDanhMucHoSo == 0 ||
                hoSo.PhongLuuTru.IdPhongLuuTru == 0 || hoSo.CheDoSuDung.IdCheDoSuDung == 0 ||
                hoSo.MucLucSo_NamHinhThanh == "" || hoSo.So_KyHieu == "")
            {
                ketQuas.Add("Thiếu thông tin");
                hoSo.KiemTraExcel.TrangThai = 0;
            };
            hoSo.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            return hoSo;
        }
        [HttpPost]
        public ActionResult upload_Excel_HoSo(HttpPostedFileBase[] files)
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
                        EXCEL_HOSOs_UPLOAD = new List<tbHoSoExtend>();
                        foreach (var sheet in workSheets)
                        {
                            // Lấy dữ liệu từ sheet hồ sơ
                            if (sheet.Name.Contains("HoSo"))
                            {
                                #region Cách 1: Theo khoảng
                                //var range = sheet.RangeUsed();
                                //for (int i = 1; i < range.RowsUsed(); i ++) {
                                //    tbHoSoExtend hoSo = new tbHoSoExtend();
                                //    hoSo.MaHoSo = row[i].Cell(1).GetString();
                                //}
                                #endregion
                                #region Cách 2: Theo bảng - Đang sử dụng
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
                                        int TongSoVanBan; row.Field("Tổng số văn bản").TryGetValue<int>(out TongSoVanBan);
                                        int SoLuongTo; row.Field("Số lượng tờ").TryGetValue<int>(out SoLuongTo);
                                        int SoLuongTrang; row.Field("Số lượng trang").TryGetValue<int>(out SoLuongTrang);

                                        tbHoSoExtend hoSo = new tbHoSoExtend();
                                        // Gán quyền truy cập cho mọi tài khoản trong cùng cơ cấu
                                        var nguoiDungs_CungCoCau = NGUOIDUNGS.Where(x => x.IdCoCauToChuc == per.CoCauToChuc.IdCoCauToChuc).Select(x => x.IdNguoiDung).ToList();
                                        if (nguoiDungs_CungCoCau != null)
                                        {
                                            hoSo.QuyenTruyCap = string.Join(",", nguoiDungs_CungCoCau);
                                        };
                                        hoSo.MaHoSo = row.Field("Mã hồ sơ *").GetString();
                                        hoSo.TieuDeHoSo = row.Field("Tiêu đề hồ sơ *").GetString();
                                        hoSo.ViTriLuuTru.TenViTriLuuTru = row.Field("Vị trí lưu trữ *").GetString();
                                        hoSo.DanhMucHoSo.TenDanhMucHoSo = row.Field("Danh mục hồ sơ *").GetString();
                                        hoSo.PhongLuuTru.TenPhongLuuTru = row.Field("Phông lưu trữ *").GetString();
                                        hoSo.CheDoSuDung.TenCheDoSuDung = row.Field("Chế độ sử dụng *").GetString();
                                        hoSo.MucLucSo_NamHinhThanh = row.Field("Mục lục số/Năm hình thành *").GetString();
                                        hoSo.So_KyHieu = row.Field("Số ký hiệu *").GetString();
                                        hoSo.ThoiHanBaoQuan = row.Field("Thời hạn bảo quản").GetString();

                                        DateTime ThoiGianBatDau;
                                        DateTime ThoiGianKetThuc;
                                        string thoiGianBatDauStr = row.Field("Thời gian bắt đầu").GetString();
                                        string thoiGianKetThucStr = row.Field("Thời gian kết thúc").GetString();
                                        if (DateTime.TryParseExact(thoiGianBatDauStr, Public.Handle.DATETIMEFORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ThoiGianBatDau))
                                        {
                                            hoSo.ThoiGianBatDau = ThoiGianBatDau;
                                        };
                                        if (DateTime.TryParseExact(thoiGianKetThucStr, Public.Handle.DATETIMEFORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ThoiGianKetThuc))
                                        {
                                            hoSo.ThoiGianKetThuc = ThoiGianKetThuc;
                                        };

                                        hoSo.TinhTrangVatLy = row.Field("Tình trạng vật lý").GetString();
                                        hoSo.NgonNgu = row.Field("Ngôn ngữ").GetString();
                                        hoSo.TuKhoa = row.Field("Từ khóa").GetString();
                                        hoSo.KyHieuThongTin = row.Field("Ký hiệu thông tin").GetString();
                                        hoSo.GhiChu = row.Field("Ghi chú").GetString();

                                        hoSo.TongSoVanBan = TongSoVanBan;
                                        hoSo.SoLuongTo = SoLuongTo;
                                        hoSo.SoLuongTrang = SoLuongTrang;
                                        EXCEL_HOSOs_UPLOAD.Add(hoSo);
                                    };
                                };
                                #endregion
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
            };
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult download_Excel_HoSo()
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Tạo excel
            using (var workBook = new XLWorkbook())
            {
                #region Tạo sheet hồ sơ
                DataTable tbHoSo = new DataTable();
                tbHoSo.Columns.Add("Mã hồ sơ *", typeof(string)); // 1
                tbHoSo.Columns.Add("Tiêu đề hồ sơ *", typeof(string)); // 2

                tbHoSo.Columns.Add("Vị trí lưu trữ *", typeof(string)); // 3
                tbHoSo.Columns.Add("Danh mục hồ sơ *", typeof(string)); // 4
                tbHoSo.Columns.Add("Phông lưu trữ *", typeof(string)); // 5
                tbHoSo.Columns.Add("Chế độ sử dụng *", typeof(string)); // 6

                tbHoSo.Columns.Add("Mục lục số/Năm hình thành *", typeof(string)); // 7
                tbHoSo.Columns.Add("Số ký hiệu *", typeof(string)); // 8
                tbHoSo.Columns.Add("Thời hạn bảo quản", typeof(string)); // 9
                tbHoSo.Columns.Add("Thời gian bắt đầu", typeof(string)); // 10
                tbHoSo.Columns.Add("Thời gian kết thúc", typeof(string)); // 11
                tbHoSo.Columns.Add("Tình trạng vật lý", typeof(string)); // 12
                tbHoSo.Columns.Add("Ngôn ngữ", typeof(string)); // 13
                tbHoSo.Columns.Add("Từ khóa", typeof(string)); // 14
                tbHoSo.Columns.Add("Ký hiệu thông tin", typeof(string)); // 15
                tbHoSo.Columns.Add("Tổng số văn bản", typeof(int)); // 16
                tbHoSo.Columns.Add("Số lượng tờ", typeof(int)); // 17
                tbHoSo.Columns.Add("Số lượng trang", typeof(int)); // 18
                tbHoSo.Columns.Add("Ghi chú", typeof(string)); // 19
                #region Thêm dữ liệu
                int EXCEL_HOSOs_DOWNLOAD_COUNT = EXCEL_HOSOs_DOWNLOAD.Count;
                for (int i = 0; i < EXCEL_HOSOs_DOWNLOAD_COUNT; i++)
                {
                    tbHoSoExtend hoSo = EXCEL_HOSOs_DOWNLOAD[i];
                    tbViTriLuuTru viTriLuuTru = VITRILUUTRUS.FirstOrDefault(x => x.IdViTriLuuTru == hoSo.ViTriLuuTru.IdViTriLuuTru) ?? new tbViTriLuuTru();
                    tbDanhMucHoSo danhMucHoSo = DANHMUCHOSOS.FirstOrDefault(x => x.IdDanhMucHoSo == hoSo.DanhMucHoSo.IdDanhMucHoSo) ?? new tbDanhMucHoSo();
                    tbDonViSuDung_PhongLuuTru phongLuuTru = PHONGLUUTRUS.FirstOrDefault(x => x.IdPhongLuuTru == hoSo.PhongLuuTru.IdPhongLuuTru) ?? new tbDonViSuDung_PhongLuuTru();
                    default_tbCheDoSuDung cheDoSuDung = CHEDOSUDUNGS.FirstOrDefault(x => x.IdCheDoSuDung == hoSo.CheDoSuDung.IdCheDoSuDung) ?? new default_tbCheDoSuDung();
                    if (tbHoSo.Rows.Count <= i)
                        tbHoSo.Rows.Add(
                            hoSo.MaHoSo,
                            hoSo.TieuDeHoSo,
                            viTriLuuTru.TenViTriLuuTru,
                            danhMucHoSo.TenDanhMucHoSo,
                            phongLuuTru.TenPhongLuuTru,
                            cheDoSuDung.TenCheDoSuDung,

                            hoSo.MucLucSo_NamHinhThanh,
                            hoSo.So_KyHieu,
                            hoSo.ThoiHanBaoQuan,
                            hoSo.ThoiGianBatDau == null ? "01/01/2020" : hoSo.ThoiGianBatDau.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                            hoSo.ThoiGianKetThuc == null ? "01/01/2020" : hoSo.ThoiGianKetThuc.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                            hoSo.TinhTrangVatLy,
                            hoSo.NgonNgu,
                            hoSo.TuKhoa,
                            hoSo.KyHieuThongTin,
                            hoSo.TongSoVanBan,
                            hoSo.SoLuongTo,
                            hoSo.SoLuongTrang,
                            hoSo.GhiChu);
                    //tbHoSo.Rows.Add(tbHoSo.NewRow());
                };
                #endregion
                #endregion
                #region Tạo sheet danh sách
                DataTable tbDanhSach = new DataTable();
                tbDanhSach.Columns.Add("TenViTruLuuTru", typeof(string));
                tbDanhSach.Columns.Add("TenDanhMucHoSo", typeof(string));
                tbDanhSach.Columns.Add("TenPhongLuuTru", typeof(string));
                tbDanhSach.Columns.Add("TenCheDoSuDung", typeof(string));
                #region Thêm dữ liệu
                int VITRILUUTRUS_Count = VITRILUUTRUS.Count;
                int DANHMUCHOSOS_Count = DANHMUCHOSOS.Count;
                int PHONGLUUTRUS_Count = PHONGLUUTRUS.Count;
                int CHEDOSUDUNGS_Count = CHEDOSUDUNGS.Count;
                for (int i = 0; i < VITRILUUTRUS_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][0] = VITRILUUTRUS[i].TenViTriLuuTru;
                };
                for (int i = 0; i < DANHMUCHOSOS_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][1] = DANHMUCHOSOS[i].TenDanhMucHoSo;
                };
                for (int i = 0; i < PHONGLUUTRUS_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][2] = PHONGLUUTRUS[i].TenPhongLuuTru;
                };
                for (int i = 0; i < CHEDOSUDUNGS_Count; i++)
                {
                    if (tbDanhSach.Rows.Count <= i)
                        tbDanhSach.Rows.Add(tbDanhSach.NewRow());
                    tbDanhSach.Rows[i][3] = CHEDOSUDUNGS[i].TenCheDoSuDung;
                };
                #endregion
                #endregion
                #region Tạo file excel
                workBook.Worksheets.Add(tbHoSo, "HoSo");
                workBook.Worksheets.Add(tbDanhSach, "DanhSach");
                tbHoSo.TableName = ""; tbDanhSach.TableName = "";
                for (int i = 1; i <= tbHoSo.Rows.Count + 1; i++)
                {
                    // List
                    workBook.Worksheets.First().Cell(i, 3).CreateDataValidation().List($"=OFFSET(DanhSach!$A$2,0,0,COUNTA(DanhSach!$A:$A),1)");
                    workBook.Worksheets.First().Cell(i, 4).CreateDataValidation().List($"=OFFSET(DanhSach!$B$2,0,0,COUNTA(DanhSach!$B:$B),1)");
                    workBook.Worksheets.First().Cell(i, 5).CreateDataValidation().List($"=OFFSET(DanhSach!$C$2,0,0,COUNTA(DanhSach!$C:$C),1)");
                    workBook.Worksheets.First().Cell(i, 6).CreateDataValidation().List($"=OFFSET(DanhSach!$D$2,0,0,COUNTA(DanhSach!$D:$D),1)");
                    // Date
                    //workBook.Worksheets.First().Cell(i, 10).CreateDataValidation().Date.GreaterThan(new DateTime(1990, 1, 1));
                    //workBook.Worksheets.First().Cell(i, 11).CreateDataValidation().Date.GreaterThan(new DateTime(1990, 1, 1));
                    // Number
                    workBook.Worksheets.First().Cell(i, 16).CreateDataValidation().Decimal.GreaterThan(0);
                    workBook.Worksheets.First().Cell(i, 17).CreateDataValidation().Decimal.GreaterThan(0);
                    workBook.Worksheets.First().Cell(i, 18).CreateDataValidation().Decimal.GreaterThan(0);
                }
                #endregion
                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                workBook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode("HOSO.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/DocumentFormation/Index");
        }
        public ActionResult save_Excel_HoSo()
        {
            string status = "";
            string mess = "";
            List<tbHoSoExtend> hoSo_HopLes = new List<tbHoSoExtend>();
            List<tbHoSoExtend> hoSo_KhongHopLes = new List<tbHoSoExtend>();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (EXCEL_HOSOs_DOWNLOAD.Count == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        foreach (tbHoSoExtend hoSo_NEW in EXCEL_HOSOs_DOWNLOAD)
                        {
                            // Kiểm tra excel
                            tbHoSoExtend hoSo_KhongHopLe = kiemTra_Excel_HoSo(hoSo_NEW);
                            if (hoSo_KhongHopLe.KiemTraExcel.TrangThai == 0)
                            {
                                hoSo_KhongHopLes.Add(hoSo_KhongHopLe);
                            }
                            else
                            {
                                // Kiểm tra mã hồ sơ này đã được thêm ở bản ghi trước đó chưa
                                if (hoSo_HopLes.Any(x => x.MaHoSo == hoSo_NEW.MaHoSo))
                                {
                                    hoSo_NEW.KiemTraExcel.TrangThai = 0;
                                    hoSo_NEW.KiemTraExcel.KetQua = "Trùng mã hồ sơ";
                                    hoSo_KhongHopLes.Add(hoSo_NEW);
                                }
                                else
                                {
                                    // Tạo folder lưu văn bản
                                    string duongDanThuMuc_HOSO = string.Format("{0}/{1}", $"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/HOSO/", hoSo_NEW.MaHoSo);
                                    string duongDanThuMuc_HOSO_SERVER = Request.MapPath(duongDanThuMuc_HOSO);
                                    if (System.IO.Directory.Exists(duongDanThuMuc_HOSO_SERVER))
                                    {
                                        System.IO.Directory.Delete(duongDanThuMuc_HOSO_SERVER, true);
                                    }
                                    else
                                    {
                                        // Tạo hồ sơ
                                        tbHoSo hoSo = new tbHoSo
                                        {
                                            MaHoSo = hoSo_NEW.MaHoSo,
                                            TieuDeHoSo = hoSo_NEW.TieuDeHoSo,
                                            QuyenTruyCap = hoSo_NEW.QuyenTruyCap,
                                            DuongDanFile = duongDanThuMuc_HOSO,

                                            IdViTriLuuTru = hoSo_NEW.ViTriLuuTru.IdViTriLuuTru,
                                            IdPhongLuuTru = hoSo_NEW.PhongLuuTru.IdPhongLuuTru,
                                            IdDanhMucHoSo = hoSo_NEW.DanhMucHoSo.IdDanhMucHoSo,
                                            IdCheDoSuDung = hoSo_NEW.CheDoSuDung.IdCheDoSuDung,

                                            MucLucSo_NamHinhThanh = hoSo_NEW.MucLucSo_NamHinhThanh,
                                            So_KyHieu = hoSo_NEW.So_KyHieu,
                                            ThoiHanBaoQuan = hoSo_NEW.ThoiHanBaoQuan,
                                            ThoiGianBatDau = hoSo_NEW.ThoiGianBatDau,
                                            ThoiGianKetThuc = hoSo_NEW.ThoiGianKetThuc,
                                            TinhTrangVatLy = hoSo_NEW.TinhTrangVatLy,
                                            NgonNgu = hoSo_NEW.NgonNgu,
                                            TuKhoa = hoSo_NEW.TuKhoa,
                                            KyHieuThongTin = hoSo_NEW.KyHieuThongTin,
                                            TongSoVanBan = hoSo_NEW.TongSoVanBan,
                                            SoLuongTo = hoSo_NEW.SoLuongTo,
                                            SoLuongTrang = hoSo_NEW.SoLuongTrang,
                                            GhiChu = hoSo_NEW.GhiChu,

                                            TrangThai = 1,
                                            NguoiTao = per.NguoiDung.IdNguoiDung,
                                            NgayTao = DateTime.Now,
                                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                        };
                                        db.tbHoSoes.Add(hoSo);
                                        System.IO.Directory.CreateDirectory(duongDanThuMuc_HOSO_SERVER); // Tạo folder cuối cùng để tránh lỗi khi tạo bản ghi
                                        hoSo_HopLes.Add(hoSo_NEW);
                                    };
                                };
                            };
                        };
                        if (hoSo_KhongHopLes.Count == 0)
                        { // Thêm bản ghi thành công và không tồn tại bản ghi không hợp lệ
                            status = "success";
                            mess = "Thêm mới bản ghi thành công";
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Tạo lập hồ sơ",
                                ThaoTac = "Thêm mới",
                                NoiDungChiTiet = "Thêm mới bằng tệp",

                                NgayTao = DateTime.Now,
                                IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            });
                            db.SaveChanges();
                            scope.Commit();

                        }
                        else
                        { // Khi thêm thành công, thay thế EXCEL_HOSOs_UPLOAD bằng hoSo_KhongHopLes
                            if (hoSo_KhongHopLes.Count == EXCEL_HOSOs_DOWNLOAD.Count)
                            { // Tất cả đều không hợp lệ
                                status = "error-1";
                                mess = "Thêm mới bản ghi không thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                            }
                            else
                            {
                                status = "warning";
                                mess = "Thêm mới bản ghi [HỢP LỆ] thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                                db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                                {
                                    TenModule = "Tạo lập hồ sơ",
                                    ThaoTac = "Thêm mới",
                                    NoiDungChiTiet = "Thêm mới bằng tệp",

                                    NgayTao = DateTime.Now,
                                    IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                });
                                db.SaveChanges();
                                scope.Commit();
                            };
                            // Trả lại danh sách bản ghi không hợp lệ
                            EXCEL_HOSOs_UPLOAD = new List<tbHoSoExtend>();
                            EXCEL_HOSOs_UPLOAD.AddRange(hoSo_KhongHopLes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = "error-0";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new
            {
                hoSo_KhongHopLes,
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public void get_HoSos_download()
        {
            string loaiTaiXuong = Request.Form["loaiTaiXuong"];
            if (loaiTaiXuong == "saoluu")
            {
                List<tbHoSoExtend> hoSos = db.Database.SqlQuery<tbHoSoExtend>($@"
                select * from tbHoSo 
                where 
                    MaDonViSuDung = {per.DonViSuDung.MaDonViSuDung} and TrangThai in (1,2)
                order by NgayTao desc").ToList() ?? new List<tbHoSoExtend>();
                foreach (tbHoSoExtend hoSo in hoSos)
                {
                    hoSo.ViTriLuuTru = new tbViTriLuuTru
                    {
                        IdViTriLuuTru = hoSo.IdViTriLuuTru.Value
                    };
                    hoSo.DanhMucHoSo = new tbDanhMucHoSo
                    {
                        IdDanhMucHoSo = hoSo.IdDanhMucHoSo.Value
                    };
                    hoSo.PhongLuuTru = new tbDonViSuDung_PhongLuuTru
                    {
                        IdPhongLuuTru = hoSo.IdPhongLuuTru.Value
                    };
                    hoSo.CheDoSuDung = new default_tbCheDoSuDung
                    {
                        IdCheDoSuDung = hoSo.IdCheDoSuDung.Value
                    };
                };
                EXCEL_HOSOs_DOWNLOAD = hoSos;
            }
            else
            {
                string str_hoSos = Request.Form["str_hoSos"];
                EXCEL_HOSOs_DOWNLOAD = new List<tbHoSoExtend>();
                EXCEL_HOSOs_DOWNLOAD.Add(new tbHoSoExtend
                {
                    MaHoSo = "Mã hồ sơ",
                    ThoiHanBaoQuan = "Vĩnh viễn",
                    ViTriLuuTru = VITRILUUTRUS.FirstOrDefault() ?? new tbViTriLuuTru(),
                    DanhMucHoSo = DANHMUCHOSOS.FirstOrDefault() ?? new tbDanhMucHoSo(),
                    PhongLuuTru = PHONGLUUTRUS.FirstOrDefault() ?? new tbDonViSuDung_PhongLuuTru(),
                    CheDoSuDung = CHEDOSUDUNGS.FirstOrDefault() ?? new default_tbCheDoSuDung(),
                    ThoiGianBatDau = DateTime.Now,
                    ThoiGianKetThuc = DateTime.Now,
                    TongSoVanBan = 0,
                    SoLuongTo = 0,
                    SoLuongTrang = 0,
                });
                if (loaiTaiXuong == "data")
                {
                    string format = "dd/MM/yyyy";
                    IsoDateTimeConverter dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                    EXCEL_HOSOs_DOWNLOAD = JsonConvert.DeserializeObject<List<tbHoSoExtend>>(str_hoSos ?? "", dateTimeConverter) ?? new List<tbHoSoExtend>();
                };
            };
        }
        public ActionResult getList_Excel_HoSo(string loai)
        {
            if (loai == "reload") EXCEL_HOSOs_UPLOAD = new List<tbHoSoExtend>();
            return PartialView($"{VIEW_PATH}/documentformation-excel.hoso/excel.hoso-getList.cshtml");
        }
        #endregion
        #region Excel - dữ liệu số
        #endregion
        #region zip/rar - văn bản
        public tbHoSoExtend kiemTra_File_VanBan(tbHoSoExtend hoSo, string hinhThucNapDuLieu)
        {
            List<string> ketQuas = new List<string>();
            tbHoSo hoSo_OLD = db.tbHoSoes.FirstOrDefault(x => x.MaHoSo == hoSo.MaHoSo && x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
            // Kiểm tra mã hồ sơ
            if (hoSo_OLD == null)
            {
                if (hinhThucNapDuLieu == "capnhat")
                {
                    ketQuas.Add("Mã hồ sơ không tồn tại");
                    hoSo.KiemTraExcel.TrangThai = 0;
                };
            }
            else if (hoSo_OLD.TrangThai == 2)
            {
                if (hinhThucNapDuLieu == "themmoi" ||
                    hinhThucNapDuLieu == "capnhat" ||
                    hinhThucNapDuLieu == "themmoi&capnhat")
                {
                    ketQuas.Add("Hồ sơ đã nộp lưu");
                    hoSo.KiemTraExcel.TrangThai = 0;
                };
            }
            else
            {
                if (hinhThucNapDuLieu == "themmoi")
                {
                    ketQuas.Add("Mã hồ sơ đã tồn tại");
                    hoSo.KiemTraExcel.TrangThai = 0;
                }
                else
                {
                    // Lấy thông tin đã có sẵn
                    hoSo.IdHoSo = hoSo_OLD.IdHoSo;
                    hoSo.DuongDanFile = hoSo_OLD.DuongDanFile;
                };
            };
            // Kiểm tra loại tệp
            //doc|docx|xls|xlsx|pdf|png|jpg|jpeg|mp4
            string danhSachTep = ".doc|.docx|.xls|.xlsx|.pdf|.png|.jpg|.jpeg|.mp4";
            var vanBan = hoSo.VanBans.FirstOrDefault();
            if (!danhSachTep.Split('|').Contains(Path.GetExtension(vanBan.TenVanBan)))
            {
                ketQuas.Add("Chỉ nhận các tệp có đuôi (doc|docx|xls|xlsx|pdf|png|jpg|jpeg|mp4)");
                hoSo.KiemTraExcel.TrangThai = 0;
            };
            // Kiểm tra tên tệp
            if (vanBan.TenVanBan.Length > 80)
            {
                ketQuas.Add("Tên tệp vượt quá 80 ký tự");
                hoSo.KiemTraExcel.TrangThai = 0;
            };
            hoSo.KiemTraExcel.KetQua = string.Join(", ", ketQuas);
            return hoSo;
        }
        [HttpPost]
        public ActionResult upload_File_VanBan(HttpPostedFileBase[] files)
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
                        EXCEL_TEPVANBANs_UPLOAD = new List<tbHoSoExtend>();
                        #region Giải nén vào thư mục
                        // Tạo thư mục chứa file giải nén
                        string thuMucGiaiNen = $"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/GIAINEN_VANBAN/";
                        string thuMucGiaiNen_SERVER = Request.MapPath(thuMucGiaiNen);
                        if (System.IO.Directory.Exists(thuMucGiaiNen_SERVER))
                            System.IO.Directory.Delete(thuMucGiaiNen_SERVER, true);
                        System.IO.Directory.CreateDirectory(thuMucGiaiNen_SERVER);
                        // Giải nén tệp ZIP vào thư mục
                        using (var stream = f.InputStream)
                        {
                            using (var archive = ArchiveFactory.Open(stream))
                            {
                                foreach (var entry in archive.Entries)
                                {
                                    /**
                                     * Quy tắc giải nén:
                                     * Đọc từ tệp sâu nhất quay về gốc
                                     * Đọc từ file rồi mới tới folder
                                     * => Loại bỏ trường hợp đọc là folder vì khi tạo file đã tạo rồi không cần tạo lại
                                     * => Kiểm tra nếu file nằm trong folder lồng nhau thì tạo folder trước rồi lưu file vào
                                     */
                                    if (!entry.IsDirectory)
                                    {
                                        string entryFileName = Path.GetFileName(entry.Key);
                                        string entryDirectory = Path.GetDirectoryName(entry.Key);
                                        string fullPath = Path.Combine(thuMucGiaiNen_SERVER, entryDirectory, entryFileName);
                                        // Lấy thông tin tệp
                                        tbHoSoExtend hoSo = new tbHoSoExtend()
                                        {
                                            MaHoSo = Path.GetFileName(entryDirectory),
                                            VanBans = new List<tbHoSo_VanBanExtend>() {
                                                new tbHoSo_VanBanExtend {
                                                    TenVanBan = entryFileName,
                                                    Loai = Path.GetExtension(entryFileName),
                                                    DuongDan = Path.Combine(thuMucGiaiNen, entryDirectory, entryFileName),
                                                }
                                            }
                                        };
                                        // Thêm văn bản vào hồ sơ
                                        tbHoSoExtend hoSo_DaTonTai = EXCEL_TEPVANBANs_UPLOAD.FirstOrDefault(x => x.MaHoSo == hoSo.MaHoSo);
                                        if (hoSo_DaTonTai != null) hoSo_DaTonTai.VanBans.AddRange(hoSo.VanBans);
                                        else EXCEL_TEPVANBANs_UPLOAD.Add(hoSo);
                                        // Nếu là thư mục và thư mục chưa tồn tại thì tạo thư mục tương tự và giải nén vào
                                        string dirPath_SERVER_COPY = Path.Combine(thuMucGiaiNen_SERVER, entryDirectory);
                                        if (!System.IO.Directory.Exists(dirPath_SERVER_COPY))
                                            Directory.CreateDirectory(dirPath_SERVER_COPY);
                                        entry.WriteToFile(fullPath, new ExtractionOptions()
                                        {
                                            ExtractFullPath = true,
                                            Overwrite = true
                                        });
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
            };
            return Json(new
            {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getList_File_VanBan(string loai)
        {
            if (loai == "reload") EXCEL_TEPVANBANs_UPLOAD = new List<tbHoSoExtend>();
            return PartialView($"{VIEW_PATH}/documentformation-file.vanban/file.vanban-getList.cshtml");
        }
        public void get_File_VanBan_download()
        {
            string loaiTaiXuong = Request.Form["loaiTaiXuong"];
            string str_file_vanBans = Request.Form["str_file_vanBans"];
            EXCEL_TEPVANBANs_DOWNLOAD = new List<tbHoSoExtend>();
            if (loaiTaiXuong == "data")
            {
                EXCEL_TEPVANBANs_DOWNLOAD = JsonConvert.DeserializeObject<List<tbHoSoExtend>>(str_file_vanBans) ?? new List<tbHoSoExtend>();
            };
        }
        [HttpPost]
        public ActionResult save_File_VanBan()
        {
            string status = "";
            string mess = "";
            string hinhThucNapDuLieu = Request.Form["hinhthucnapdulieu"];
            List<tbHoSoExtend> hoSo_HopLes = new List<tbHoSoExtend>();
            List<tbHoSoExtend> hoSo_KhongHopLes = new List<tbHoSoExtend>();
            using (var scope = db.Database.BeginTransaction())
            {
                try
                {
                    if (EXCEL_TEPVANBANs_UPLOAD.Count == 0)
                    {
                        status = "error";
                        mess = "Chưa có bản ghi nào";
                    }
                    else
                    {
                        foreach (tbHoSoExtend hoSo in EXCEL_TEPVANBANs_UPLOAD)
                        {
                            // Kiểm tra excel
                            tbHoSoExtend hoSo_KhongHopLe = kiemTra_File_VanBan(hoSo: hoSo, hinhThucNapDuLieu: hinhThucNapDuLieu);
                            if (hoSo_KhongHopLe.KiemTraExcel.TrangThai == 0)
                            {
                                hoSo_KhongHopLes.Add(hoSo_KhongHopLe);
                            }
                            else
                            {
                                #region Kiểm tra theo hình thức
                                if (hinhThucNapDuLieu == "themmoi") // Không thay đổi các bản ghi đã tồn tại
                                {
                                    var ketQua = create_HoSo(str_hoSo: "");
                                }
                                else if (hinhThucNapDuLieu == "capnhat") // Chỉ thay đổi các bản ghi đã tồn tại
                                {

                                }
                                else
                                {

                                }
                                #endregion

                                var vanBans_NEW = hoSo.VanBans;
                                foreach (var vanBan_NEW in vanBans_NEW)
                                {
                                    /**
                                     * Mô tả sơ đồ tệp
                                     * Ghi chú: 
                                     *  - ext: loại tệp (doc|docx|xls|xlsx|png|jpg|jpeg|mp4) không bao gồm pdf
                                     * ---------------------------------------------
                                     * (1) |(2)   |(3)      |(4)         |(5)
                                     * HoSo|
                                     *     |VanBan|VanBan.ext
                                     *            |CHUYENDOI|[.docx]     |VanBan.pdf
                                     *                      |[.xlsx]     |VanBan.pdf
                                     *                      ...
                                     *            |PHIEUMUON|(Mã phiếu 1)|VanBan.pdf
                                     *                      |(Mã phiếu 2)|VanBan.pdf
                                     *                      ...
                                     */
                                    string tenVanBan_BANDAU = Path.GetFileName(vanBan_NEW.TenVanBan);
                                    var duongDanVanBan = LayDuongDanTep(duongDanHoSo: hoSo.DuongDanFile, tenVanBan_BANDAU: tenVanBan_BANDAU);

                                    #region Nếu chưa có văn bản này thì tạo mới trong db
                                    tbHoSo_VanBan vanBan_OLD = db.tbHoSo_VanBan.FirstOrDefault(x =>
                                    x.IdHoSo == hoSo.IdHoSo &&
                                    x.TenVanBan == duongDanVanBan.tenVanBan_KHONGDAU_KHONGLOAI && x.Loai == duongDanVanBan.loaiVanBan &&
                                    x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung);
                                    if (vanBan_OLD != null)
                                    {
                                        vanBan_OLD.TrangThai = 1; // Khôi phục dữ liệu này nếu đang xóa
                                    }
                                    else
                                    {
                                        tbHoSo_VanBan vanBan = new tbHoSo_VanBan
                                        {
                                            IdHoSo = hoSo.IdHoSo,
                                            IdBieuMau = 0,
                                            TenVanBan = duongDanVanBan.tenVanBan_KHONGDAU_KHONGLOAI,
                                            TenVanBan_BanDau = Path.GetFileNameWithoutExtension(tenVanBan_BANDAU),
                                            Loai = duongDanVanBan.loaiVanBan,
                                            TrangThai = 1,
                                            NguoiTao = per.NguoiDung.IdNguoiDung,
                                            NgayTao = DateTime.Now,
                                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                        };
                                        db.tbHoSo_VanBan.Add(vanBan);
                                    };
                                    #endregion

                                    #region Sao chép văn bản sang thư mục hồ sơ
                                    // Kiểm tra thư mục hồ sơ
                                    if (!System.IO.Directory.Exists(duongDanVanBan.duongDanThuMuc_BANDAU_SERVER))
                                        System.IO.Directory.CreateDirectory(duongDanVanBan.duongDanThuMuc_BANDAU_SERVER);
                                    // (Nếu có rồi thì xóa)
                                    if (System.IO.File.Exists(duongDanVanBan.duongDanVanBan_BANDAU_SERVER))
                                        System.IO.File.Delete(duongDanVanBan.duongDanVanBan_BANDAU_SERVER);
                                    // Sao chép tệp
                                    string duongDanVanBan_COPY = string.Format("{0}/{1}", duongDanVanBan.duongDanThuMuc_BANDAU, vanBan_NEW.TenVanBan);
                                    string duongDanVanBan_COPY_SERVER = Request.MapPath(duongDanVanBan_COPY);
                                    // 2 văn bản phải cùng tên nên phải có dấu
                                    System.IO.File.Copy(Request.MapPath(vanBan_NEW.DuongDan), duongDanVanBan_COPY_SERVER, true);
                                    // Đôi tên tệp thành đúng định dạng không dấu
                                    System.IO.File.Move(duongDanVanBan_COPY_SERVER, duongDanVanBan.duongDanVanBan_BANDAU_SERVER);
                                    #endregion

                                    #region Chuyển đổi file office thành dạng PDF
                                    if (duongDanVanBan.loaiVanBan.Contains("xls") || duongDanVanBan.loaiVanBan.Contains("doc"))
                                    {
                                        #region Bước 1: Tạo thư mục lưu PDF
                                        if (!System.IO.Directory.Exists(duongDanVanBan.duongDanThuMuc_CHUYENDOI_SERVER))
                                        {
                                            System.IO.Directory.CreateDirectory(duongDanVanBan.duongDanThuMuc_CHUYENDOI_SERVER);
                                        };
                                        #endregion
                                        #region Bước 2: Chuyển đổi sang PDF
                                        if (duongDanVanBan.loaiVanBan.Contains("xls"))
                                        { // Excel
                                          // Tạo một đối tượng Workbook từ tập tin Excel
                                            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(duongDanVanBan.duongDanVanBan_BANDAU_SERVER);
                                            foreach (var worksheet in workbook.Worksheets)
                                            {
                                                // Cho các cell vừa với text bên trong
                                                worksheet.AutoFitColumns();
                                                // Căn lề
                                                worksheet.PageSetup.LeftMargin = 1;
                                                worksheet.PageSetup.RightMargin = 1;
                                                // Cho chiều dài bảng tối đa 2 lề
                                                worksheet.AutoFitRows();
                                            };
                                            // Tạo đối tượng PdfSaveOptions để cấu hình định dạng PDF
                                            Aspose.Cells.PdfSaveOptions pdfSaveOptions = new Aspose.Cells.PdfSaveOptions();
                                            pdfSaveOptions.OnePagePerSheet = true; // Cho các sheet vào chung 1 trang pdf
                                                                                   // Lưu PDF vào thư mục
                                            workbook.Save(duongDanVanBan.duongDanVanBan_CHUYENDOI_SERVER, pdfSaveOptions);
                                        }
                                        else
                                        { // Word
                                          // Tạo đối tượng Document từ tệp Word
                                            Aspose.Words.Document document = new Aspose.Words.Document(duongDanVanBan.duongDanVanBan_BANDAU_SERVER);
                                            // Chuyển đổi tài liệu Word sang PDF và lưu vào tệp PDF mới
                                            document.Save(duongDanVanBan.duongDanVanBan_CHUYENDOI_SERVER);
                                        };
                                        #endregion
                                    };
                                    #endregion
                                };

                                hoSo_HopLes.Add(hoSo);
                            };
                        };
                        if (hoSo_KhongHopLes.Count == 0)
                        { // Thêm bản ghi thành công và không tồn tại bản ghi không hợp lệ
                            // Xóa thư mục giải nén
                            System.IO.Directory.Delete(Request.MapPath($"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/GIAINEN_VANBAN/"), true);
                            status = "success";
                            mess = "Thêm mới bản ghi thành công";
                            db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                            {
                                TenModule = "Tạo lập hồ sơ",
                                ThaoTac = "Thêm mới",
                                NoiDungChiTiet = "Thêm mới bằng tệp",

                                NgayTao = DateTime.Now,
                                IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                            });
                            db.SaveChanges();
                            scope.Commit();
                        }
                        else
                        { // Khi thêm thành công, thay thế EXCEL_HOSOs_UPLOAD bằng fileVanBan_KhongHopLes
                            if (hoSo_KhongHopLes.Count == EXCEL_TEPVANBANs_UPLOAD.Count)
                            { // Tất cả đều không hợp lệ
                                status = "error-1";
                                mess = "Thêm mới bản ghi không thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                            }
                            else
                            {
                                status = "warning";
                                mess = "Thêm mới bản ghi [HỢP LỆ] thành công, vui lòng kiểm tra lại những bản ghi [CHƯA HỢP LỆ]";
                                db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap
                                {
                                    TenModule = "Tạo lập hồ sơ",
                                    ThaoTac = "Thêm mới tệp văn bản",
                                    NoiDungChiTiet = "Thêm mới bằng tệp",

                                    NgayTao = DateTime.Now,
                                    IdNguoiDung = per.NguoiDung.IdNguoiDung,
                                    MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                                });
                                db.SaveChanges();
                                scope.Commit();
                            };
                            // Trả lại danh sách bản ghi không hợp lệ
                            EXCEL_TEPVANBANs_UPLOAD = new List<tbHoSoExtend>();
                            EXCEL_TEPVANBANs_UPLOAD.AddRange(hoSo_KhongHopLes);
                        }
                    }
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
                hoSo_KhongHopLes,
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Ký số hàng loạt
        [HttpPost]
        public ActionResult displayModal_KySoHangLoat()
        {
            string str_idHoSos = Request.Form["str_idHoSos"];
            List<tbHoSoExtend> hoSos = get_HoSos(loai: "single", idHoSos: str_idHoSos);
            ViewBag.hoSos = hoSos.Where(x => x.VanBans.Count > 0).ToList();
            return PartialView($"{VIEW_PATH}/documentformation-kysohangloat/kysohangloat.cshtml");
        }
        [HttpPost]
        public JsonResult layDuongDanVanBanKySo()
        {
            string status = "success";
            try
            {
                string str_idVanBans = Request.Form["str_idVanBans"];
                //List<int>
            }
            catch (Exception ex)
            {
                status = "error";
            }
            return Json(new
            {

                status,
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Sao lưu hồ sơ - văn bản
        public ActionResult saoLuu()
        {
            string status = "success";
            string mess = "Sao lưu dữ liệu thành công";

            #region Tạo đường dẫn
            /** Mô tả đường dẫn
             * IdNguoiDung|ThuMucSaoLuu.zip
             *            |ThuMucSaoLuu    |ThongTinHoSo.xlsx (không tải cùng nữa)
             *                             |ThuMucVanBan     |MaHoSo1
             *                                               |MaHoSo2
             */
            string duongDanGoc = string.Format("/Assets/uploads/{0}/SAOLUU/{1}", per.DonViSuDung.MaDonViSuDung, per.NguoiDung.IdNguoiDung);
            string duongDanGoc_SERVER = Request.MapPath(duongDanGoc);

            string duongDan_TepSaoLuu = string.Format("{0}/ThuMucSaoLuu.zip", duongDanGoc);
            string duongDan_TepSaoLuu_SERVER = Request.MapPath(duongDan_TepSaoLuu);

            string duongDan_ThuMucSaoLuu = string.Format("{0}/ThuMucSaoLuu", duongDanGoc);
            string duongDan_ThuMucSaoLuu_SERVER = Request.MapPath(duongDan_ThuMucSaoLuu);

            string duongDan_ThuMucVanBan_SaoLuu = string.Format("{0}/ThuMucVanBan", duongDan_ThuMucSaoLuu);
            string duongDan_ThuMucVanBan_SaoLuu_SERVER = Request.MapPath(duongDan_ThuMucSaoLuu);

            if (Directory.Exists(duongDanGoc_SERVER)) Directory.Delete(duongDanGoc_SERVER, true);
            Directory.CreateDirectory(duongDan_ThuMucVanBan_SaoLuu_SERVER);
            #endregion

            try
            {
                #region Cách 1: Lấy danh sách hồ sơ và sao lưu (Không dùng)
                //string str_idHoSos = Request.Form["str_idHoSos"];
                //List<int> idHoSos = str_idHoSos.Split(',').Select(x => int.Parse(x)).ToList();
                //List<tbHoSoExtend> hoSos = get_HoSos(loai: "all").Where(x => idHoSos.Contains(x.IdHoSo)).ToList();
                //List<tbHoSoExtend> hoSos = get_HoSos(loai: "all").ToList();

                //foreach (tbHoSoExtend hoSo in hoSos)
                //{
                //    #region Excel - Hồ sơ

                //    #endregion

                //    #region Văn bản
                //    hoSo.VanBans = get_VanBans(hoSo: hoSo, loai: "all");
                //    // Tạo thư mục hồ sơ
                //    string duongDan_MaHoSo_SAOLUU = string.Format("{0}/{1}", duongDan_ThuMucVanBan_SaoLuu, hoSo.MaHoSo);
                //    string duongDan_MaHoSo_SAOLUU_SERVER = Request.MapPath(duongDan_MaHoSo_SAOLUU);
                //    Directory.CreateDirectory(duongDan_MaHoSo_SAOLUU_SERVER);
                //    // Chỉ lấy tệp đang sử dụng để hiển thị trên hệ thống và sao chép vào thư mục hồ sơ
                //    List<tbHoSo_VanBanExtend> vanBans = hoSo.VanBans;
                //    foreach (tbHoSo_VanBanExtend vanBan in vanBans)
                //    {
                //        string duongDan_VanBan_SAOLUU = string.Format("{0}/{1}{2}", duongDan_MaHoSo_SAOLUU, vanBan.TenVanBan, Path.GetExtension(vanBan.DuongDan));
                //        string duongDan_VanBan_SAOLUU_SERVER = Request.MapPath(duongDan_VanBan_SAOLUU);
                //        string duongDan_VanBan_GOC_SERVER = Request.MapPath(vanBan.DuongDan);
                //        if (System.IO.File.Exists(duongDan_VanBan_GOC_SERVER))
                //            System.IO.File.Copy(duongDan_VanBan_GOC_SERVER, duongDan_VanBan_SAOLUU_SERVER);
                //    };
                //    #endregion
                //};

                #region Nén tệp sao lưu
                //MemoryStream memoryStream = new MemoryStream();
                //ZipFile.CreateFromDirectory(duongDan_ThuMucVanBan_SaoLuu_SERVER, duongDanGoc_SERVER);
                //using (var zip = ArchiveFactory.Create(ArchiveType.Rar))
                //{
                //    // Thêm tất cả các tệp và thư mục từ thư mục gốc vào tệp zip
                //    zip.AddAllFromDirectory(duongDan_ThuMucVanBan_SaoLuu_SERVER);
                //    // Lưu tệp zip vào thư mục
                //    //zip.WriteToDirectory(destinationDirectory: duongDanGoc_SERVER, new ExtractionOptions { Overwrite = true, ExtractFullPath = true });

                //    zip.SaveTo(stream: memoryStream, options: new SharpCompress.Writers.WriterOptions(CompressionType.Rar));
                //    memoryStream.Position = 0;
                //    downloadDialog(data: memoryStream, fileName: Server.UrlEncode("ThuMucSaoLuu.zip"), contentType: "application/zip");
                //};
                #endregion
                #endregion

                #region Cách 2: Lấy danh sách hồ sơ và sao lưu
                string duongDanThuMuc_HOSO = string.Format("/Assets/uploads/{0}/HOSO", per.DonViSuDung.MaDonViSuDung);
                ZipFile.CreateFromDirectory(sourceDirectoryName: Request.MapPath(duongDanThuMuc_HOSO), destinationArchiveFileName: duongDan_TepSaoLuu_SERVER);
                #endregion
            }
            catch (Exception ex)
            {
                status = "error";
                mess = "Sao lưu dữ liệu thất bại";
            };
            return Json(new
            {
                duongDan_TepSaoLuu,
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Tools
        // Sửa sai vị trí tệp văn bản
        public ActionResult DoIt_1()
        {
            string duongDan_BanDau = "C:\\Users\\Admin\\Downloads\\2\\2\\HOSO";
            DirectoryInfo thuMuc_BanDau = new DirectoryInfo(duongDan_BanDau);
            DirectoryInfo[] thuMucs = thuMuc_BanDau.GetDirectories();
            foreach (DirectoryInfo thuMuc in thuMucs)
            {
                FileInfo[] teps = thuMuc.GetFiles();
                foreach (FileInfo tep in teps)
                {
                    string duongDan_ThuMuc_BanDau = string.Format("{0}/{1}", duongDan_BanDau, thuMuc.Name);
                    string duongDan_Tep_BanDau = string.Format("{0}/{1}", duongDan_ThuMuc_BanDau, tep.Name);
                    string duongDan_ThuMuc_SaoChep = string.Format("{0}/{1}[.pdf]", duongDan_ThuMuc_BanDau, Path.GetFileNameWithoutExtension(tep.Name));
                    string duongDan_Tep_SaoChep = string.Format("{0}/{1}", duongDan_ThuMuc_SaoChep, tep.Name);
                    if (!System.IO.Directory.Exists(duongDan_ThuMuc_SaoChep))
                    {
                        System.IO.Directory.CreateDirectory(duongDan_ThuMuc_SaoChep);
                        System.IO.File.Move(duongDan_Tep_BanDau, duongDan_Tep_SaoChep);
                    };
                };
            };
            return Json(new
            {
                mess = "thành công"
            }, JsonRequestBehavior.AllowGet);
        }
        // Sửa sai tên văn bản
        public ActionResult DoIt_TrongThuMuc()
        {
            string duongDan_GapLoi = "";
            try
            {
                /**
                 * 1. timTep
                 * 1.1. Lấy danh sách đường dẫn con
                 * 1.2. Đổi tên thư mục gốc => mới
                 * 1.3. Duyệt danh sách đường dẫn con
                 *  1.3.1. Nếu là tệp thì đổi tên
                 *  1.3.2. Nếu là thư mục thì đệ quy lại 1
                 */
                string doiTen(string tenTruoc)
                {
                    return Regex.Replace(Public.Handle.ConvertToUnSign(s: tenTruoc, khoangCach: " "), @"[^\w\d]+", "-");
                };
                string tachTen(string ten_TRUOC = "")
                {
                    string mauDuoiTep = @"(?=\[.*?\])";

                    string[] ten_TRUOCs = Regex.Split(ten_TRUOC, mauDuoiTep);
                    for (int i = 0; i < ten_TRUOCs.Length; i++)
                    {
                        if (!Regex.IsMatch(ten_TRUOCs[i], mauDuoiTep)) // Nếu không phải đuôi tệp và không nằm ở vị trí cuối cùng
                        {
                            if (ten_TRUOCs.Length == 1 ||
                                (ten_TRUOCs.Length > 1 && (i != (ten_TRUOCs.Length - 1))))
                                ten_TRUOCs[i] = doiTen(ten_TRUOCs[i]);
                        };
                    };
                    string ten_SAU = string.Join("", ten_TRUOCs);
                    return ten_SAU;
                };
                void timTep(string duongDanCha = "", string tenThuMuc = "")
                {
                    string duongDanGoc = string.Format("{0}\\{1}", duongDanCha, tenThuMuc);
                    string duongDanMoi = string.Format("{0}\\{1}", duongDanCha, tachTen(tenThuMuc));
                    duongDan_GapLoi = string.Format("{0}****{1}", duongDanGoc, duongDanMoi);

                    #region 1. Đổi tên thư mục gốc => mới
                    if (duongDanGoc != duongDanMoi)
                    {
                        if (Directory.Exists(duongDanMoi))
                            duongDanMoi = string.Format("{0}\\cp-{1}", Path.GetDirectoryName(duongDanMoi), Path.GetFileName(duongDanMoi));
                        Directory.Move(duongDanGoc, duongDanMoi);
                    };
                    #endregion

                    #region 2. Đổi tên các thư mục con và tệp con
                    string[] tatCaDuongDanCon = Directory.GetFileSystemEntries(duongDanMoi);
                    foreach (string duongDanCon in tatCaDuongDanCon)
                    {
                        if (System.IO.File.Exists(duongDanCon))
                        {
                            string tenTepHienTai = duongDanCon.Split('\\').Last();
                            string duongDanMoi_TepCon = string.Format("{0}\\{1}{2}",
                                duongDanMoi,
                                tachTen(Path.GetFileNameWithoutExtension(tenTepHienTai)),
                                Path.GetExtension(tenTepHienTai));
                            if (duongDanCon != duongDanMoi_TepCon)
                                System.IO.File.Move(duongDanCon, duongDanMoi_TepCon);
                        }
                        else if (Directory.Exists(duongDanCon))
                        {
                            string tenThuMucHienTai = duongDanCon.Split('\\').Last();
                            timTep(duongDanCha: duongDanMoi, tenThuMuc: tenThuMucHienTai);
                        };
                    };
                    #endregion
                };
                timTep(duongDanCha: Request.MapPath($"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}"), tenThuMuc: "HOSO");
                return Json(new
                {
                    mess = "thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    mess = $@"{ex}: 
                            {duongDan_GapLoi}"
                }, JsonRequestBehavior.AllowGet);
            };
        }
        public ActionResult DoIt_TrongDb()
        {
            try
            {
                string doiTen(string tenTruoc)
                {
                    return Regex.Replace(Public.Handle.ConvertToUnSign(s: tenTruoc, khoangCach: " "), @"[^\w\d]+", "-");
                };
                var hoSo_VanBans = db.tbHoSo_VanBan.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList(); // Đổi tên văn bản
                foreach (var hoSo_VanBan in hoSo_VanBans)
                {
                    hoSo_VanBan.TenVanBan = doiTen(hoSo_VanBan.TenVanBan);
                    if (hoSo_VanBan.TenVanBan_BanDau == null)
                        hoSo_VanBan.TenVanBan_BanDau = hoSo_VanBan.TenVanBan;
                };
                var hoSos = db.tbHoSoes.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList(); // Đổi tên hồ sơ
                foreach (var hoSo in hoSos)
                {
                    hoSo.MaHoSo = doiTen(hoSo.MaHoSo);
                    hoSo.DuongDanFile = string.Format("/Assets/uploads/{0}/HOSO/{1}", per.DonViSuDung.MaDonViSuDung, hoSo.MaHoSo);
                };
                db.SaveChanges();
                return Json(new
                {
                    mess = "thành công"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    mess = "teo"
                }, JsonRequestBehavior.AllowGet);
            };
        }
        public ActionResult DoIt_InThuMucThieu()
        {
            Response.Clear();
            Response.ClearHeaders();
            #region Lấy danh sách mã hồ sơ đã tồn tại thư mục ảnh
            string duongDan_KiemTra = Request.MapPath($"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/HOSO");
            List<string> thuMucs_HoSo_CoVanBan = new List<string>();
            List<string> thuMucs_HoSo_TatCa = Directory.GetDirectories(duongDan_KiemTra).ToList();
            foreach (string thuMuc_HoSo in thuMucs_HoSo_TatCa)
            {
                // Lấy danh sách thư mục nào không có (thư mục/tệp) bên trong
                if (Directory.GetFileSystemEntries(thuMuc_HoSo).Length > 0)
                    thuMucs_HoSo_CoVanBan.Add(thuMuc_HoSo.Split('\\').Last());
            };
            #endregion

            #region Lấy danh sách hồ sơ chưa được tạo thư mục văn bản
            var hoSos = db.tbHoSoes.Where(x => x.TrangThai != 0 && x.MaDonViSuDung == per.DonViSuDung.MaDonViSuDung).ToList();
            List<tbHoSo> hoSos_ChuaTaoThuMuc = new List<tbHoSo>();
            foreach (var hoSo in hoSos)
            {
                // Lưu lại tên các hồ sơ chưa được tạo thư mục
                if (!thuMucs_HoSo_CoVanBan.Contains(hoSo.MaHoSo))
                {
                    hoSos_ChuaTaoThuMuc.Add(new tbHoSo
                    {
                        MaHoSo = hoSo.MaHoSo,
                        TieuDeHoSo = hoSo.TieuDeHoSo,
                        GhiChu = hoSo.TrangThai == 1 ? "Lưu trữ" : hoSo.TrangThai == 2 ? "Nộp lưu" : "Khác"
                    });
                    // Xóa hết bản ghi văn bản có trong csdl của hồ sơ này
                    List<tbHoSo_VanBan> hoSo_VanBans = db.tbHoSo_VanBan.Where(x => x.IdHoSo == hoSo.IdHoSo).ToList();
                    foreach (tbHoSo_VanBan hoSo_VanBan in hoSo_VanBans)
                        hoSo_VanBan.TrangThai = 0;
                    db.SaveChanges();
                };
                // Tạo thư mục hồ sơ cho hồ sơ chưa có
                string duongDan = Request.MapPath(hoSo.DuongDanFile);
                if (!Directory.Exists(duongDan))
                    Directory.CreateDirectory(duongDan);
            };
            #endregion

            #region Tạo excel
            using (var workBook = new XLWorkbook())
            {
                #region Tạo sheet hồ sơ
                DataTable tbHoSo = new DataTable();
                tbHoSo.Columns.Add("Mã hồ sơ", typeof(string)); // 1
                tbHoSo.Columns.Add("Tiêu đề hồ sơ", typeof(string)); // 2
                tbHoSo.Columns.Add("Ghi chú", typeof(string)); // 2
                #region Thêm dữ liệu
                int hoSos_ChuaTaoThuMuc_COUNT = hoSos_ChuaTaoThuMuc.Count;
                for (int i = 0; i < hoSos_ChuaTaoThuMuc_COUNT; i++)
                {
                    tbHoSo hoSo = hoSos_ChuaTaoThuMuc[i];
                    if (tbHoSo.Rows.Count <= i)
                        tbHoSo.Rows.Add(
                            hoSo.MaHoSo,
                            hoSo.TieuDeHoSo,
                            hoSo.GhiChu
                         );
                };
                #endregion
                #endregion

                #region Tạo file excel
                workBook.Worksheets.Add(tbHoSo, "HoSo");
                tbHoSo.TableName = "";
                #endregion
                #region Tải file excel về máy client
                MemoryStream memoryStream = new MemoryStream();
                workBook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                downloadDialog(data: memoryStream, fileName: Server.UrlEncode("HOSO_THIEUVANBAN.xlsx"), contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                #endregion
            }
            #endregion
            return Redirect("/DocumentFormation/Index");
        }
        #endregion
    }
}