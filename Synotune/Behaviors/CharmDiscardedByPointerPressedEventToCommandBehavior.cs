using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Synotune.Behaviors
{
    public class CharmDiscardedByPointerPressedEventToCommandBehavior
    {
        public static ICommand GetPointerPressedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(PointerPressedCommandProperty);
        }

        public static void SetPointerPressedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(PointerPressedCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for PointerPressedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerPressedCommandProperty =
            DependencyProperty.RegisterAttached("PointerPressedCommand", "Object", typeof(CharmDiscardedByPointerPressedEventToCommandBehavior).FullName, new PropertyMetadata(null, OnPointerPressedCommandChanged));

        private static void OnPointerPressedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FrameworkElement)d).PointerPressed += (s, ea) =>
            {
                var command = GetPointerPressedCommand(d);
                if (command.CanExecute(null))
                {                    
                    command.Execute(null);
                }
            };
        }

      

        
    }
}
