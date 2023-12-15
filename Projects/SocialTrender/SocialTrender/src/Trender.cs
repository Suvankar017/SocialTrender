using System;
using System.IO;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SocialTrender
{
    public class Trender
    {
        private IWebDriver m_Browser;
        private List<Searcher> m_Searcher;
        private Action<float, string> m_OnProgressReceived;
        private float m_Progress;

        private static Trender s_Instance;

        private Trender(bool showBrowser, Action<string> onLogMessage)
        {
            string userDataDirectoryPath = $"{Directory.GetCurrentDirectory()}\\WebUserData\\Headless";

            ChromeOptions options = CreateChromeOptions(showBrowser, userDataDirectoryPath);
            m_Browser = new ChromeDriver(options);
            m_Searcher = new List<Searcher>()
            {
                new InstagramSearcher(m_Browser, userDataDirectoryPath, onLogMessage, OnProgressReceived)
            };
        }

        private void Close()
        {
            m_Browser.Quit();
        }

        private async void OnSearch(string keyword, int maxPostCount)
        {
            foreach (Searcher searcher in m_Searcher)
            {
                await searcher.Search(keyword, maxPostCount);
            }
        }

        private void SetCompleteCallback(Action<string[], List<byte[]>> onComplete)
        {
            m_Searcher.ForEach(searcher => searcher.SetOnCompleteCallback(onComplete));
        }

        private void SetFailCallback(Action onFail)
        {
            m_Searcher.ForEach(searcher => searcher.SetOnFailCallback(onFail));
        }

        private void SetProgressCallback(Action<float, string> onProgress)
        {
            m_OnProgressReceived = onProgress;
        }

        private void OnProgressReceived(float progress, string info)
        {
            m_Progress = progress / m_Searcher.Count;
            m_OnProgressReceived?.Invoke(m_Progress, info);
        }

        private ChromeOptions CreateChromeOptions(bool showBrowser, string userDataDirectoryPath)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            //options.AddArgument("--window-size=800,600");
            options.AddArgument("--disable-notifications");
            options.AddArguments($"--user-data-dir={userDataDirectoryPath}");
            options.AddArgument("--log-level=3");
            if (!showBrowser)
                options.AddArgument("--headless");

            return options;
        }


        public static bool Init(bool showBrowser, Action<string> onLogMessage)
        {
            if (s_Instance != null)
                return false;

            s_Instance = new Trender(showBrowser, onLogMessage);
            return true;
        }

        public static bool Shutdown()
        {
            if (s_Instance == null)
                return false;

            s_Instance.Close();
            s_Instance = null;
            return true;
        }

        public static void Search(string keyword, int maxPostCount)
        {
            if (s_Instance == null)
                return;

            s_Instance.OnSearch(keyword, maxPostCount);
        }

        public static void SetSearchCompleteCallback(Action<string[], List<byte[]>> onSearchComplete)
        {
            if (s_Instance == null)
                return;

            s_Instance.SetCompleteCallback(onSearchComplete);
        }

        public static void SetSearchFailedCallback(Action onSearchFail)
        {
            if (s_Instance == null)
                return;

            s_Instance.SetFailCallback(onSearchFail);
        }

        public static void SetProgressReceivedCallback(Action<float, string> onProgressReceived)
        {
            if (s_Instance == null)
                return;

            s_Instance.SetProgressCallback(onProgressReceived);
        }

    }
}
