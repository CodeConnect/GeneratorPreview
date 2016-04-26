using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConnect.GeneratorPreview.View
{
    public interface ISetGeneratorName
    {
        string GeneratorName { set; }
    }

    public interface ISetTargetName
    {
        string TargetName { set; }
    }

    public interface ISetGeneratedCode
    {
        string GeneratedCode { set; }
    }

    public interface IShowAll
    {
        string GeneratorName { get; }
        string TargetName { get; }
        string GeneratedCode { get; }
    }
}
