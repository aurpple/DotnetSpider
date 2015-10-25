namespace Java2Dotnet.Spider.Extension.Model.Formatter
{
	public interface IObjectFormatter
	{
		dynamic Format(string raw);
		void InitParam(string[] extra);
	}
}
