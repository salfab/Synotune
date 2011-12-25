using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synology.AudioStationApi;

namespace Synotune
{
    public class ArtistSearchResultsViewModel
    {
        private IEnumerable<Synology.AudioStationApi.SynoItem> items;

        public ArtistSearchResultsViewModel(IEnumerable<SynoItem> items)
        {
            // TODO: Complete member initialization
            this.items = items;
        }
    }
}
