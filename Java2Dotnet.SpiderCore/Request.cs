using System;
using System.Collections.Generic;
using System.Web;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Object contains url to crawl. 
	/// It contains some additional information. 
	/// </summary>
	[Serializable]
	public class Request : IDisposable, ICloneable
	{
		public const string CycleTriedTimes = "_cycle_tried_times";
		public const string StatusCode = "statusCode";
		public const string Proxy = "proxy";

		private readonly string _url;

		public Request(string url, IDictionary<string, dynamic> extras)
		{
			_url = HttpUtility.HtmlDecode(url);
			Extras = extras;
		}

		/// <summary>
		/// Set the priority of request for sorting. 
		/// Need a scheduler supporting priority. 
		/// </summary>
		[Experimental]
		public long Priority { get; set; }

		public dynamic GetExtra(string key)
		{
			if (Extras == null)
			{
				return null;
			}

			if (Extras.ContainsKey(key))
			{
				return Extras[key];
			}
			return null;
		}

		public Request PutExtra(string key, dynamic value)
		{
			if (Extras == null)
			{
				Extras = new Dictionary<string, dynamic>();
			}

			if (Extras.ContainsKey(key))
			{
				Extras[key] = value;
			}
			else
			{
				Extras.Add(key, value);
			}

			return this;
		}

		/// <summary>
		/// Store additional information in extras.
		/// </summary>
		public IDictionary<string, dynamic> Extras { get; private set; }

		/// <summary>
		/// The http method of the request. Get for default.
		/// </summary>
		public string Method { get; set; }

		public string Url => _url;

		public override bool Equals(object o)
		{
			if (this == o) return true;
			if (o == null || GetType() != o.GetType()) return false;

			Request request = (Request)o;

			if (!Url.Equals(request.Url)) return false;

			return true;
		}

		public override int GetHashCode()
		{
			if (Url != null)
			{
				return Url.GetHashCode();
			}

			return -1;
		}

		public void Dispose()
		{
			Extras.Clear();
		}

		public override string ToString()
		{
			return "Request{" +
					"url='" + Url + '\'' +
					", method='" + Method + '\'' +
					", extras=" + Extras +
					", priority=" + Priority +
					'}';
		}

		public object Clone()
		{
			IDictionary<string, dynamic> extras = new Dictionary<string, dynamic>();
			foreach (var entry in Extras)
			{
				extras.Add(entry.Key, entry.Value);
			}
			Request newObj = new Request(Url, extras);
			return newObj;
		}
	}
}
