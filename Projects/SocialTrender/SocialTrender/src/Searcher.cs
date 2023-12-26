using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SocialTrender
{
    public abstract class Searcher
    {
        protected IWebDriver m_Browser;
        protected bool m_IsLoggedIn;
        protected string m_SaveFilePath;
        protected string m_UserDataDirectoryPath;

        protected Action<UnityData> m_SearchCompleteCallback;
        protected Action m_SearchFailCallback;
        protected Action<string> m_LogMessageCallback;
        protected Action<ProgressData> m_ProgressCallback;

        private const string c_SaveFileName = "SaveData";

        public Searcher(IWebDriver browser, string userDataDirectoryPath)
        {
            m_Browser = browser;
            m_IsLoggedIn = false;
            m_UserDataDirectoryPath = userDataDirectoryPath;
            m_SaveFilePath = $"{userDataDirectoryPath}\\{c_SaveFileName}.txt";

            m_SearchCompleteCallback = (data) => { };
            m_SearchFailCallback = () => { };
            m_LogMessageCallback = (msg) => { };
            m_ProgressCallback = (data) => { };
        }

        public abstract Task SearchAsync(string keyword, int maxSearchResultCount, bool forceWebSearch);

        public void SetCallbacks(CallbackData data)
        {
            m_SearchCompleteCallback = data.SearchCompleteCallback ?? ((data) => { });
            m_SearchFailCallback = data.SearchFailCallback ?? (() => { });
            m_LogMessageCallback = data.LogMessageCallback ?? ((msg) => { });
            m_ProgressCallback = data.ProgressCallback ?? ((data) => { });
        }

        protected byte[] TakeScreenshot(int i)
        {
            string dir = Directory.GetCurrentDirectory() + "\\Output";

            Screenshot screenshot = ((ITakesScreenshot)m_Browser).GetScreenshot();
            Directory.CreateDirectory($"Desktop\\Output");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            screenshot.SaveAsFile($"{dir}\\{i}.png");

            return screenshot.AsByteArray;
        }


        protected static WebDriverWait Wait(IWebDriver driver, double timeoutInSeconds)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
        }

        protected static Func<IWebDriver, IWebElement> ElementExists(By by)
        {
            Func<IWebDriver, IWebElement> condition = (driver) =>
            {
                try
                {
                    var element = driver.FindElement(by);
                    return element;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (NoSuchWindowException)
                {
                    return null;
                }
            };

            return condition;
        }

        protected static Func<IWebDriver, IWebElement> ElementExists(IWebElement parent, By by)
        {
            Func<IWebDriver, IWebElement> condition = (driver) =>
            {
                try
                {
                    var element = parent.FindElement(by);
                    return element;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (NoSuchWindowException)
                {
                    return null;
                }
            };

            return condition;
        }

        protected static Func<IWebDriver, ReadOnlyCollection<IWebElement>> ElementsExists(By by)
        {
            Func<IWebDriver, ReadOnlyCollection<IWebElement>> condition = (driver) =>
            {
                try
                {
                    var element = driver.FindElements(by);
                    return element;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (NoSuchWindowException)
                {
                    return null;
                }
            };

            return condition;
        }

        protected static Func<IWebDriver, ReadOnlyCollection<IWebElement>> ElementsExists(IWebElement parent, By by)
        {
            Func<IWebDriver, ReadOnlyCollection<IWebElement>> condition = (driver) =>
            {
                try
                {
                    var element = parent.FindElements(by);
                    return element;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
                catch (NoSuchWindowException)
                {
                    return null;
                }
            };

            return condition;
        }
    }
}
