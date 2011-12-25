using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Synotune.ViewModels;

namespace Synotune
{
    public class ArtistViewModel
    {
        public string Name { get; set; }

        public ObservableCollection<AlbumViewModel> Albums { get; set; }

        public ArtistViewModel()
        {
            Albums = new ObservableCollection<AlbumViewModel>();
            Albums.Add(new AlbumViewModel { Title = "1" });
            Albums.Add(new AlbumViewModel { Title = "2" });
        }
    }
}
