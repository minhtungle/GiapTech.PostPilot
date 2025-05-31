using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Applications.QuanLyAITool.Dtos
{
    public class WorkWithAITool_Input_Dto
    {
        public Guid IdAITool { get; set; }
        public string Prompt { get; set; }  = string.Empty;
    }
}