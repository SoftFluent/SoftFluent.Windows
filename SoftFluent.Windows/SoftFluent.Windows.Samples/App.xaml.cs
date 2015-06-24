using System.Windows;
using System.Windows.Input;
using SoftFluent.Windows.Diagnostics;

namespace SoftFluent.Windows.Samples
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
#if DEBUG
            Tracing.Enable();
#endif
        }

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
