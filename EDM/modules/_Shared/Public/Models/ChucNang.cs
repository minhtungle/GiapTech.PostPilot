using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Models
{
    public class ChucNangs
    {
        public ChucNang ChucNang { get; set; } = new ChucNang();
        public List<ThaoTac> ThaoTacs { get; set; } = new List<ThaoTac>();
    }
    public class ChucNang
    {
        public Guid IdChucNang { set; get; }
        public string MaChucNang { set; get; }
    }
}