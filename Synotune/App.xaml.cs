using System;
using System.Linq;
using Synotune.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Synology.AudioStationApi;
using Windows.UI.Core;
using System.Collections.Generic;

namespace Synotune
{
    partial class App
    {
        // TODO: Create a data model appropriate for your problem domain to replace the sample data

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ShowCollection();
            Window.Current.Activate();
        }

        public static void ShowCollection()
        {
            // TODO : use a factory instead
            AudioStationSession audioStationSession = new AudioStationSession();
            CollectionPageViewModel viewModel = new CollectionPageViewModel(audioStationSession);

            var page = new CollectionPage(viewModel);
            //if (_sampleData == null) _sampleData = new SampleDataSource(page.BaseUri);
            //page.Items = _sampleData.GroupedCollections.Select((obj) => (Object)obj);
            Window.Current.Content = page;
        }

        public static void ShowSplit(IGroupInfo collection)
        {
            var page = new SplitPage();
            //if (_sampleData == null) _sampleData = new SampleDataSource(page.BaseUri);
            //if (collection == null) collection = _sampleData.GroupedCollections.First();
            page.Items = collection;
            //page.Context = collection.Key;
            Window.Current.Content = page;
        }

        internal static void ShowArtistSearchResults(IEnumerable<SynoItem> items)
        {
            var viewModel = new ArtistSearchResultsViewModel(items);
            var page = new ArtistSearchResults(viewModel);
            Window.Current.Content = page;

        }
    }
}
