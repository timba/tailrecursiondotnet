using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailCS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Recursive(0));
        }

        public static int Recursive(int i)
        {
            if (i % 1000 == 0)
            {
                Console.WriteLine(i);
            }

            return Recursive(i + 1);
        }
    }
}
