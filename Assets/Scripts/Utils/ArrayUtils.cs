using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utils
{
    public class ArrayUtils
    {
        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            int length = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                length += arrays[i].Length;
            }

            T[] result = new T[length];
            int offset = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                arrays[i].CopyTo(result, offset);
                offset += arrays[i].Length;
            }

            return result;
        }

        public static T[] ConcatArrays<T>(T[] array1, T[] array2)
        {
            T[] result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result;
        }

        public static T[] RemoveAt<T>(T[] array, int index)
        {
            T[] result = new T[array.Length - 1];
            Array.Copy(array, 0, result, 0, index);
            Array.Copy(array, index + 1, result, index, array.Length - index - 1);
            return result;
        }

        public static T[] Add<T>(T[] array, T element)
        {
            T[] result = new T[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = element;
            return result;
        }
    }
}
