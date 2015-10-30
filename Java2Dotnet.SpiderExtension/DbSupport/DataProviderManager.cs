using System;
using System.Configuration;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public class DataProviderManager : IDataProviderManager
	{
		private string _dapperConnectString = "dotnetdb";
		public DataSettings Settings { get; set; }

		public DataProviderManager()
		{
			Settings = LoadSettings(_dapperConnectString);
		}

		public IDataProvider LoadDataProvider()
		{
			var providerName = Settings.DataProvider;
			if (string.IsNullOrEmpty(providerName))
				throw new ArgumentException("Data Settings doesn't contain a providerName");

			switch (providerName)
			{
				case "System.Data.SqlClient":
					return new SqlServerDataProvider(Settings);
				case "System.Data.MySqlClient":
					return new MySqlDataProvider(Settings);
				default:
					throw new ArgumentException($"Not supported dataprovider name: {providerName}");
			}
		}

		private DataSettings ParseSettings(ConnectionStringSettings connectionStringSettings)
		{
			return new DataSettings
			{
				DataProvider = connectionStringSettings.ProviderName,
				DataConnectionString = connectionStringSettings.ConnectionString
			};
		}

		/// <summary>
		/// Load settings
		/// </summary>
		/// <param name="key">File path; pass null to use default settings file path</param>
		/// <returns></returns>
		public DataSettings LoadSettings(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				// ReSharper disable once NotResolvedInText
				throw new ArgumentNullException("data settings key should not be null.");
			}
			if (ConfigurationManager.ConnectionStrings[key] != null)
			{
				return ParseSettings(ConfigurationManager.ConnectionStrings[key]);
			}
			else
			{
				throw new ArgumentNullException($"Didn't find connect string for key {key}.");
			}
		}
	}
}
