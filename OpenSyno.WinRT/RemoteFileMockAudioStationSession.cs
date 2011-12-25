namespace Synology.AudioStationApi
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class RemoteFileMockAudioStationSession : IAudioStationSession
    {
        private void OnFileDownloadResponseReceived(IAsyncResult ar)
        {
            var userState = (FileDownloadResponseReceivedUserState)ar.AsyncState;

            WebResponse response = userState.Request.EndGetResponse(ar);

            userState.GetResponseCallback(response, userState.SynoTrack);
        }


        public void LoginAsync(string login, string password, Action<string> callback, Action<Exception> callbackError, bool useSsl)
        {
            callback("#FAKETOKEN!");
        }

        public void SearchAllMusic(string pattern, Action<IEnumerable<SynoTrack>> callback, Action<Exception> callbackError)
        {
            throw new NotImplementedException();
        }

        public void SearchArtist(string pattern, Action<IEnumerable<SynoItem>> callback, Action<Exception> callbackError)
        {
            var results = new List<SynoItem>();

            results.Add(new SynoItem
            {
                Title = "Tom Waits",
                ItemPid = "musiclib_music_aa"
            });

            results.Add(new SynoItem
            {
                Title = "Mike Patton",
                ItemPid = "musiclib_music_aa"
            });

            results.Add(new SynoItem
            {
                Title = "65daysofstatic",
                ItemPid = "musiclib_music_aa"
            });

            callback(results);
        }

        public void GetAlbumsForArtist(SynoItem artist, Action<IEnumerable<SynoItem>, long, SynoItem> callback, Action<Exception> callbackError)
        {
            throw new NotImplementedException();
        }

        public void GetTracksForAlbum(SynoItem album, Action<IEnumerable<SynoTrack>, long, SynoItem> callback, Action<Exception> callbackError)
        {
            throw new NotImplementedException();
        }

        public bool IsSignedIn
        {
            get
            {
                return true;
            }
        }

        public string Host
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Port
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Token
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}