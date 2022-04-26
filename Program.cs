using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSCase
{
    class Program
    {
        static void Main(string[] args)
        {
            //Creating webdriver instance.
            IWebDriver webDriver;

            //Configuring here chrome web driver object options.
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("headless");//We add headless parameter to driver. That is important to improving speed, performance, multitasking and testing.

            //Configuring here chrome web driver service settings.
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;//We hiding chrome web driver window because we work only console.

            //Creating new web driver object.
            webDriver = new ChromeDriver(chromeDriverService, chromeOptions);//We add service and option configurations to webdriver object parameters.
            webDriver.Navigate().GoToUrl("https://www.sahibinden.com/");

            var sectionLinks = webDriver.FindElements(By.XPath("//*[@id='container']/div[3]/div/div[3]/div[3]/ul/li/a"));//That is point to "Homepage showcase" section.

            //For the each listing we have to keep price value.
            double singleListingPrice = 0;

            //Created dictionary for storing title and price data.
            Dictionary<string, double> listingItems = new Dictionary<string, double>();

            Console.WriteLine("Data collection process started...");
            for (int i = 0; i < sectionLinks.Count; i++)
            {
                try
                {
                    //We get single listing link.
                    var singlelistingLink = webDriver.FindElement(By.XPath("//*[@id='container']/div[3]/div/div[3]/div[3]/ul/li[" + (i + 1) + "]/a")).GetAttribute("href");
                    
                    //We have to check listing links points to listing or advertisement.
                    if (singlelistingLink != "https://www.sahibinden.com/param-guvende/bireysel?widget_type=param-guvende-bireysel&widget_source=d-v-i" && singlelistingLink != "https://www.sahibinden.com/doping-tanitim/#doping-3")
                    {
                        webDriver.Navigate().GoToUrl(singlelistingLink);//Service is in the listing now.
                        var singlelistingTitle = webDriver.FindElement(By.XPath("//*[@id='classifiedDetail']/div/div[1]/h1")).Text;//Title data.

                        //Price data. We have to clean this data from currencies, blanks and dots.
                        var singlelistingPrice = webDriver.FindElement(By.XPath("//*[@id='favoriteClassifiedPrice']")).GetAttribute("value").Trim().Replace(".", "").Replace("TL", "").Replace("$", "").Replace("€", "").Replace("£", "").Replace("<sup>", "").Replace("</sup>", "").Replace(",", ".");

                        //Check to not add the same listing more than once.
                        if (!listingItems.ContainsKey(singlelistingTitle) || !listingItems.ContainsValue(Convert.ToDouble(singlelistingPrice)))
                        {
                            //Now we collected title and price data from listing. We have to store this values in our dictionary.
                            listingItems.Add(singlelistingTitle, Convert.ToDouble(singlelistingPrice));
                        }

                        //Each price data keeping in this variable.
                        singleListingPrice += Convert.ToDouble(singlelistingPrice);

                        //Loop end. Collected all we need. We have to go back to the homepage showcase.
                        webDriver.Navigate().GoToUrl("https://www.sahibinden.com/");
                    }
                }
                catch (NoSuchElementException ex)
                {
                    //Ignoring exceptions for work properly.
                }
            }
            Console.WriteLine("Data collection process finished.\n");

            //Creating stream writer object for create txt file.
            StreamWriter streamWriter = new StreamWriter("ss_case_output.txt");

            //Print dictionary.
            foreach (var item in listingItems)
            {
                Console.WriteLine("Listing title: " + item.Key + " - Price: " + item.Value);

                //Writing all listing data to txt file.
                streamWriter.WriteLine("Listing title: " + item.Key + " - Price: " + item.Value);
            }

            //Print prices average.
            Console.WriteLine("\nAverage price: " + Math.Round((singleListingPrice / (listingItems.Count)), 2));
            streamWriter.Close();
        }
    }
}
