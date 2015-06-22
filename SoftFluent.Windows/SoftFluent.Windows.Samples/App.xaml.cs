using System.Windows;
using System.Windows.Input;

namespace SoftFluent.Windows.Samples
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnEditorWindowCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnEditorWindowCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window)sender;
            window.DialogResult = false;
            window.Close();
        }
    }
}
