using DocumentFormat.OpenXml.Wordprocessing;
using EDM_DB;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.OpenSsl;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Public
{
    public static class Handle
    {
        public static string[] DATETIMEFORMAT = {
            "yyyy-dd-MM"            , "yyyy-MM-dd"            , "dd/MM/yyyy"            , "MM/dd/yyyy"            , "yyyyMMdd" ,
            "yyyy-dd-MM HH:mm:ss"   , "yyyy-MM-dd HH:mm:ss"   , "dd/MM/yyyy HH:mm:ss"   , "MM/dd/yyyy HH:mm:ss"   , "yyyyMMdd HHmmss",
            "yyyy-dd-MM HH:mm:ss tt", "yyyy-MM-dd HH:mm:ss tt", "dd/MM/yyyy HH:mm:ss tt", "MM/dd/yyyy HH:mm:ss tt", "yyyyMMdd HHmmss tt",

            "yyyy-dd-MM h:mm:ss"   , "yyyy-MM-dd h:mm:ss"   , "dd/MM/yyyy h:mm:ss"  , "MM/dd/yyyy h:mm:ss"   , "yyyyMMdd hmmss",
            "yyyy-dd-MM h:mm:ss tt", "yyyy-MM-dd h:mm:ss tt", "dd/MM/yyyy h:mm:sstt", "MM/dd/yyyy h:mm:ss tt", "yyyyMMdd hmmss tt",

            "yyyy-d-M"            , "yyyy-M-d"            , "d/M/yyyy"            , "M/d/yyyy"            , "yyyyMd" ,
            "yyyy-d-M HH:mm:ss"   , "yyyy-M-d HH:mm:ss"   , "d/M/yyyy HH:mm:ss"   , "M/d/yyyy HH:mm:ss"   , "yyyyMd HHmmss",
            "yyyy-d-M HH:mm:ss tt", "yyyy-M-d HH:mm:ss tt", "d/M/yyyy HH:mm:ss tt", "M/d/yyyy HH:mm:ss tt", "yyyyMd HHmmss tt",

            "yyyy-d-M h:mm:ss"   , "yyyy-M-d h:mm:ss"   , "d/M/yyyy h:mm:ss"  , "M/d/yyyy h:mm:ss"   , "yyyyMd hmmss",
            "yyyy-d-M h:mm:ss tt", "yyyy-M-d h:mm:ss tt", "d/M/yyyy h:mm:sstt", "M/d/yyyy h:mm:ss tt", "yyyyMd hmmss tt",

        };

        #region B64
        public static string EncodeTo64(string toEncode = "")
        {
            toEncode = toEncode ?? "";
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string DecodeFrom64(string encodedData = "")
        {
            encodedData = encodedData ?? "";
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return Encoding.ASCII.GetString(encodedDataAsBytes);
        }
        #endregion

        #region Sha1 & MD5
        public static string HashToSha1(string input)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            SHA1 sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            string hash = sb.ToString();
            return hash;
        }
        public static string HashToMD5(string input)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            string hash = sb.ToString();
            return hash;
        }
        #endregion

        #region JSON
        public static string JsonSerializer(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
        public static object JsonDeserialize(string data)
        {
            return JsonConvert.DeserializeObject<object>(data);
        }
        // Hàm mã hóa
        public static string EncryptJson(object jsonData, string key)
        {
            string jsonString = JsonConvert.SerializeObject(jsonData);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[16];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cs))
                        {
                            writer.Write(jsonString);
                        }
                    }
                    byte[] encrypted = ms.ToArray();
                    byte[] combinedIvAndEncrypted = new byte[iv.Length + encrypted.Length];
                    Array.Copy(iv, 0, combinedIvAndEncrypted, 0, iv.Length);
                    Array.Copy(encrypted, 0, combinedIvAndEncrypted, iv.Length, encrypted.Length);
                    return Convert.ToBase64String(combinedIvAndEncrypted);
                }
            }
        }

        // Hàm giải mã
        public static object DecryptJson(string encryptedData, string key)
        {
            byte[] combinedIvAndEncrypted = Convert.FromBase64String(encryptedData);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[16];
            byte[] encrypted = new byte[combinedIvAndEncrypted.Length - iv.Length];
            Array.Copy(combinedIvAndEncrypted, 0, iv, 0, iv.Length);
            Array.Copy(combinedIvAndEncrypted, iv.Length, encrypted, 0, encrypted.Length);
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                using (MemoryStream ms = new MemoryStream(encrypted))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            string jsonString = reader.ReadToEnd();
                            return JsonConvert.DeserializeObject(jsonString);
                        }
                    }
                }
            }
        }
        #endregion

        #region Xử lý Object
        public static List<DuLieu_LichSu_ChiTiet> ChiTietThayDoi(List<Tuple<string, object, object>> thayDois)
        {
            List<DuLieu_LichSu_ChiTiet> chiTiets = new List<DuLieu_LichSu_ChiTiet>();
            foreach (var thayDoi in thayDois)
            {
                chiTiets.Add(new DuLieu_LichSu_ChiTiet
                {
                    TenTruongDuLieu = thayDoi.Item1,
                    GiaTri_Cu = thayDoi.Item2 != null ? thayDoi.Item2.ToString() : "",
                    GiaTri_Moi = thayDoi.Item3 != null ? thayDoi.Item3.ToString() : "",
                });
            }
            return chiTiets;
        }
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
        /// <summary>
        /// So sánh sự thay đổi của object
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <param name="fieldsToCompare"></param>
        /// <param name="fieldsToExclude"></param>
        /// <returns></returns>
        public static List<Tuple<string, object, object>> CompareSpecificFields<T>(
            T obj1, T obj2,
            List<string> fieldsToCompare, // Trường dữ liệu so sánh
            List<string> fieldsToExclude // Trường dữ liệu không so sánh
            )
        {
            List<Tuple<string, object, object>> differences = new List<Tuple<string, object, object>>();

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                bool compareField = true;

                // Kiểm tra nếu fieldsToCompare không rỗng và thuộc tính không nằm trong fieldsToCompare thì bỏ qua
                if (fieldsToCompare.Count > 0 && !fieldsToCompare.Contains(property.Name))
                {
                    compareField = false;
                }

                // Kiểm tra nếu thuộc tính nằm trong fieldsToExclude thì bỏ qua
                if (fieldsToExclude.Contains(property.Name))
                {
                    compareField = false;
                }

                if (compareField)
                {
                    var value1 = property.GetValue(obj1);
                    var value2 = property.GetValue(obj2);

                    if (!Equals(value1, value2))
                    {
                        differences.Add(new Tuple<string, object, object>(property.Name, value1, value2));
                    }
                }
            }

            return differences;
        }
        #endregion

        #region Tính toán
        public static async Task<double> EvaluateAsync(string expression, object variables)
        {
            try
            {
                // Thực thi biểu thức và trả về kết quả
                var result = await CSharpScript.EvaluateAsync<double>(
                    expression,
                    ScriptOptions.Default.WithReferences(AppDomain.CurrentDomain.GetAssemblies()),
                    globals: variables
                );

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đánh giá biểu thức: {ex.Message}");
                throw;
            }
        }
        public static string FormatCurrency(long amount, string culture = "vi-VN", string currency = "VND", bool showCurrencySymbol = true)
        {
            try
            {
                // Lấy thông tin văn hóa
                var cultureInfo = new CultureInfo(culture);
                string currencySymbol = cultureInfo.NumberFormat.CurrencySymbol;

                // Định dạng số tiền với 2 chữ số thập phân
                string formattedAmount = amount.ToString("N2", cultureInfo);
                // Loại bỏ phần ,00 hoặc .00 dựa trên dấu phân cách thập phân
                string decimalSeparator = cultureInfo.NumberFormat.CurrencyDecimalSeparator;

                if (formattedAmount.EndsWith($"{decimalSeparator}00"))
                {
                    formattedAmount = formattedAmount.Substring(0, formattedAmount.Length - 3);
                }
                ;

                return showCurrencySymbol ? $"{formattedAmount} {currencySymbol}" : formattedAmount;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về chuỗi mặc định
                return $"Invalid format: {ex.Message}";
            }
        }
        #endregion
        public static Dictionary<string, (bool status, string error)> CheckPassPattern(string password = "")
        {
            int minLength = 8; // Độ dài tối thiểu
            bool hasLowerCase = Regex.IsMatch(password, "[a-z]"); // Có chữ thường
            bool hasUpperCase = Regex.IsMatch(password, "[A-Z]"); // Có chữ hoa
            bool hasNumber = Regex.IsMatch(password, "[0-9]"); // Có số
            bool hasSpecialChar = Regex.IsMatch(password, "[!@#$%^&*(),.?\":{}|<>]"); // Có ký tự đặc biệt

            return new Dictionary<string, (bool status, string error)>
        {
            { "minLength", (password.Length >= minLength, $"Độ dài tối thiểu {minLength} ký tự") },
            { "hasLowerCase", (hasLowerCase, "Phải chứa ký tự in thường") },
            { "hasUpperCase", (hasUpperCase, "Phải chứa ký tự in hoa") },
            { "hasNumber", (hasNumber, "Phải chứa ký tự số") },
            { "hasSpecialChar", (hasSpecialChar, "Phải chứa ký tự đặc biệt") }
        };
        }
        public static void GetHost(out string url)
        {
            url = "";
        }
        public static string ConvertToUnSign(string s = "", string khoangCach = "", bool toUpper = false, bool toLower = false)
        {
            s = s ?? "";
            if (toUpper) s = s.ToUpper();
            if (toLower) s = s.ToLower();
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace(" ", khoangCach).Replace("(*)", String.Empty);
        }
        public static bool SendEmail(string sendTo, string subject, string body, bool isHTML, tbDonViSuDung donViSuDung, List<(string base64String, string fileName)> files = null)
        {
            SmtpSection cfg = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(cfg.From, donViSuDung.TenDonViSuDung);
                    //mail.From = new MailAddress("admin@giaptech.com", donViSuDung.TenDonViSuDung);
                    var mId = Guid.NewGuid().ToString();
                    mail.Headers.Add("Message-ID", mId);
                    mail.Headers.Add("X-Entity-Ref-ID", mId);
                    mail.To.Add(sendTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = isHTML;

                    // Khởi tạo danh sách rỗng nếu files chưa được cung cấp
                    if (files == null)
                    {
                        files = new List<(string base64String, string fileName)>();
                    }
                    // Lặp qua từng file trong danh sách files
                    foreach (var file in files)
                    {
                        string base64String = file.base64String;
                        string fileName = file.fileName;

                        // Chuyển đổi chuỗi Base64 thành tệp đính kèm
                        byte[] fileBytes = Convert.FromBase64String(base64String);
                        using (MemoryStream ms = new MemoryStream(fileBytes))
                        {
                            Attachment attachment = new Attachment(ms, fileName);
                            mail.Attachments.Add(attachment);
                        }
                    }


                    SmtpClient client = new SmtpClient();
                    client.Credentials = new NetworkCredential(cfg.Network.UserName, cfg.Network.Password);
                    client.Host = cfg.Network.Host;
                    client.Port = cfg.Network.Port;
                    client.EnableSsl = cfg.Network.EnableSsl;
                    //client.Credentials = new NetworkCredential("admin@giaptech.com", "[ #:Z'n'_\"AV:[OUQ=/A");
                    //client.Host = "smtp.gmail.com";
                    //client.Port = 587;
                    //client.EnableSsl = true;
                    client.Send(mail);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static string readFileText(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
        public static string RenderViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        public static string RandomString(string input, int outputLength)
        {
            Random random = new Random();
            string randomString = new string(Enumerable.Repeat(input, outputLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
        public static string TruncateString(string input, int maxLength)
        {
            const string ellipsis = "...";
            input = input ?? "";
            if (input.Length <= maxLength)
            {
                return input;
            }
            else if (maxLength <= ellipsis.Length)
            {
                return input.Substring(0, maxLength);
            }
            else
            {
                return String.Format("{0} {1}", input.Substring(0, maxLength - ellipsis.Length), ellipsis);
            }
            ;
        }
        public static int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;

            // Tính số năm
            int age = today.Year - birthDate.Year;

            // Kiểm tra nếu sinh nhật trong năm chưa tới thì giảm đi 1 tuổi
            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
        public static string GetImgSrcByPath(string imagePath)
        {
            // Đảm bảo đường dẫn đầy đủ
            //string fullPath = Request.MapPath(imagePath.Replace("~", ""));

            string fullPath = Path.Combine(string.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, imagePath.Replace("~", "")));

            // Lấy phần mở rộng của tệp
            string fileExtension = Path.GetExtension(fullPath).ToLower();

            // Xác định loại MIME tương ứng
            string mimeType = string.Empty;
            if (fileExtension == ".png")
            {
                mimeType = "image/png";
            }
            else if (fileExtension == ".jpg" || fileExtension == ".jpeg")
            {
                mimeType = "image/jpeg";
            }
            else
            {
                throw new NotSupportedException("Unsupported image format");
            }
            ;

            if (File.Exists(fullPath))
            {
                // Đọc nội dung của tệp hình ảnh
                byte[] imageBytes = File.ReadAllBytes(fullPath);

                // Chuyển đổi nội dung tệp sang chuỗi Base64
                string base64String = Convert.ToBase64String(imageBytes);

                // Trả về chuỗi Base64 với định dạng MIME phù hợp cho HTML img tag
                return $"data:{mimeType};base64,{base64String}";
            }
            return fullPath;
        }
        public static string GetImgSrcByByte(byte[] data)
        {
            var base64 = Convert.ToBase64String(data); // ImgData là byte[]
            var imgSrc = $"data:image/png;base64,{base64}";     // hoặc image/jpeg nếu ảnh là jpg
            return imgSrc;
        }
    }
}