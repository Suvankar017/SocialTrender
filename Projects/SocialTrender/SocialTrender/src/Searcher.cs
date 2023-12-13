using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SocialTrender
{
    public abstract class Searcher
    {

        protected IWebDriver m_Browser;
        protected string m_SaveFilePath;
        protected string m_UserDataDirectoryPath;
        protected bool m_IsLoggedIn;

        protected Action<string[], List<byte[]>> m_OnSearchComplete;
        protected Action m_OnSearchFail;
        protected Action<string> m_OnLogMessage;

        private const string c_SaveFileName = "SaveData";

        public Searcher(IWebDriver browser, string userDataDirectoryPath, Action<string> onLogMessage)
        {
            m_Browser = browser;
            m_UserDataDirectoryPath = userDataDirectoryPath;
            m_SaveFilePath = $"{userDataDirectoryPath}\\{c_SaveFileName}.txt";
            m_IsLoggedIn = false;
            m_OnLogMessage = onLogMessage;
        }

        public abstract Task Search(string keyword, int maxPostCount);

        public void SetOnCompleteCallback(Action<string[], List<byte[]>> onSearchComplete)
        {
            m_OnSearchComplete = onSearchComplete;
        }

        public void SetOnFailCallback(Action onSearchFail)
        {
            m_OnSearchFail = onSearchFail;
        }

        protected byte[] TakeScreenshot()
        {
            Screenshot screenshot = ((ITakesScreenshot)m_Browser).GetScreenshot();
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
