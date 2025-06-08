using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Dtos
{
    public class EnumInfoDto
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
    }
}