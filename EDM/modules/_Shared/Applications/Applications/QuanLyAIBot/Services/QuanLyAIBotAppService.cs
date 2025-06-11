using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAIBot.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Helpers;
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
        public async Task<Index_OutPut_Dto> Index_OutPut()
        {
            var thaoTacs = GetThaoTacs(maChucNang: "QuanLyAIBot");

            return new Index_OutPut_Dto
            {
                ThaoTacs = thaoTacs,
            };
        }

        #region AIBot
        public async Task<List<tbAIBotExtend>> GetAIBots(
           string loai = "all",
           List<Guid> idAIBots = null,
           LocThongTin_AIBot locThongTin = null)
        {
            var query = _aiBotRepo.Query()
                .Where(x =>
                    x.TrangThai != 0 &&
                    x.MaDonViSuDung == CurrentDonViId);

            if (loai == "single" && idAIBots != null && idAIBots.Any())
            {
                query = query.Where(x => idAIBots.Contains(x.IdAIBot));
            }

            // LEFT JOIN với bảng trung gian AIBot_LoaiAIBot
            var result = await query
                .GroupJoin(
                    _aiBotLoaiAIBotRepo.Query(),
                    aiBot => aiBot.IdAIBot,
                    gate => gate.IdAIBot,
                    (aiBot, gates) => new { AIBot = aiBot, Gates = gates.DefaultIfEmpty() }
                )
                .SelectMany(
                    x => x.Gates,
                    (x, gate) => new { x.AIBot, Gate = gate }
                )
                .GroupJoin(
                    _loaiAIBotRepo.Query(),
                    x => x.Gate != null ? x.Gate.IdLoaiAIBot : Guid.Empty, // tránh null
                    loaiAIBot => loaiAIBot.IdLoaiAIBot,
                    (x, loais) => new { x.AIBot, x.Gate, LoaiAIBots = loais.DefaultIfEmpty() }
                )
                .SelectMany(
                    x => x.LoaiAIBots,
                    (x, loaiAIBot) => new { x.AIBot, LoaiAIBot = loaiAIBot }
                )
                .GroupBy(x => x.AIBot.IdAIBot)
                .Select(gr => new tbAIBotExtend
                {
                    AIBot = gr.FirstOrDefault().AIBot,
                    LoaiAIBots = gr.Select(x => x.LoaiAIBot).Where(x => x != null).Distinct().ToList()
                })
                .OrderByDescending(x => x.AIBot.NgayTao)
                .ToListAsync();

            return result;
        }
        public async Task<bool> IsExisted_AIBot(tbAIBot aiBot)
        {
            var aiBot_OLD = await _aiBotRepo.Query()
                .FirstOrDefaultAsync(x => x.TenAIBot == aiBot.TenAIBot
            && x.IdAIBot != aiBot.IdAIBot
            && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return aiBot_OLD != null;
        }
        public async Task Create_AIBot(tbAIBotExtend aiBot)
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

                foreach (var idLoaiAIBot in aiBot.LoaiAIBots)
                {
                    var aiBotLoaiAIBot = new tbAIBotLoaiAIBot
                    {
                        IdAIBotLoaiAIBot = Guid.NewGuid(),
                        IdAIBot = entity.IdAIBot,
                        IdLoaiAIBot = idLoaiAIBot.IdLoaiAIBot,

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
        public async Task Update_AIBot(tbAIBotExtend aiBot)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entity = await _aiBotRepo.GetByIdAsync(aiBot.AIBot.IdAIBot);

                if (entity == null)
                    throw new Exception("AI Bot không tồn tại.");

                // Cập nhật thông tin chính
                entity.TenAIBot = aiBot.AIBot.TenAIBot;
                entity.Prompt = aiBot.AIBot.Prompt;
                entity.Keywords = aiBot.AIBot.Keywords;
                entity.GhiChu = aiBot.AIBot.GhiChu;
                entity.NgaySua = DateTime.Now;
                entity.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                await _unitOfWork.UpdateAsync<tbAIBot, Guid>(entity);

                // Xóa liên kết loại cũ
                var oldMappings = await _aiBotLoaiAIBotRepo.Query()
                    .Where(x => x.IdAIBot == entity.IdAIBot)
                    .ToListAsync();
                foreach (var old in oldMappings)
                {
                    await _unitOfWork.DeleteAsync<tbAIBotLoaiAIBot, Guid>(old);
                }

                // Thêm liên kết loại mới
                foreach (var loai in aiBot.LoaiAIBots)
                {
                    var newMapping = new tbAIBotLoaiAIBot
                    {
                        IdAIBotLoaiAIBot = Guid.NewGuid(),
                        IdAIBot = entity.IdAIBot,
                        IdLoaiAIBot = loai.IdLoaiAIBot,
                        TrangThai = 1,
                        MaDonViSuDung = CurrentDonViSuDung.MaDonViSuDung,
                        IdNguoiTao = CurrentNguoiDung.IdNguoiDung,
                        NgayTao = DateTime.Now
                    };
                    await _unitOfWork.InsertAsync<tbAIBotLoaiAIBot, Guid>(newMapping);
                }
            });
        }
        public async Task Delete_AIBot(List<Guid> idAIBots)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entities = await _aiBotRepo.Query()
                .Where(x => idAIBots.Contains(x.IdAIBot))
                .ToListAsync();

                foreach (var entity in entities)
                {
                    entity.TrangThai = 0;
                    entity.NgaySua = DateTime.Now;
                    entity.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbAIBot, Guid>(entity);
                }
            });
        }

        #endregion

        #region Loại AIBot
        public async Task<List<tbLoaiAIBot>> GetLoaiAIBots(
        string loai = "all",
        List<Guid> idLoaiAIBots = null,
        LocThongTin_AITool locThongTin = null)
        {
            var query = _loaiAIBotRepo.Query()
                .Where(x =>
                x.TrangThai != 0 &&
                x.MaDonViSuDung == CurrentDonViId);

            if (loai == "single" && idLoaiAIBots != null)
            {
                query = query.Where(x => idLoaiAIBots.Contains(x.IdLoaiAIBot));
            }
        ;

            var data = await query
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();

            return data;
        }
        public async Task<bool> IsExisted_LoaiAIBot(tbLoaiAIBot loaiAIBot)
        {
            var loaiAiBot_OLD = await _loaiAIBotRepo.Query()
                .FirstOrDefaultAsync(x => x.TenLoaiAIBot == loaiAIBot.TenLoaiAIBot
            && x.IdLoaiAIBot != loaiAIBot.IdLoaiAIBot
            && x.TrangThai != 0 && x.MaDonViSuDung == CurrentDonViSuDung.MaDonViSuDung);
            return loaiAiBot_OLD != null;
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

        public async Task Delete_LoaiAIBot(List<Guid> idLoaiAIBots)
        {
            await _unitOfWork.ExecuteInTransaction(async () =>
            {
                var entities = await _loaiAIBotRepo.Query()
                    .Where(x => idLoaiAIBots.Contains(x.IdLoaiAIBot))
                    .ToListAsync();

                foreach (var entity in entities)
                {
                    entity.TrangThai = 0;
                    entity.NgaySua = DateTime.Now;
                    entity.IdNguoiSua = CurrentNguoiDung.IdNguoiDung;

                    await _unitOfWork.UpdateAsync<tbLoaiAIBot, Guid>(entity);
                }
            });
        }

        #endregion
    }
}