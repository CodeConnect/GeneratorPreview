using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generators
{
    public partial class A
    {
        replace public int Number
        {
            get
            {
                return original + 2;
            }
        }

        replace private string Word = "Sample Text";
    }
}
