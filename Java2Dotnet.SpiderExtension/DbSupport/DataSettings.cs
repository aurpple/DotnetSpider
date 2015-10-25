namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public class DataSettings
	{
		public string DataProvider { get; set; }

		public string DataConnectionString { get; set; }

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(DataProvider) && !string.IsNullOrEmpty(DataConnectionString);
		}
	}
}
