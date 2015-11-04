using System.Data;
using System.Data.SqlClient;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public class SqlServerDataProvider : IDataProvider
	{
		private readonly DataSettings _dataSettings;
		private readonly string _host;

		public SqlServerDataProvider(DataSettings dataSettings)
		{
			_dataSettings = dataSettings;
			_host = new SqlConnection(dataSettings.DataConnectionString).DataSource;
		}

		public IDbConnection CreateConnection()
		{
			try
			{
				SqlConnection cnn = new SqlConnection(_dataSettings.DataConnectionString);
				cnn.Open();
				return cnn;
			}
			catch
			{
				return null;
			}
		}

		public string GetHost()
		{
			return _host;
		}
	}
}
