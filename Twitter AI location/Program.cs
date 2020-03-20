using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tweetinvi;

namespace Twitter_AI_location
{
    class Program
    {

        static void Main(string[] args)
        {

            if(args.Length < 1)
            {
                PrintHelp();
                Environment.Exit(0);
            }

            int max_result = 35;

            if (args.Length == 2)
            {
                if (Int32.TryParse(args[1], out int max) && max <= 70) { 
                    max_result = max;
                    PrintLogo();
                }
                else  Console.WriteLine("The second argument has to be an integer and have a value lesser than 70.");
            }

            Auth.SetUserCredentials(
                "qpHCXcV4IEap3T02zJHi234Ev",
                "zjSizlzsC9YmN9qm1BTheXF4delq1CwALA7fBnykBtLCaMb946",
                "316723929-0zRG0RzpDYRwMTw3UvgUu37wni0PHKJBU9IL5BKQ",
                "cCJN2wIPubXBsNCBbVTRXzelIp9vHRXh9Jur5bF9SG52i"
            );

            void PrintLogo()
            {
                Console.WriteLine("       _____      _   _                     _   _             ");
                Console.WriteLine("      / ____|    | | | |                   | | (_)            ");
                Console.WriteLine("     | |  __  ___| |_| |     ___   ___ __ _| |_ _  ___  _ __  ");
                Console.WriteLine(@"     | | |_ |/ _ \ __| |    / _ \ / __/ _` | __| |/ _ \| '_ \ ");
                Console.WriteLine("     | |__| |  __/ |_| |___| (_) | (_| (_| | |_| | (_) | | | |");
                Console.WriteLine(@"      \_____|\___|\__|______\___/_\___\__,_|\__|_|\___/|_| |_|");
                Console.WriteLine("                   |__   __|     (_) | | |                     ");
                Console.WriteLine("                      | |_      ___| |_| |_ ___ _ __           ");
                Console.WriteLine(@"                      | \ \ /\ / / | __| __/ _ \ '__|          ");
                Console.WriteLine(@"                      | |\ V  V /| | |_| ||  __/ |             ");
                Console.WriteLine(@"                      |_| \_/\_/ |_|\__|\__\___|_|             ");
                Console.WriteLine(@"                                                               ");
                Console.WriteLine(@"                           [+] Sharki.syp@gmail.com        ");
                Console.WriteLine(@"                           [+] Created by Sharki.       ");
            }

            void PrintHelp() => Console.WriteLine("Invalid username. Usage: [ProgramName.exe] + Target's username (@Example/example) to track. [Optional: (Integer) Maximum number of searches allowed < 70] {0}", args.Length);

            string findLocation(List<string> locations)
            {
                IEnumerable<dynamic> query = locations.GroupBy(r => r)
                    .Select(grp => new
                    {
                        Location = grp.Key,
                        Count = grp.Count()
                    });

                int n = 0;
                foreach (var Q in query)
                    if (Q.Count > n) n = Q.Count;

                return query.Where(x => x.Count == n).First().Location;
            }

            Tweetinvi.Models.IUser User = Tweetinvi.User.GetUserFromScreenName(args[0]);

            if (User == null) {
                Console.WriteLine("This twitter's account doesn't exist or the rate limit has been reached");
                Environment.Exit(0);
            }

            IEnumerable<long> People = User.FollowersCount < User.FriendsCount ? User.GetFollowerIds() : User.GetFriendIds();
            List<string> Location_firstlist = new List<string>();
            List<string> Location_secondlist = new List<string>();

            int count = 0;

            foreach (long person in People.Reverse())
            {
                var Relation = Friendship.GetRelationshipDetailsBetween(person, User.UserIdentifier);

                if (Relation == null) { Console.WriteLine("Exception"); break; }
                if (Relation.Following && Relation.FollowedBy)
                {
                    var person_ = Tweetinvi.User.GetUserFromId(person);
                    string[] Location = person_.Location.Split(',', '-');

                    if (Location[0] != "" && Location[0] != "??")
                    {
                        Location_firstlist.Add(Location[0]);
                        try { Location_secondlist.Add(Location[1]); } catch { };
                        count++;
                        Console.WriteLine("[+] ---> " + person_.ScreenName);
                    }
                    else
                        Console.WriteLine("[-] ---> " + person_.ScreenName);
                }
                if (count > max_result) break;
            }

            Console.WriteLine("\n{0} Probable location based on target's followers/follows", findLocation(Location_firstlist));
            Console.WriteLine("{0} Probable zone/country based on target's followers/follows", findLocation(Location_secondlist));

            if(!String.IsNullOrEmpty(User.Location))
                Console.WriteLine("User's set location: {0}", User.Location);

        }
    }
}
