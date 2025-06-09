using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class FormThongTinChung_Dto
    {
        public tbBaiDangExtend BaiDang { get; set; }
        public FormAddBaiDangDto Data { get; set; }
    }
}