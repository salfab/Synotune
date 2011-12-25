using System;
using System.Collections.Generic;
using Synology.AudioStationApi;
using Windows.Data.Json;
using System.Linq;

namespace OpenSyno.WinRT
{
    public class SynologyJsonDeserializationHelper
    {
        public static void ParseSynologyArtists(string result, out IEnumerable<SynoItem> artists, out long total, string urlBase)
        {

            JsonObject jObject = new JsonObject(result);
            artists = null;
            artists = from album in jObject["items"].GetArray()
                      select new SynoItem
                      {
                          AlbumArtUrl = BuildAbsoluteAlbumArtUrl(urlBase, album.GetObject()["albumArtURL"].GetString()),
                          Icon = album.GetObject()["icon"].GetString(),
                          IsContainer = album.GetObject()["is_container"].GetBoolean(),
                          IsTrack = album.GetObject()["is_track"].GetBoolean(),
                          ItemID = album.GetObject()["item_id"].GetString(),
                          ItemPid = album.GetObject()["item_pid"].GetString(),
                          Sequence = (int)album.GetObject()["sequence"].GetNumber(),
                          Support = album.GetObject()["support"].GetBoolean(),
                          Title = album.GetObject()["title"].GetString()
                      };
            total = (long)jObject["total"].GetNumber();
        }

        private static string BuildAbsoluteAlbumArtUrl(string urlBase, string relativeAlbumArtUrl)
        {
            // In the iPhone app : 
           // http://host/audio/iPhone/enumerate.cgi?action=get_cover&music_id=54289&sid=*******

            // in DSM 3.0
            // http://host/audio/[albumArtURL]

            // in DSM 3.1
            // http://host/webman/[albumArtURL]

            string rootPath = "/webman/";
            return urlBase + rootPath + relativeAlbumArtUrl;
        }

        public static void ParseSynologyAlbums(string content, out IEnumerable<SynoItem> albums, out long total, string urlBase)
        {
            ParseSynologyArtists(content, out albums, out total, urlBase);

            // need to do this for Synology 3.0 : 
            // NOTE : albums is passed with ToArray : the reason is that somehow, editing the albums fields while still in a form of a JSON.NET enumerable, modifications don't apply.
            // I must admit I didn't quite understand if it was a normal behavior or a bug in JSON.NET, but I was in a hurry and needed to fix this quickly - shame on me ;)
            albums = WorkaroundAlbumArtBug(albums.ToArray());
        }

        /// <summary>
        /// Workarounds the album art bug.
        /// </summary>
        /// <param name="albums">The albums.</param>
        /// <remarks>
        /// There a bug in Audio Station 3.0 where the AlbumArtUrl field does not point to an album art, but rather to an artist art. fixing the url on the client will work just fine untill they fix it.
        /// </remarks>
        private static IEnumerable<SynoItem> WorkaroundAlbumArtBug(IEnumerable<SynoItem> albums)
        {
            foreach (var synoItem in albums)
            {
                synoItem.AlbumArtUrl = synoItem.AlbumArtUrl.Replace("webUI/getcover.cgi/artist", "webUI/getcover.cgi/album");
            }
            return albums;
        }

        public static void ParseSynologyTracks(string content, out IEnumerable<SynoTrack> tracks, out long total, string urlBase)
        {
            JsonObject o = new JsonObject();
            o.Parse(content);
            var items = o["items"].GetArray();
            total = (long)o["total"].GetNumber();
            tracks = null;
            //tracks = from p in items
            //        select new SynoTrack
            //        {
            //            Album = p["album"].Value<string>(),
            //            AlbumArtUrl = BuildAbsoluteAlbumArtUrl(urlBase, p["albumArtURL"].Value<string>()),
            //            Artist = p["artist"].Value<string>(),
            //            Bitrate = p["bitrate"].Value<long>(),
            //            Channels = p["channels"].Value<int>(),
            //            //Class = p["class"].Value<string>(),
            //            Disc = p["disc"].Value<int>(),
            //            Duration = TimeSpan.FromSeconds(p["duration"].Value<int>()),
            //            //DateTime.ParseExact(p["duration"].Value<string>(), "m:ss", CultureInfo.InvariantCulture,DateTimeStyles.AllowLeadingWhite).TimeOfDay, // doesn't work if m > 59
            //            Genre = p["genre"].Value<string>(),
            //            Icon = p["icon"].Value<string>(),
            //            IsContainer = p["is_container"].Value<bool>(),
            //            IsTrack = p["is_track"].Value<bool>(),
            //            ItemID = p["item_id"].Value<string>(),
            //            ItemPid = p["item_pid"].Value<string>(),
            //            //ProtocolInfo = p["protocolinfo"].Value<string>(),
            //            Res = p["res"].Value<string>(),
            //            Sample = p["sample"].Value<long>(),
            //            Sequence = p["sequence"].Value<int>(),
            //            Size = long.Parse(p["size"].Value<string>()),
            //            Support = p["support"].Value<bool>(),
            //            Title = p["title"].Value<string>(),
            //            Track = p["track"].Value<int>(),
            //            Year = p["year"].Value<int>()
            //        }
            //    ;
        }
    }
}
