﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace DIPS.UI.Editors
{
    /// <summary>
    /// Represents the editor used to choose files.
    /// </summary>
    public class FileEditor : ITypeEditor
    {
        /// <summary>
        /// Creates and returns a <see cref="FrameworkElement"/> allowing editing
        /// of the provided <see cref="PropertyItem"/> as a file.
        /// </summary>
        /// <param name="propertyItem">The <see cref="PropertyItem"/> to be treated
        /// as a file editor.</param>
        /// <returns>A <see cref="FrameworkElement"/> representing a file
        /// editor.</returns>
        public FrameworkElement ResolveEditor( PropertyItem propertyItem )
        {
            DockPanel container = new DockPanel();
            container.LastChildFill = true;

            Button openDialogButton = new Button();
            openDialogButton.Content = "...";
            openDialogButton.Click += new RoutedEventHandler( _onButtonClick );
            DockPanel.SetDock( openDialogButton, Dock.Right );

            _fileDisplayTextBox = new TextBox();
            _fileDisplayTextBox.IsReadOnly = true;
            _fileDisplayTextBox.Text = string.Empty;

            container.Children.Add( openDialogButton );
            container.Children.Add( _fileDisplayTextBox );

            // Value of property is bound to path of file.
            Binding binding = new Binding( "Value" );
            binding.Source = propertyItem;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnDataErrors = true;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding( _fileDisplayTextBox, TextBox.TextProperty, binding );

            return container;
        }


        /// <summary>
        /// Occurs when the button used to select files has been clicked.
        /// </summary>
        /// <param name="sender">N/A</param>
        /// <param name="e">N/A</param>
        private void _onButtonClick( object sender, RoutedEventArgs e )
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool? result = dialog.ShowDialog();
            if( result.HasValue && result.Value )
            {
                _fileDisplayTextBox.Text = dialog.FileName;
            }
        }


        /// <summary>
        /// Contains the TextBox used to display the current file.
        /// </summary>
        private TextBox _fileDisplayTextBox;
    }
}
