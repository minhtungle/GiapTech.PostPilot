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
        public List<tbChienDich> ChienDichs { get; set; } = new List<tbChienDich>)();
        public List<tbNenTang> NenTangs { get; set; } = new List<tbNenTang>();
        public List<tbAIBot> AIBots { get; set; } = new List<tbAIBot>();
        public List<tbAITool> AITools { get; set; } = new List<tbAITool>();
        public string LoaiView { get; set; }
    }
}