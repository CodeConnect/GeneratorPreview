using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        public static string Message { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine(Message);
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
