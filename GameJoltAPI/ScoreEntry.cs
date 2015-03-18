using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameJoltAPI.JsonObjects;

namespace GameJoltAPI
{
    /// <summary>
    /// An entry in a highscore table
    /// </summary>
    public class ScoreEntry
    {
        /// <summary>
        /// This is a string value associated with the score. Example: "234 Jumps".
        /// </summary>
        public string Score = "";
        /// <summary>
        /// This is a numerical sorting value associated with the score. All sorting will work off of this number. Example: "234".
        /// </summary>
        public string Sort = "";
        /// <summary>
        /// Any extra data associated with the score.
        /// </summary>
        public string ExtraData = "";
        /// <summary>
        /// When the score was logged by the user.
        /// </summary>
        public string Stored = "";

        /// <summary>
        /// If this is a user score, this is the display name for the user.
        /// But if this is a guest score, this is the guest's submitted name.
        /// </summary>
        public string Name = "";
        /// <summary>
        /// If this is a user score, this is the user's ID.
        /// </summary>
        public string UserID = "";

        /// <summary>
        /// If the score was submitted by a guest (if not, it was by a user).
        /// </summary>
        public bool IsGuest = true;

        // Constructor(s)
        public ScoreEntry()
        {
        }
        public ScoreEntry(ScoreJson score)
        {
            // Copy data
            Score = score.Score;
            Sort = score.Sort;
            ExtraData = score.ExtraData;
            UserID = score.UserID;
            Stored = score.Stored;

            // If the score is from a guest
            IsGuest = (score.Guest == "");

            // Get the name
            if (IsGuest)
                Name = score.User;
            else
                Name = score.Guest;
        }
    }
}
