using System;
using System.Data;
using log4net;
using MySql.Data.MySqlClient;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public class MySqlDataProvider : IDataProvider
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(MySqlDataProvider));
		private readonly string _host;
		private readonly DataSettings _settings;

		public MySqlDataProvider(DataSettings settings)
		{
			_settings = settings;
			_host = new MySqlConnection(settings.DataConnectionString).DataSource;
		}

		public IDbConnection CreateConnection()
		{
			try
			{
				MySqlConnection cnn = new MySqlConnection(_settings.DataConnectionString);
				cnn.Open();
				return cnn;
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				return null;
			}
		}

		public string GetHost()
		{
			return _host;
		}
	}
}
