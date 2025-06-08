using Public.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Public.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public static string GetDescription<TEnum>(string tenTrangThai) where TEnum : struct, Enum
        {
            return ((TEnum)Enum.Parse(typeof(TEnum), tenTrangThai)).GetDescription();
        }
        public static string GetDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field.GetCustomAttributes(typeof(DisplayAttribute), false)
                            .FirstOrDefault() as DisplayAttribute;
            return attr?.Name ?? value.ToString();
        }
        public static List<EnumInfoDto> GetEnumInfoList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e =>
                {
                    var field = e.GetType().GetField(e.ToString());

                    var description = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .Cast<DescriptionAttribute>()
                        .FirstOrDefault()?.Description ?? "";

                    var displayName = field.GetCustomAttributes(typeof(DisplayAttribute), false)
                        .Cast<DisplayAttribute>()
                        .FirstOrDefault()?.Name ?? "";

                    return new EnumInfoDto
                    {
                        Value = Convert.ToInt32(e),
                        Name = e.ToString(),
                        Description = description,
                        DisplayName = displayName
                    };
                })
                .OrderBy(x => x.Value)
                .ToList();
        }
    }
}