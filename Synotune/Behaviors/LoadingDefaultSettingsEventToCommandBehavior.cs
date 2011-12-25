using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synotune.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Synotune.Behaviors
{
    public class LoadingDefaultSettingsEventToCommandBehavior
    {
        public static ICommand GetLoadingDefaultSettingsCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadingDefaultSettingsCommandProperty);
        }

        public static void SetLoadingDefaultSettingsCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadingDefaultSettingsCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for LoadingDefaultSettingsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadingDefaultSettingsCommandProperty =
            DependencyProperty.RegisterAttached("LoadingDefaultSettingsCommand", "Object", typeof(LoadingDefaultSettingsEventToCommandBehavior).FullName, new PropertyMetadata(null, OnLoadingDefaultSettingsCommandChanged));

        private static void OnLoadingDefaultSettingsCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SettingsView)d).LoadingDefaultSettings += (s, ea) =>
                {
                    if (GetLoadingDefaultSettingsCommand(d).CanExecute(ea))
                    {
                        GetLoadingDefaultSettingsCommand(d).Execute(ea);
                    }
                };
        }
    }
}
