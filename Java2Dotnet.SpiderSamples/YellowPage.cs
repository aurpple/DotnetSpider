//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using MicroOrm.Pocos.SqlGenerator.Attributes;
//using Java2Dotnet.Spider.Extension.Model;
//using Java2Dotnet.Spider.Extension.Data;
//using Java2Dotnet.Spider.Core;
//using Java2Dotnet.Spider.Extension.Model.Attribute;
//using Java2Dotnet.Spider.Extension.Pipeline;

//namespace Java2Dotnet.Spider.Products
//{
//	[TestClass]
//	public class YellowPage
//	{
//		[Scheme("yellowpage")]
//		[StoredAs("yellowurl_cat")]
//		[ExtractBy(Value = "//ul[@class='typical']/li", Type = ExtractBy.ExtracType.XPath, Multi = true)]
//		public class Category : BaseEntity
//		{
//			[StoredAs("name", StoredAs.ValueType.Varchar, false, 50)]
//			[ExtractBy(Value = "/li/a/@title", Type = ExtractBy.ExtracType.XPath)]
//			public string Name { get; set; }
//			[ExtractBy(Value = "/li/a/@href", Type = ExtractBy.ExtracType.XPath)]
//			[StoredAs("url", StoredAs.ValueType.Text)]
//			public string Url { get; set; }
//			[StoredAs("taskflag", StoredAs.ValueType.Char, false, 1)]
//			public char TaskFlag { get; set; }
//		}

//		[TestMethod]
//		public void YellowPage_Main()
//		{
//			// 必须指定Provider
//			DbProviderUtil.Provider = new DapperDataProviderManager().LoadDataProvider();
//			var site = new Site();
//			site.AddStartUrl("http://company.yellowurl.cn/");
//			site.SleepTime = 0;
//			OoSpider ooSpider = OoSpider.Create(site, typeof(Category));
//			ooSpider.AddPipeline(new PageModelCollectorPipeline(typeof(Category)));
//			ooSpider.Run();

//		}
//	}
//}
