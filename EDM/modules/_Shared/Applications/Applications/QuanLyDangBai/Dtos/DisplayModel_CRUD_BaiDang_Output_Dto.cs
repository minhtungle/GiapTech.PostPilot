using Applications.QuanLyDangBai.Models;

namespace Applications.QuanLyDangBai.Dtos
{
    public class DisplayModel_CRUD_BaiDang_Output_Dto
    {
        public tbBaiDangExtend BaiDang { get; set; } = new tbBaiDangExtend();
        public string Loai { get; set; }
    }
}