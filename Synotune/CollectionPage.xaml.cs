using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
namespace Synotune
{
    public sealed partial class CollectionPage
    {
        public CollectionPage(CollectionPageViewModel viewModel)
        {
            SettingsPane pane = SettingsPane.GetForCurrentView();
            SettingsCommand cmd = new SettingsCommand(KnownSettingsCommand.Preferences, (a) =>
                                                                                            {
                                                                                                SettingPanel.Margin = ThicknessHelper.FromUniformLength(0);
                                                                                                viewModel.ViewIsShowingCharm = true;
                                                                                            });
            pane.ApplicationCommands.Add(cmd);
            

            InitializeComponent();
            DataContext = viewModel;
            BackButton.IsEnabled = false;
        }

        void ItemView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Construct the appropriate destination page and set its context appropriately
            var selectedItem = (sender as Selector).SelectedItem as IGroupInfo;
            App.ShowSplit(selectedItem);
        }

        private IEnumerable<Object> _items;
        public IEnumerable<Object> Items
        {
            get
            {
                return this._items;
            }

            set
            {
                this._items = value;
                CollectionViewSource.Source = value;
            }
        }

        // View state management for switching among Full, Fill, Snapped, and Portrait states

        private DisplayPropertiesEventHandler _displayHandler;
        private TypedEventHandler<ApplicationLayout, ApplicationLayoutChangedEventArgs> _layoutHandler;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_displayHandler == null)
            {
                _displayHandler = Page_OrientationChanged;
                _layoutHandler = Page_LayoutChanged;
            }
            DisplayProperties.OrientationChanged += _displayHandler;
            ApplicationLayout.GetForCurrentView().LayoutChanged += _layoutHandler;
            SetCurrentViewState(this);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DisplayProperties.OrientationChanged -= _displayHandler;
            ApplicationLayout.GetForCurrentView().LayoutChanged -= _layoutHandler;
        }

        private void Page_LayoutChanged(object sender, ApplicationLayoutChangedEventArgs e)
        {
            SetCurrentViewState(this);
        }

        private void Page_OrientationChanged(object sender)
        {
            SetCurrentViewState(this);
        }

        private void SetCurrentViewState(Control viewStateAwareControl)
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

        private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerEventArgs e)
        {
            if (SettingPanel.Margin.Right == 0)
            {
                SettingPanel.Margin = ThicknessHelper.FromLengths(0, 0, -346, 0);
            }           
        }
    }
}
