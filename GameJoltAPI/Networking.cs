using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Net;
using System.IO;

namespace GameJoltAPI
{
    /// <summary>
    /// Contains data about a call
    /// (If Connected & Successed & Return data & Sent parameters)
    /// </summary>
    public class DataArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the call was replied
        /// </summary>
        public bool Connected;
        /// <summary>
        /// Whether the call was successful or not
        /// </summary>
        public bool Success;
        /// <summary>
        /// The data returned from the call
        /// The type of the data depends on what call was made and whether it was successful or not
        /// TODO
        /// </summary>
        public object Data;
        /// <summary>
        /// The parameters passed when calling the API method (starting after the method to call when done)
        /// The parameters are in the same order they were entered
        /// TODO
        /// </summary>
        public object[] Parameters;

        // Constructor(s)
        public DataArgs()
        {
        }
        public DataArgs(bool connected)
        {
            Connected = connected;
        }
        public DataArgs(bool connected, bool success)
        {
            Connected = connected;
            Success = success;
        }
        public DataArgs(bool connected, bool success, object data)
        {
            Connected = connected;
            Success = success;
            Data = data;
        }
        public DataArgs(bool connected, bool success, object data, object[] parameters)
        {
            Connected = connected;
            Success = success;
            Data = data;
            Parameters = parameters;
        }

