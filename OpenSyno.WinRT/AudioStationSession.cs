using System;
using System.Collections.Generic;
using System.Linq;
using OpenSyno.WinRT;
using Windows.Management;

namespace Synology.AudioStationApi
{
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.Networking.Sockets;

    [DataContract]
    public class AudioStationSession : IAudioStationSession
    {
        [DataMember]
        public string Host { get;  set; }

        [DataMember]
        public int Port { get;  set; }

        [DataMember]
        public string Token { get; set; }

        public void LoginAsync(string login, string password, Action<string> callback, Action<Exception> callbackError, bool useSsl)
        {
            if (login == null) throw new ArgumentNullException("login");
            if (password == null) throw new ArgumentNullException("password");

            

            

            Uri uri = new UriBuilder
                {
                    Host = this.Host,
                    Path = @"/webman/login.cgi",
                    Query = string.Format("username={0}&passwd={1}", login, password),
                    Port = this.Port,
                    Scheme = useSsl ? "https" : "http"
                }.Uri;

            // uri = new Uri("https://ds509.hamilcar.ch:5001/webman/index.cgi");
            // uri = new Uri("http://www.google.com/accounts/ServiceLogin?service=mail&passive=true&rm=false&continue=https%3A%2F%2Fmail.google.com%2Fmail%2F%3Fui%3Dhtml%26zy%3Dl&bsv=llya694le36z&ss=1&scc=1&ltmpl=default&ltmplcache=2&from=login");

            HttpClient client = new HttpClient();

            var response = client.GetAsync(uri.AbsoluteUri);
            response.ContinueWith<Task<HttpResponseMessage>>(message =>
                                                  {

                                                      HttpResponseMessage result = message.Result;
                                                      if (message.Exception != null)
                                                      {
                                                          if (uri.Scheme == "https")
                                                          {
                                                              throw new SynoNetworkException("Open Syno could not connect to the server. Please make sure your server's SSL certificate has been issued by a trusted Certificate Authority. see http://bit.ly/qODji5  for further detail.", message.Exception);
                                                          }

                                                          throw new SynoNetworkException("Open Syno could not complete the operation. Please check that your phone is not in flight mode.", message.Exception);
                                                      }
                                                      else
                                                      {
                                          
                                                          string rawCookie = result.Headers.First(header => header.Key.Contains("Set-Cookie")).Value.First(); // message.Result.Headers.["Set-Cookie"]
                                                          if (rawCookie == null)
                                                          {
                                                              throw new SynoLoginException("The login and the password don't match, please check your credentials", null);
                                                          }

                                                          string cookie = rawCookie.Split(';').Single(s => s.StartsWith("id="));
                                                          this.Token = cookie;
                                                          
                                                      }
                                                      return message;
                                                  });
            
        }

        public void SearchAllMusic(string pattern, Action<IEnumerable<SynoTrack>> callback, Action<Exception> callbackError)
        {
            string urlBase = string.Format("http://{0}:{1}", this.Host, this.Port);
            var url = urlBase + "/webman/modules/AudioStation/webUI/audio_browse.cgi";

            HttpWebRequest request = BuildRequest(url);

            int limit = 100;
            string postString = string.Format(@"action=search&target=musiclib_root&server=musiclib_root&category=all&keyword={0}&start=0&limit={1}", pattern, limit);
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postString);


