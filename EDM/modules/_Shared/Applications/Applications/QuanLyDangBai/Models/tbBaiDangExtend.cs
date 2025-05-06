using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyDangBai.Models
{
    public class tbBaiDangExtend
    {
        public tbBaiDang BaiDang { get; set; } = new tbBaiDang();
        public List<tbAnhMoTa> AnhMoTas { get; set; } = new List<tbAnhMoTa>();
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
    }
}