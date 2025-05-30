using Applications.QuanLyAITool.Dtos;
using Applications.QuanLyAITool.Interfaces;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
                .FirstOrDefaultAsync(x => x.TenAITool == aiTool.TenAITool
            && x.IdAITool != aiTool.IdAITool
            && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return aiTool_OLD != null;
        }
        public async Task Create_AITool(tbAITool aiTool)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entity = new tbAITool
                {
                    IdAITool = Guid.NewGuid(),
                    TenAITool = aiTool.TenAITool,
                    ApiEndpoint = aiTool.ApiEndpoint,
                    Model = aiTool.Model,
                    APIKey = aiTool.APIKey,
                    IsEncrypted = true,
                    AdditionalHeaders= aiTool.AdditionalHeaders,
                    RequestBodyTemplate = aiTool.RequestBodyTemplate,
                    GhiChu = aiTool.GhiChu,

                    TrangThai = 1,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                    // Thêm các trường khác nếu có
                };

                await _unitOfWork.InsertAsync<tbAITool, Guid>(entity);
                // Thêm các thao tác async khác
            });
        }
    }
}