using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeConnect.GeneratorPreview.View;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeConnect.GeneratorPreview.Execution
{
    public class GeneratorManager
    {
        public delegate void OnGenerateEventArgs();
        public event OnGenerateEventArgs OnGenerate;

        private BaseMethodDeclarationSyntax _generator;
        private BaseMethodDeclarationSyntax _target;
        private PreviewWindowPackage _previewWindowPackage;
        private IViewModel _viewModel;

        public GeneratorManager(PreviewWindowPackage previewWindowPackage, IViewModel viewModel)
        {
            _previewWindowPackage = previewWindowPackage;
            _viewModel = viewModel;
        }

        public void SetGenerator(BaseMethodDeclarationSyntax method)
        {
            _generator = method;
            _viewModel.GeneratorName = getUIName(method);
        }

        public void SetTarget(BaseMethodDeclarationSyntax method)
        {
            _target = method;
            _viewModel.TargetName = getUIName(method);
        }

        public void Generate()
        {
            _viewModel.GeneratedCode = _target.ToFullString();
            _viewModel.Errors = String.Join(Environment.NewLine, _target.GetDiagnostics().Select(n => n.ToString()));
        }

        private string getUIName(BaseMethodDeclarationSyntax baseMethod)
        {
            var method = baseMethod as MethodDeclarationSyntax;
            if (method != null)
            {
                return method.Identifier.ToString();
            }
            else
            {
                return baseMethod.GetType().ToString();
            }
        }
    }
}
