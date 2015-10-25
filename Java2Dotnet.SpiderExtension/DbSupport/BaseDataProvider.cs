using System.Data;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public abstract class BaseDataProvider : IDataProvider
	{
		public abstract IDbConnection CreateConnection();

		public abstract IDbConnection CreateConnection(string connStr);

		public abstract string GetHost();
	}
}
