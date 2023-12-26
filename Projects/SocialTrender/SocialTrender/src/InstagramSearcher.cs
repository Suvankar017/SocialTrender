using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
//using MongoDB.Driver;

namespace SocialTrender
{
    public class InstagramSearcher : Searcher
    {
        //private readonly IMongoCollection<SearchDataModel> m_Collection;

        private const string c_WebsiteURL = "https://www.instagram.com/";
        private const string c_Username = "botintern2.0";
        private const string c_Password = "BotIntern@143";
        private const string c_IsLoggedIn = "isInstagramLoggedIn";
        private const string c_DatabaseCollectionName = "Instagram";

        public InstagramSearcher(IWebDriver browser/*, IMongoDatabase database*/, string userDataDirectoryPath) : base(browser, userDataDirectoryPath)
        {
            //try
            //{
            //    m_Collection = database.GetCollection<SearchDataModel>(c_DatabaseCollectionName);
            //}
            //catch
            //{
            //    database.CreateCollection(c_DatabaseCollectionName);
            //    m_Collection = database.GetCollection<SearchDataModel>(c_DatabaseCollectionName);
            //}

            LoadSavedData();
        }

        public override async Task SearchAsync(string keyword, int maxSearchResultCount, bool forceWebSearch)
        {
            await OnSearchAsync(keyword, maxSearchResultCount, forceWebSearch);
        }


        private void LoadSavedData()
        {
            if (!Directory.Exists(m_UserDataDirectoryPath))
                Directory.CreateDirectory(m_UserDataDirectoryPath);

            if (!File.Exists(m_SaveFilePath))
            {
                using FileStream stream = File.Create(m_SaveFilePath);
            }

            string savedContent = File.ReadAllText(m_SaveFilePath);
            if (string.IsNullOrEmpty(savedContent))
                return;

            JObject json = JObject.Parse(savedContent);

            JSONUtil.SaveTo(json[c_IsLoggedIn], ref m_IsLoggedIn);
        }

        private void SaveSessionData()
        {
            JObject json = new JObject();
            json[c_IsLoggedIn] = m_IsLoggedIn;

            string content = JsonConvert.SerializeObject(json, Formatting.Indented);
            File.WriteAllText(m_SaveFilePath, content);
        }

        //private async Task SaveIntoDatabase(SearchData searchData)
        //{
        //    var results = (await m_Collection.FindAsync(model => model.Data.Keyword == searchData.Keyword)).ToList();

        //    if (results.Count == 0)
        //    {
        //        SearchDataModel model = new SearchDataModel(searchData);
        //        await m_Collection.InsertOneAsync(model);
        //    }
        //    else
        //    {
        //        SearchDataModel model = results.First();
        //        model.Data = searchData;
        //        await ReplaceAsync(model);
        //    }
        //}

        //private async Task ReplaceAsync(SearchDataModel model)
        //{
        //    var filter = Builders<SearchDataModel>.Filter.Eq("ID", model.ID);
        //    await m_Collection.ReplaceOneAsync(filter, model);
        //}

        private void SendProgress(float progress, string message)
        {
            ProgressData data = new ProgressData()
            {
                Progress = progress,
                Message = message
            };
            m_ProgressCallback(data);
        }

        private bool Login()
        {
            m_LogMessageCallback("Signing In");

            try
            {
                var usernameField = Wait(m_Browser, 10.0).Until(ElementExists(By.CssSelector("input[name='username']")));
                usernameField?.Clear();
                usernameField?.SendKeys(c_Username);

                m_LogMessageCallback("Username field found");
            }
            catch
            {
                m_LogMessageCallback("Username field not found");
                return false;
            }

            try
            {
                var passwordField = Wait(m_Browser, 10.0).Until(ElementExists(By.CssSelector("input[name='password']")));
                passwordField?.Clear();
                passwordField?.SendKeys(c_Password);

                m_LogMessageCallback("Password field found");
            }
            catch
            {
                m_LogMessageCallback("Password field not found");
                return false;
            }

            try
            {
                Wait(m_Browser, 10.0).Until(ElementExists(By.CssSelector("button[type='submit']")))?.Click();

                m_LogMessageCallback("Submit button found");
            }
            catch
            {
                m_LogMessageCallback("Submit button not found");
                return false;
            }

            m_LogMessageCallback("Signed In successful");
            return true;
        }

