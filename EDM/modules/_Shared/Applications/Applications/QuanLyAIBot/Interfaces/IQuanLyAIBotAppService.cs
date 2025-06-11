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
        #region AIBot
        Task<bool> IsExisted_AIBot(tbAIBot aiBot);
        Task<List<tbAIBotExtend>> GetAIBots(
            string loai = "all",
            List<Guid> idAIBots = null,
            LocThongTin_AIBot locThongTin = null);
        Task Create_AIBot(tbAIBotExtend aiBot);
        Task Update_AIBot(tbAIBotExtend aiBot);
        Task Delete_AIBot(List<Guid> idAIBots);
        #endregion

        #region LoaiAIBot
        Task<List<tbLoaiAIBot>> GetLoaiAIBots(
         string loai = "all",
         List<Guid> idLoaiAIBots = null,
         LocThongTin_AITool locThongTin = null);
        Task<bool> IsExisted_LoaiAIBot(tbLoaiAIBot loaiAIBot);
        Task Create_LoaiAIBot(tbLoaiAIBot loaiAIBot);
        Task Update_LoaiAIBot(tbLoaiAIBot loaiAIBot);
        Task Delete_LoaiAIBot(List<Guid> idLoaiAIBots);
        #endregion
    }
}