        /// <summary>
        /// Checks if the call was replied and successful
        /// </summary>
        /// <returns>Whether or not the call was replied and successful</returns>
        public bool IfSuccessful()
        {
            return (Connected && Success);
        }
    }

    /// <summary>
    /// Operations for data calls
    /// </summary>
    public enum DataOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Append,
        Prepend
    }

    /// <summary>
    /// All the API interaction will happen here (fetching and such)
    /// </summary>
    public static class Networking
    {
        /// <summary>
        /// Contains URLs and parameter format related strings
        /// </summary>
        internal static class URLContainer
        {
            internal static readonly string 
                                            // Root
                                            APIRoot = @"http://gamejolt.com/api/game/v1/",
                                            
                                            // Parameters
                                            GameID = @"&game_id=",
                                            UserID = @"&user_id=",
                                            Username = @"&username=",
                                            UserToken = @"&user_token=",
                                            Key = @"&key=",
                                            Operation = @"&operation=",
                                            Value = @"&value=",
                                            Data = @"&data=",
                                            Score = @"&score=",
                                            Sort = @"&sort=",
                                            Guest = @"&guest=",
                                            Signature = @"&signature=",
                                            ExtraData = @"&extra_data=",
                                            Limit = @"&limit=",
                                            TableID = @"&table_id=",
                                            TrophyID = @"&trophy_id=",
                                            Achieved = @"&achieved=",
                                            Status = @"&status=",
                                            Format = @"&format=",
                                            FormatDump = @"&format=dump",
                                            FormatJson = @"&format=json",
                                            FormatKeypair = @"&format=keypair",
                                            FormatXml = @"&format=xml",

                                            // User URLs
                                            FetchUser = APIRoot + @"users/?" + FormatJson,
                                            AuthUser = APIRoot + @"users/auth/?" + FormatJson,

                                            // Session URLs
                                            OpenSession = APIRoot + @"sessions/open/?" + FormatJson,
                                            PingSession = APIRoot + @"sessions/ping/?" + FormatJson,
                                            CloseSession = APIRoot + @"sessions/close/?" + FormatJson,

                                            // Trophy URLs
                                            FetchTrophy = APIRoot + @"trophies/?" + FormatJson,
                                            AchieveTrophy = APIRoot + @"sessions/ping/?" + FormatJson,
                                
                                            // Score URLs
                                            FetchScore = APIRoot + @"scores/?" + FormatJson,
                                            AddScore = APIRoot + @"scores/add/?" + FormatJson,
                                            FetchScoreTable = APIRoot + @"scores/tables/?" + FormatJson,

                                            // Data-Store URLs
                                            FetchData = APIRoot + @"data-store/?" + FormatDump,
                                            SetData = APIRoot + @"data-store/set/?" + FormatJson,
                                            UpdateData = APIRoot + @"data-store/update/?" + FormatDump,
                                            RemoveData = APIRoot + @"data-store/remove/?" + FormatJson,
                                            GetKeysData = APIRoot + @"hdata-store/get-keys/?" + FormatJson;
        }

        // Serializer settings
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
            {
                Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(HandleDeserializationError)
            };

        private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs) // Handles errors (exceptions) caused when deserializing
        {
#if DEBUG
            // Types error in console
            Console.WriteLine(string.Format("Error in \"{0}\"\n{1}",
                                            errorArgs.ErrorContext.Error.Source,
                                            errorArgs.ErrorContext.Error.Message));

            // Break if debug mode is active
            //if (System.Diagnostics.Debugger.IsAttached)
                //System.Diagnostics.Debugger.Break();
#endif
        }

        /// <summary>
        /// Generates a Signiture for a request url
        /// </summary>
        /// <param name="url">Request</param>
        /// <returns>The signiture</returns>
        public static string Signiture(string url, string key)
        {
            MD5 m = MD5.Create();
            byte[] data = m.ComputeHash(Encoding.UTF8.GetBytes(url + key)); // Slap a gamekey on their and encode hash

            StringBuilder sbuild = new StringBuilder();

            for (int i = 0; i < data.Length; ++i)
                sbuild.Append(data[i].ToString("x2"));

            return sbuild.ToString();
        }

        /// <summary>
        /// Request to get something from GameJolt
        /// </summary>
        /// <param name="url">URL with all parameters except for game specific information GameID and Signature</param>
        /// <param name="key">The games private</param>
        /// <param name="onComplete">Method that will "run" when the fetch is complete</param>
        private static void GetRequest(string url, string gid, string key, AsyncCallback onComplete)
        {
            // Completing the request url
            string rs = url + URLContainer.GameID + gid; // Append GameID
            rs += URLContainer.Signature + Signiture(rs, key); // Append Signiture

            //TODO: Web API Call
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rs);
            request.Method = "GET";
            request.BeginGetResponse(onComplete, request);
        }

        private static string ReadResult(IAsyncResult result) // Returns the data from a result 
        {
            // Gets the request from the result
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;

            // Safely handles the response (connection problems only)
            WebResponse response;
            try { response = request.EndGetResponse(result); }
            catch (Exception e)
            {
                // Return null if the call failed due to connection problems
                if (e.GetType() == typeof(WebException))
                    return null;

                // Throw exception
                throw e;
            }

            // Reads the data from the stream
            string data = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                data = sr.ReadToEnd();
                sr.Close();
            }

            // Return data
            return data;
        }
        private static T DeserializeResult<T>(string data) // Reads the result and returns a class containing the data (JSON) 
        {
            // Deserialize and return data
            return JsonConvert.DeserializeObject<T>(data, SerializerSettings);
        }
        private static bool DeserializeDataResult(IAsyncResult result, out string outData) // Reads the result and returns the raw data (dump format) 
        {
            // Gets the request from the result
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            string data = "";
            bool success;

            // Reads the data from the stream
            using (StreamReader sr = new StreamReader(request.EndGetResponse(result).GetResponseStream()))
            {
                // If the request was successful
                success = (sr.ReadLine()[0] == 'S');

                // Get data (alternatively the error message)
                if (!sr.EndOfStream) // Not sure if it will throw an exception if it was successful and there is no data
                    data = sr.ReadToEnd();

                sr.Close();
            }

            // Return data
            outData = data;
            return success;
        }

        private static void SuccessComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters) // TODO RENAME THIS FKIN TANG
        {
            if (result.IsCompleted)
            {
                // Don't even bother if onComplete is null
                if (onComplete == null)
                    return;

                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));

                    // Abort
                    return;
                }

                // Deserialize
                JsonObjects.SuccessResponse successResponse = DeserializeResult<JsonObjects.SuccessResult>(resultData).Response;

                // 
                bool success = successResponse.Success[0] == 't';

                // Call
                onComplete(null, new DataArgs(true, success, null, parameters));
            }
        }


        //  +-----------+---------+-----------+
        //  |-----------| USER(S) |-----------|
        //  +-----------+---------+-----------+

        private static void FetchUserComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters)
        {
            if (result.IsCompleted)
            {
                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));
                    
                    // Abort
                    return;
                }

                // Deserialize
                JsonObjects.UserResult userResult = DeserializeResult<JsonObjects.UserResult>(resultData);

                if (userResult.Response.Success[0] == 't')
                {
                    // Get the user data
                    JsonObjects.UserJson user = userResult.Response.Users[0];

                    // Copy (and format) the user data
                    UserData userData = new UserData(user);

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, true, userData, parameters));
                }
                else
                {
                    // Deserialize error message
                    JsonObjects.SuccessResponse success = DeserializeResult<JsonObjects.SuccessResult>(resultData).Response;

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, false, success.Message, parameters));
                }
            }
        }
        private static void FetchUsersComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters)
        {
            if (result.IsCompleted)
            {
                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));

                    // Abort
                    return;
                }

                // Deserialize
                JsonObjects.UserResult userResult = DeserializeResult<JsonObjects.UserResult>(resultData);

                if (userResult.Response.Success[0] == 't')
                {
                    // Copy and convert
                    UserData[] users = new UserData[userResult.Response.Users.Count];

                    int length = users.Length;
                    for (int i = 0; i < length; i++)
                    {
                        users[i] = new UserData(userResult.Response.Users[i]);
                    }

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, true, users, parameters));
                }
                else
                {
                    // Deserialize error message
                    JsonObjects.SuccessResponse success = DeserializeResult<JsonObjects.SuccessResult>(resultData).Response;

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, false, success.Message, parameters));
                }
            }
        }

        /// <summary>
        /// Fetches a user by their username (not to confuse with display-name)
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method to call when the requested data has been recieved</param>
        /// <param name="username">The user's username</param>
        public static void FetchUser(string gid, string key, EventHandler<DataArgs> onComplete, string username)
        {
            // Fetch
            GetRequest(URLContainer.FetchUser + URLContainer.Username + username, gid, key,
                (r) => { FetchUserComplete(r, onComplete, new object[] { username }); });
        }
        /// <summary>
        /// Fetches a user by their username (not to confuse with display-name)
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method to call when the requested data has been recieved</param>
        /// <param name="username">The user's username</param>
        public static void FetchUser(ClientData clientData, EventHandler<DataArgs> onComplete, string username)
        {
            // Fetch
            GetRequest(URLContainer.FetchUser + URLContainer.Username + username, clientData.GameID, clientData.GameKey,
                (r) => { FetchUserComplete(r, onComplete, new object[] { username }); });
        }

        /// <summary>
        /// Fetches a users by their usernames (NOT SURE IF THIS IS EVEN SUPPORTED [TODO])
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        public static void FetchUsers(string gid, string key, EventHandler<DataArgs> onComplete, params string[] username)
        {
            // URL with parameter
            StringBuilder url = new StringBuilder(URLContainer.FetchUser + URLContainer.Username);

            // Adds all the usernames to the url
            int length = username.Length;
            for (int i = 0; i < length; i++)
            {
                url.Append(username[i]);
                url.Append(',');
            }

            // Removes the last ',' (Because it's not supposed to be there)
            url.Remove(url.Length - 1, 1);

            // Fetch
            GetRequest(url.ToString(), gid, key,
                (r) =>
                { FetchUsersComplete(r, onComplete, username); }); // Not sure if I should copy or just pass "username" because of _params_
        }
        /// <summary>
        /// Fetches a users by their usernames (NOT SURE IF THIS IS EVEN SUPPORTED [TODO])
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        public static void FetchUsers(ClientData clientData, EventHandler<DataArgs> onComplete, params string[] username)
        {
            // URL with parameter
            StringBuilder url = new StringBuilder(URLContainer.FetchUser + URLContainer.Username);

            // Adds all the usernames to the url
            int length = username.Length;
            for (int i = 0; i < length; i++)
            {
                url.Append(username[i]);
                url.Append(',');
            }

            // Removes the last ',' (Because it's not supposed to be there)
            url.Remove(url.Length - 1, 1);

            // Fetch
            GetRequest(url.ToString(), clientData.GameID, clientData.GameKey,
                (r) =>
                { FetchUsersComplete(r, onComplete, username); }); // Not sure if I should copy or just pass "username" because of _params_
        }

        /// <summary>
        /// Fetches a user by their user id
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="userID">The ID of the user</param>
        public static void FetchUserByID(string gid, string key, EventHandler<DataArgs> onComplete, string userID)
        {
            // Fetch
            GetRequest(URLContainer.FetchUser + URLContainer.UserID + userID, gid, key,
                (r) => { FetchUserComplete(r, onComplete, new object[] { userID }); });
        }
        /// <summary>
        /// Fetches a user by their user id
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="userID">The ID of the user</param>
        public static void FetchUserByID(ClientData clientData, EventHandler<DataArgs> onComplete, string userID)
        {
            // Fetch
            GetRequest(URLContainer.FetchUser + URLContainer.UserID + userID, clientData.GameID, clientData.GameKey,
                (r) => { FetchUserComplete(r, onComplete, new object[] { userID }); });
        }

        /// <summary>
        /// Fetches a users by their user ids
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="userID">The ID of the user</param>
        public static void FetchUsersByID(string gid, string key, EventHandler<DataArgs> onComplete, params string[] userID)
        {
            // URL with parameter
            StringBuilder url = new StringBuilder(URLContainer.FetchUser + URLContainer.UserID);

            // Adds all the user id's to the url
            int length = userID.Length;
            for (int i = 0; i < length; i++)
            {
                url.Append(userID[i]);
                url.Append(',');
            }

            // Removes the last ',' (Because it's not supposed to be there)
            url.Remove(url.Length - 1, 1);

            // Fetch
            GetRequest(url.ToString(), gid, key,
                (r) => { FetchUsersComplete(r, onComplete, userID); });
        }
        /// <summary>
        /// Fetches a users by their user ids
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="userID">The ID of the user</param>
        public static void FetchUsersByID(ClientData clientData, EventHandler<DataArgs> onComplete, params string[] userID)
        {
            // URL with parameter
            StringBuilder url = new StringBuilder(URLContainer.FetchUser + URLContainer.UserID);

            // Adds all the user id's to the url
            int length = userID.Length;
            for (int i = 0; i < length; i++)
            {
                url.Append(userID[i]);
                url.Append(',');
            }

            // Removes the last ',' (Because it's not supposed to be there)
            url.Remove(url.Length - 1, 1);

            // Fetch
            GetRequest(url.ToString(), clientData.GameID, clientData.GameKey,
                (r) => { FetchUsersComplete(r, onComplete, userID); });
        }

        /// <summary>
        /// Authenticate a user by their username and token
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void AuthenticateUser(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Authenticate
            GetRequest(URLContainer.AuthUser + URLContainer.Username + username + URLContainer.UserToken + token, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }
        /// <summary>
        /// Authenticate a user by their username and token
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void AuthenticateUser(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Authenticate
            GetRequest(URLContainer.AuthUser + URLContainer.Username + username + URLContainer.UserToken + token, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }


        //  +----------+------------+----------+
        //  |----------| SESSION(S) |----------|
        //  +----------+------------+----------+

        /// <summary>
        /// Opens a user specific session
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void OpenSession(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Open Session
            GetRequest(URLContainer.OpenSession + URLContainer.Username + username + URLContainer.UserToken + token, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }
        /// <summary>
        /// Opens a user specific session
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void OpenSession(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Open Session
            GetRequest(URLContainer.OpenSession + URLContainer.Username + username + URLContainer.UserToken + token, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }

        /// <summary>
        /// Pings a open session
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void PingSession(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Ping Session
            GetRequest(URLContainer.PingSession + URLContainer.Username + username + URLContainer.UserToken + token, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }
        /// <summary>
        /// Pings a open session
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void PingSession(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Ping Session
            GetRequest(URLContainer.PingSession + URLContainer.Username + username + URLContainer.UserToken + token, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }

        /// <summary>
        /// Pings a open session and updates the status
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="status">The user's status, either "active" or "idle"</param>
        public static void PingSession(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string status)
        {
            // Ping Session and update the status
            GetRequest(URLContainer.PingSession + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Status + status, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }
        /// <summary>
        /// Pings a open session and updates the status
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="status">The user's status, either "active" or "idle"</param>
        public static void PingSession(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string status)
        {
            // Ping Session and update the status
            GetRequest(URLContainer.PingSession + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Status + status, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }

        /// <summary>
        /// Closes a open session
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void CloseSession(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Close Session
            GetRequest(URLContainer.CloseSession + URLContainer.Username + username + URLContainer.UserToken + token, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }
        /// <summary>
        /// Closes a open session
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void CloseSession(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Close Session
            GetRequest(URLContainer.CloseSession + URLContainer.Username + username + URLContainer.UserToken + token, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token }); });
        }


        //  +---------+--------------+---------+
        //  |---------| TROPHY(-IES) |---------|
        //  +---------+--------------+---------+

        private static void FetchTrophiesComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters)
        {
            if (result.IsCompleted)
            {
                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));

                    // Abort
                    return;
                }

                // Deserialize
                JsonObjects.TrophyResponse trophyResponse = DeserializeResult<JsonObjects.TrophyResult>(resultData).Response;

                if (trophyResponse.Success[0] == 't')
                {
                    // Copy and convert
                    Trophy[] newTrophies = new Trophy[trophyResponse.Trophies.Count];

                    int length = newTrophies.Length;
                    for (int i = 0; i < length; i++)
                    {
                        newTrophies[i] = new Trophy(trophyResponse.Trophies[i]);
                    }

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, true, newTrophies, parameters));
                }
                else
                {
                    // Deserialize
                    JsonObjects.SuccessResponse successResponse = DeserializeResult<JsonObjects.SuccessResult>(resultData).Response;

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, false, successResponse.Message, parameters));
                }
            }
        }
        private static void FetchTrophyComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters)
        {
            if (result.IsCompleted)
            {
                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));

                    // Abort
                    return;
                }

                // Deserialize
                JsonObjects.TrophyResponse trophyResponse = DeserializeResult<JsonObjects.TrophyResult>(resultData).Response;

                if (trophyResponse.Success[0] == 't')
                {
                    // Get the user data
                    JsonObjects.TrophyJson json = trophyResponse.Trophies[0];

                    // Copy (and format) the user data
                    Trophy trophy = new Trophy(json);

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, true, trophy, parameters));
                }
                else
                {
                    // Deserialize
                    JsonObjects.SuccessResponse successResponse = DeserializeResult<JsonObjects.SuccessResult>(resultData).Response;

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, false, successResponse.Message, parameters));
                }
            }
        }

        /// <summary>
        /// Fetch all trophies from a game
        /// (The user is irrelevant as long as it's authentic)
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void FetchAllTrophies(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Fetch all Trophies
            GetRequest(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token, gid, key,
                (r) => { FetchTrophiesComplete(r, onComplete, new object[] { username, token }); });
        }
        /// <summary>
        /// Fetch all trophies from a game
        /// (The user is irrelevant as long as it's authentic)
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        public static void FetchAllTrophies(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token)
        {
            // Fetch all Trophies
            GetRequest(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token, clientData.GameID, clientData.GameKey,
                (r) => { FetchTrophiesComplete(r, onComplete, new object[] { username, token }); });
        }

        /// <summary>
        /// Fetch all trophies from a game that the user have either achieved or not achieved
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="achieved">If you want to fetch either the "achieved" or "not achieved" trophies (achieved = "true", not achieved = "false")</param>
        public static void FetchAllAchievedTrophies(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string achieved)
        {
            // Fetch all "achieved" / "not achieved" Trophies
            GetRequest(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Achieved + achieved, gid, key,
                (r) => { FetchTrophiesComplete(r, onComplete, new object[] { username, token, achieved }); });
        }
        /// <summary>
        /// Fetch all trophies from a game that the user have either achieved or not achieved
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="achieved">If you want to fetch either the "achieved" or "not achieved" trophies (achieved = "true", not achieved = "false")</param>
        public static void FetchAllAchievedTrophies(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string achieved)
        {
            // Fetch all "achieved" / "not achieved" Trophies
            GetRequest(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Achieved + achieved, clientData.GameID, clientData.GameKey,
                (r) => { FetchTrophiesComplete(r, onComplete, new object[] { username, token, achieved }); });
        }

        /// <summary>
        /// Fetch all trophies from a game
        /// (The user is irrelevant as long as it's authentic)
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="id">The trophy's id</param>
        public static void FetchTrophy(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string id)
        {
            // Fetch trophy
            GetRequest(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TrophyID + id, gid, key,
                (r) => { FetchTrophyComplete(r, onComplete, new object[] { username, token, id }); });
        }
        /// <summary>
        /// Fetch all trophies from a game
        /// (The user is irrelevant as long as it's authentic)
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="id">The trophy's id</param>
        public static void FetchTrophy(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string id)
        {
            // Fetch trophy
            GetRequest(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TrophyID + id, clientData.GameID, clientData.GameKey,
                (r) => { FetchTrophyComplete(r, onComplete, new object[] { username, token, id }); });
        }

        /// <summary>
        /// Fetch all trophies from a game
        /// (The user is irrelevant as long as it's authentic)
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="id">The trophies ids</param>
        public static void FetchTrophies(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, params string[] id)
        {
            // URL with parameter
            StringBuilder url = new StringBuilder(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TrophyID);

            // Adds all the trophy-ids to the url
            int length = id.Length;
            for (int i = 0; i < length; i++)
            {
                url.Append(id[i]);
                url.Append(',');
            }

            // Removes the last ',' (Because it's not supposed to be there)
            url.Remove(url.Length - 2, 1);

            // Fetch all "achieved" / "not achieved" Trophies
            GetRequest(url.ToString(), gid, key,
                (r) => { FetchTrophiesComplete(r, onComplete, new object[] { username, token, id }); });
        }
        /// <summary>
        /// Fetch all trophies from a game
        /// (The user is irrelevant as long as it's authentic)
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="id">The trophies ids</param>
        public static void FetchTrophies(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, params string[] id)
        {
            // URL with parameter
            StringBuilder url = new StringBuilder(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TrophyID);

            // Adds all the trophy-ids to the url
            int length = id.Length;
            for (int i = 0; i < length; i++)
            {
                url.Append(id[i]);
                url.Append(',');
            }

            // Removes the last ',' (Because it's not supposed to be there)
            url.Remove(url.Length - 2, 1);

            // Fetch all "achieved" / "not achieved" Trophies
            GetRequest(url.ToString(), clientData.GameID, clientData.GameKey,
                (r) => { FetchTrophiesComplete(r, onComplete, new object[] { username, token, id }); });
        }

        /// <summary>
        /// Fetch all trophies from a game that the user have either achieved or not achieved
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="achieved">If you want to fetch either a "achieved" or "not achieved" trophy (achieved = "true", not achieved = "false")</param>
        /// <param name="id">The trophies ids</param>
        public static void FetchAchievedTrophies(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string achieved, params string[] id)
        {
            // URL with parameter
            StringBuilder url = new StringBuilder(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Achieved + achieved + URLContainer.TrophyID);

            // Adds all the trophy-ids to the url
            int length = id.Length;
            for (int i = 0; i < length; i++)
            {
                url.Append(id[i]);
                url.Append(',');
            }

            // Removes the last ',' (Because it's not supposed to be there)
            url.Remove(url.Length - 2, 1);

            // Fetch all "achieved" / "not achieved" Trophies
            GetRequest(url.ToString(), gid, key,
                (r) => { FetchTrophiesComplete(r, onComplete, new object[] { username, token, achieved, id }); });
        }
        /// <summary>
        /// Fetch all trophies from a game that the user have either achieved or not achieved
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="achieved">If you want to fetch either a "achieved" or "not achieved" trophy (achieved = "true", not achieved = "false")</param>
        /// <param name="id">The trophies ids</param>
        public static void FetchAchievedTrophies(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string achieved, params string[] id)
        {
            // URL with parameter
            StringBuilder url = new StringBuilder(URLContainer.FetchTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Achieved + achieved + URLContainer.TrophyID);

            // Adds all the trophy-ids to the url
            int length = id.Length;
            for (int i = 0; i < length; i++)
            {
                url.Append(id[i]);
                url.Append(',');
            }

            // Removes the last ',' (Because it's not supposed to be there)
            url.Remove(url.Length - 2, 1);

            // Fetch all "achieved" / "not achieved" Trophies
            GetRequest(url.ToString(), clientData.GameID, clientData.GameKey,
                (r) => { FetchTrophiesComplete(r, onComplete, new object[] { username, token, achieved, id }); });
        }

        /// <summary>
        /// Achieves a trophy for the user
        /// </summary>
        /// <param name="gid">The games id</param>
        /// <param name="key">The games private key</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="achieved">If you want to fetch either a "achieved" or "not achieved" trophy (achieved = "true", not achieved = "false")</param>
        /// <param name="id">The trophy's id</param>
        public static void AchieveTrophy(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string id)
        {
            // Fetch trophy
            GetRequest(URLContainer.AchieveTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TrophyID + id, gid, key,
                       (r) => { SuccessComplete(r, onComplete, new object[] { username, token, id }); });
        }
        /// <summary>
        /// Achieves a trophy for the user
        /// </summary>
        /// <param name="clientData">The games client data (id and key)</param>
        /// <param name="onComplete">Method that will "run" when the request is complete</param>
        /// <param name="username">The user's username</param>
        /// <param name="token">The user's token</param>
        /// <param name="achieved">If you want to fetch either a "achieved" or "not achieved" trophy (achieved = "true", not achieved = "false")</param>
        /// <param name="id">The trophy's id</param>
        public static void AchieveTrophy(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string id)
        {
            // Fetch trophy
            GetRequest(URLContainer.AchieveTrophy + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TrophyID + id, clientData.GameID, clientData.GameKey,
                       (r) => { SuccessComplete(r, onComplete, new object[] { username, token, id }); });
        }


        //  +-----------+----------+-----------+
        //  |-----------| SCORE(S) |-----------|
        //  +-----------+----------+-----------+

        private static void FetchScoresComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters)
        {
            if (result.IsCompleted)
            {
                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));

                    // Abort
                    return;
                }

                // Deserialize
                JsonObjects.ScoreResponse scoreResponse = DeserializeResult<JsonObjects.ScoreResult>(resultData).Response;

                if (scoreResponse.Success[0] == 't')
                {
                    // Copy and convert
                    ScoreEntry[] scores = new ScoreEntry[scoreResponse.Scores.Count];

                    int length = scores.Length;
                    for (int i = 0; i < length; i++)
                    {
                        scores[i] = new ScoreEntry(scoreResponse.Scores[i]);
                    }

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, true, scores, parameters));
                }
                else
                {
                    // Deserialize
                    JsonObjects.SuccessResponse successResponse = DeserializeResult<JsonObjects.SuccessResult>(resultData).Response;

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, false, successResponse.Message, parameters));
                }
            }
        }
        private static void FetchScoreTablesComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters)
        {
            if (result.IsCompleted)
            {
                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));

                    // Abort
                    return;
                }

                // Deserialize
                JsonObjects.ScoreTableResponse tableResponse = DeserializeResult<JsonObjects.ScoreTableResult>(resultData).Response;

                if (tableResponse.Success[0] == 't')
                {
                    // Copy and convert
                    ScoreTable[] tables = new ScoreTable[tableResponse.Tables.Count];

                    int length = tables.Length;
                    for (int i = 0; i < length; i++)
                    {
                        tables[i] = new ScoreTable(tableResponse.Tables[i]);
                    }

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, true, tables, parameters));
                }
                else
                {
                    // Deserialize
                    JsonObjects.SuccessResponse successResponse = DeserializeResult<JsonObjects.SuccessResult>(resultData).Response;

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, false, successResponse.Message, parameters));
                }
            }
        }

        public static void FetchScores(string gid, string key, EventHandler<DataArgs> onComplete)
        {
            GetRequest(URLContainer.FetchScore, gid, key,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { }); });
        }
        public static void FetchScores(ClientData clientData, EventHandler<DataArgs> onComplete)
        {
            GetRequest(URLContainer.FetchScore, clientData.GameID, clientData.GameKey,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { }); });
        }
        public static void FetchScores(string gid, string key, EventHandler<DataArgs> onComplete, string table)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.TableID + table, gid, key,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { table }); });
        }
        public static void FetchScores(ClientData clientData, EventHandler<DataArgs> onComplete, string table)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.TableID + table, clientData.GameID, clientData.GameKey,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { table }); });
        }
        public static void FetchScores(string gid, string key, EventHandler<DataArgs> onComplete, string table, string limit)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.TableID + table + URLContainer.Limit + limit, gid, key,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { table, limit }); });
        }
        public static void FetchScores(ClientData clientData, EventHandler<DataArgs> onComplete, string table, string limit)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.TableID + table + URLContainer.Limit + limit, clientData.GameID, clientData.GameKey,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { table, limit }); });
        }

        public static void FetchUserScores(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TableID, gid, key,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { username, token }); });
        }
        public static void FetchUserScores(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TableID, clientData.GameID, clientData.GameKey,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { username, token }); });
        }
        public static void FetchUserScores(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string table)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TableID + table, gid, key,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { username, token, table }); });
        }
        public static void FetchUserScores(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string table)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TableID + table, clientData.GameID, clientData.GameKey,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { username, token, table }); });
        }
        public static void FetchUserScores(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string table, string limit)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TableID + table + URLContainer.Limit + limit, gid, key,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { username, token, table, limit }); });
        }
        public static void FetchUserScores(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string table, string limit)
        {
            GetRequest(URLContainer.FetchScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.TableID + table + URLContainer.Limit + limit, clientData.GameID, clientData.GameKey,
                (r) => { FetchScoresComplete(r, onComplete, new object[] { username, token, table, limit }); });
        }

        public static void AddScore(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string score, string sort)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Score + score + URLContainer.Sort + sort, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, score, sort }); });
        }
        public static void AddScore(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string score, string sort)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Score + score + URLContainer.Sort + sort, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, score, sort }); });
        }
        public static void AddScore(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string score, string sort, string extra)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Score + score + URLContainer.Sort + sort + URLContainer.ExtraData + extra, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, score, sort, extra }); });
        }
        public static void AddScore(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string score, string sort, string extra)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Score + score + URLContainer.Sort + sort + URLContainer.ExtraData + extra, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, score, sort, extra }); });
        }
        public static void AddScore(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string score, string sort, string extra, string table)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Score + score + URLContainer.Sort + sort + URLContainer.ExtraData + extra + URLContainer.TableID + table, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, score, sort, extra, table }); });
        }
        public static void AddScore(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string score, string sort, string extra, string table)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Score + score + URLContainer.Sort + sort + URLContainer.ExtraData + extra + URLContainer.TableID + table, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, score, sort, extra, table }); });
        }

        public static void AddScoreAsGuest(string gid, string key, EventHandler<DataArgs> onComplete, string name, string score, string sort)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Sort + sort, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { name, score, sort }); });
        }
        public static void AddScoreAsGuest(ClientData clientData, EventHandler<DataArgs> onComplete, string name, string score, string sort)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Sort + sort, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { name, score, sort }); });
        }
        public static void AddScoreAsGuest(string gid, string key, EventHandler<DataArgs> onComplete, string name, string score, string sort, string extra)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Sort + sort + URLContainer.ExtraData + extra, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { name, score, sort, extra }); });
        }
        public static void AddScoreAsGuest(ClientData clientData, EventHandler<DataArgs> onComplete, string name, string score, string sort, string extra)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Sort + sort + URLContainer.ExtraData + extra, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { name, score, sort, extra }); });
        }
        public static void AddScoreAsGuest(string gid, string key, EventHandler<DataArgs> onComplete, string name, string score, string sort, string extra, string table)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Sort + sort + URLContainer.ExtraData + extra + URLContainer.TableID + table, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { name, score, sort, extra, table }); });
        }
        public static void AddScoreAsGuest(ClientData clientData, EventHandler<DataArgs> onComplete, string name, string score, string sort, string extra, string table)
        {
            GetRequest(URLContainer.AddScore + URLContainer.Sort + sort + URLContainer.ExtraData + extra + URLContainer.TableID + table, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { name, score, sort, extra, table }); });
        }

        public static void FetchScoreTables(string gid, string key, EventHandler<DataArgs> onComplete)
        {
            GetRequest(URLContainer.FetchScoreTable, gid, key,
                (r) => { FetchScoreTablesComplete(r, onComplete, new object[] { }); });
        }
        public static void FetchScoreTables(ClientData clientData, EventHandler<DataArgs> onComplete)
        {
            GetRequest(URLContainer.FetchScoreTable, clientData.GameID, clientData.GameKey,
                (r) => { FetchScoreTablesComplete(r, onComplete, new object[] { }); });
        }


        //  +----------+------------+----------+
        //  |----------| DATA STORE |----------|
        //  +----------+------------+----------+

        // Fetch (RawData)
        private static void FetchDataComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters) // TODO RENAME THIS FKIN TANG
        {
            if (result.IsCompleted)
            {
                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));

                    // Abort
                    return;
                }

                // Deserialize
                string data;
                bool success = DeserializeDataResult(result, out data);

                // Call
                if (onComplete != null)
                    onComplete(null, new DataArgs(true, success, data, parameters));
            }
        }
        private static void FetchDataKeysComplete(IAsyncResult result, EventHandler<DataArgs> onComplete, object[] parameters)
        {
            if (result.IsCompleted)
            {
                // Read result
                string resultData = ReadResult(result);
                if (resultData == null) // If there is no result
                {
                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(false, false, null, parameters));

                    // Abort
                    return;
                }

                // Deserialize
                JsonObjects.DataKeysResponse keysResponse = DeserializeResult<JsonObjects.DataKeysResult>(resultData).Response;

                if (keysResponse.Success[0] == 't')
                {
                    // Copy and convert
                    string[] keys = new string[keysResponse.Keys.Count];

                    int length = keys.Length;
                    for (int i = 0; i < length; i++)
                    {
                        keys[i] = keysResponse.Keys[i].Key;
                    }

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, true, keys, parameters));
                }
                else
                {
                    // Deserialize
                    JsonObjects.SuccessResponse successResponse = DeserializeResult<JsonObjects.SuccessResult>(resultData).Response;

                    // Call
                    if (onComplete != null)
                        onComplete(null, new DataArgs(true, false, successResponse.Message, parameters));
                }
            }
        }

        public static void FetchData(string gid, string key, EventHandler<DataArgs> onComplete, string dataKey)
        {
            GetRequest(URLContainer.FetchData + URLContainer.Key + dataKey, gid, key,
                (r) => { FetchDataComplete(r, onComplete, new object[] { dataKey }); });
        }
        public static void FetchData(ClientData clientData, EventHandler<DataArgs> onComplete, string dataKey)
        {
            GetRequest(URLContainer.FetchData + URLContainer.Key + dataKey, clientData.GameID, clientData.GameKey,
                (r) => { FetchDataComplete(r, onComplete, new object[] { dataKey }); });
        }

        public static void FetchUserData(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string dataKey)
        {
            GetRequest(URLContainer.FetchData + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Key + dataKey, gid, key,
                (r) => { FetchDataComplete(r, onComplete, new object[] { username, token, dataKey }); });
        }
        public static void FetchUserData(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string dataKey)
        {
            GetRequest(URLContainer.FetchData + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Key + dataKey, clientData.GameID, clientData.GameKey,
                (r) => { FetchDataComplete(r, onComplete, new object[] { username, token, dataKey }); });
        }

        // Set data (Suc or Fail)
        public static void SetData(string gid, string key, EventHandler<DataArgs> onComplete, string dataKey, string value)
        {
            GetRequest(URLContainer.SetData + URLContainer.Key + dataKey + URLContainer.Data + value, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { dataKey, value }); });
        }
        public static void SetData(ClientData clientData, EventHandler<DataArgs> onComplete, string dataKey, string value)
        {
            GetRequest(URLContainer.SetData + URLContainer.Key + dataKey + URLContainer.Data + value, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { dataKey, value }); });
        }

        public static void SetUserData(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string dataKey, string value)
        {
            GetRequest(URLContainer.SetData + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Key + dataKey + URLContainer.Data + value, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, dataKey, value }); });
        }
        public static void SetUserData(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string dataKey, string value)
        {
            GetRequest(URLContainer.SetData + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Key + dataKey + URLContainer.Data + value, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, dataKey, value }); });
        }

        // Update data (RawData)
        public static void UpdateData(string gid, string key, EventHandler<DataArgs> onComplete, string dataKey, DataOperation operation, string value)
        {
            GetRequest(URLContainer.UpdateData + URLContainer.Key + dataKey + URLContainer.Operation + operation.ToString().ToLower() + URLContainer.Value + value, gid, key,
                (r) => { FetchDataComplete(r, onComplete, new object[] { dataKey, operation, value }); });
        }
        public static void UpdateData(ClientData clientData, EventHandler<DataArgs> onComplete, string dataKey, DataOperation operation, string value)
        {
            GetRequest(URLContainer.UpdateData + URLContainer.Key + dataKey + URLContainer.Operation + operation.ToString().ToLower() + URLContainer.Value + value, clientData.GameID, clientData.GameKey,
                (r) => { FetchDataComplete(r, onComplete, new object[] { dataKey, operation, value }); });
        }

        public static void UpdateUserData(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string dataKey, DataOperation operation, string value)
        {
            GetRequest(URLContainer.UpdateData + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Key + dataKey + URLContainer.Operation + operation.ToString().ToLower() + URLContainer.Value + value, gid, key,
                (r) => { FetchDataComplete(r, onComplete, new object[] { username, token, dataKey, operation, value }); });
        }
        public static void UpdateUserData(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string dataKey, DataOperation operation, string value)
        {
            GetRequest(URLContainer.UpdateData + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Key + dataKey + URLContainer.Operation + operation.ToString().ToLower() + URLContainer.Value + value, clientData.GameID, clientData.GameKey,
                (r) => { FetchDataComplete(r, onComplete, new object[] { username, token, dataKey, operation, value }); });
        }

        // Remove data (Suc or Fail)
        public static void RemoveData(string gid, string key, EventHandler<DataArgs> onComplete, string dataKey)
        {
            GetRequest(URLContainer.RemoveData + URLContainer.Key + dataKey, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { dataKey }); });
        }
        public static void RemoveData(ClientData clientData, EventHandler<DataArgs> onComplete, string dataKey)
        {
            GetRequest(URLContainer.RemoveData + URLContainer.Key + dataKey, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { dataKey }); });
        }

        public static void RemoveUserData(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token, string dataKey)
        {
            GetRequest(URLContainer.RemoveData + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Key + dataKey, gid, key,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, dataKey }); });
        }
        public static void RemoveUserData(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token, string dataKey)
        {
            GetRequest(URLContainer.RemoveData + URLContainer.Username + username + URLContainer.UserToken + token + URLContainer.Key + dataKey, clientData.GameID, clientData.GameKey,
                (r) => { SuccessComplete(r, onComplete, new object[] { username, token, dataKey }); });
        }

        public static void FetchDataKeys(string gid, string key, EventHandler<DataArgs> onComplete)
        {
            GetRequest(URLContainer.GetKeysData, gid, key,
                (r) => { FetchDataKeysComplete(r, onComplete, new object[] { }); });
        }
        public static void FetchDataKeys(ClientData clientData, EventHandler<DataArgs> onComplete)
        {
            GetRequest(URLContainer.GetKeysData, clientData.GameID, clientData.GameKey,
                (r) => { FetchDataKeysComplete(r, onComplete, new object[] { }); });
        }
        public static void FetchUserDataKeys(string gid, string key, EventHandler<DataArgs> onComplete, string username, string token)
        {
            GetRequest(URLContainer.GetKeysData + URLContainer.Username + username + URLContainer.UserToken + token, gid, key,
                (r) => { FetchDataKeysComplete(r, onComplete, new object[] { username, token }); });
        }
        public static void FetchUserDataKeys(ClientData clientData, EventHandler<DataArgs> onComplete, string username, string token)
        {
            GetRequest(URLContainer.GetKeysData + URLContainer.Username + username + URLContainer.UserToken + token, clientData.GameID, clientData.GameKey,
                (r) => { FetchDataKeysComplete(r, onComplete, new object[] { username, token }); });
        }
    }
}
