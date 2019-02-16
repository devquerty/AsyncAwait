using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TwitterProfile
{
    public class FetchProfiles
    {
        public void Fetch(string fileName)
        {
            var watch = new Stopwatch();

            watch.Start();

            var userNames = GetUserNamesFromFile(fileName);

            foreach (var username in userNames)
            {
                var result = FetchProfile(username);

                SaveToFile($"{username}.html", result);
            }

            watch.Stop();

            Console.WriteLine($"Total Seconds Taken for Sync Process: {watch.ElapsedMilliseconds }");
        }

        public async Task FetchAsync(string fileName)
        {
            var watch = new Stopwatch();

            watch.Start();

            var userNames = await GetUserNamesFromFileAsync(fileName);

            var taskListFetchProfile = new List<Task<(string userName, string result)>>();

            foreach (var username in userNames)
            {
                taskListFetchProfile.Add(FetchProfileAsync(username));
            }

            await Task.WhenAll(taskListFetchProfile.ToArray());

            var taskListSaveToFiles = new List<Task>();

            foreach (var task in taskListFetchProfile)
            {

                var fileSaveTask = SaveToFileAsync($"{task.Result.userName}_async.html", task.Result.result);

                taskListSaveToFiles.Add(fileSaveTask);
            }

            await Task.WhenAll(taskListSaveToFiles.ToArray());

            watch.Stop();

            Console.WriteLine($"Total Seconds Taken for Async Process: {watch.ElapsedMilliseconds }");
        }

        private IEnumerable<string> GetUserNamesFromFile(string fileName)
        {
            List<string> userNames = new List<string>();

            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var userName = reader.ReadLine();
                    userNames.Add(userName);
                }
            }

            return userNames;
        }

        private void SaveToFile(string fileName, string fileContent)
        {
            File.WriteAllText(fileName, fileContent);
        }

        private string FetchProfile(string username)
        {
            var url = $"https://twitter.com/{username}";

            using (var client = new HttpClient())
            {
                var task = client.GetAsync(url);
                return task.Result.Content.ReadAsStringAsync().Result;
            }
        }

        private async Task<IEnumerable<string>> GetUserNamesFromFileAsync(string fileName)
        {
            List<string> userNames = new List<string>();

            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var userName = await reader.ReadLineAsync();
                    userNames.Add(userName);
                }
            }

            return userNames;
        }

        private async Task SaveToFileAsync(string fileName, string fileContent)
        {
            var task = File.WriteAllTextAsync(fileName, fileContent);

            await task;
        }

        private async Task<(string userName, string result)> FetchProfileAsync(string username)
        {
            var url = $"https://twitter.com/{username}";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var result = response.Content.ReadAsStringAsync().Result;

                return (username, result);
            }
        }

    }
}
