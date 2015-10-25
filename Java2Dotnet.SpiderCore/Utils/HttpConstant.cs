namespace Java2Dotnet.Spider.Core.Utils
{
	/// <summary>
	/// Some constants of Http protocal.
	/// </summary>
	public static class HttpConstant
	{
		public static class Method
		{
			public static readonly string Get = "GET";

			public static readonly string Head = "HEAD";

			public static readonly string Post = "POST";

			public static readonly string Put = "PUT";

			public static readonly string Delete = "DELETE";

			public static readonly string Trace = "TRACE";

			public static readonly string Connect = "CONNECT";
		}

		public static class Header
		{
			public static readonly string Referer = "Referer";
			public static readonly string UserAgent = "User-Agent";
		}
	}
}
