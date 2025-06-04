using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAIBot.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LocThongTin_AIBot = Applications.QuanLyAIBot.Dtos.LocThongTinDto;
using LocThongTin_AITool = Applications.QuanLyAITool.Dtos.LocThongTinDto;

namespace Applications.QuanLyAIBot.Services
{
    public class QuanLyAIBotAppService : BaseAppService, IQuanLyAIBotAppService
    {
        private readonly IRepository<tbAIBot, Guid> _aiBotRepo;
        private readonly IRepository<tbAITool, Guid> _aiToolRepo;
        private readonly IRepository<tbLoaiAIBot, Guid> _loaiAIBotRepo;
        private readonly IRepository<tbAIBotLoaiAIBot, Guid> _aiBotLoaiAIBotRepo;

        public QuanLyAIBotAppService(
            IUserContext userContext,
            IUnitOfWork unitOfWork,
            IRepository<tbAIBot, Guid> aiBotRepo,
            IRepository<tbLoaiAIBot, Guid> loaiAIBotRepo,
            IRepository<tbAIBotLoaiAIBot, Guid> aiBotLoaiAIBotRepo,
            IRepository<tbAITool, Guid> aiToolRepo
            )
            : base(userContext, unitOfWork)
        {
            _aiBotRepo = aiBotRepo;
            _loaiAIBotRepo = loaiAIBotRepo;
            _aiBotLoaiAIBotRepo = aiBotLoaiAIBotRepo;
            _aiToolRepo = aiToolRepo;
        }
        public List<ThaoTac> GetThaoTacs(string maChucNang) => GetThaoTacByIdChucNang(maChucNang);
        public async Task<List<tbAIBotExtend>> GetAIBots(
            string loai = "all",
            List<Guid> idAIBot = null,
            LocThongTin_AIBot locThongTin = null)
        {
            var query = _aiBotRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId)
                .Join(_aiBotLoaiAIBotRepo.Query(),
                    aiBot => aiBot.IdAIBot,
                    aiBotLoaiAIBot => aiBotLoaiAIBot.IdAIBot,
                    (aiBot, aiBotLoaiAIBot) => new { AIBot = aiBot, Gate = aiBotLoaiAIBot })
                .Join(_loaiAIBotRepo.Query(),
                    x => x.Gate.IdLoaiAIBot,
                    loaiAIBot => loaiAIBot.IdLoaiAIBot,
                    (x, loaiAIBot) => new { AIBot = x.AIBot, LoaiAIBot = loaiAIBot })
                .GroupBy(x => x.AIBot.IdAIBot)
                .Select(gr => new tbAIBotExtend
                {
                    AIBot = gr.FirstOrDefault().AIBot,
                    LoaiAIBots = gr.Select(x => x.LoaiAIBot).ToList(),
                });

            if (loai == "single" && idAIBot != null)
            {
                query = query.Where(x => idAIBot.Contains(x.AIBot.IdAIBot));
            }
            ;

            var data = await query
                .OrderByDescending(x => x.AIBot.NgayTao)
                .ToListAsync();

            return data;
        }
        public async Task<List<tbLoaiAIBot>> GetLoaiAIBots(
            string loai = "all",
            List<Guid> idLoaiAIBot = null,
            LocThongTin_AITool locThongTin = null)
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
        public async Task Create_AIBot(tbAIBotExtend aiBot, List<Guid> idLoaiAIBots)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entity = new tbAIBot
                {
                    IdAIBot = Guid.NewGuid(),
                    TenAIBot = aiBot.AIBot.TenAIBot,
                    Prompt = aiBot.AIBot.Prompt,
                    Keywords = aiBot.AIBot.Keywords,
                    GhiChu = aiBot.AIBot.GhiChu,

                    TrangThai = 1,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                    // Thêm các trường khác nếu có
                };

                await _unitOfWork.InsertAsync<tbAIBot, Guid>(entity);

                foreach (var idLoaiAIBot in idLoaiAIBots)
                {
                    var aiBotLoaiAIBot = new tbAIBotLoaiAIBot
                    {
                        IdAIBotLoaiAIBot = Guid.NewGuid(),
                        IdAIBot = entity.IdAIBot,
                        IdLoaiAIBot = idLoaiAIBot,

                        TrangThai = 1,
                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                        IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                        NgayTao = DateTime.Now
                    };

                    await _unitOfWork.InsertAsync<tbAIBotLoaiAIBot, Guid>(aiBotLoaiAIBot);
                }
                // Thêm các thao tác async khác
            });
        }
        public async Task Create_LoaiAIBot(tbLoaiAIBot loaiAIBot)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entity = new tbLoaiAIBot
                {
                    IdLoaiAIBot = Guid.NewGuid(),
                    TenLoaiAIBot = loaiAIBot.TenLoaiAIBot,
                    GhiChu = loaiAIBot.GhiChu,

                    TrangThai = 1,
                    MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                    IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                    NgayTao = DateTime.Now,
                    // Thêm các trường khác nếu có
                };
                await _unitOfWork.InsertAsync<tbLoaiAIBot, Guid>(entity);
                // Thêm các thao tác async khác
            });
        }
    }
}