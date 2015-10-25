using System.Data;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public static class DbProviderUtil
	{
		public static IDataProvider Provider { get; set; }

		public static IDbConnection GetProvider()
		{
			return Provider.CreateConnection();
		}
	}
}
