using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameJoltAPI.JsonObjects;

namespace GameJoltAPI
{
    /// <summary>
    /// GameJolt user types
    /// </summary>
    public enum UserType
    {
        User,
        Moderator,
        Admin
    }
    /// <summary>
    /// GameJolt user (ban) status
    /// </summary>
    public enum UserStatus
    {
        Active,
        Banned
    }
    /// <summary>
    /// GameJolt users session status
    /// </summary>
    public enum UserSessionStatus
    {
        Active,
        Idle
    }

    /// <summary>
    /// Contains most data assigned to a GameJolt user
    /// (except for highscores and trophies)
    /// </summary>
    public class UserData
    {
        // Data
        /// <summary>
        /// The ID of the user
        /// </summary>
        public string ID = "";
        /// <summary>
        /// Can be "User", "Moderator" or "Admin"
        /// </summary>
        public UserType Type = UserType.User;
        /// <summary>
        /// The user's username
        /// </summary>
        public string Name = "";
        /// <summary>
        /// The URL of the user's avatar
        /// </summary>
        public string AvatarURL = "";
        /// <summary>
        /// How long ago the user signed up
        /// </summary>
        public string SignedUp = "";
        /// <summary>
        /// How long ago the user was last logged in. Will be "Online Now" if the user is currently online
        /// </summary>
        public string LastLogin = "";
        /// <summary>
        /// "Active" if the user is still a member on the site. "Banned" if they've been banned
        /// </summary>
        public UserStatus Status = UserStatus.Active;
        /// <summary>
        /// The developer's name
        /// </summary>
        public string DeveloperName = "";
        /// <summary>
        /// The developer's website, if they put one in
        /// </summary>
        public string DeveloperWebsite = "";
        /// <summary>
        /// The description that the developer put in for themselves. HTML tags and new lines have been removed
        /// </summary>
        public string DeveloperDescription = "";
        /// <summary>
        /// If the user is a developer or not
        /// </summary>
        public bool IsDeveloper = false;

        // Constructor(s)
        public UserData()
        {
        }
        public UserData(UserJson user)
        {
            // Copy the user data
            ID = user.ID;
            Name = user.Name;
            AvatarURL = user.AvatarURL;
            SignedUp = user.SignedUp;
            LastLogin = user.LastLogin;
            DeveloperName = user.DeveloperName;
            DeveloperWebsite = user.DeveloperWebsite;
            DeveloperDescription = user.DeveloperDescription;
            IsDeveloper = user.IsDeveloper;

            // Get user type
            if (user.Type[0] == 'A')
                Type = UserType.Admin;
            else if (user.Type[0] == 'M')
                Type = UserType.Moderator;

            // Get user status
            if (user.Status[0] == 'B')
                Status = UserStatus.Banned;
        }

        /// <summary>
        /// Gets the users display name
        /// (it is the "developer name" for developer)
        /// </summary>
        /// <returns>Users display name</returns>
        public string GetDisplayName()
        {
            if (IsDeveloper)
                return DeveloperName;
            return Name;
        }
        /// <summary>
        /// Gets the URL to the users profile (requires the ID to be correct)
        /// </summary>
        /// <returns>URL to users profile page</returns>
        public string GetProfileURL()
        {
            return string.Format(@"http://gamejolt.com/profile/{0}/", ID);
        }

        // Copy
        public UserData Copy()
        {
            // Creates (and returns) a new instance that is identical to this (data wise)
            return new UserData()
            {
                ID = this.ID,
                Type = this.Type,
                Name = this.Name,
                AvatarURL = this.AvatarURL,
                SignedUp = this.SignedUp,
                LastLogin = this.LastLogin,
                Status = this.Status,
                DeveloperName = this.DeveloperName,
                DeveloperWebsite = this.DeveloperWebsite,
                DeveloperDescription = this.DeveloperDescription,
                IsDeveloper = this.IsDeveloper
            };
        }
        public void CopyTo(UserData target)
        {
            //
            target.ID = ID;
            target.Type = Type;
            target.Name = Name;
            target.AvatarURL = AvatarURL;
            target.SignedUp = SignedUp;
            target.LastLogin = LastLogin;
            target.Status = Status;
            target.DeveloperName = DeveloperName;
            target.DeveloperWebsite = DeveloperWebsite;
            target.DeveloperDescription = DeveloperDescription;
            target.IsDeveloper = IsDeveloper;
        }
    }
}
