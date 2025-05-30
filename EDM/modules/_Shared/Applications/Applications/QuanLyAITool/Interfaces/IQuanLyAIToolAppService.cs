using Applications.QuanLyAITool.Dtos;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyAITool.Interfaces
{
    public interface IQuanLyAIToolAppService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<List<tbAITool>> GetAITools(
            string loai = "all",
            List<Guid> idAITool = null,
            LocThongTinDto locThongTin = null);
        Task<bool> IsExisted_AITool(tbAITool aiTool);
        Task Create_AITool(tbAITool aiTool);
    }
}