using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class DisplayModel_CRUD_BaiDang_Input_Dto
    {
        public List<Guid> IdBaiDangs { get; set; }
        public string Loai { get; set; }
    }
}