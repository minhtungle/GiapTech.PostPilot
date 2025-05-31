using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using System.Collections.Generic;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class DisplayModel_CRUD_BaiDang_Output_Dto
    {
        public string Loai { get; set; }
        public tbBaiDangExtend BaiDang { get; set; } = new tbBaiDangExtend();
    }
}