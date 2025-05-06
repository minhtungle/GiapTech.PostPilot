using Applications.QuanLyDangBai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDangBai.Dtos
{
    public class DisplayModel_CRUD_BaiDang_Output_Dto
    {
        public tbBaiDangExtend BaiDang { get; set; }
        public string Loai { get; set; }
    }
}