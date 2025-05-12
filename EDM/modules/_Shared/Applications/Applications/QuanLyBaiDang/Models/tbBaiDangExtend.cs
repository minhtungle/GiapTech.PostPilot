using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Models
{
    public class tbBaiDangExtend
    {
        public tbBaiDang BaiDang { get; set; } = new tbBaiDang();
        public int TuTaoAnh { get; set; } = 0;
        public List<tbTepDinhKem> TepDinhKems { get; set; } = new List<tbTepDinhKem>();
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
    }
}