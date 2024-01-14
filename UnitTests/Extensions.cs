using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class Extensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            var random = new Random();

            var tempList = new List<T>(list);
            list.Clear();

            while (tempList.Count > 0)
            {
                int n = random.Next() % tempList.Count;
                list.Add(tempList[n]);
                tempList.RemoveAt(n);
            }
        }
    }
}
