using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static int IndexOf<TType>(this TType[] array, Func<TType, bool> comparer)
        {
            for (int i = 0; i < array.Length; i++)
                if (comparer(array[i])) return i;

            return -1;
        }
    }
}
