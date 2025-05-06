using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyDangBai.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "n8n test";

        // Google Sheet ID (lấy từ link bạn cung cấp)
        static readonly string spreadsheetId = "1ONCqKxCJBwGHyeDY0AZ55ydzOtnypNVQcG4-7vTb7UA";

        // Tên sheet/tab (mặc định thường là "Sheet1")
        static readonly string sheetName = "Sheet1";

        public ActionResult PushSampleDataToGoogleSheet()
        {
            try
            {
                // Đường dẫn thực tế của file JSON đã upload
                string jsonPath = Server.MapPath("~/App_Data/meta-buckeye-458819-m8-2044fdff02b5.json");

                // Khởi tạo credential
                GoogleCredential credential;
                using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }

                // Khởi tạo Sheets API service
                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Tạo dữ liệu mẫu
                var values = new List<IList<object>>();
                for (int i = 1; i <= 10; i++)
                {
                    values.Add(new List<object>
                {
                    $"ID-{i}",
                    DateTime.Now.ToString("yyyy-MM-dd"),
                    $"Raw content line {i}",
                    $"GPT Cooked content {i}",
                    $"https://example.com/image{i}.jpg"
                });
                }

                // Tạo yêu cầu ghi dữ liệu
                var valueRange = new ValueRange
                {
                    Values = values
                };

                var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, $"{sheetName}!A:E");
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                var appendResponse = appendRequest.Execute();

                return Content("✅ Dữ liệu mẫu đã được đẩy lên Google Sheet thành công.");
            }
            catch (Exception ex)
            {
                return Content("❌ Lỗi: " + ex.Message);
            }
        }
    }
}