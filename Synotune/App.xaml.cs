using System;
using System.Linq;
using Synotune.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Synology.AudioStationApi;
using Windows.UI.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Synotune.ViewModels;

namespace Synotune
{
    partial class App
    {
        private static Synology.AudioStationApi.AudioStationSession audioStationSession;
        // TODO: Create a data model appropriate for your problem domain to replace the sample data

        public App()
        {
            audioStationSession = new AudioStationSession();
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
            var albumViewModels = new ObservableCollection<ArtistViewModel>(items.Select(o => new ArtistViewModel(o)));
            foreach (var viewModel in albumViewModels)
            {
                var getAlbumsTask = App.audioStationSession.GetAlbumsForArtistAsync(viewModel.Artist);

                getAlbumsTask.ContinueWith(task => albumViewModels.Single(o=>o.Artist == task.AsyncState).Albums = new ObservableCollection<AlbumViewModel>(task.Result.Select(o => new AlbumViewModel(o))));
            } 
            var artistSearchViewModel = new ArtistSearchResultsViewModel(albumViewModels);
            var page = new ArtistSearchResults(artistSearchViewModel);
            Window.Current.Content = page;
        }
    }
}
