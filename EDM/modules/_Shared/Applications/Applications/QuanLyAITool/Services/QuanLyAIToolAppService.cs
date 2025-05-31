using Applications.Enums;
using Applications.QuanLyAITool.Dtos;
using Applications.QuanLyAITool.Extensions;
using Applications.QuanLyAITool.Interfaces;
using EDM_DB;
using Infrastructure.Interfaces;
using Newtonsoft.Json;
using Public.AppServices;
using Public.Helpers;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Applications.QuanLyAITool.Services
{
    public class QuanLyAIToolAppService : BaseAppService, IQuanLyAIToolAppService
    {
        private readonly IRepository<tbAITool, Guid> _aiToolRepo;

        public QuanLyAIToolAppService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbAITool, Guid> AIToolRepo)
            : base(userContext, unitOfWork)
        {
            _aiToolRepo = AIToolRepo;
        }

        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<List<tbAITool>> GetAITools(
            string loai = "all",
            List<Guid> idAITool = null,
            LocThongTinDto locThongTin = null)
        {
            var query = _aiToolRepo.Query()
                .Where(x =>
                x.TrangThai != 0 &&
                x.MaDonViSuDung == CurrentDonViId);

            if (loai == "single" && idAITool != null)
            {
                query = query.Where(x => idAITool.Contains(x.IdAITool));
            }
            ;

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();

            return data;
        }
        public async Task<bool> IsExisted_AITool(tbAITool aiTool)
        {
            var aiTool_OLD = await _aiToolRepo.Query()
                .FirstOrDefaultAsync(x => x.ToolCode == aiTool.ToolCode
            && x.IdAITool != aiTool.IdAITool
            && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return aiTool_OLD != null;
        }
        public async Task Create_AITool(tbAITool aiTool)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                // 1. Mã hóa API Key
                var encrypted = CryptoHelper.Encrypt(aiTool.APIKey);
                if (string.IsNullOrEmpty(encrypted))
                    throw new Exception("Không thể mã hóa API Key. Vui lòng kiểm tra lại.");

                // 2. Lấy template theo ToolCode
                if (!Enum.TryParse<AIToolTypeEnum>(aiTool.ToolCode, out var toolEnum))
                    throw new Exception($"ToolCode không hợp lệ: {aiTool.ToolCode}");

                if (!AIToolTemplates.RequestBodyTemplates.TryGetValue(toolEnum, out var template))
                    throw new Exception($"Không tìm thấy RequestBodyTemplate cho ToolCode: {aiTool.ToolCode}");

                // 3. Làm gọn template (1 dòng)
                var normalizedTemplate = template.Replace("\n", "").Replace("\r", "").Trim();

                // 4. Tạo entity
                var entity = new tbAITool
                {
                    IdAITool = Guid.NewGuid(),
                    ToolCode = aiTool.ToolCode,
                    ToolName = aiTool.ToolName,
                    ApiEndpoint = aiTool.ApiEndpoint,
                    Model = aiTool.Model,
                    APIKey = encrypted,
                    IsEncrypted = true,
                    AdditionalHeaders = aiTool.AdditionalHeaders,
                    RequestBodyTemplate = normalizedTemplate,
                    GhiChu = aiTool.GhiChu,

                    TrangThai = 1,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                };

                // 5. Lưu
                await _unitOfWork.InsertAsync<tbAITool, Guid>(entity);
            });
        }

        public async Task<string> WorkWithAITool(string toolCode, string prompt)
        {
            var tool = await _aiToolRepo.Query()
                .Where(x => x.ToolCode == toolCode && 
                (x.IsEncrypted.HasValue && x.IsEncrypted.Value))
                .FirstOrDefaultAsync();

            if (tool == null)
                return $"Không tìm thấy cấu hình cho tool: {toolCode}";

            // Giải mã API Key nếu cần
            var apiKey = (tool.IsEncrypted.HasValue && tool.IsEncrypted.Value) ? CryptoHelper.Decrypt(tool.APIKey) : tool.APIKey;

            using (var client = new HttpClient())
            {
                // Gắn Authorization nếu có
                if (!string.IsNullOrEmpty(apiKey))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                }

                // Gắn các headers bổ sung (nếu có)
                if (!string.IsNullOrEmpty(tool.AdditionalHeaders))
                {
                    var headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(tool.AdditionalHeaders);
                    foreach (var h in headers)
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value);
                    }
                }

                // Tạo nội dung body từ template
                string requestBody = tool.RequestBodyTemplate;

                // Áp dụng dữ liệu động nếu dùng {0} (model), {1} (prompt)
                if (!string.IsNullOrEmpty(requestBody) && (requestBody.Contains("{0}") || requestBody.Contains("{1}")))
                {
                    requestBody = string.Format(requestBody, tool.Model ?? "", prompt);
                }

                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Gửi request
                var response = await client.PostAsync(tool.ApiEndpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"Lỗi từ {tool.ToolCode}: {response.StatusCode} - {responseString}";
                }

                // Giải kết quả: với OpenAI, check message.content
                dynamic json = JsonConvert.DeserializeObject(responseString);
                if (json?.choices != null && json.choices.Count > 0)
                {
                    return (string)json.choices[0].message.content;
                }

                return "Không nhận được nội dung từ API.";
            }
        }
    }
}