namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public interface IDataProviderManager
	{
		DataSettings Settings { get; set; }

		IDataProvider LoadDataProvider();
	}
}
