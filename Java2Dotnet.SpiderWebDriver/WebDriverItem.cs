using OpenQA.Selenium;

namespace Java2Dotnet.Spider.WebDriver
{
	public class WebDriverItem
	{
		public WebDriverItem(IWebDriver webDriver)
		{
			WebDriver = webDriver;
		}

		public IWebDriver WebDriver { get; }
		public bool IsLogined { get; set; }
	}
}
