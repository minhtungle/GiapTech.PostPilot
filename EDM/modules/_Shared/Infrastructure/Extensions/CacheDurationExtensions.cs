using Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Extentions
{
    public static class CacheDurationExtensions
    {
        public static int ToMinutes(this CacheDuration duration)
        {
            return (int)duration;
        }
    }

}