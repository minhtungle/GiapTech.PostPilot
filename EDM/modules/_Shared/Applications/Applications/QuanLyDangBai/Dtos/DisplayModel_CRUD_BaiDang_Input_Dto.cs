using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDangBai.Dtos
{
    public class DisplayModel_CRUD_BaiDang_Input_Dto
    {
        public Guid IdBaiDang { get; set; }
        public string Loai { get; set; }
    }
}