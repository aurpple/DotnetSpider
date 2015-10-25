using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;

namespace Java2Dotnet.Spider.Extension.Model
{
	public class BaseEntity
	{
		[StoredAs("id", StoredAs.ValueType.Long, true)]
		[KeyProperty(Identity = true)]
		public long Id { get; set; }
	}
}
