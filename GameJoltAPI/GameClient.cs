using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameJoltAPI
{
    /// <summary>
    /// Connects to a game using the GameJolt API
    /// (One instance per game)
    /// </summary>
    public class GameClient
    {
        // Private
        private ClientData _data;

        // Public
        public ClientData Data
        { get { return _data; } }

        // Constructor(s)
        public GameClient(ClientData data)
        {
            _data = data;
        }
        public GameClient(string gameid, string key)
        {
            _data = new ClientData(gameid, key);
        }

        /* Todo:
         * + Add list for connected users (and methods for fetching and pinging)
         * + Add list with trophies from game (and fetching methods)
         * + Add list with highscore tables (and methods for fetching, refreshing and uploading scores)
         * + Add methods for fetching and setting data (in the data store)
        */
    }
}
