using Applications.QuanLyDangBai.Interfaces;
using EDM_DB;
using EDM_DB.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;

namespace Applications.QuanLyDangBai
{
    public class QuanLyDangBaiService : IQuanLyDangBai
    {
        private readonly IRepository<tbBaiDang> _postRepo;

        public QuanLyDangBaiService(IRepository<tbBaiDang> postRepo)
        {
            _postRepo = postRepo;
        }

        public async Task<List<tbBaiDang>> GetListAsync()
        {
            var items = await _postRepo.GetAllAsync();
            return items.Select(p => new tbBaiDang {  }).ToList();
        }

        public async Task<tbBaiDang> GetAsync(int id)
        {
            var p = await _postRepo.GetAsync(id);
            return new tbBaiDang {  };
        }

        public async Task<tbBaiDang> CreateAsync(tbBaiDang dto)
        {
            var entity = new tbBaiDang {  };
            var saved = await _postRepo.InsertAsync(entity);
            //dto.Id = saved.Id;
            return dto;
        }

        public async Task<tbBaiDang> UpdateAsync(tbBaiDang dto)
        {
            var entity = new tbBaiDang {  };
            await _postRepo.UpdateAsync(entity);
            return dto;
        }

        public async Task DeleteAsync(int id) => await _postRepo.DeleteAsync(id);
    }
}