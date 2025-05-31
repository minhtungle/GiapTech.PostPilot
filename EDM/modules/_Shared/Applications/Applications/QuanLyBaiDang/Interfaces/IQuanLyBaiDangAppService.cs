using Applications.QuanLyBaiDang.Dtos;
using Applications.QuanLyBaiDang.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Applications.QuanLyBaiDang.Interfaces
{
    public interface IQuanLyBaiDangAppService
    {
        List<ThaoTac> GetThaoTacs(string maChucNang);
        Task<IEnumerable<tbBaiDangExtend>> GetBaiDangs
           (string loai = "all",
           List<Guid> idBaiDangs = null,
           LocThongTinDto locThongTin = null);
        Task<FreeImageUploadResponse> UploadToFreeImageHost(
            HttpPostedFileBase file);
        Task Create_BaiDang(
            List<tbBaiDangExtend> baiDangs,
            HttpPostedFileBase[] files, 
            Guid[] rowNumbers);
        Task Delete_BaiDangs(
            List<Guid> idBaiDangs);
    }
}
