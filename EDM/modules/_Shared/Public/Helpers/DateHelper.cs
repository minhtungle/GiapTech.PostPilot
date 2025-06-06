using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Helpers
{
    public static class DateHelper
    {
        public static (DateTime? Start, DateTime? End) ParseThangNam(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (null, null);

            var parts = input.Split('/');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int month) &&
                int.TryParse(parts[1], out int year))
            {
                try
                {
                    var start = new DateTime(year, month, 1);
                    var end = start.AddMonths(1).AddDays(-1);
                    return (start, end);
                }
                catch
                {
                    return (null, null);
                }
            }
            else if (parts.Length == 1 && int.TryParse(parts[0], out int onlyYear))
            {
                try
                {
                    var start = new DateTime(onlyYear, 1, 1);
                    var end = new DateTime(onlyYear, 12, 31);
                    return (start, end);
                }
                catch
                {
                    return (null, null);
                }
            }

            return (null, null);
        }
    }

}