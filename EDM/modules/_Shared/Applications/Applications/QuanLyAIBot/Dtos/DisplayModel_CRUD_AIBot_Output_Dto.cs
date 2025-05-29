using Applications.QuanLyAIBot.Models;
using Applications.QuanLyBaiDang.Models;
using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyAIBot.Dtos
{
    public class DisplayModel_CRUD_AIBot_Output_Dto
    {
        public tbAIBotExtend AIBot { get; set; } = new tbAIBotExtend();
        public List<tbLoaiAIBot> LoaiAIBot { get; set; } = new List<tbLoaiAIBot>();
        public string Loai { get; set; }
    }
}