//------------------------------------------------------------------------------
// <copyright file="PreviewWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace CodeConnect.GeneratorPreview.View
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using Execution;

    /// <summary>
    /// Interaction logic for PreviewWindowControl.
    /// </summary>
    public partial class PreviewWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewWindowControl"/> class.
        /// </summary>
        public PreviewWindowControl()
        {
            this.InitializeComponent();
            this.DataContext = GeneratorManager.ViewModel;
            generateButton.Click += GenerateButtonClickHandler;
        }

        private async void GenerateButtonClickHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                await GeneratorManager.Instance.Generate();
                StatusBar.ShowStatus("Generation successful.");
            }
            catch (Exception ex)
            {
                StatusBar.ShowStatus("Generation failed: " + ex.Message);
            }
        }
    }
}