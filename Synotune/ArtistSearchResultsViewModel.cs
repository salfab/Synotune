using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Synology.AudioStationApi;

namespace Synotune
{
    public class ArtistSearchResultsViewModel
    {


        public ArtistSearchResultsViewModel(IEnumerable<SynoItem> items)
        {
            // TODO: Complete member initialization
            this.Artists = new ObservableCollection<SynoItem>(items);
        }

        public ObservableCollection<SynoItem> Artists { get; set; }
    }
}
