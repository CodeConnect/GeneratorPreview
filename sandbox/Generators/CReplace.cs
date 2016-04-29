using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generators
{
    public partial class C
    {
        replace void F() { original(); }
        replace int P
        {
            get { return original; }
            set { original += value; } // P.get and P.set
        }
        replace object this[int index]
        {
            get { return original[index]; }
        }
        replace event EventHandler E
        {
            add { original += value; }
            remove { original -= value; }
        }
    }
}
