using Synotune.Views;
using Windows.UI.Xaml.Input;
using Windows.Storage;
using Synology.AudioStationApi;
using Windows.UI.Xaml;


namespace Synotune
{
    public class CollectionPageViewModel 
    {
        private ApplicationData applicationData;
        private ApplicationDataContainer localSettings;
        private ApplicationDataCompositeValue credentials;
        private AudioStationSession session;
        public ICommand SettingsChangedCommand { get; set; }
        public ICommand ValidatedSearchQueryCommand { get; set; }

        public CollectionPageViewModel(AudioStationSession audioStationSession)
        {
            this.session = audioStationSession;
            InitializeApplicationSettingsSections();

            SettingsChangedCommand = new DelegateCommand<SettingsView>(OnSettingsChanged);
            LoadingDefaultSettingsCommand = new DelegateCommand<LoadingDefaultSettingsEventArgs>(OnLoadingDefaultSettings);
            PointerPressedCommand = new DelegateCommand(OnPointerPressed);
            ValidatedSearchQueryCommand = new DelegateCommand(OnValidatedSearchQuery);
        }

        private void OnValidatedSearchQuery()
        {
            // if artist is selected
            this.session.SearchArtist("tom waits" ,(items) => 
                {
                    
                }
            ,null);
        }

        private void OnPointerPressed()
        {
            if (!ViewIsShowingCharm)
            {
                return;
            }
            ViewIsShowingCharm = false;
            // TODO : execute only if the settings view is shown...
            if (credentials["Hostname"] != null)
            {
                this.session.Host = (string)credentials["Hostname"];
                
            }
            if (credentials["Port"] != null)
            {
                this.session.Port = int.Parse((string)credentials["Port"]);
            }
            this.session.LoginAsync((string)credentials["Username"], (string)credentials["Password"], LoginAsyncCallback, LoginAsyncCallbackError, false);
        }

        private void LoginAsyncCallbackError(System.Exception obj)
        {
            throw new System.NotImplementedException();
        }

        private void LoginAsyncCallback(string obj)
        {
            throw new System.NotImplementedException();
        }

        private void InitializeApplicationSettingsSections()
        {
            applicationData = ApplicationData.Current;
            localSettings = applicationData.LocalSettings;
            if (localSettings.Values["Credentials"] == null)
            {
                credentials = new Windows.Storage.ApplicationDataCompositeValue();
            }
            else
            {
                credentials = (ApplicationDataCompositeValue)localSettings.Values["Credentials"];
            }
        }

        private void OnSettingsChanged(SettingsView settingsView)
        {           
            credentials["Username"] = settingsView.Username;
            credentials["Password"] = settingsView.Password;
            credentials["Hostname"] = settingsView.Hostname;
            credentials["Port"] = settingsView.Port;
            localSettings.Values["Credentials"] = credentials;
            //ApplicationData.Current.LocalSettings.Values.
            applicationData.SignalDataChanged();
        }

        private void OnLoadingDefaultSettings(LoadingDefaultSettingsEventArgs overridableSettings)
        {
            if (credentials["Username"] != null)
	        {
                overridableSettings.Username = (string)credentials["Username"];		 
	        }

            if (credentials["Password"] != null)
            {
                overridableSettings.Password = (string)credentials["Password"];                
            }

            if (credentials["Hostname"] != null)
            {
                overridableSettings.Hostname = (string)credentials["Hostname"];                
            }

            if (credentials["Port"] != null)
            {
                overridableSettings.Port = (string)credentials["Port"];                
            }
        }

        public ICommand PointerPressedCommand { get; set; }

        public ICommand LoadingDefaultSettingsCommand { get; set; }

        public bool ViewIsShowingCharm { get; set; }
    }
}