            request.BeginGetRequestStream(ar =>
            {
                // Just make sure we retrieve the right web request : no access to modified closure.
                HttpWebRequest webRequest = (HttpWebRequest)ar.AsyncState;

                var requestStream = webRequest.EndGetRequestStream(ar);
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Dispose();

                request.BeginGetResponse(
                    responseAr =>
                    {
                        // Just make sure we retrieve the right web request : no access to modified closure.                        
                        var httpWebRequest = responseAr.AsyncState;

                        var webResponse = webRequest.EndGetResponse(responseAr);
                        var responseStream = webResponse.GetResponseStream();
                        var reader = new StreamReader(responseStream);
                        var content = reader.ReadToEnd();

                        long count;
                        IEnumerable<SynoTrack> tracks;
                        SynologyJsonDeserializationHelper.ParseSynologyTracks(content, out tracks, out count, urlBase);

                        //var isOnUiThread = Windows.Management.Deployment.Current.Dispatcher.CheckAccess();
                        //if (isOnUiThread)
                        //{
                        //    if (count > limit)
                        //    {
                        //        MessageBox.Show(string.Format("number of available artists ({0}) exceeds supported limit ({1})", count, limit));
                        //    }
                        //    callback(tracks);
                        //}
                        //else
                        //{
                        //    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        //    {
                        //        if (count > limit)
                        //        {
                        //            MessageBox.Show(string.Format("number of available artists ({0}) exceeds supported limit ({1})", count, limit));
                        //        }
                        //        callback(tracks);
                        //    });
                        //}
                    },
                    webRequest);
            },
                request);
        }

        public void SearchArtist(string pattern, Action<IEnumerable<SynoItem>> callback, Action<Exception> callbackError)
        {
            string urlBase = string.Format("http://{0}:{1}", this.Host, this.Port);
            var url = urlBase + "/audio/webUI/audio_browse.cgi";

            HttpWebRequest request = BuildRequest(url);

            // TODO : Find a way to retrieve the whole list by chunks of smaller size to have something to show earlier... or stream the JSON and parse it on the fly if it is possible
            int limit = 5000;
            string postString = string.Format(@"sort=title&dir=ASC&action=browse&target=musiclib_music_aa&server=musiclib_music_aa&category=&keyword={0}&start=0&limit={1}", pattern, limit);
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postString);

            

            var asyncResult = request.BeginGetRequestStream(null , request);

            Task t = Task.Factory.FromAsync(asyncResult, (ar) => 
                {
                    // Just make sure we retrieve the right web request : no access to modified closure.
                    HttpWebRequest webRequest = (HttpWebRequest)ar.AsyncState;

                    var requestStream = webRequest.EndGetRequestStream(ar);
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Dispose();

                    var getResponseAr = request.BeginGetResponse(null, webRequest);
                    Task getResponseTask = Task.Factory.FromAsync(getResponseAr, (responseAr) =>
                    {
                        // Just make sure we retrieve the right web request : no access to modified closure.                        
                        var httpWebRequest = responseAr.AsyncState;
                        if (!webRequest.HaveResponse)
                        {
                            throw new SynoSearchException("Error connecting to search engine", null);
                            // FIXME : Use an error handling service
                            //var action = new Action(() =>MessageBox.Show("Error connecting to search engine", "Connection error",MessageBoxButton.OK));   

                            //if (Deployment.Current.CheckAccess())
                            //{
                            //    action();
                            //}
                            //else
                            //{
                            //    Deployment.Current.Dispatcher.BeginInvoke(action);
                            //}
                            return;
                        }
                        var webResponse = webRequest.EndGetResponse(responseAr);

                        var responseStream = webResponse.GetResponseStream();

                        if (webResponse.Headers["Content-Encoding"].Contains("gzip"))
                            responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                        else if (webResponse.Headers["Content-Encoding"].Contains("deflate"))
                            responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                        StreamReader reader = new StreamReader(responseStream);

                        var content = reader.ReadToEnd();

                        long count;
                        IEnumerable<SynoItem> artists;
                        SynologyJsonDeserializationHelper.ParseSynologyArtists(content, out artists, out count, urlBase);



                        //var isOnUiThread = Windows.Management.Deployment.Current.Dispatcher.CheckAccess();
                        //if (isOnUiThread)
                        //{
                        //    if (count > limit)
                        //    {
                        //        // FIXME : Use an error handling service
                        //        MessageBox.Show(string.Format("number of available artists ({0}) exceeds supported limit ({1})", count, limit));
                        //    }
                        //    callback(artists);
                        //}
                        //else
                        //{
                        //    Windows.Management.Deployment.Current.Dispatcher.BeginInvoke(() =>
                        //        {
                        //            if (count > limit)
                        //            {
                        //                // FIXME : Use an error handling service
                        //                MessageBox.Show(string.Format("number of available artists ({0}) exceeds supported limit ({1})", count, limit));
                        //            }
                        callback(artists);
                        //        });
                        //}
                    },
                    TaskCreationOptions.None, 
                    TaskScheduler.FromCurrentSynchronizationContext()
                    );

                },
                TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            //CookieAwareWebClient wc = new CookieAwareWebClient();

            //Uri uri =
            //    new UriBuilder
            //    {
            //        Host = _host,
            //        Path = @"/webman/login.cgi",
            //        Query = postString,
            //        Port = _port
            //    }.Uri;

            //HttpWebRequest webRequest = (HttpWebRequest)wc.GetWebRequest(uri);

            //webRequest.CookieContainer = new CookieContainer();
            //webRequest.CookieContainer.SetCookies(new Uri(url), _token);

            //wc.DownloadStringCompleted += (sender, ea) =>
            //                                  {
            //                                      if (ea.Error != null)
            //                                      {
            //                                          callbackError(ea.Error);
            //                                          return;
            //                                      }
            //                                      long count;
            //                                      IEnumerable<SynoItem> artists;
            //                                      SynologyJsonDeserializationHelper.ParseSynologyArtists(
            //                                                                        ea.Result,
            //                                                                        out artists,
            //                                                                        out count,
            //                                                                        urlBase);
            //                                      callback(artists);
            //                                  };
            //wc.DownloadStringAsync(uri);          
        }

