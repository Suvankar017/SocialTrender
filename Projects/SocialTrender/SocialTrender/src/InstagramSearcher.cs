using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;

namespace SocialTrender
{
    public class InstagramSearcher : Searcher
    {
        private const string c_WebsiteURL = "https://www.instagram.com/";
        private const string c_Username = "botintern2.0";
        private const string c_Password = "BotIntern@143";
        private const string c_IsLoggedIn = "isInstagramLoggedIn";

        public InstagramSearcher(IWebDriver browser, string userDataDirectoryPath, Action<string> onLogMessage) :
            base(browser, userDataDirectoryPath, onLogMessage)
        {
        }

        public override async Task Search(string keyword, int maxPostCount)
        {
            await Task.Run(() => OnSearch(keyword, maxPostCount));
        }

        private void OnSearch(string keyword, int maxPostCount)
        {
            // Load previous session data
            m_OnLogMessage?.Invoke("Loading saved datas");
            LoadSavedData();
            m_OnLogMessage?.Invoke("Loaded saved datas");

            // Open instagram
            m_OnLogMessage?.Invoke("Opening instagram");
            m_Browser.Navigate().GoToUrl(c_WebsiteURL);
            m_OnLogMessage?.Invoke("Opened instagram");

            // Login
            if (!m_IsLoggedIn)
            {
                m_OnLogMessage?.Invoke("Logging in");
                if (!Login(m_Browser))
                {
                    m_OnSearchFail?.Invoke();
                    m_OnLogMessage?.Invoke("Login failed");
                    return;
                }
                else
                {
                    m_OnLogMessage?.Invoke("Login successful");
                }

                // Remember Login Info Menu
                try
                {
                    Wait(m_Browser, 10.0).Until(ElementExists(By.XPath("//div[contains(text(), 'Not now')]")))?.Click();
                }
                catch
                {
                    m_OnSearchFail?.Invoke();
                    m_OnLogMessage?.Invoke("Remember login info menu 'Not now' button not found");
                    return;
                }

                m_IsLoggedIn = true;
            }
            else
            {
                m_OnLogMessage?.Invoke("Already logged in");
            }

            // Click on search button
            try
            {
                string xPath = "/html/body/div[1]/div/div/div[2]/div/div/div[1]/div[1]/div[1]/div/div/div/div/div[2]/div[2]";
                Wait(m_Browser, 10.0).Until(ElementExists(By.XPath(xPath)))?.Click();
            }
            catch
            {
                m_OnSearchFail?.Invoke();
                m_OnLogMessage?.Invoke("Search button not found");
                return;
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
                m_OnSearchFail?.Invoke();
                m_OnLogMessage?.Invoke("Search field not found");
                return;
            }

            // Get search results
            List<IWebElement> searchableElements = new List<IWebElement>();
            try
            {
                Thread.Sleep(5000);

                Wait(m_Browser, 10.0).Until((d) =>
                {
                    string xPath = "/html/body/div[1]/div/div/div[2]/div/div/div[1]/div[1]/div[1]/div/div/div[2]/div/div/div[2]/div/div/div[2]/div";

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
                m_OnSearchFail?.Invoke();
                m_OnLogMessage?.Invoke("Search results not found");
                return;
            }

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

                            if (postLinks.Count >= maxPostCount)
                                break;
                        }

                        if (postLinks.Count >= maxPostCount)
                            break;
                    }
                    catch
                    {
                        m_OnSearchFail?.Invoke();
                        m_OnLogMessage?.Invoke("Failed to get all post links");
                        return;
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

                                if (postLinks.Count >= maxPostCount)
                                    break;
                            }

                            if (postLinks.Count >= maxPostCount)
                                break;
                        }
                        catch
                        {
                            m_OnSearchFail?.Invoke();
                            m_OnLogMessage?.Invoke("Failed to get all post links");
                            return;
                        }

                    }

                    if (postLinks.Count >= maxPostCount)
                        break;
                }
            }

            // Get all post's snapshot
            List<byte[]>images = new List<byte[]>();
            for (int i = 0; i < postLinks.Count; i++)
            {
                m_Browser.Navigate().GoToUrl(postLinks[i]);
                images.Add(TakeScreenshot());
            }

            // Save current session data
            m_OnLogMessage?.Invoke("Saving current session datas");
            SaveSessionDatas();
            m_OnLogMessage?.Invoke("Current session data saved");

            // Notify complete signal
            m_OnSearchComplete?.Invoke(postLinks.ToArray(), images);
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

        private void SaveSessionDatas()
        {
            JObject json = new JObject();
            json[c_IsLoggedIn] = m_IsLoggedIn;

            string content = JsonConvert.SerializeObject(json, Formatting.Indented);
            File.WriteAllText(m_SaveFilePath, content);
        }

        protected bool Login(IWebDriver driver)
        {
            m_OnLogMessage?.Invoke("Trying to Login");

            try
            {
                var usernameField = Wait(driver, 10.0).Until(ElementExists(By.CssSelector("input[name='username']")));
                usernameField?.Clear();
                usernameField?.SendKeys(c_Username);

                m_OnLogMessage?.Invoke("Username field found");
            }
            catch
            {
                m_OnLogMessage?.Invoke("Username field not found");
                return false;
            }

            try
            {
                var passwordField = Wait(driver, 10.0).Until(ElementExists(By.CssSelector("input[name='password']")));
                passwordField?.Clear();
                passwordField?.SendKeys(c_Password);

                m_OnLogMessage?.Invoke("Password field found");
            }
            catch
            {
                m_OnLogMessage?.Invoke("Password field not found");
                return false;
            }

            try
            {
                Wait(driver, 10.0).Until(ElementExists(By.CssSelector("button[type='submit']")))?.Click();

                m_OnLogMessage?.Invoke("Submit button found");
            }
            catch
            {
                m_OnLogMessage?.Invoke("Submit button not found");
                return false;
            }

            return true;
        }
    }
}
