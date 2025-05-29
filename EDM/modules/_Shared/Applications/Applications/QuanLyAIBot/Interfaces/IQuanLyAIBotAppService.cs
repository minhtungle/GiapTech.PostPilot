using Applications.QuanLyAIBot.Dtos;
using Applications.QuanLyAIBot.Models;
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
    }
}