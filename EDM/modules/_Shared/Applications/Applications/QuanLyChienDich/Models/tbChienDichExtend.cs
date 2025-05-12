using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyChienDich.Models
{
    public class tbChienDichExtend
    {
        public tbChienDich ChienDich { get; set; }
        public List<tbBaiDangExtend> BaiDangs { get; set; }
    }
}