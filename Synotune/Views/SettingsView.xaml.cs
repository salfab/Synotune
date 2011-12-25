using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Synotune.Views
{
    public sealed partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
            this.Loaded += (s, ea) =>
                {
                    if (LoadingDefaultSettings != null)
                    {
                        var potentialOverride = new LoadingDefaultSettingsEventArgs();
                        LoadingDefaultSettings(this, potentialOverride);
                        this.Username = potentialOverride.Username;
                        this.Password = potentialOverride.Password;
                        this.Hostname = potentialOverride.Hostname;
                        this.Port = potentialOverride.Port;
                    }
                };
        }

      


        public event EventHandler<LoadingDefaultSettingsEventArgs> LoadingDefaultSettings;


        public ICommand SettingsChanged
        {
            get { return (ICommand)GetValue(SettingsChangedProperty); }
            set { SetValue(SettingsChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SettingsChanged.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingsChangedProperty =
            DependencyProperty.Register("SettingsChanged", "Object", typeof(SettingsView).FullName, new PropertyMetadata(null));

        

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Username.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", "Object", typeof(SettingsView).FullName, new PropertyMetadata(null, OnSettingsChanged ));

        private static void OnSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var settingsChangedCommand = (DelegateCommand<SettingsView>) ((SettingsView) d).SettingsChanged;

            if (settingsChangedCommand != null)
            {
                if (settingsChangedCommand.CanExecute(d))
                {
                    settingsChangedCommand.Execute(d);
                }
            }
        }


        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", "Object", typeof(SettingsView).ToString(), new PropertyMetadata(null, OnSettingsChanged));




        public string Hostname
        {
            get { return (string)GetValue(HostnameProperty); }
            set { SetValue(HostnameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hostname.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HostnameProperty =
            DependencyProperty.Register("Hostname", "Object", typeof(SettingsView).ToString(), new PropertyMetadata(null, OnSettingsChanged));





        public string Port
        {
            get { return (string)GetValue(PortProperty); }
            set { SetValue(PortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Port.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PortProperty =
            DependencyProperty.Register("Port", "Object", typeof(SettingsView).FullName, new PropertyMetadata(null, OnSettingsChanged));

        

        

        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerEventArgs e)
        {
            this.Focus();
            base.OnPointerPressed(e);
            e.Handled = true;
        }

        // View state management for switching among Full, Fill, Snapped, and Portrait states

        private DisplayPropertiesEventHandler _displayHandler;
        private TypedEventHandler<ApplicationLayout, ApplicationLayoutChangedEventArgs> _layoutHandler;
        private FrameworkElement lastFocusedTextbox;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_displayHandler == null)
            {
                _displayHandler = Page_OrientationChanged;
                _layoutHandler = Page_LayoutChanged;
            }
            DisplayProperties.OrientationChanged += _displayHandler;
            ApplicationLayout.GetForCurrentView().LayoutChanged += _layoutHandler;
            SetCurrentOrientation(this);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DisplayProperties.OrientationChanged -= _displayHandler;
            ApplicationLayout.GetForCurrentView().LayoutChanged -= _layoutHandler;
        }

        private void Page_LayoutChanged(object sender, ApplicationLayoutChangedEventArgs e)
        {
            SetCurrentOrientation(this);
        }

        private void Page_OrientationChanged(object sender)
        {
            SetCurrentOrientation(this);
        }

        private void SetCurrentOrientation(Control viewStateAwareControl)
        {
            VisualStateManager.GoToState(viewStateAwareControl, this.GetViewState(), false);
        }

        private String GetViewState()
        {
            var orientation = DisplayProperties.CurrentOrientation;
            if (orientation == DisplayOrientations.Portrait ||
                orientation == DisplayOrientations.PortraitFlipped) return "Portrait";
            var layout = ApplicationLayout.Value;
            if (layout == ApplicationLayoutState.Filled) return "Fill";
            if (layout == ApplicationLayoutState.Snapped) return "Snapped";
            return "Full";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // FocusManager.GetFocusedElement does not exist in the developer preview... ( http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/94ccb346-2e4d-426b-8324-3ba7bbcd24fe )
            this.lastFocusedTextbox = (FrameworkElement)sender;
        }


    }
}
