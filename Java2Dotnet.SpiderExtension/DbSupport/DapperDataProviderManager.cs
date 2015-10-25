using System;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public class DapperDataProviderManager : IDataProviderManager
	{
		private string _dapperConnectString = "dapperConnectString";
		public DataSettings Settings { get; set; }

		public DapperDataProviderManager()
		{
			var dataSettingsManager = new DataSettingsManager();
			Settings = dataSettingsManager.LoadSettings(_dapperConnectString);
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
	}
}
