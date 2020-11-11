using System;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Edge.SeleniumTools;
using System.Linq;

namespace SimpleDotNetCoreApp.Selenium
{
    public class SeleniumUITests : IDisposable
    {
        private static IWebDriver _webDriver = null;
        private static string _webAppBaseURL;
        private static string _defaultWebAppBaseURL = "https://localhost:5001/"; // A website kept running and not changed. Use to show working in VS, remove if prefer to show failure

        public SeleniumUITests()
        {
            InitWebDriver();

            //Set page load timeout to 20 seconds (occasionally 5 secs is too tight after a deployment)
            _webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);

            SetWebAppURL();
        }

        private static void SetWebAppURL()
        {
            try
            {
                // Get the URL for the current environment (e.g. Dev, QA, Prod) as set in the release environment
                string releaseEnvironmentAppBaseURL = Environment.GetEnvironmentVariable("WebAppName");
                if (releaseEnvironmentAppBaseURL != null)
                {
                    _webAppBaseURL = "https://" + releaseEnvironmentAppBaseURL + ".azurewebsites.net";
                    Console.WriteLine("WebApp Base URL found: " + _webAppBaseURL);
                }
                else
                {
                    // The environment variable exists but has no value, so use a default
                    _webAppBaseURL = _defaultWebAppBaseURL;
                    Console.WriteLine("WebApp Base URL not set, using default: " + _defaultWebAppBaseURL);
                }
            }
            catch (Exception Ex)
            {
                // The environment variable probably doesn't exist (might be called from within VS)
                Console.WriteLine("Exception thrown accessing environment variable: " + Ex.Message);
                Console.WriteLine("Using default: " + _defaultWebAppBaseURL);
                _webAppBaseURL = _defaultWebAppBaseURL;
            }
        }

        private static void InitWebDriver()
        {
            var browser = Environment.GetEnvironmentVariable("Browser");
            if (string.IsNullOrEmpty(browser)) browser = "edge";

            switch (browser.ToLower())
            {
                case "chrome" : 
                    _webDriver = new ChromeDriver(@"C:\tools\selenium");
                    break;
                default:        
                    _webDriver = new EdgeDriver(@"C:\tools\selenium");
                    break;
            }

        }

        public void Dispose()
        {
            if (_webDriver != null)
            {
                _webDriver.Quit();
            }
        }

        [Fact]
        public void HomePageFoundTest()
        {          
             _webDriver.Navigate().GoToUrl(_webAppBaseURL);
            string actualPageTitle = _webDriver.Title;
            string expectedPageTitle = "Home page - SimpleDotNetCoreApp";

            Assert.Equal(expectedPageTitle, actualPageTitle);
        }

        [Fact]
        public void ContactPageFoundTest()
        {
            _webDriver.Navigate().GoToUrl(_webAppBaseURL + "contact");

            string actualPageTitle = _webDriver.Title;
            string expectedPageTitle = "Contact - SimpleDotNetCoreApp";

            Assert.Equal(expectedPageTitle, actualPageTitle);
        }
        [Fact]
        public void SupportEmailAddressTest()
        {
            string supportEmailAddress = "Support@example.com";
            
             _webDriver.Navigate().GoToUrl(_webAppBaseURL+ "contact");
            var supportEmailElement = _webDriver.FindElement(By.PartialLinkText(supportEmailAddress));

            Assert.Equal(supportEmailAddress, supportEmailElement.Text);
        }

        [Fact]
        public void MarketingEmailAddressTest()
        {
            string marketingEmailAddress = "Marketing@example.com";

            _webDriver.Navigate().GoToUrl(_webAppBaseURL + "contact");
            var marketingEmailElement = _webDriver.FindElement(By.PartialLinkText(marketingEmailAddress));

            Assert.Equal(marketingEmailAddress, marketingEmailElement.Text);
        }

        [Fact]
         public void PrivacyPageFoundTest()
        {
            string privacyHeading = "Privacy Policy";
            
             _webDriver.Navigate().GoToUrl(_webAppBaseURL+ "Privacy");
            var privacyHeadingElement = _webDriver.FindElement(By.TagName("h1"));

            Assert.Equal(privacyHeading, privacyHeadingElement.Text);
        }
    }
}
