using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameJoltAPI
{
    public class AuthentionCompletedArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the call was replied
        /// </summary>
        public bool Connected;
        /// <summary>
        /// Whether or not the authentication succeeded
        /// </summary>
        public bool Success;

        // Constructor(s)
        public AuthentionCompletedArgs()
        {
        }
        public AuthentionCompletedArgs(bool connected)
        {
            Connected = connected;
        }
        public AuthentionCompletedArgs(bool connected, bool success)
        {
            Connected = connected;
            Success = success;
        }

        /// <summary>
        /// Checks if the call was replied and successful
        /// </summary>
        /// <returns>Whether or not the call was replied and successful</returns>
        public bool CallSuccessful()
        {
            return (Connected && Success);
        }
    }
    public class LoginCompletedArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the call was replied
        /// </summary>
        public bool Connected;
        /// <summary>
        /// Whether or not the authentication succeeded
        /// </summary>
        public bool Success;

        // Constructor(s)
        public LoginCompletedArgs()
        {
        }
        public LoginCompletedArgs(bool connected)
        {
            Connected = connected;
        }
        public LoginCompletedArgs(bool connected, bool success)
        {
            Connected = connected;
            Success = success;
        }

        /// <summary>
        /// Checks if the call was replied and successful
        /// </summary>
        /// <returns>Whether or not the call was replied and successful</returns>
        public bool CallSuccessful()
        {
            return (Connected && Success);
        }
    }

    /// <summary>
    /// Connects to a game using the GameJolt API
    /// (One instance per game)
    /// </summary>
    public class GameClient
    {
        // Private
        private ClientData _data;
        private bool _authenticated;

        private Dictionary<string, User> _openSessions;

        // Public
        public ClientData Data
        { get { return _data; } }
        public bool Authenticated
        { get { return _authenticated; } }

        // Events
        public event EventHandler<AuthentionCompletedArgs> AuthenticationCompleted;
        public event EventHandler<AuthentionCompletedArgs> SessionOpened;
        public event EventHandler<AuthentionCompletedArgs> SessionPinged;
        public event EventHandler<AuthentionCompletedArgs> SessionClosed;

        // Constructor(s)
        public GameClient(ClientData data)
        {
            _data = data;

            _openSessions = new Dictionary<string, User>();
        }
        public GameClient(string gameid, string key)
            : this(new ClientData(gameid, key))
        {
        }

        /// <summary>
        /// Check if the GameID & GameKey combination is valid
        /// </summary>
        public void Authenticate()
        {
            EventHandler<DataArgs> func = (s, d) =>
                {
                    // If the authentication (user fetch) was successfull (should be if the GameID / GameKey combination is valid)
                    _authenticated = d.Success;

                    // Call authentication 
                    AuthenticationCompleted(this, new AuthentionCompletedArgs(d.Connected, d.Success));
                };

            // Fetch (a user because it only requieres a valid GameID & GameKey combinaton)
            Networking.FetchUser(_data, func, "cros"); // (Fetch cros because his account will probably stay longer than GameJolt itself)
        }

        //
        public void OpenSession(string user, string token)
        {
            EventHandler<DataArgs> func = (s, d) =>
                {
                    // If the session was opened successfully
                    _authenticated = d.Success;

                    // Call authentication
                    AuthenticationCompleted(this, new AuthentionCompletedArgs(true));
                };

            // 
            Networking.OpenSession(_data, func, user, token);
        }

        /* Todo:
         * + Add list for connected users (and methods for fetching and pinging)
         * + Add list with trophies from game (and fetching methods)
         * + Add list with highscore tables (and methods for fetching, refreshing and uploading scores)
         * + Add methods for fetching and setting data (in the data store)
        */
    }
}
