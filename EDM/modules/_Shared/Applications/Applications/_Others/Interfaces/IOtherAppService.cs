using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications._Others.Interfaces
{
    public interface IOtherAppService
    {
        Task<List<tbNenTang>> GetNenTangs(
            string loai = "all",
            List<Guid> idNenTangs = null);
    }
}
