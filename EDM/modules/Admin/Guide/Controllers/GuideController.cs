using Aspose.Cells;
using Aspose.Words;
using EDM_DB;
using Newtonsoft.Json;
using Public.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Guide.Controllers {
    public class GuideController : RouteConfigController {
        private readonly string VIEW_PATH = "~/Views/Admin/_SystemUtilities/Guide";
        public ActionResult Index() {
            return View($"{VIEW_PATH}/guide.cshtml");
        }
        [HttpGet]
        public ActionResult getFile() {
            string FOLDERPATH = $"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/HUONGDANSUDUNG";
            string FOLDERPATH_SERVER = Request.MapPath(FOLDERPATH);
            bool hasFile = false;

            if (System.IO.Directory.Exists(FOLDERPATH_SERVER)) {
                //string[] files = System.IO.Directory.GetFiles(FOLDERPATH_SERVER);
                DirectoryInfo dr = new DirectoryInfo(FOLDERPATH_SERVER);
                FileInfo[] files = dr.GetFiles();
                if (files.Length > 0) {
                    hasFile = true;
                    string inputFileName = files[0].Name;
                    string inputFilePath = string.Format("{0}/{1}", FOLDERPATH, inputFileName);
                    string inputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
                    string inputFileExtension = files[0].Extension;
                    #region Lấy đường dẫn văn bản
                    if (inputFileExtension.Contains("xls") || inputFileExtension.Contains("doc")) {
                        string outputFolderPath = string.Format("{0}/{1}", FOLDERPATH, $"{inputFileNameWithoutExtension}[{inputFileExtension}]");
                        string outputFileName = $"{inputFileNameWithoutExtension}.pdf";
                        string outputFilePath = string.Format("{0}/{1}", outputFolderPath, outputFileName);
                        ViewBag.iframeHtml = $"<iframe src=\"{outputFilePath}#view=fit\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh\"></iframe>";
                    } else {
                        ViewBag.iframeHtml = $"<iframe src=\"{inputFilePath}#view=fit\" title=\"Chi tiết\" style=\"width: 100%;height: 70vh\"></iframe>";
                        if (inputFileExtension.Contains("mp4")) {
                            ViewBag.iframeHtml = $"<video src=\"{inputFilePath}\" controls style=\"width: 100%; height: 70vh; border: 1px solid var(--bs-body-color)\"></video>";
                        };
                    };
                    #endregion
                };
            };
            ViewBag.hasFile = hasFile;
            return PartialView($"{VIEW_PATH}/guide-getFile.cshtml");
        }
        [HttpPost]
        public void Test(HttpPostedFileBase[] files) {
            //string a = "";
        }
        [HttpPost]
        public ActionResult create_HuongDan(HttpPostedFileBase[] files) {
            string status = "success";
            string mess = "Thêm mới bản ghi thành công";
            using (var scope = db.Database.BeginTransaction()) {
                try {
                    if (files == null || files.Length == 0) {
                        status = "error";
                        mess = "Tệp đính kèm sai định dạng";
                    } else {
                        #region Bước 1: Xóa folder và tạo lại
                        string FOLDERPATH = $"/Assets/uploads/{per.DonViSuDung.MaDonViSuDung}/HUONGDANSUDUNG";
                        string FOLDERPATH_SERVER = Request.MapPath(FOLDERPATH);
                        if (System.IO.Directory.Exists(FOLDERPATH_SERVER)) {
                            System.IO.Directory.Delete(FOLDERPATH_SERVER, true);
                        };
                        System.IO.Directory.CreateDirectory(FOLDERPATH_SERVER);
                        #endregion
                        #region Bước 2: Thêm tệp
                        foreach (HttpPostedFileBase f in files) {
                            string inputFileName = Public.Handle.ConvertToUnSign(s: Path.GetFileName(f.FileName), khoangCach: "-");
                            string inputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
                            string inputFileExtension = Path.GetExtension(inputFileName);
                            string inputFilePath = string.Format("{0}/{1}", FOLDERPATH, inputFileName);
                            string inputFilePath_SERVER = Request.MapPath(inputFilePath);
                            f.SaveAs(inputFilePath_SERVER);
                            #region Chuyển đổi file office thành dạng PDF
                            if (inputFileExtension.Contains("xls") || inputFileExtension.Contains("doc")) {
                                #region Bước 1: Tạo folder lưu PDF
                                string outputFolderPath = string.Format("{0}/{1}", FOLDERPATH, $"{inputFileNameWithoutExtension}[{inputFileExtension}]");
                                string outputFolderPath_SERVER = Request.MapPath(outputFolderPath);
                                string outputFileName = $"{inputFileNameWithoutExtension}.pdf";
                                string outputFilePath = string.Format("{0}/{1}", outputFolderPath, outputFileName);
                                string outputFilePath_SERVER = Request.MapPath(outputFilePath);
                                if (System.IO.Directory.Exists(outputFolderPath_SERVER)) {
                                    System.IO.Directory.Delete(outputFolderPath_SERVER, true);
                                };
                                System.IO.Directory.CreateDirectory(outputFolderPath_SERVER);
                                #endregion
                                #region Bước 2: Chuyển đổi file
                                if (inputFileExtension.Contains("xls")) { // Excel
                                    // Tạo một đối tượng Workbook từ tập tin Excel
                                    Workbook workbook = new Workbook(inputFilePath_SERVER);
                                    foreach (var worksheet in workbook.Worksheets) {
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
                                    workbook.Save(outputFilePath_SERVER, pdfSaveOptions);
                                } else { // Word
                                    // Tạo đối tượng Document từ tệp Word
                                    Document document = new Document(inputFilePath_SERVER);
                                    // Chuyển đổi tài liệu Word sang PDF và lưu vào tệp PDF mới
                                    document.Save(outputFilePath_SERVER);
                                };
                                #endregion
                            };
                            #endregion
                        };
                        #endregion
                        db.tbLichSuTruyCaps.Add(new tbLichSuTruyCap {
                            TenModule = "Hướng dẫn sử dụng",
                            ThaoTac = "Thêm mới",
                            NoiDungChiTiet = "Thêm mới tài liệu hướng dẫn sử dụng",

                            NgayTao = DateTime.Now,
                            IdNguoiDung = per.NguoiDung.IdNguoiDung,
                            MaDonViSuDung = per.DonViSuDung.MaDonViSuDung
                        });
                        db.SaveChanges();
                        scope.Commit();
                    }
                } catch (Exception ex) {
                    status = "error";
                    mess = ex.Message;
                    scope.Rollback();
                }
            }
            return Json(new {
                status,
                mess
            }, JsonRequestBehavior.AllowGet);
        }
    }
}