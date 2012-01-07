using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using Synology.AudioStationApi;
using System.Collections.ObjectModel;

namespace Synotune
{
    public class ArtistSearchResultsViewModel
    {


        public ArtistSearchResultsViewModel(IEnumerable<ArtistViewModel> items)
        {
            // TODO: Complete member initialization
            this.Artists = new ObservableCollection<ArtistViewModel>(items);
        }

        public ObservableCollection<ArtistViewModel> Artists { get; set; }
    }
}
