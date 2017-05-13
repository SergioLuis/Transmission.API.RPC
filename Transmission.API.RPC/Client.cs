using System;
using System.IO;
using System.Net;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Transmission.API.RPC.Arguments;
using Transmission.API.RPC.Common;
using Transmission.API.RPC.Entity;

namespace Transmission.API.RPC
{
    public class Client
    {
        public string Host { get { return mHost; } }

        public string SessionID { get { return mSessionId; } }

        public int CurrentTag { get { return mCurrentTag; } }

        /// <summary>
        /// Initialize client
        /// </summary>
        /// <param name="host">Host adresse</param>
        /// <param name="sessionID">Session ID</param>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        public Client(
            string host,
            string sessionID = null,
            string login = null,
            string password = null)
        {
            mHost = host;
            mSessionId = sessionID;

            if (string.IsNullOrWhiteSpace(login))
                return;

            mAuthorization = GetBasicHttpAuthHeader(login, password);
            mAuthorizationNeeded = true;
        }

        /// <summary>
        /// Close current session (API: session-close)
        /// </summary>
        public TransmissionResponse CloseSession()
        {
            return SendRequest(new TransmissionRequest(RpcMethods.Session.Close));
        }

        /// <summary>
        /// Get information of current session (API: session-get)
        /// </summary>
        /// <returns>Session information</returns>
        public SessionInfo GetSessionInformation()
        {
            return SendRequest(
                new TransmissionRequest(RpcMethods.Session.Get)).Deserialize<SessionInfo>();
        }

        /// <summary>
        /// Set information to current session (API: session-set)
        /// </summary>
        /// <param name="settings">New session settings</param>
        public TransmissionResponse SetSessionSettings(SessionSettings settings)
        {
            TransmissionRequest request =
                new TransmissionRequest(RpcMethods.Session.Set);

            request.AddArguments(settings);

            return SendRequest(request);
        }

        /// <summary>
        /// Get session stat
        /// </summary>
        /// <returns>Session stat</returns>
        public Statistic GetSessionStatistic()
        {
            return SendRequest(
                new TransmissionRequest(RpcMethods.Session.Stats)).Deserialize<Statistic>();
        }

        /// <summary>
        /// Add torrent (API: torrent-add)
        /// </summary>
        /// <returns>Torrent info (ID, Name and HashString)</returns>
        public NewTorrentInfo TorrentAdd(NewTorrent newTorrent)
        {
            AssertNewTorrentValid(newTorrent);

            TransmissionRequest request =
                new TransmissionRequest(RpcMethods.Torrent.Add);
            request.AddArguments(newTorrent);

            TransmissionResponse response = SendRequest(request);

            JObject responseModel = response.Deserialize<JObject>();

            if (responseModel == null || responseModel.First == null)
                return null;

            JToken value = null;

            if (responseModel.TryGetValue("torrent-duplicate", out value))
                return JsonConvert.DeserializeObject<NewTorrentInfo>(value.ToString());

            if (responseModel.TryGetValue("torrent-added", out value))
                return JsonConvert.DeserializeObject<NewTorrentInfo>(value.ToString());

            return null;
        }

        /// <summary>
        /// Set torrent params (API: torrent-set)
        /// </summary>
        /// <param name="torrentSet">New torrent params</param>
        public TransmissionResponse TorrentSet(TorrentSettings settings)
        {
            TransmissionRequest request =
                new TransmissionRequest(RpcMethods.Torrent.Set);

            request.AddArguments(settings);

            return SendRequest(request);
        }

        /// <summary>
        /// Get fields of torrents from ids (API: torrent-get)
        /// </summary>
        /// <param name="fields">Fields of torrents</param>
        /// <param name="ids">IDs of torrents (null or empty for get all torrents)</param>
        /// <returns>Torrents info</returns>
        public TransmissionTorrents TorrentGet(string[] fields, params int[] ids)
        {
            TransmissionRequest request = new TransmissionRequest(RpcMethods.Torrent.Get);
            request.AddArgument("fields", fields);

            if (ids != null && ids.Length > 0)
                request.AddArgument("ids", ids);

            TransmissionResponse response = SendRequest(request);
            TransmissionTorrents result =
                response.Deserialize<TransmissionTorrents>();

            return result;
        }

