using System.Windows.Input;

namespace SoftFluent.Windows
{
    public interface IPropertyGridCommandHandler
    {
        void CanExecute(PropertyGridProperty property, object sender, CanExecuteRoutedEventArgs e);
        void Executed(PropertyGridProperty property, object sender, ExecutedRoutedEventArgs e);
    }
}