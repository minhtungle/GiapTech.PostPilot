using EDM_DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EDM
{
    public partial class FileUploadHandler : System.Web.UI.Page
    {
        private EDM_DBEntities db;
        public FileUploadHandler()
        {
            db = new EDM_DBEntities();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Response.Clear();
            this.Page.Response.ClearHeaders();
            if (base.Request.Files["uploadfile"] != null)
            {
                this.Upload();
            }
            else if (base.Request.QueryString["download"] != null)
            {
                string filename = base.Server.MapPath("~/congvan.pdf");
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.ClearContent();
                response.Clear();
                response.ContentType = "application/pdf";
                response.AddHeader("Content-Disposition", "attachment; filename=" + "testfilename.pdf" + ";");
                response.TransmitFile(filename);
            }
            this.Page.Response.Flush();
            this.Page.Response.End();
        }

        private void Upload()
        {
            this.Page.Response.Clear();
            this.Page.Response.ClearHeaders();
            try
            {
                string currentDomain = Request.Url.Host.ToLower();
                tbDonViSuDung donViSuDung = db.Database.SqlQuery<tbDonViSuDung>($@"
                    select * from tbDonViSuDung
                        where TrangThai = 1
                        AND RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(TenMien, 'https://', ''), 'http://', ''), 'www.', ''), '/','')) = '{currentDomain}'
                    ").FirstOrDefault() ?? new tbDonViSuDung();

                var builder = new UriBuilder(Request.Url.Scheme, Request.Url.Host, Request.Url.Port);
                HttpPostedFile file = base.Request.Files["uploadfile"];
                string tenVanBan_KYSO = Path.GetFileName(file.FileName);
                string duongDanThuMuc_KYSO = $"/Assets/uploads/{donViSuDung.MaDonViSuDung}/KYSO";
                string duongDanThuMuc_KYSO_SERVER = Request.MapPath(duongDanThuMuc_KYSO);
                if (!System.IO.Directory.Exists(duongDanThuMuc_KYSO_SERVER))
                    System.IO.Directory.CreateDirectory(duongDanThuMuc_KYSO_SERVER);
                string duongDanTepVanBan_KySo_SERVER = string.Format("{0}/{1}", duongDanThuMuc_KYSO_SERVER, tenVanBan_KYSO);
                file.SaveAs(duongDanTepVanBan_KySo_SERVER);
                //this.Page.Response.Write("{\"Status\":true, \"Message\": \"hahahaha\", \"FileName\": \"" + tenVanBan_KYSO + "\", \"FileServer\": \"" + duongDanThuMuc_KYSO + "\"}");
                dynamic response = new
                {
                    Status = true, // Mã lỗi: tùy chỉnh
                    Message = "Thành công", // Mô tả lỗi
                    DocumentNumber = "", // Mô tả số của văn bản (công văn)
                    DocumentDate = "", // Mô tả ngày ký số
                    FileName = tenVanBan_KYSO, // Mô tả tên văn bản
                    FileServer = duongDanThuMuc_KYSO // Mô tả đường dẫn văn bản trên server
                };
                this.Page.Response.Write(JsonConvert.SerializeObject(response)); // Chả thấy vào đéo gì cả, bí ẩn vcl
            }
            catch (Exception ex)
            {
                //this.Page.Response.Write("{\"Status\":false, \"Message\": \"" + ex.Message + "\", \"FileName\": \"\", \"FileServer\": \"\"}");
                this.Page.Response.Write(JsonConvert.SerializeObject(new
                {
                    Status = true, // Mã lỗi: tùy chỉnh
                    Message = $"Có lỗi: {ex.Message}", // Mô tả lỗi
                    DocumentNumber = "", // Mô tả số của văn bản (công văn)
                    DocumentDate = "", // Mô tả ngày ký số
                    FileName = "", // Mô tả tên văn bản
                    FileServer = "" // Mô tả đường dẫn văn bản trên server
                }));
            }
        }
    }
}