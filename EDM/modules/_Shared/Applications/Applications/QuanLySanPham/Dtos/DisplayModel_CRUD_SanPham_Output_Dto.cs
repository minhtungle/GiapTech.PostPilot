using Applications.QuanLySanPham.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLySanPham.Dtos
{
    public class DisplayModel_CRUD_SanPham_Output_Dto
    {
        public tbSanPhamExtend SanPham { get; set; }
        public string Loai { get; set; }
    }
}