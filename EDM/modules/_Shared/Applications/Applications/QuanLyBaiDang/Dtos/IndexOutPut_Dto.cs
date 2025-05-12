using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class IndexOutPut_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public tbChienDich ChienDich { get; set; }
    }
}