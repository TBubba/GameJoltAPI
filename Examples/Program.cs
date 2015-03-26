using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameJoltAPI;

namespace Examples
{
    class Program
    {
        private static bool _exit; // If the main loop should be exited

        static void Main(string[] args)
        {
            // Writes header to console (visuals only)
            WriteHeader();

            // Create a game client (with the details from this wrappers gamejolt page)
            GameClient client = new GameClient("22926", "a9d883aa16a61f2af1d14d09c0d44c71");

            // Set up event for when the authentication is complete
            client.AuthenticationCompleted += (o, e) =>
                {
                    // Check the state of the call response
                    if (e.CallSuccessful()) // If the response says the call was successful
                        ExConsole.WriteLine("GameClient: Authentication successful!");
                    else // If the response says the call failed
                    {
                        if (!e.Connected) // If the server did not respond
                            ExConsole.Write("GameClient: Authentication failed! Could not connect to server.");
                        else if (!e.Success) // If the server responded, but the call was invalid
                            ExConsole.Write("GameClient: Authentication failed! GameID / GameKey combination invalid!");
                    }

                    // Ready write buffer
                    ExConsole.ReadyWriteBuffer();
                };

            // Authenticate client
            client.Authenticate();

            /* Todo:
             * + Add example of user fetching and authenticating
             * + Add example of user-session usage (opening, pinging, closing)
             * + Add example of trophy fetching and achieving (for users)
             * + Add example of fetching highscore tables (all tables from a game)
             * + Add example of fetching (and setting) highscores from tables (that earlier was fetched)
             * + Add example of fetching/setting/upadting/etc. data from the data-bank
             * + Add example of any feature that will be in the library not listed above
            */

            // Wipes any input
            AnyKeyPressed();

            // Console type loop
            while (!_exit)
            {
                // Exit loop (next step) if any key is pressed
                if (AnyKeyPressed())
                    _exit = true;

                // Updates console write buffer (writes text if buffer is ready)
                ExConsole.UpdateWriteBuffer();
            }

            // Write any "left overs" in the console writer buffer
            ExConsole.ReadyWriteBuffer();
            ExConsole.UpdateWriteBuffer();

            // Log out users
            //client.LogOutUsers();

            // Waits for final input before termination
            Console.WriteLine("\nPress any key to continue...\n");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Checks if any key was pressed since last check
        /// (only for making example more readable)
        /// </summary>
        /// <returns></returns>
        static bool AnyKeyPressed()
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes a header for the example
        /// (nothing but console visuals, no need to keep this code)
        /// </summary>
        static void WriteHeader()
        {
            // Constructs header
            ConsoleSegment[] header = new ConsoleSegment[]{new ConsoleSegment("+------------------------------------------------+\n", ConsoleColor.Green),
                                                           new ConsoleSegment("|", ConsoleColor.Green),
                                                           new ConsoleSegment(" Welcome to Bubba's GameJolt API Wrapper for C# ", ConsoleColor.Yellow),
                                                           new ConsoleSegment("|\n", ConsoleColor.Green),
                                                           new ConsoleSegment("+------------------------------------------------+\n", ConsoleColor.Green)};

            // Calculate distance between header edge and console window edge
            int step = 0;
            for (int i = 0; i < header.Length; i++)
                if (header[i].Text.Length > step)
                    step = (ExConsole.ConsoleWidth / 2 - header[i].Text.Length / 2);

            // Align header at center
            header[0].Text = new string(' ', step) + header[0].Text;
            for (int i = 0; i < header.Length; i++)
                header[i].Text = header[i].Text.Replace("\n", "\n" + new string(' ', step));

            // Write Header to buffer
            for (int i = 0; i < header.Length; i++)
                ExConsole.Write(header[i]);
            ExConsole.Write("\n");

            // Additional Information
            ExConsole.Write("Press any key to log out all users (Once they are logged in)\n");
            ExConsole.Write("Press once more to close this program\n");
            ExConsole.Write("\n");

            // Write buffer to console
            ExConsole.ReadyWriteBuffer();
        }
    }
}
