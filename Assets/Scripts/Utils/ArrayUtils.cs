using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public class ArrayUtils
    {
        public static T[] Concat<T>(T[] first, T[] second)
        {
            T[] result = new T[first.Length + second.Length];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Length);
            return result;
        }

        public static T[] RemoveAt<T>(T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        public static T[] Remove<T>(T[] source, T item)
        {
            int index = Array.IndexOf(source, item);
            if (index == -1)
                return source;

            return RemoveAt(source, index);
        }

        public static T[] Add<T>(T[] source, T item) 
        {
            T[] dest = new T[source.Length + 1];
            source.CopyTo(dest, 0);
            dest[source.Length] = item;
            return dest;
        }
    }
}
