using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameJoltAPI
{
    /// <summary>
    /// Contains data for connecting to a game through the GameJolt API
    /// </summary>
    public class ClientData
    {
        /// <summary>
        /// The id of the game (found in the URL to the games GameJolt page)
        /// </summary>
        public string GameID;

        /// <summary>
        /// The private key of the game (found under "Manage Achievements" on the game dashboard)
        /// </summary>
        public string GameKey;

        /// <summary>
        /// 
        /// </summary>
        public ClientData()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameid">The id of the game (found in the URL to the games GameJolt page)</param>
        /// <param name="key">The private key of the game (found under "Manage Achievements" on the game dashboard)</param>
        public ClientData(string gameid, string key)
        {
            GameID = gameid;
            GameKey = key;
        }
    }
}
