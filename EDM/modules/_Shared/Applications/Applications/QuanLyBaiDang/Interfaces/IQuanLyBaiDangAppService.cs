using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applications.QuanLyBaiDang.Interfaces
{
    public interface IQuanLyBaiDangAppService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<IEnumerable<tbBaiDangExtend>> GetBaiDangs
           (Guid idChienDich,
           string loai = "all",
           List<Guid> idBaiDangs = null,
           LocThongTinDto locThongTin = null);
    }
}
