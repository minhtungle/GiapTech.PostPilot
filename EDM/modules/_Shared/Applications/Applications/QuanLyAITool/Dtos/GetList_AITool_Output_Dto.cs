using Applications.QuanLyAIBot.Models;
using EDM_DB;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyAITool.Dtos
{
    public class GetList_AITool_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbAITool> AITools { get; set; }
    }
}