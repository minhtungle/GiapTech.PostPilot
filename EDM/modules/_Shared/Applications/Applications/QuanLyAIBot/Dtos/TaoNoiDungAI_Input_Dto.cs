using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyAIBot.Dtos
{
    public class TaoNoiDungAI_Input_Dto
    {
        public Guid IdAITool { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
    }
}