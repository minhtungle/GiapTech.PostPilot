using Applications.QuanLyBaiDang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class FormAddBaiDangDto
    {
        public tbBaiDangExtend BaiDang { get; set; }
        public string LoaiView { get; set; }
    }
}