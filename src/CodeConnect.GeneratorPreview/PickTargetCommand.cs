//------------------------------------------------------------------------------
// <copyright file="PickTargetCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using CodeConnect.GeneratorPreview.View;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeConnect.GeneratorPreview
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class PickTargetCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("1ecf9a6a-6521-444a-b5db-20951ade188a");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private readonly ISetTargetName _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickTargetCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private PickTargetCommand(Package package, ISetTargetName viewModel)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            _viewModel = viewModel;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static PickTargetCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package, ISetTargetName viewModel)
        {
            Instance = new PickTargetCommand(package, viewModel);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                var textManager = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
                var node = (await Helpers.WorkspaceHelpers.GetSelectedSyntaxNode(textManager));
                var baseMethod = node.AncestorsAndSelf().OfType<BaseMethodDeclarationSyntax>().FirstOrDefault();
                var method = baseMethod as MethodDeclarationSyntax;
                if (method != null)
                {
                    _viewModel.TargetName = method.Identifier.ToString();
                }
                else
                {
                    _viewModel.TargetName = baseMethod.GetType().ToString();
                }
                StatusBar.ShowStatus("Target picked.");

                var hack = _viewModel as ISetGeneratedCode;
                hack.GeneratedCode = baseMethod.ToFullString();
                hack.Errors = String.Join(Environment.NewLine, baseMethod.GetDiagnostics().Select(n => n.ToString()));
            }
            catch (Exception ex)
            {
                StatusBar.ShowStatus("Target not picked: " + ex);
            }
        }
    }
}
