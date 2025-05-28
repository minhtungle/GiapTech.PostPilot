using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.QuanLyBaiDang.Interfaces
{
    public interface IQuanLyBaiDangAppService
    {
        List<tbBaiDangExtend> GetBaiDangs(Guid idChienDich, string loai = "all", List<Guid> idBaiDangs = null, LocThongTinDto locThongTin = null);
    }
}
