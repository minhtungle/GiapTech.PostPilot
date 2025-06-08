using EDM_DB;
using Public.Dtos;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyBaiDang.Dtos
{
    public class Index_OutPut_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbNenTang> NenTangs { get; set; }
        public List<tbNguoiDung> NguoiTaos { get; set; }
        public List<tbChienDich> ChienDichs { get; set; }
        public List<tbAIBot> AIBots { get; set; } = new List<tbAIBot>();
        public List<tbAITool> AITools { get; set; } = new List<tbAITool>();
        public List<EnumInfoDto> TrangThaiDangBais { get; set; } = new List<EnumInfoDto>();
    }
}