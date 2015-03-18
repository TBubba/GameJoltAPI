using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameJoltAPI.JsonObjects;

namespace GameJoltAPI
{
    /// <summary>
    /// Contains data assigned to a score table
    /// (not the scores themself, though they can be fetched)
    /// </summary>
    public class ScoreTable
    {
        // Data
        /// <summary>
        /// The high score table identifier
        /// </summary>
        public string ID = "";
        /// <summary>
        /// The developer-defined high score table name
        /// </summary>
        public string Name = "";
        /// <summary>
        /// The developer-defined high score table description
        /// </summary>
        public string Description = "";
        /// <summary>
        /// Whether or not this is the default high score table
        /// High scores are submitted to the primary table by default
        /// </summary>
        public bool Primary;

        // Public
        public ClientData Client;

        // Constructor(s)
        public ScoreTable()
        {
        }
        public ScoreTable(ClientData clientData)
        {
            Client = clientData;
        }
        public ScoreTable(ScoreTableJson table)
        {
            // Copy data
            ID = table.ID;
            Name = table.Name;
            Description = table.Description;

            //
            Primary = (table.Primary[0] == 't');
        }
        public ScoreTable(ScoreTableJson table, ClientData clientData)
            : this(table)
        {
            Client = clientData;
        }

        // Fetch
        public void AsyncFetchScores(EventHandler<DataArgs> onComplete, string table)
        {
            Networking.FetchScores(Client.GameID, Client.GameKey, onComplete, table);
        }
        public void AsyncFetchScores(EventHandler<DataArgs> onComplete, string table, string limit)
        {
            Networking.FetchScores(Client.GameID, Client.GameKey, onComplete, table, limit);
        }
    }
}
