using System.IO;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
//using MongoDB.Driver;

namespace SocialTrender
{
    public class Trender
    {
        private IWebDriver m_Browser;
        private List<Searcher> m_Searchers;
        private static Trender s_Instance;

        private Trender(bool showBrowser, string userDataDirectoryPath)
        {
            ChromeOptions options = CreateChromeOptions(showBrowser, userDataDirectoryPath);
            m_Browser = new ChromeDriver(options);

            //try
            //{
            //    MongoClient client = new MongoClient(MongoDBUtil.c_ConnectionURI);
            //    IMongoDatabase database = client.GetDatabase(MongoDBUtil.c_DatabaseName);

            //    m_Searchers = new List<Searcher>()
            //    {
            //        new InstagramSearcher(m_Browser, database, userDataDirectoryPath)
            //    };
            //}
            //catch
            //{
            //    m_Searchers = new List<Searcher>();
            //}

            m_Searchers = new List<Searcher>()
            {
                new InstagramSearcher(m_Browser, userDataDirectoryPath)
            };
        }


        public static bool Init(bool showBrowser, string userDataDirectoryPath)
        {
            if (s_Instance != null)
                return false;

            if (!string.IsNullOrEmpty(userDataDirectoryPath))
                userDataDirectoryPath += "\\WebUserData";

            s_Instance = new Trender(showBrowser, userDataDirectoryPath);
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

        public static void SetCallback(CallbackData data)
        {
            if (s_Instance == null)
                return;

            s_Instance.SetCallbackData(data);
        }

        public static void Search(string keyword, int maxSearchResultCount, bool forceWebSearch = false)
        {
            if (s_Instance == null)
                return;

            s_Instance.SearchAsync(keyword, maxSearchResultCount, forceWebSearch);
        }


        private void SetCallbackData(CallbackData data)
        {
            m_Searchers.ForEach((scraper) => scraper.SetCallbacks(data));
        }

        private void Close()
        {
            m_Browser.Quit();
        }

        private async void SearchAsync(string keyword, int maxSearchResultCount, bool forceWebSearch)
        {
            foreach (Searcher searcher in m_Searchers)
            {
                await searcher.SearchAsync(keyword, maxSearchResultCount, forceWebSearch);
            }
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
    }
}
