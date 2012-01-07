using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Synology.AudioStationApi;
using Synotune.ViewModels;

namespace Synotune
{
    public class ArtistViewModel
    {
        public string Name { get; set; }

        public ObservableCollection<AlbumViewModel> Albums { get; set; }
        public SynoItem Artist { get; set; }

        public ArtistViewModel(SynoItem synoItem)
        {
            Artist = synoItem;

        }
    }
}
