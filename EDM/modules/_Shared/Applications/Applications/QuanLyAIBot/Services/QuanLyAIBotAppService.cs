using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Interfaces;
using Applications.QuanLyAIBot.Models;
using EDM_DB;
using Infrastructure.Interfaces;
using Public.AppServices;
using Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Applications.QuanLyAIBot.Services
{
    public class QuanLyAIBotAppService : BaseAppService, IQuanLyAIBotAppService
    {
        private readonly IRepository<tbAIBot, Guid> _AIBotRepo;

        public QuanLyAIBotAppService(IUserContext userContext, IRepository<tbAIBot, Guid> AIBotRepo)
            : base(userContext)
        {
            _AIBotRepo = AIBotRepo;
        }

        public async Task<IEnumerable<tbAIBotExtend>> GetAIBots(
            string loai = "all",
            List<Guid> idAIBot = null,
            LocThongTinDto locThongTin = null)
        {
            var query = _AIBotRepo.Query()
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
    }
}