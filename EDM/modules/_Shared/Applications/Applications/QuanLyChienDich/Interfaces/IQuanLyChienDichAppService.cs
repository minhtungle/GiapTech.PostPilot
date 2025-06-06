using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocThongTin_ChienDich = Applications.QuanLyChienDich.Dtos.LocThongTinDto;

namespace Applications.QuanLyChienDich.Interfaces
{
    public interface IQuanLyChienDichAppService
    {
        Task<IEnumerable<tbChienDich>> GetChienDichs(
            string loai = "all",
            List<Guid> idChienDichs = null,
            LocThongTin_ChienDich locThongTin = null);
        Task Create_ChienDich(
            tbChienDich chienDich);
        Task Delete_ChienDichs(
            List<Guid> idChienDichs);
    }
}
