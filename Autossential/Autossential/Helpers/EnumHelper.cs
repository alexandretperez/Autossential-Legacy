using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Autossential.Helpers
{
    public static class EnumHelper
    {
        public static Dictionary<string, Enum> EnumAsDictionary<TEnum>()
        {
            var list = new Dictionary<string, Enum>();
            var type = typeof(TEnum);
            var values = Enum.GetValues(type);
            foreach (Enum v in values)
            {
                var name = type.GetEnumName(v);
                var field = type.GetField(name);
                var attr = field?.GetCustomAttribute<DescriptionAttribute>();
                list.Add(attr?.Description ?? name, v);
            }
            return list;
        }
    }
}