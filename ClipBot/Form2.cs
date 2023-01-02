using FFMpegCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
//using WMPLib;

namespace ClipBot
{
    public partial class Form2 : Form
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>(5);

        Stopwatch stopwatch = new Stopwatch();
        List<string> commandsForCropping = new List<string>();
        string sourceDir = Directory.GetCurrentDirectory();
        public Form2(string gameName, string amount, string started_at, string ended_at, string streamerId)
        {
            void Form2_Shown(Object sender, EventArgs e)
            {
                if (!string.IsNullOrEmpty(gameName) && !string.IsNullOrEmpty(streamerId))
                {
                    parameters.Add("broadcaster_id", GetStreamerId(streamerId));
                }

                if (!string.IsNullOrEmpty(gameName) && string.IsNullOrEmpty(streamerId))
                {
                    parameters.Add("game_id", GetGameId(gameName));
                }
                else if (string.IsNullOrEmpty(gameName) && !string.IsNullOrEmpty(streamerId))
                {
                    parameters.Add("broadcaster_id", GetStreamerId(streamerId));
                }

                if (!string.IsNullOrEmpty(started_at))
                {
                    parameters.Add("started_at", started_at);
                    parameters.Add("ended_at", ended_at);
                }
                parameters.Add("first", amount);

                stopwatch.Start();
                GetClipLinks(gameName, amount, streamerId);

                this.Close();
            }
            InitializeComponent();
            Shown += Form2_Shown;

        }

        public void AddToProgress(int value)
        {
            progressBar1.Value += value;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        public void GetClipLinks(string gameName, string amount, string streamerName)
        {
            //Sets progress to 10
            AddToProgress(10);
            //Need: (broadcaster_id or game_id)
            //Need: (started_at and ended_at)
            //Need: (first) = amount of clips *max 100

            //Setting up parameters and headers for the HTTP request
            //need either broadcaster_id or game_id

            //parameters = dictionary
            List<string> compiledParameters = new List<string>();
            string finishedParameters = null;
            foreach (var param in parameters)
            {
                compiledParameters.Add($@"{param.Key}={param.Value}");

                finishedParameters = string.Join("&", compiledParameters);

            }

            string url = $@"https://api.twitch.tv/helix/clips?{finishedParameters}";

            var request = WebRequest.Create(url);
            request.Headers.Add("Authorization", "Bearer w6e0u5v6iky3885udqof7uc0o7mp3n");
            request.Headers.Add("Client-Id", "lw7irfw3qhufcx6mh7x599tsay8ux6");
            ContinueUsingUrl(request, gameName, streamerName);

        }

        private void ContinueUsingUrl(WebRequest request, string gameName, string streamerName)
        {
            request.Method = "GET";

            //Completing the http request
            using
            var webResponse = request.GetResponse();
            using
            var webStream = webResponse.GetResponseStream();

            //Converts to a string and parses to find the url of each clip
            using
            var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();
            var responseData = JObject.Parse(data);

            int amountOfClips = responseData["data"].Count();
            MessageBox.Show($"{amountOfClips.ToString()} clips found.");

            List<string> command = new List<string>();

            for (int i = 0; i < amountOfClips; i++)
            {
                //Gets the clip urls
                var rawClipId = responseData["data"][i]["id"];
                string clipId = rawClipId.ToString();

                if (i == amountOfClips - 1)
                {

                    command.Add($"TwitchDownloaderCLI.exe clipdownload -u {clipId} -o video{i}.mp4 & exit");
                    commandsForCropping.Add($"ffmpeg -ss 00:00:00 -y -i video{i}.mp4 -to 00:00:10 -c:v copy -c:a copy -avoid_negative_ts make_zero video{i} & exit");
                    string combinedString = string.Join("", command);
                    AddToProgress(30 * (1 / amountOfClips));
                    //Progress should be at 40
                    ExecuteCommand(combinedString);

                    foreach (string seperateCommand in commandsForCropping)
                    {
                        ExecuteCommand(seperateCommand);
                    }

                    AddToProgress(40);
                    //Progress should be at 80

                    EditVideo(amountOfClips, gameName, streamerName);
                }
                else
                {
                    AddToProgress(30 / (amountOfClips - 1));
                    commandsForCropping.Add($"ffmpeg -ss 00:00:00 -y -i video{i}.mp4 -to 00:00:10 -c:v copy -c:a copy -avoid_negative_ts make_zero video{i} & exit");
                    command.Add($"TwitchDownloaderCLI.exe clipdownload -u {clipId} -o video{i}.mp4 & ");
                }
            }
        }

        private void EditVideo(int amountOfClips, string gameName, string streamerName)
        {
            Console.WriteLine("Compiling Clips");
            List<string> list = new List<string>();
            for (int i = 0; i < amountOfClips; i++)
            {
                AddToProgress(20 / amountOfClips);
                list.Add(@$"{sourceDir}\video{i}.mp4");
                String[] input = list.ToArray();
                if (i == amountOfClips - 1)
                {
                    DateTime now = DateTime.Now;
                    string formattedTime = now.ToString("MMddyy_Hmmss");
                    string outputPath2 = @$"{sourceDir}\Finished\{streamerName}{gameName}{formattedTime}.mp4";
                    FFMpeg.Join(outputPath2, input);
                    stopwatch.Stop();
                    MessageBox.Show($@"Finished all {amountOfClips} clips in {stopwatch.ElapsedMilliseconds / 1000} seconds at {sourceDir}\Finished\{gameName}{formattedTime}");
                }

            }
        }

        static void ExecuteCommand(string command)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();
        }
        private static string GetGameId(string gameName)
        {

            string url = $"https://api.twitch.tv/helix/games?name={gameName}";
            var request3 = WebRequest.Create(url);
            request3.Headers.Add("Authorization", "Bearer w6e0u5v6iky3885udqof7uc0o7mp3n");
            request3.Headers.Add("Client-Id", "lw7irfw3qhufcx6mh7x599tsay8ux6");
            request3.Method = "GET";

            //Completing the http request
            using
            var webResponse3 = request3.GetResponse();
            using
            var webStream3 = webResponse3.GetResponseStream();

            //Converts to a string and parses to find the url of each clip
            using
            var reader3 = new StreamReader(webStream3);
            var data3 = reader3.ReadToEnd();
            var responseData3 = JObject.Parse(data3);
            var rawGameId = responseData3["data"][0]["id"];
            string gameId = rawGameId.ToString();

            return gameId;
        }
        private static string GetStreamerId(string streamerName)
        {
            //uses: login: "pokimane"
            //gets data[id]
            string url = $"https://api.twitch.tv/helix/users?login={streamerName}";
            var request4 = WebRequest.Create(url);
            request4.Headers.Add("Authorization", "Bearer w6e0u5v6iky3885udqof7uc0o7mp3n");
            request4.Headers.Add("Client-Id", "lw7irfw3qhufcx6mh7x599tsay8ux6");
            request4.Method = "GET";

            //Completing the http request
            using
            var webResponse4 = request4.GetResponse();
            using
            var webStream4 = webResponse4.GetResponseStream();

            //Converts to a string and parses to find the url of each clip
            using
            var reader4 = new StreamReader(webStream4);
            var data4 = reader4.ReadToEnd();
            var responseData4 = JObject.Parse(data4);

            var rawStreamerId = responseData4["data"][0]["id"];
            string streamerId = rawStreamerId.ToString();

            return streamerId;
        }
    }
}