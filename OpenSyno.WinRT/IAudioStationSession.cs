namespace Synology.AudioStationApi
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public interface IAudioStationSession
    {
        void LoginAsync(string login, string password, Action<string> callback, Action<Exception> callbackError, bool useSsl);

        void SearchAllMusic(string pattern, Action<IEnumerable<SynoTrack>> callback, Action<Exception> callbackError);
        void SearchArtist(string pattern, Action<IEnumerable<SynoItem>> callback, Action<Exception> callbackError);
        void GetAlbumsForArtist(SynoItem artist, Action<IEnumerable<SynoItem>, long, SynoItem> callback, Action<Exception> callbackError);
        void GetTracksForAlbum(SynoItem album, Action<IEnumerable<SynoTrack>, long, SynoItem> callback, Action<Exception> callbackError);

        bool IsSignedIn { get; }

        [DataMember]
        string Host { get; set; }

        [DataMember]
        int Port { get; set; }

        [DataMember]
        string Token { get; set; }
    }
}