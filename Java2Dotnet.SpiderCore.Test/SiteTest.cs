using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Java2Dotnet.Spider.Core.Test
{
    [TestClass]
    public class SiteTest
    {

        private const string wName = "WebSite";
        private const string wValue = "12580emall";
        private const string wDomain = "www.12580emall.com"; // 沪动商城
        private const string URL = @"http://www.12580emall.com/emall/mall/index.html";
        private Site Site = new Site() { Domain = wDomain, Encoding = Encoding.UTF8, Timeout = 3000,};

        [TestMethod]
        public void TestAddCookies()
        {
            Site.AddCookie(wName, wValue);
            Site.AddCookie(wDomain, wName, wValue + "Testing");
            Assert.IsNotNull(Site.GetCookies()[wName]);
            Assert.IsNotNull(Site.GetAllCookies()[wDomain]);
            Assert.IsNotNull(Site.GetAllCookies()[wDomain][wName]);
            Assert.AreEqual(wValue, Site.GetAllCookies()[wDomain][wName]);
        }
        
        [TestMethod]
        public void TestAddRequests()
        {
            Site.ClearStartRequests();
            Site.AddStartUrl(URL);
            Site.AddStartRequest(new Request(URL, 1, null));
            Assert.AreEqual(Site.Domain, wDomain);
            Assert.IsTrue(Site.GetStartRequests().Contains(new Request(URL, 1, null)));
        }

        [TestMethod]
        public void TestAddHeaders()
        {
            Site.AddHeader(wName, wValue);
            Assert.IsNotNull(Site.GetHeaders());
            Assert.IsTrue(Site.GetHeaders().Count > 0);
        }
    }
}
