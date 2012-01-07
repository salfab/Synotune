using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Synology.AudioStationApi;

namespace Synotune
{
    public class SplitPageViewModel
    {
        public SplitPageViewModel()
        {
            Artists = new ObservableCollection<ArtistViewModel>();
            Artists.Add(new ArtistViewModel(null) { Name = "Bob" });
            Artists.Add(new ArtistViewModel(null) { Name = "Ralph" });
            Artists.Add(new ArtistViewModel(null) { Name = "Noah" });

            SearchCriteria = "Role";

            AudioStationSession session = new AudioStationSession();
            session.Host = "ds509.hamilcar.ch";
            session.Port = 5000;            
            session.LoginAsync("fabio", "fabioFTP",
                o =>
                {
                },
                e =>
                {
                },
                    false);

        }
        public ObservableCollection<ArtistViewModel> Artists { get; set; }

        public string SearchCriteria { get; set; }
    }
}
