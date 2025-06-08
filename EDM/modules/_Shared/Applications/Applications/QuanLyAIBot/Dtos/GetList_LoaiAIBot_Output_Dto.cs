using Applications.QuanLyAIBot.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyAIBot.Dtos
{
    public class GetList_LoaiAIBot_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbLoaiAIBot> LoaiAIBots { get; set; }
    }
}