        /// <summary>
        /// Remove torrents (API: torrent-remove)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        /// <param name="deleteLocalData">Remove local data</param>
        public void TorrentRemove(int[] ids, bool deleteData = false)
        {
            var request = new TransmissionRequest(RpcMethods.Torrent.Remove);
            request.AddArgument("ids", ids);
            request.AddArgument("delete-local-data", deleteData);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Start torrents (API: torrent-start)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentStart(int[] ids)
        {
            var request = new TransmissionRequest(RpcMethods.Torrent.Start);
            request.AddArgument("ids", ids);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Start now torrents (API: torrent-start-now)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentStartNow(int[] ids)
        {
            var request = new TransmissionRequest(RpcMethods.Torrent.StartNow);
            request.AddArgument("ids", ids);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Stop torrents (API: torrent-stop)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentStop(int[] ids)
        {
            var request = new TransmissionRequest(RpcMethods.Torrent.Stop);
            request.AddArgument("ids", ids);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Verify torrents (API: torrent-verify)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentVerify(int[] ids)
        {
            var request = new TransmissionRequest(RpcMethods.Torrent.Verify);
            request.AddArgument("ids", ids);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Move torrents in queue on top (API: queue-move-top)
        /// </summary>
        /// <param name="ids">Torrents id</param>
        public void TorrentQueueMoveTop(int[] ids)
        {
            var request = new TransmissionRequest(RpcMethods.Queue.MoveTop);
            request.AddArgument("ids", ids);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Move up torrents in queue (API: queue-move-up)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveUp(int[] ids)
        {
            var request = new TransmissionRequest(RpcMethods.Queue.MoveUp);
            request.AddArgument("ids", ids);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Move down torrents in queue (API: queue-move-down)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveDown(int[] ids)
        {
            var request = new TransmissionRequest(RpcMethods.Queue.MoveDown);
            request.AddArgument("ids", ids);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Move torrents to bottom in queue  (API: queue-move-bottom)
        /// </summary>
        /// <param name="ids"></param>
        public void TorrentQueueMoveBottom(int[] ids)
        {
            var request = new TransmissionRequest(RpcMethods.Queue.MoveBottom);
            request.AddArgument("ids", ids);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Set new location for torrents files (API: torrent-set-location)
        /// </summary>
        /// <param name="ids">Torrent ids</param>
        /// <param name="location">The new torrent location</param>
        /// <param name="move">Move from previous location</param>
        public void TorrentSetLocation(int[] ids, string location, bool move)
        {
            var request = new TransmissionRequest(RpcMethods.Torrent.SetLocation);
            request.AddArgument("ids", ids);
            request.AddArgument("location", location);
            request.AddArgument("move", move);

            var response = SendRequest(request);
        }

        /// <summary>
        /// Rename a file or directory in a torrent (API: torrent-rename-path)
        /// </summary>
        /// <param name="ids">The torrent whose path will be renamed</param>
        /// <param name="path">The path to the file or folder that will be renamed</param>
        /// <param name="name">The file or folder's new name</param>
        public RenameTorrentInfo TorrentRenamePath(int id, string path, string name)
        {
            var request = new TransmissionRequest(RpcMethods.Torrent.RenamePath);
            request.AddArgument("ids", new int[] { id });
            request.AddArgument("path", path);
            request.AddArgument("name", name);

            var response = SendRequest(request);

            var result = response.Deserialize<RenameTorrentInfo>();

            return result;
        }

#warning method name not recognized
        ///// <summary>
        ///// Reannounce torrent (API: torrent-reannounce)
        ///// </summary>
        ///// <param name="ids"></param>
        //public void ReannounceTorrents(int[] ids)
        //{
        //    var arguments = new Dictionary<string, object>();
        //    arguments.Add("ids", ids);

        //    var request = new TransmissionRequest("torrent-reannounce", arguments);
        //    var response = SendRequest(request);
        //}

        /// <summary>
        /// See if your incoming peer port is accessible from the outside world (API: port-test)
        /// </summary>
        /// <returns>Accessible state</returns>
        public bool PortTest()
        {
            var request = new TransmissionRequest(RpcMethods.PortTest);
            var response = SendRequest(request);

            var data = response.Deserialize<JObject>();
            var result = (bool)data.GetValue("port-is-open");
            return result;
        }

        /// <summary>
        /// Update blocklist (API: blocklist-update)
        /// </summary>
        /// <returns>Blocklist size</returns>
        public int BlocklistUpdate()
        {
            var request = new TransmissionRequest(RpcMethods.BlocklistUpdate);
            var response = SendRequest(request);

            var data = response.Deserialize<JObject>();
            var result = (int)data.GetValue("blocklist-size");
            return result;
        }

        /// <summary>
        /// Get free space is available in a client-specified folder.
        /// </summary>
        /// <param name="path">The directory to query</param>
        public long FreeSpace(string path)
        {
            var request = new TransmissionRequest(RpcMethods.FreeSpace);
            request.AddArgument("path", path);

            var response = SendRequest(request);

            var data = response.Deserialize<JObject>();
            var result = (long)data.GetValue("size-bytes");
            return result;
        }

        TransmissionResponse SendRequest(CommunicateBase request)
        {
            TransmissionResponse result = new TransmissionResponse();

            request.Tag = ++mCurrentTag;

            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(request.ToJson());

                //Prepare http web request
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Host);

                webRequest.ContentType = "application/json-rpc";
                webRequest.Headers["X-Transmission-Session-Id"] = mSessionId;
                webRequest.Method = "POST";

                if (mAuthorizationNeeded)
                    webRequest.Headers["Authorization"] = mAuthorization;

                var requestTask = webRequest.GetRequestStreamAsync();
                requestTask.WaitAndUnwrapException();
                using (Stream dataStream = requestTask.Result)
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                var responseTask = webRequest.GetResponseAsync();
                responseTask.WaitAndUnwrapException();

                //Send request and prepare response
                using (var webResponse = responseTask.Result)
                {
                    using (Stream responseStream = webResponse.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseString = reader.ReadToEnd();
                        result = JsonConvert.DeserializeObject<TransmissionResponse>(responseString);

                        if (result.Result != "success")
                            throw new Exception(result.Result);
                    }
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Conflict)
                {
                    if (ex.Response.Headers.Count > 0)
                    {
                        //If session id expiried, try get session id and send request
                        mSessionId = ex.Response.Headers["X-Transmission-Session-Id"];

                        if (SessionID == null)
                            throw new Exception("Session ID Error");

                        result = SendRequest(request);
                    }
                }
                else
                    throw ex;
            }

            return result;
        }

        static string GetBasicHttpAuthHeader(string login, string password)
        {
            string basicAuth = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(login + ":" + password));

            return $"Basic {basicAuth}";
        }

        static void AssertNewTorrentValid(NewTorrent torrent)
        {
            if (!string.IsNullOrWhiteSpace(torrent.Metainfo) || !string.IsNullOrWhiteSpace(torrent.Filename))
                return;

            throw new ArgumentException(
                "Either \"filename\" or \"metainfo\" must be included.");
        }

        string mAuthorization;
        bool mAuthorizationNeeded;

        string mHost;
        string mSessionId;
        int mCurrentTag;
    }
}