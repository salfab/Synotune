using System;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Synotune.Behaviors
{
    public class TextBoxHelper
    {


        public static ICommand GetValidatedSearchQueryCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ValidatedSearchQueryCommandProperty);
        }

        public static void SetValidatedSearchQueryCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ValidatedSearchQueryCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for ValidatedSearchQueryCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValidatedSearchQueryCommandProperty =
            DependencyProperty.RegisterAttached("ValidatedSearchQueryCommand", "Object", typeof(TextBoxHelper).ToString(), new PropertyMetadata(null, ValidatedSearchQueryCommandChanged));

        private static void ValidatedSearchQueryCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox searchQueryTextbox = (TextBox)d;
            searchQueryTextbox.KeyDown += (s, ea) =>
            {
                if (ea.Key.HasFlag(VirtualKey.Enter))
                {
                    var command = GetValidatedSearchQueryCommand(d);
                    if (command.CanExecute(null))
	                {
                        command.Execute(null);
	                }
                }
            };
        }

        
    }
}
