using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using FFMpegCore;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using ClipBot;
using static ClipBot.Form1;
using System.DirectoryServices.ActiveDirectory;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
//access token: w6e0u5v6iky3885udqof7uc0o7mp3n

namespace Functions
    {
    public class Program
    {
        static void Main()
        {
            Application.Run(new Form1());
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //GetClipLinks();
            //stopwatch.Stop();
            //Console.WriteLine("Finished in " + stopwatch.ElapsedMilliseconds/1000 + "seconds");
        }
        public static void GetClipLinks(string gameName, string amount)
        {
            //Need: (broadcaster_id or game_id)
            //Need: (started_at and ended_at)
            //Need: (first) = amount of clips *max 100

            //Setting up parameters and headers for the HTTP request
            
            string gameId = GetGameId(gameName);
            var first = amount;
            MessageBox.Show($"GameID: {gameId}");

            string url = $"https://api.twitch.tv/helix/clips?game_id={gameId}&first={first}";
            var request = WebRequest.Create(url);
            request.Headers.Add("Authorization", "Bearer w6e0u5v6iky3885udqof7uc0o7mp3n");
            request.Headers.Add("Client-Id", "lw7irfw3qhufcx6mh7x599tsay8ux6");
            request.Method = "GET";

            //Completing the http request
            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            //Converts to a string and parses to find the url of each clip
            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();
            var responseData = JObject.Parse(data);



            int amountOfClips = responseData["data"].Count();
            MessageBox.Show($"{amountOfClips} clips found.");
            List<string> command = new List<string>();

            for (int i = 0; i<amountOfClips; i++)
            {
                //Gets the clip urls
                var rawClipId = responseData["data"][i]["id"];
                string clipId = rawClipId.ToString();
                MessageBox.Show($"ClipID: {clipId}");
                

                

                if(i == amountOfClips-1)
                {
                    command.Add($"TwitchDownloaderCLI.exe clipdownload -u {clipId} -o video{i}.mp4");
                    string combinedString = string.Join("", command);
                    ExecuteCommand(combinedString);
                    MessageBox.Show(combinedString);
                    EditVideo(amountOfClips);
                }
                else
                {
                    command.Add($"TwitchDownloaderCLI.exe clipdownload -u {clipId} -o video{i}.mp4 & ");
                }
            }
        }
        private static string GetGameId(string gameName)
        {
            string url = $"https://api.twitch.tv/helix/games?name={gameName}";
            var request3 = WebRequest.Create(url);
            request3.Headers.Add("Authorization", "Bearer w6e0u5v6iky3885udqof7uc0o7mp3n");
            request3.Headers.Add("Client-Id", "lw7irfw3qhufcx6mh7x599tsay8ux6");
            request3.Method = "GET";

            //Completing the http request
            using var webResponse3 = request3.GetResponse();
            using var webStream3 = webResponse3.GetResponseStream();

            //Converts to a string and parses to find the url of each clip
            using var reader3 = new StreamReader(webStream3);
            var data3 = reader3.ReadToEnd();
            var responseData3 = JObject.Parse(data3);


            var rawGameId = responseData3["data"][0]["id"];
            string gameId = rawGameId.ToString();

            
            return gameId;
        }




        //This method joins videos using FFMpegCore
        private static void EditVideo(int amountOfClips)
        {
            Console.WriteLine("Compiling Clips");
            List<string> list = new List<string>();
            for (int i = 0; i < amountOfClips; i++)
            { 
                list.Add(@$"C:\Users\Ethan\source\repos\ClipBot\ClipBot\bin\Debug\net6.0-windows\video{i}.mp4");
                String[] input = list.ToArray();
                if(i == amountOfClips-1)
                {
                    string outputPath2 = @"C:\Users\Ethan\source\repos\ClipBot\ClipBot\bin\Debug\net6.0-windows\edited.mp4";
                    FFMpeg.Join(outputPath2, input);
                    Console.WriteLine("Compiled all clips as: edited.mp4");
                }
            
            }
        }



        static void ExecuteCommand(string command)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = false;
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
        }
}