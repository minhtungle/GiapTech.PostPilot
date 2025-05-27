using Public.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Public.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
        //public static string GetDescription(Enum value)
        //{
        //    FieldInfo field = value.GetType().GetField(value.ToString());
        //    var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        //    return attribute != null ? attribute.Description : value.ToString();
        //}
        public static string GetDescription<TEnum>(string tenTrangThai) where TEnum : struct, Enum
        {
            return ((TEnum)Enum.Parse(typeof(TEnum), tenTrangThai)).GetDescription();
        }
    }
}