using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Samples
{
	[ExtractBy(Value = "//*[@id=\"listofficial\"]/div[1]/div", Multi = true)]
	[Scheme("youku")]
	[StoredAs("video3")]
	public class VideoEntity
	{
		[KeyProperty]
		[StoredAs("id", StoredAs.ValueType.Int, true)]
		public int Id { get; set; }
		[StoredAs("Url", StoredAs.ValueType.Text)]
		[ExtractBy(Value = "/div[1]/div[1]/div[2]/a/@href")]
		public string Url { get; set; }
		[StoredAs("Name", StoredAs.ValueType.Varchar)]
		[ExtractBy(Value = "/div[1]/div[1]/div[4]/div[1]/a/@title")]
		public string Name { get; set; }
		[StoredAs("Count", StoredAs.ValueType.Varchar)]
		[ExtractBy(Value = "/div[1]/div[1]/div[4]/div[3]/span")]
		public string Count { get; set; }
	}
}
