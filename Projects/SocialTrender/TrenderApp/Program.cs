using SocialTrender;
using System;
using System.IO;

public class Program
{
    public static void Main()
    {
        CallbackData callbackData = new CallbackData()
        {
            SearchCompleteCallback = (data) =>
            {
                Log.Success("Search completed");
                Log.Info($"Result count : {data.Images.Count}");
            },
            SearchFailCallback = () =>
            {
                Log.Error("Search failed");
            },
            LogMessageCallback = (msg) =>
            {
                //Log.Info(msg);
            },
            ProgressCallback = (data) =>
            {
                Log.Info($"Progress : {data.Progress * 100}%\nMessage : {data.Message}\n");
            }
        };

        Trender.Init(false, Directory.GetCurrentDirectory());
        Trender.SetCallback(callbackData);

        Run();

        Trender.Shutdown();
    }

    private static void Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            Console.Clear();

            Console.Write("Write a search keyword : ");
            string keyword = Console.ReadLine();

            Trender.Search("#" + keyword, 8);

            Console.ReadLine();
        }
    }
}