        private HttpWebRequest BuildRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Accept = "*/*";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            // Not supported yet, but that would decrease the bandwidth usage from 1.3 Mb to 83 Kb ... Pretty dramatic, ain't it ?
            request.Headers["Accept-Encoding"] = "gzip, deflate";

            //request.UserAgent = "OpenSyno";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.SetCookies(new Uri(url), this.Token);

            request.Method = "POST";
            return request;
        }

        public void GetAlbumsForArtist(SynoItem artist, Action<IEnumerable<SynoItem>, long, SynoItem> callback, Action<Exception> callbackError)
        {
            string urlBase = string.Format("http://{0}:{1}", this.Host, this.Port);
            var url = urlBase + "/audio/webUI/audio_browse.cgi";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Accept = "*/*";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            // Not supported yet, but that would decrease the bandwidth usage from 1.3 Mb to 83 Kb ... Pretty dramatic, ain't it ?
            //request.Headers["Accept-Encoding"] = "gzip, deflate";

            //request.UserAgent = "OpenSyno";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.SetCookies(new Uri(url), this.Token);

            request.Method = "POST";

            int limit = 10000;
            string urlEncodedItemId = System.Uri.EscapeDataString(artist.ItemID);
            string postString = string.Format(@"action=browse&target={0}&server=musiclib_music_aa&category=&keyword=&start=0&sort=title&dir=ASC&limit={1}", urlEncodedItemId, limit);
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postString);



            request.BeginGetRequestStream(ar =>
            {
                // Just make sure we retrieve the right web request : no access to modified closure.
                HttpWebRequest webRequest = (HttpWebRequest)ar.AsyncState;

                var requestStream = webRequest.EndGetRequestStream(ar);
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Dispose();

                request.BeginGetResponse(
                    responseAr =>
                    {
                        // Just make sure we retrieve the right web request : no access to modified closure.                        
                        var httpWebRequest = responseAr.AsyncState;

                        var webResponse = webRequest.EndGetResponse(responseAr);
                        var responseStream = webResponse.GetResponseStream();
                        var reader = new StreamReader(responseStream);
                        var content = reader.ReadToEnd();

                        long count;
                        IEnumerable<SynoItem> albums;
                        SynologyJsonDeserializationHelper.ParseSynologyAlbums(content, out albums, out count, urlBase);



                        //var isOnUiThread = Deployment.Current.Dispatcher.CheckAccess();
                        //if (isOnUiThread)
                        //{
                        //    if (count > limit)
                        //    {
                        //        MessageBox.Show(string.Format("number of available albums ({0}) exceeds supported limit ({1})", count, limit));
                        //    }
                        //    callback(albums, count, artist);
                        //}
                        //else
                        //{
                        //    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        //    {
                        //        if (count > limit)
                        //        {
                        //            MessageBox.Show(string.Format("number of available artists ({0}) exceeds supported limit ({1})", count, limit));
                        //        }
                        //        callback(albums, count, artist);
                        //    });
                        //}
                    },
                    webRequest);
            },
                request);
        }

        public void GetTracksForAlbum(SynoItem album, Action<IEnumerable<SynoTrack>, long, SynoItem> callback, Action<Exception> callbackError)
        {
            string urlBase = string.Format("http://{0}:{1}", this.Host, this.Port);

            var url = urlBase + "/audio/webUI/audio_browse.cgi";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Accept = "*/*";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

            // Not supported yet, but that would decrease the bandwidth usage from 1.3 Mb to 83 Kb ... Pretty dramatic, ain't it ?
            //request.Headers["Accept-Encoding"] = "gzip, deflate";

            //request.UserAgent = "OpenSyno";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.SetCookies(new Uri(url), this.Token);

            request.Method = "POST";

            int limit = 10000;

            string urlEncodedItemId = System.Uri.EscapeDataString(album.ItemID);
            string postString = string.Format(@"action=browse&target={0}&server=musiclib_music_aa&category=&keyword=&start=0&sort=title&dir=ASC&limit={1}",urlEncodedItemId, limit);
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postString);

            request.BeginGetRequestStream(ar =>
            {
                // Just make sure we retrieve the right web request : no access to modified closure.
                HttpWebRequest webRequest = (HttpWebRequest)ar.AsyncState;

                var requestStream = webRequest.EndGetRequestStream(ar);
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Dispose();

                request.BeginGetResponse(
                    responseAr =>
                    {
                        // Just make sure we retrieve the right web request : no access to modified closure.                        
                        var httpWebRequest = responseAr.AsyncState;

                        var webResponse = webRequest.EndGetResponse(responseAr);
                        var responseStream = webResponse.GetResponseStream();
                        var reader = new StreamReader(responseStream);
                        var content = reader.ReadToEnd();

                        long total;
                        IEnumerable<SynoTrack> tracks;
                        SynologyJsonDeserializationHelper.ParseSynologyTracks(content, out tracks, out total, urlBase);

                        tracks = tracks.OrderBy(o => o.Track);

                        //var isOnUiThread = Deployment.Current.Dispatcher.CheckAccess();
                        //if (isOnUiThread)
                        //{
                        //    if (total > limit)
                        //    {
                        //        MessageBox.Show(string.Format("number of available albums ({0}) exceeds supported limit ({1})", total, limit));
                        //    }
                        //    callback(tracks, total, album);
                        //}
                        //else
                        //{
                        //    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        //    {
                        //        if (total > limit)
                        //        {
                        //            MessageBox.Show(string.Format("number of available artists ({0}) exceeds supported limit ({1})", total, limit));
                        //        }
                        //        callback(tracks, total, album);
                        //    });
                        //}
                    },
                    webRequest);
            },
                request);
        }

        public bool IsSignedIn
        {
            get
            {
                return this.Token != null;
            }
        }
    }

    public class SynoSearchException : Exception
    {
        public SynoSearchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class SynoLoginException : Exception
    {
        public SynoLoginException(string message, Exception innerException) : base(message, innerException)
        {            
        }
    }

    public class SynoNetworkException : Exception
    {
        public SynoNetworkException(string message, Exception innerException) : base(message, innerException)
        {            
        }
    }
}