        private UnityData GenerateUnityData(SearchData searchData)
        {
            UnityData data = new UnityData();
            data.Posts = searchData.Posts;
            data.Images = new List<byte[]>();

            if (searchData.Posts == null)
                return data;

            int i = 0;
            foreach (PostData post in searchData.Posts)
            {
                string link = post.Link;

                SendProgress(0.0f, $"Rendering post in {link}");
                m_Browser.Navigate().GoToUrl(link);

                Thread.Sleep(3000);

                data.Images.Add(TakeScreenshot(i + 1));
                i++;
            }

            return data;
        }

        private SearchData SearchIntoWeb(string keyword, int maxSearchResultCount)
        {
            // 1. Opening instagram
            SendProgress(0.0f, "Opening instagram");
            {
                m_LogMessageCallback("Opening instagram");
                try
                {
                    m_Browser.Navigate().GoToUrl(c_WebsiteURL);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                m_LogMessageCallback("Instagram opened");
            }


            // 2. Login
            SendProgress(0.0f, "Signing in");
            if (!m_IsLoggedIn)
            {
                if (!Login())
                {
                    m_SearchFailCallback();
                    m_LogMessageCallback("Sign In failed");
                    return default;
                }

                // Remember Login Info Menu
                try
                {
                    Wait(m_Browser, 10.0).Until(ElementExists(By.XPath("//div[contains(text(), 'Not now')]")))?.Click();
                }
                catch
                {
                    m_SearchFailCallback();
                    m_LogMessageCallback("Remember signin info menu 'Not now' button not found");
                    return default;
                }

                m_IsLoggedIn = true;
            }
            else
            {
                m_LogMessageCallback("Already signed in");
            }


            // 3. Searching
            SendProgress(0.0f, "Searching");
            // Click on search button
            try
            {
                string xPath = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div[1]/div[1]/div/div/div/div/div[2]/div[2]";
                Wait(m_Browser, 10.0).Until(ElementExists(By.XPath(xPath)))?.Click();
            }
            catch
            {
                m_SearchFailCallback();
                m_LogMessageCallback("Search button not found");
                return default;
            }

            // Write on search bar
            try
            {
                string xPath = "//input[@placeholder='Search']";
                var searchField = Wait(m_Browser, 10.0).Until(ElementExists(By.XPath(xPath)));
                searchField?.Clear();
                searchField?.SendKeys(keyword);
            }
            catch
            {
                m_SearchFailCallback();
                m_LogMessageCallback("Search field not found");
                return default;
            }


            // 4. Collecting results
            SendProgress(0.0f, "Collecting results");
            // Get search results
            List<IWebElement> searchableElements = new List<IWebElement>();
            try
            {
                Thread.Sleep(5000);

                Wait(m_Browser, 10.0).Until((d) =>
                {
                    string xPath = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div[1]/div[1]/div/div/div[2]/div/div/div[2]/div/div/div[2]/div";

                    var searchLinksContainer = Wait(m_Browser, 10.0f).Until(ElementExists(By.XPath(xPath)));
                    if (searchLinksContainer == null)
                        return false;

                    var anchorElements = Wait(m_Browser, 10.0).Until(ElementsExists(searchLinksContainer, By.XPath(".//a")));
                    if (anchorElements == null)
                        return false;

                    searchableElements = anchorElements.ToList();
                    return true;
                });
            }
            catch
            {
                m_SearchFailCallback();
                m_LogMessageCallback("Search results not found");
                return default;
            }


            // 5. Collecting channel links
            SendProgress(0.0f, "Collecting channel links");
            // Get all channel links
            List<string> channelLinks = new List<string>();
            if (keyword[0] == '#')
            {
                foreach (IWebElement link in searchableElements)
                {
                    string text = link.Text;
                    if (string.IsNullOrEmpty(text))
                        continue;

                    string[] profile = text.Split('\n');
                    string username = profile[0].Trim().Split('#')[1].Trim();
                    string profileLink = $"{c_WebsiteURL}explore/tags/{username}/";

                    channelLinks.Add(profileLink);
                }
            }
            else
            {
                foreach (IWebElement link in searchableElements)
                {
                    string text = link.Text;
                    if (string.IsNullOrEmpty(text))
                        continue;

                    string[] profile = text.Split('\n');
                    string username = profile[0].Trim();
                    string profileLink = (username[0] == '#') ? $"{c_WebsiteURL}explore/tags/{username.Split('#')[1].Trim()}/" : $"{c_WebsiteURL}{username}/";

                    channelLinks.Add(profileLink);
                }
            }


            // 6. Collecting post links
            SendProgress(0.0f, "Collecting post links");
            // Get all post links
            List<string> postLinks = new List<string>();
            if (keyword[0] == '#')
            {
                foreach (string link in channelLinks)
                {
                    m_Browser.Navigate().GoToUrl(link);

                    try
                    {
                        string path = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div[1]/div[2]/section/main/article/div/div";

                        var searchLinksContainer = Wait(m_Browser, 10.0).Until(ElementExists(By.XPath(path)));
                        if (searchLinksContainer == null)
                            continue;

                        var anchorElements = Wait(m_Browser, 10.0).Until(ElementsExists(searchLinksContainer, By.XPath(".//a")));
                        if (anchorElements == null)
                            continue;

                        foreach (IWebElement element in anchorElements)
                        {
                            string hrefValue = element.GetAttribute("href");
                            postLinks.Add(hrefValue);

                            if (postLinks.Count >= maxSearchResultCount)
                                break;
                        }

                        if (postLinks.Count >= maxSearchResultCount)
                            break;
                    }
                    catch
                    {
                        m_SearchFailCallback();
                        m_LogMessageCallback("Failed to get all post links");
                        return default;
                    }
                }
            }
            else
            {
                foreach (string link in channelLinks)
                {
                    m_Browser.Navigate().GoToUrl(link);

                    string[] paths = new string[2];
                    paths[0] = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div[1]/div[2]/div[2]/section/main/div/div[3]/article/div[1]";
                    if (link.Split('/').Length > 5)
                        paths[0] = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div[1]/div[2]/section/main/article/div/div";

                    paths[1] = "/html/body/div[2]/div/div/div[2]/div/div/div[1]/div[1]/div[2]/div[2]/section/main/div/div[2]/article/div";

                    foreach (string path in paths)
                    {
                        try
                        {
                            var searchLinksContainer = Wait(m_Browser, 10.0).Until(ElementExists(By.XPath(path)));
                            if (searchLinksContainer == null)
                                continue;

                            var anchorElements = Wait(m_Browser, 10.0).Until(ElementsExists(searchLinksContainer, By.XPath(".//a")));

                            if (anchorElements == null)
                                continue;

                            foreach (IWebElement element in anchorElements)
                            {
                                string hrefValue = element.GetAttribute("href");
                                postLinks.Add(hrefValue);

                                if (postLinks.Count >= maxSearchResultCount)
                                    break;
                            }

                            if (postLinks.Count >= maxSearchResultCount)
                                break;
                        }
                        catch
                        {
                            m_SearchFailCallback();
                            m_LogMessageCallback("Failed to get all post links");
                            return default;
                        }

                    }

                    if (postLinks.Count >= maxSearchResultCount)
                        break;
                }
            }


            // 7. Creating post data
            SendProgress(0.0f, "Creating post data");

            if (postLinks.Count == 0)
                return new SearchData(keyword, null);

            PostData[] posts = new PostData[postLinks.Count];
            for (int i = 0; i < posts.Length; i++)
            {
                string[] tags = new string[] { "Testing", "SimpleTag", "Debugging" };
                posts[i] = new PostData(postLinks[i], tags);
            }

            SearchData data = new SearchData(keyword, posts);
            return data;
        }

        //private async Task<SearchDataModel> SearchIntoDatabase(string keyword)
        //{
        //    try
        //    {
        //        var results = (await m_Collection.FindAsync(model => model.Data.Keyword == keyword)).ToList();
        //        return (results.Count == 0) ? null : results[0];
        //    }
        //    catch (Exception e)
        //    {
        //        m_SearchFailCallback();
        //        m_LogMessageCallback(e.Message);
        //        return null;
        //    }
        //}

        private async Task OnSearchAsync(string keyword, int maxSearchResultCount, bool forceWebSearch)
        {
            UnityData unityData;

            SendProgress(Utils.Remap01(0.0f, 0.0f, 5.0f), "Searching into web");
            SearchData searchData = await Task.Run(() => SearchIntoWeb(keyword, maxSearchResultCount));

            SendProgress(Utils.Remap01(2.0f, 0.0f, 5.0f), "Generating data for unity");
            unityData = GenerateUnityData(searchData);

            // 8. Generating posts
            SendProgress(Utils.Remap01(4.0f, 0.0f, 5.0f), "Saving session data");
            // Save current session data
            m_LogMessageCallback("Saving current session datas");
            SaveSessionData();
            m_LogMessageCallback("Current session data saved");

            SendProgress(Utils.Remap01(5.0f, 0.0f, 5.0f), "Search completed");
            // Notify complete signal
            m_SearchCompleteCallback(unityData);
        }
    }
}
