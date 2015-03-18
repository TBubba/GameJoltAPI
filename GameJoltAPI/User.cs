using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameJoltAPI
{
    /// <summary>
    /// Contains most data assigned to a GameJolt user and has the ability to fetch/send data using the API
    /// (does not contain highscore data)
    /// </summary>
    public class User : UserData
    {
        // Private
        private List<string> _trophies;
        private UserSessionStatus _session;

        // Public
        public ClientData Client;
        public string Token;

        public UserSessionStatus Session
        { get { return _session; } }

        // Constructor(s)
        public User(ClientData clientData)
        {
            Client = clientData;
        }
        public User(ClientData clientData, UserData userData)
        {
            Client = clientData;
            userData.CopyTo(this);
        }

        /* Todo:
         * + Add Fetching methods for trophies
         * + Add Session opening, pinging and closing methods
        */
    }
}
