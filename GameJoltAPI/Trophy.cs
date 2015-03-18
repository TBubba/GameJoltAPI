using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameJoltAPI.JsonObjects;

namespace GameJoltAPI
{
    /// <summary>
    /// The difficulty to obtain trophy
    /// ("Bronze", "Silver", "Gold" or "Platinum")
    /// </summary>
    public enum TrophyDifficulty
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    /// <summary>
    /// Contains most data assigned to a GameJolt trophy
    /// (except for who has achieved it)
    /// </summary>
    public class Trophy
    {
        // Data
        /// <summary>
        /// The ID of the trophy
        /// </summary>
        public string ID = "";
        /// <summary>
        /// The title of the trophy on the site
        /// </summary>
        public string Title = "";
        /// <summary>
        /// The trophy description text
        /// </summary>
        public string Description = "";
        /// <summary>
        /// The difficulty to obtain trophy
        /// </summary>
        public TrophyDifficulty Difficulty;
        /// <summary>
        /// The URL to the trophy's thumbnail
        /// </summary>
        public string ImageURL = "";

        // Constructor(s)
        public Trophy()
        {
        }
        public Trophy(TrophyJson trophy)
        {
            ID = trophy.ID;
            Title = trophy.Title;
            Description = trophy.Description;
            ImageURL = trophy.ImageURL;

            switch (trophy.Difficulty[0])
            {
                default: // Bronze
                    Difficulty = TrophyDifficulty.Bronze;
                    break;
                case 's': // Silver
                    Difficulty = TrophyDifficulty.Silver;
                    break;
                case 'g': // Gold
                    Difficulty = TrophyDifficulty.Gold;
                    break;
                case 'p': // Platinum
                    Difficulty = TrophyDifficulty.Platinum;
                    break;
            }
        }

        // Copy
        public Trophy Copy()
        {
            // Creates (and returns) a new instance that is identical to this (data wise)
            return new Trophy()
            {
                ID = this.ID,
                Title = this.Title,
                Description = this.Description,
                Difficulty = this.Difficulty,
                ImageURL = this.ImageURL
            };
        }
    }
}
