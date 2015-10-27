using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Model.Formatter
{
	public sealed class ReplaceFormatter : CustomizeFormatter
	{
		protected override dynamic FormatTrimmed(string raw)
		{
			if (Extra == null || Extra.Length != 2)
			{
				throw new SpiderExceptoin("ReplaceFormatter need 2 parameters.");
			}

			return raw.Replace(Extra[0], Extra[1]);
		}
	}
}
