using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TwitterProfile
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var fetchProfiles = new FetchProfiles();

                fetchProfiles.Fetch("Twitterhandles.csv");

                var task = fetchProfiles.FetchAsync("Twitterhandles.csv");

                task.Wait();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
