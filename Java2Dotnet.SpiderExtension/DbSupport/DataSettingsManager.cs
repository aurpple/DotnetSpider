using System;
using System.Configuration;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public sealed class DataSettingsManager
	{
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
