using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
		public const string CycleTriedTimes = "983009ae-baee-467b-92cd-44188da2b021";
		public const string StatusCode = "02d71099-b897-49dd-a180-55345fe9abfc";
		public const string Proxy = "6f09c4d6-167a-4272-8208-8a59bebdfe33";

		private const string Deep = "deep";

		private readonly string _url;

		public Request(string url, int grade, IDictionary<string, dynamic> extras)
		{
			_url = HttpUtility.HtmlDecode(url);

			foreach (var extra in extras)
			{
				PutExtra(extra.Key, extra.Value);
			}

			PutExtra(Deep, grade);
		}

		public int NextDeep()
		{
			return GetExtra(Deep) + 1;
		}

		/// <summary>
		/// Set the priority of request for sorting. 
		/// Need a scheduler supporting priority. 
		/// </summary>
		[Experimental]
		public long Priority { get; set; }

		[MethodImpl(MethodImplOptions.Synchronized)]
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

		[MethodImpl(MethodImplOptions.Synchronized)]
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
		public Dictionary<string, dynamic> Extras { get; set; }

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

		[MethodImpl(MethodImplOptions.Synchronized)]
		public object Clone()
		{
			IDictionary<string, dynamic> extras = new Dictionary<string, dynamic>();
			foreach (var entry in Extras)
			{
				extras.Add(entry.Key, entry.Value);
			}
			Request newObj = new Request(Url, GetExtra(Deep), extras);
			return newObj;
		}
	}
}
