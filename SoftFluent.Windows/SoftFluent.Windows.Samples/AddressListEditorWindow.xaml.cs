using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SoftFluent.Windows.Samples
{
    /// <summary>
    /// Interaction logic for AddressListEditorWindow.xaml
    /// </summary>
    public partial class AddressListEditorWindow : Window
    {
        public AddressListEditorWindow()
        {
            InitializeComponent();
        }

        private void NewCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
            if (cvs == null)
                return;

            var collection = cvs.SourceCollection as ICollection<Address>;
            if (collection != null)
            {
                Address address = new Address();
                address.Line1 = "Empty";
                collection.Add(address);
                cvs.MoveCurrentToLast();
            }
        }

        private void DeleteCommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
            if (cvs == null)
                return;

            e.CanExecute = cvs.CurrentItem != null;
        }

        private void DeleteCommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var cvs = CollectionViewSource.GetDefaultView(EditorSelector.ItemsSource);
            if (cvs == null) 
                return;

            var currentItem = cvs.CurrentItem as Address;
            if (currentItem == null) 
                return;

            var collection = cvs.SourceCollection as ICollection<Address>;
            if (collection != null)
            {
                collection.Remove(currentItem);
            }
        }

        protected virtual void OnEditorWindowCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window)sender;
            PropertyGridProperty prop = window.DataContext as PropertyGridProperty;
            if (prop != null)
            {
                prop.Executed(sender, e);
                if (e.Handled)
                    return;
            }
            window.Close();
        }

        protected virtual void OnEditorWindowCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Window window = (Window)sender;
            PropertyGridProperty prop = window.DataContext as PropertyGridProperty;
            if (prop != null)
            {
                prop.CanExecute(sender, e);
                if (e.Handled)
                    return;
            }
            e.CanExecute = true;
        }
    }
}
