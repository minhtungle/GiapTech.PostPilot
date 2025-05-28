using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyAIBot.Interfaces
{
    public interface IQuanLyAIBotAppService
    {
        Task<IEnumerable<tbAIBotExtend>> GetAIBots(
            string loai = "all",
            List<Guid> idAIBot = null,
            LocThongTinDto locThongTin = null);
    }
}