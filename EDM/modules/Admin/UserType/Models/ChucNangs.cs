using EDM_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserType.Models
{
    public class default_tbChucNangExtend : default_tbChucNang
    {
        public List<default_tbChucNangExtend> ChucNangs { get; set; } = new List<default_tbChucNangExtend>();
    }
}