using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Models
{
    public class Tree<T>
    {
        public int SoThuTu { get; set; }
        public T root { get; set; }
        public List<Tree<T>> nodes { get; set; } = new List<Tree<T>>();
    }
}