using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWPF
{
    public static class Extensions
    {
        public static void ChangeEach<T>(this T[] array, Func<T, T> mutator)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = mutator(array[i]);
            }
        }
    }
}
