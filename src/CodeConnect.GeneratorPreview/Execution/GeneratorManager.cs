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
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeConnect.GeneratorPreview.Execution
{
    public class GeneratorManager
    {
        private TypeDeclarationSyntax _generator;
        private BaseMethodDeclarationSyntax _target;
        private Document _targetDocument;
        private Document _generatorDocument;
        private PreviewWindowPackage _previewWindowPackage;
        private IViewModel _viewModel;

        public static GeneratorManager Instance { get; private set; }
        public static object ViewModel { get; private set; }

        public GeneratorManager(PreviewWindowPackage previewWindowPackage, IViewModel viewModel)
        {
            _previewWindowPackage = previewWindowPackage;
            _viewModel = viewModel;

            // Set up static properties for other classes
            Instance = this;
            ViewModel = viewModel;
        }

        public void SetGenerator(TypeDeclarationSyntax type, Document document)
        {
            _generator = type;
            _generatorDocument = document;
            _viewModel.GeneratorName = $"{getUIName(type)} at {document.Name}";
        }

        public void SetTarget(BaseMethodDeclarationSyntax method, Document document)
        {
            _target = method;
            _targetDocument = document;
            _viewModel.TargetName = $"{getUIName(method)} at {document.Name}";
        }

        public async Task Generate(CancellationToken token = default(CancellationToken))
        {
            if (_target == null)
                throw new InvalidOperationException("Pick the target method.");
            if (_targetDocument == null)
                throw new InvalidOperationException("Target document is not set.");
            if (_generator == null)
                throw new InvalidOperationException("Pick the generator class.");
            if (_generatorDocument == null)
                throw new InvalidOperationException("Generator document is not set.");

            var generatorName = _generator.Identifier.ToString() + "_generated";

            var generator = new MyGenerator(context => context.AddCompilationUnit(generatorName, _generator.SyntaxTree));
            var generatorProjectPath = _generatorDocument.Project.OutputFilePath;
            var generatorReference = new MyGeneratorReference(ImmutableArray.Create<SourceGenerator>(generator), generatorProjectPath);

            var compilation = await _targetDocument.Project.GetCompilationAsync(token);
            var targetProjectId = _targetDocument.Project.Id;
            var generatorProjectId = _targetDocument.Project.Id;

            // TODO: An analyzer must be inside a .dll,
            // so we will need to create a dll project that will host the generator
            /*
             * or try to properly reference the other project as having analyzers...
             * 
            var workspace = new AdhocWorkspace(); // load existing solution there?
            var projectInfo = ProjectInfo.Create(
                projectId,
                VersionStamp.Default,
                name: "C",
                assemblyName: "C.dll",
                language: LanguageNames.CSharp,
                outputFilePath: outputPath + Path.DirectorySeparatorChar);
            var solution = workspace.CurrentSolution
                .AddProject(projectInfo)
                .AddMetadataReference(projectId, MscorlibRef)
                .AddDocument(docId, "C.cs", source0)
                .AddAnalyzerReference(projectId, generatorReference);
                */

            var workspace = Helpers.WorkspaceHelpers.CurrentWorkspace;
            var solution = workspace.CurrentSolution.AddAnalyzerReference(generatorProjectId, generatorReference);
            workspace.TryApplyChanges(solution);
            workspace.UpdateGeneratedDocumentsIfNecessary(targetProjectId);

            var updatedSolution = workspace.CurrentSolution;
            var updatedProject = updatedSolution.GetProject(targetProjectId);
            var doc = updatedSolution.GetDocument(_targetDocument.Id);
            var model = await doc.GetSemanticModelAsync();
            var updatedCompilation = model.Compilation;
            var updatedTrees = updatedCompilation.SyntaxTrees.ToList();
            var actualSource = updatedTrees.FirstOrDefault(n => n.FilePath == _targetDocument.FilePath)?.GetText().ToString();

            // In the end, try to decompile the IL back to C# and present this instead.

            _viewModel.GeneratedCode = actualSource;
            _viewModel.Errors = String.Join(Environment.NewLine, updatedCompilation.GetDiagnostics().Where(n => n.Severity >= DiagnosticSeverity.Error).Select(n => n.ToString()));
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

        private string getUIName(TypeDeclarationSyntax type)
        {
            return type.Identifier.ToString();
        }

        // Copied these from SourceGeneratorTests:

        private sealed class MyGenerator : SourceGenerator
        {
            private readonly Action<SourceGeneratorContext> _execute;

            internal MyGenerator(Action<SourceGeneratorContext> execute)
            {
                _execute = execute;
            }

            public override void Execute(SourceGeneratorContext context)
            {
                _execute(context);
            }
        }

        private sealed class MyGeneratorReference : AnalyzerReference
        {
            private readonly ImmutableArray<SourceGenerator> _generators;
            private readonly string _fullPath;

            internal MyGeneratorReference(ImmutableArray<SourceGenerator> generators, string path)
            {
                _generators = generators;
                _fullPath = path;
            }

            public override string FullPath
            {
                get { return _fullPath; }
            }

            public override object Id
            {
                get { return Guid.NewGuid(); }
            }

            public override ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(string language)
            {
                return ImmutableArray<DiagnosticAnalyzer>.Empty;
            }

            public override ImmutableArray<DiagnosticAnalyzer> GetAnalyzersForAllLanguages()
            {
                return ImmutableArray<DiagnosticAnalyzer>.Empty;
            }

            public override ImmutableArray<SourceGenerator> GetSourceGenerators(string language)
            {
                return _generators;
            }
        }
    }
}
