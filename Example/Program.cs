using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameJoltAPI;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a game client (with the details from this wrappers gamejolt page)
            GameClient client = new GameClient("22926", "a9d883aa16a61f2af1d14d09c0d44c71");

            /* Todo:
             * + Add example of user fetching and authenticating
             * + Add example of user-session usage (opening, pinging, closing)
             * + Add example of trophy fetching and achieving (for users)
             * + Add example of fetching highscore tables (all tables from a game)
             * + Add example of fetching (and setting) highscores from tables (that earlier was fetched)
             * + Add example of fetching/setting/upadting/etc. data from the data-bank
             * + Add example of any feature that will be in the library not listed above
            */
        }
    }
}
