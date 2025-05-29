using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAIBot.Models;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Newtonsoft.Json;
using Public.AppServices;
using Public.Enums;
using Public.Interfaces;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Applications.QuanLyAIBot.Services
{
    public class QuanLyAIBotAppService : BaseAppService, IQuanLyAIBotAppService
    {
        private readonly IRepository<tbAIBot, Guid> _aiBotRepo;
        private readonly IRepository<tbLoaiAIBot, Guid> _loaiAIBotRepo;

        public QuanLyAIBotAppService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbAIBot, Guid> AIBotRepo,
            IRepository<tbLoaiAIBot, Guid> loaiAIBotRepo)
            : base(userContext, unitOfWork)
        {
            _aiBotRepo = AIBotRepo;
            _loaiAIBotRepo = loaiAIBotRepo;
        }

        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<List<tbAIBotExtend>> GetAIBots(
            string loai = "all",
            List<Guid> idAIBot = null,
            LocThongTinDto locThongTin = null)
        {
            var query = _aiBotRepo.Query()
                .Where(x =>
                x.TrangThai != 0 &&
                x.MaDonViSuDung == CurrentDonViId);

            if (loai == "single" && idAIBot != null)
            {
                query = query.Where(x => idAIBot.Contains(x.IdAIBot));
            }
            ;

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new tbAIBotExtend { AIBot = x })
                .ToListAsync();

            return data;
        }
        public async Task<List<tbLoaiAIBot>> GetLoaiAIBots(
            string loai = "all",
            List<Guid> idLoaiAIBot = null,
            LocThongTinDto locThongTin = null)
        {
            var query = _loaiAIBotRepo.Query()
                .Where(x =>
                x.TrangThai != 0 &&
                x.MaDonViSuDung == CurrentDonViId);

            if (loai == "single" && idLoaiAIBot != null)
            {
                query = query.Where(x => idLoaiAIBot.Contains(x.IdLoaiAIBot));
            }
            ;

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();

            return data;
        }
        public async Task<bool> IsExisted_AIBot(tbAIBot aiBot)
        {
            var aiBot_OLD = await _aiBotRepo.Query()
                .FirstOrDefaultAsync(x => x.TenAIBot == aiBot.TenAIBot
            && x.IdAIBot != aiBot.IdAIBot
            && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return aiBot_OLD != null;
        }
        public async Task<bool> IsExisted_LoaiAIBot(tbLoaiAIBot loaiAIBot)
        {
            var loaiAiBot_OLD = await _loaiAIBotRepo.Query()
                .FirstOrDefaultAsync(x => x.TenLoaiAIBot == loaiAIBot.TenLoaiAIBot
            && x.IdLoaiAIBot != loaiAIBot.IdLoaiAIBot
            && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return loaiAiBot_OLD != null;
        }
        public async Task Create_AIBot(tbAIBotExtend aiBot)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entity = new tbAIBot
                {
                    IdAIBot = Guid.NewGuid(),
                    MaDonViSuDung = CurrentDonViId,
                    NgayTao = DateTime.Now,
                    IdNguoiTao = CurrentUserId,
                    TrangThai = 1,

                    TenAIBot = aiBot.AIBot.TenAIBot,
                    GhiChu = aiBot.AIBot.GhiChu,
                    // Thêm các trường khác nếu có
                };

                await _unitOfWork.InsertAsync<tbAIBot, Guid>(entity);
                // Thêm các thao tác async khác
            });
        }
    }
}