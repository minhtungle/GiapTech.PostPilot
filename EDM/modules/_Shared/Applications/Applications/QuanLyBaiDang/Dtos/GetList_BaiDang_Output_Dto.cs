using Applications.QuanLyBaiDang.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class GetList_BaiDang_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public IEnumerable<tbBaiDangExtend> BaiDangs { get; set; }
    }
}