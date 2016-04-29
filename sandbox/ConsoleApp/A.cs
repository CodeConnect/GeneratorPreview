using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public partial class A
    {
        public int Number => Word.Length;

        private string Word = "hey";

        public int GetNumber()
        {
            return Number * 2;
        }
    }
}
