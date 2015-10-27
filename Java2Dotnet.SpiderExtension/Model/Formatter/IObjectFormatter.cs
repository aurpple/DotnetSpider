using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Model.Formatter
{
	public interface IObjectFormatter
	{
		IObjectFormatter NextFormatter { get; set; }
		dynamic Format(string raw);
		void InitParam(string[] extra);
	}
}
