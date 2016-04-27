//------------------------------------------------------------------------------
// <copyright file="PreviewWindowCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using CodeConnect.GeneratorPreview.View;
using CodeConnect.GeneratorPreview.Execution;

namespace CodeConnect.GeneratorPreview
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class PreviewWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f4423be5-96d2-4f5f-b206-3b872a7db132");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;
        private GeneratorManager _manager;
        private readonly IShowAll _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private PreviewWindowCommand(Package package, IShowAll viewModel, GeneratorManager manager)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            _viewModel = viewModel;

            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            _manager = manager;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.ShowToolWindow, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static PreviewWindowCommand Instance
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
                return this._package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package, IShowAll viewModel, GeneratorManager manager)
        {
            Instance = new PreviewWindowCommand(package, viewModel, manager);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this._package.FindToolWindow(typeof(PreviewWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var previewWindow = window.Content as PreviewWindowControl;
            if (previewWindow != null)
            {
                previewWindow.Manager = _manager;
                previewWindow.DataContext = _viewModel;
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
