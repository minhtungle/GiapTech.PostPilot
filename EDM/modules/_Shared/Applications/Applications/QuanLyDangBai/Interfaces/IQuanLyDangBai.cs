using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.QuanLyDangBai.Interfaces
{
    public interface IQuanLyDangBai
    {
        Task<List<tbBaiDang>> GetListAsync();
        Task<tbBaiDang> GetAsync(int id);
        Task<tbBaiDang> CreateAsync(tbBaiDang dto);
        Task<tbBaiDang> UpdateAsync(tbBaiDang dto);
        Task DeleteAsync(int id);
    }
}
