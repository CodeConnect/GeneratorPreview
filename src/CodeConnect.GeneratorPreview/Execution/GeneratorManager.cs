using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeConnect.GeneratorPreview.View;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.IO;

namespace CodeConnect.GeneratorPreview.Execution
{
    public class GeneratorManager
    {
        private BaseMethodDeclarationSyntax _generator;
        private BaseMethodDeclarationSyntax _target;
        private Document _targetDocument;
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

        public void SetTarget(BaseMethodDeclarationSyntax method, Document document)
        {
            _target = method;
            _targetDocument = document;
            _viewModel.TargetName = $"{getUIName(method)} at {document.Name}";
        }

        public void Generate()
        {
            if (_target == null)
                throw new InvalidOperationException("Pick the target method.");
            if (_generator == null)
                throw new InvalidOperationException("Pick the generator.");

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
