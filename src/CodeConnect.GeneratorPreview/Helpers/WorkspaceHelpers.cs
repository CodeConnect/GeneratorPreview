using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CodeConnect.GeneratorPreview.Helpers
{
    public static class WorkspaceHelpers
    {
        internal static Workspace CurrentWorkspace
        {
            get
            {
                var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                return componentModel.GetService<VisualStudioWorkspace>();
            }
        }

        internal static Solution CurrentSolution
        {
            get { return CurrentWorkspace?.CurrentSolution; }
        }

        internal static Document GetDocument(string filePath)
        {
            var project = CurrentSolution.Projects.Where(n => n.Documents.Any(m => m.FilePath == filePath)).FirstOrDefault();
            var document = project.Documents.Where(n => n.FilePath == filePath).Single();
            return document;
        }

        internal static async Task<Tuple<SyntaxNode, Document>> GetSelectedSyntaxNode(IVsTextManager textManager)
        {
            int startPosition, endPosition;
            string filePath;
            if (textManager.TryFindDocumentAndPosition(out filePath, out startPosition, out endPosition))
            {
                Document document;
                try
                {
                    document = WorkspaceHelpers.GetDocument(filePath);
                }
                catch (NullReferenceException ex)
                {
                    StatusBar.ShowStatus($"Error accessing the document. Try building the solution.");
                    return null;
                }
                var root = await document.GetSyntaxRootAsync();
                var element = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(startPosition, endPosition - startPosition));
                return Tuple.Create(element, document);
            }
            else
            {
                StatusBar.ShowStatus("To use generator preview, please navigate to C# code.");
                return null;
            }
        }
    }
}
