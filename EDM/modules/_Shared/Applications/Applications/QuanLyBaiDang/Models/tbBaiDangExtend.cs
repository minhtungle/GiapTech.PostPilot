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
        public Guid RowNumber { get; set; } = Guid.NewGuid();
        public tbBaiDang BaiDang { get; set; } = new tbBaiDang();
        public tbNenTang NenTang { get; set; } = new tbNenTang();
        public tbNguoiDung NguoiTao { get; set; } = new tbNguoiDung();
        public List<tbTepDinhKem> TepDinhKems { get; set; } = new List<tbTepDinhKem>();
        public KiemTraExcel KiemTraExcel { get; set; } = new KiemTraExcel();
    }
}