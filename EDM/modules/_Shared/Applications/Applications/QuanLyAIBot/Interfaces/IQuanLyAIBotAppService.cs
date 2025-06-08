using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocThongTin_AIBot = Applications.QuanLyAIBot.Dtos.LocThongTinDto;
using LocThongTin_AITool = Applications.QuanLyAITool.Dtos.LocThongTinDto;

namespace Applications.QuanLyAIBot.Interfaces
{
    public interface IQuanLyAIBotAppService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<Index_OutPut_Dto> Index_OutPut();
        Task<List<tbAIBotExtend>> GetAIBots(
            string loai = "all",
            List<Guid> idAIBot = null,
            LocThongTin_AIBot locThongTin = null);
        Task<List<tbLoaiAIBot>> GetLoaiAIBots(
           string loai = "all",
           List<Guid> idLoaiAIBot = null,
           LocThongTin_AITool locThongTin = null);
        Task<bool> IsExisted_AIBot(tbAIBot aiBot);
        Task<bool> IsExisted_LoaiAIBot(tbLoaiAIBot loaiAIBot);
        Task Create_AIBot(tbAIBotExtend aiBot, List<Guid> idLoaiAIBots);
        Task Create_LoaiAIBot(tbLoaiAIBot loaiAIBot);
    }
}