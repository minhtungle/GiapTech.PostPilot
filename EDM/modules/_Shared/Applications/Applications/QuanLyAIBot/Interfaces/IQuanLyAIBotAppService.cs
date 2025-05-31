using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyAIBot.Interfaces
{
    public interface IQuanLyAIBotAppService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<List<tbAIBotExtend>> GetAIBots(
            string loai = "all",
            List<Guid> idAIBot = null,
            LocThongTinDto locThongTin = null);
        Task<List<tbLoaiAIBot>> GetLoaiAIBots(
           string loai = "all",
           List<Guid> idLoaiAIBot = null,
           LocThongTinDto locThongTin = null);
        Task<bool> IsExisted_AIBot(tbAIBot aiBot);
        Task<bool> IsExisted_LoaiAIBot(tbLoaiAIBot loaiAIBot);
        Task Create_AIBot(tbAIBotExtend aiBot, List<Guid> idLoaiAIBots);
        Task Create_LoaiAIBot(tbLoaiAIBot loaiAIBot);
    }
}