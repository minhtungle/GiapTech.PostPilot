using Applications.QuanLyAIBot.Models;
using Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyAIBot.Dtos
{
    public class GetList_AIBot_Output_Dto
    {
        public List<ThaoTac> ThaoTacs { get; set; }
        public List<tbAIBotExtend> AIBots { get; set; }
    }
}