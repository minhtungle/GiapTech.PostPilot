using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class FormAddBaiDangDto
    {
        public tbBaiDangExtend BaiDang { get; set; }
        public List<tbNenTang> NenTangs { get; set; } = new List<tbNenTang>();
        public string LoaiView { get; set; }
    }
}