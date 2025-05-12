using Applications.QuanLyBaiDang.Models;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class DisplayModel_CRUD_BaiDang_Output_Dto
    {
        public tbBaiDangExtend BaiDang { get; set; } = new tbBaiDangExtend();
        public string Loai { get; set; }
    }
}