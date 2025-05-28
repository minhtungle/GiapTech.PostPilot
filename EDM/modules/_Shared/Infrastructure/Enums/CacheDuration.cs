using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Enums
{
    public enum CacheDuration
    {
        NeverExpire = -1,
        Minutes10 = 10,
        Minutes30 = 30,
        Hours1 = 60,
        Hours6 = 360,
        Days1 = 1440,
        Custom = 0 // Dự phòng nếu bạn cần logic riêng
    }
}