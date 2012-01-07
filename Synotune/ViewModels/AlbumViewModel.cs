using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Synology.AudioStationApi;
using Windows.UI.Xaml.Data;

namespace Synotune.ViewModels
{
    public class AlbumViewModel : ObservableCollection<TrackViewModel>, IGroupInfo
    {
        public string Title { get; set; }

        public object Key 
        {
            get
            {
                return Title;
            }            
        }

        public AlbumViewModel(SynoItem album)
        {
            Album = album;
        }

        public new IEnumerator<object> GetEnumerator()
        {
            return (System.Collections.Generic.IEnumerator<object>)base.GetEnumerator();
        }

        public SynoItem Album { get; set; }
    }
}
