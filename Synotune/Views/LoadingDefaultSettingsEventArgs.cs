using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synotune.Views
{
    public class LoadingDefaultSettingsEventArgs
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Hostname { get; set; }
        public string Port { get; set; }
    }
}
