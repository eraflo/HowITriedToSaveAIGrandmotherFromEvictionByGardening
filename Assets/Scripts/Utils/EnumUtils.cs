using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public static class EnumUtils
    {
        public static string GetEnumFieldToString<T>(T enumField)
        {
            return Enum.GetName(typeof(T), enumField);
        }

        public static T GetEnumFieldFromString<T>(string enumField)
        {
            return (T)Enum.Parse(typeof(T), enumField);
        }

        public static List<string> GetEnumFields<T>()
        {
            return Enum.GetNames(typeof(T)).ToList();
        }

        public static List<T> GetEnumFields<T>(bool includeNone = false)
        {
            var fields = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            if (includeNone)
            {
                fields.Insert(0, default(T));
            }
            return fields;
        }
    }
}
