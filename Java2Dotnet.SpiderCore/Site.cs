using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core.Proxy;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Object contains setting for crawler.
	/// </summary>
	public class Site
	{
		private readonly Dictionary<string, string> _defaultCookies = new Dictionary<string, string>();
		private readonly Dictionary<string, Dictionary<string, string>> _cookies = new Dictionary<string, Dictionary<string, string>>();
		private readonly HashSet<Request> _startRequests = new HashSet<Request>();
		private readonly Hashtable _headers = new Hashtable();
		private ProxyPool _httpProxyPool = new ProxyPool();

		//public static Site NewSite()
		//{
		//	return new Site();
		//}

		/// <summary>
		/// Add a cookie with domain {@link #getDomain()}
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Site AddCookie(string name, string value)
		{
			if (_defaultCookies.ContainsKey(name))
			{
				_defaultCookies[name] = value;
			}
			else
			{
				_defaultCookies.Add(name, value);
			}
			return this;
		}

		/// <summary>
		/// Add a cookie with specific domain.
		/// </summary>
		/// <param name="domain"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Site AddCookie(string domain, string name, string value)
		{
			if (_cookies.ContainsKey(domain))
			{
				var cookie = _cookies[domain];
				if (cookie.ContainsKey(name))
				{
					cookie[name] = value;
				}
				else
				{
					cookie.Add(name, value);
				}
			}
			else
			{
				_cookies.Add(name, new Dictionary<string, string> { { name, value } });
			}
			return this;
		}

		/// <summary>
		/// User agent
		/// </summary>
		public string UserAgent { get; set; }

		/// <summary>
		/// Get cookies
		/// </summary>
		public IDictionary GetCookies()
		{
			return _defaultCookies;
		}

		/// <summary>
		/// Get cookies of all domains
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, Dictionary<string, string>> GetAllCookies()
		{
			return _cookies;
		}

		/// <summary>
		/// Set the domain of site.
		/// </summary>
		/// <returns></returns>
		public string Domain { get; set; }

		/// <summary>
		/// Set charset of page manually. 
		/// When charset is not set or set to null, it can be auto detected by Http header.
		/// </summary>
		public Encoding Encoding { get; set; } = Encoding.Default;

		/// <summary>
		/// Set or Get timeout for downloader in ms
		/// </summary>
		public int Timeout { get; set; } = 5000;

		/// <summary>
		/// Get or Set acceptStatCode. 
		/// When status code of http response is in acceptStatCodes, it will be processed. 
		/// {200} by default. 
		/// It is not necessarily to be set.
		/// </summary>
		public HashSet<int> AcceptStatCode { get; set; } = new HashSet<int> { 200 };

		/// <summary>
		/// Get start urls
		/// </summary>
		/// <returns></returns>
		// @Deprecated
		public IList<string> GetStartUrls()
		{
			return UrlUtils.ConvertToUrls(_startRequests);
		}

		public HashSet<Request> GetStartRequests()
		{
			return _startRequests;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ClearStartRequests()
		{
			//Request tmpTequest;
			//while (_startRequests.TryTake(out tmpTequest))
			//{
			//	tmpTequest.Dispose();
			//}
			//while (_startRequests.TryTake(out tmpTequest))
			//{
			//	tmpTequest.Dispose();
			//}
			_startRequests.Clear();
			GC.Collect();
		}

		/// <summary>
		/// Add a url to start url. 
		/// Because urls are more a Spider's property than Site, move it to {@link Spider#addUrl(string...)}}
		/// </summary>
		/// <param name="startUrl"></param>
		/// <returns></returns>
		public Site AddStartUrl(string startUrl)
		{
			return AddStartRequest(new Request(startUrl, 1, null));
		}

		/// <summary>
		/// Add a url to start url. 
		/// Because urls are more a Spider's property than Site, move it to {@link Spider#addUrl(string...)}}
		/// </summary>
		/// <param name="startUrl"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public Site AddStartUrl(string startUrl, IDictionary<string, object> data)
		{
			return AddStartRequest(new Request(startUrl, 1, data));
		}

		public Site AddStartUrls(IList<string> startUrls)
		{
			Parallel.ForEach(startUrls, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, url =>
			{
				AddStartUrl(url);
			});

			return this;
		}

		public Site AddStartUrls(IDictionary<string, IDictionary<string, object>> startUrls)
		{
			Parallel.ForEach(startUrls, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, url =>
			{
				AddStartUrl(url.Key, url.Value);
			});

			return this;
		}

		/// <summary>
		/// Add a url to start url. 
		/// Because urls are more a Spider's property than Site, move it to {@link Spider#addRequest(Request...)}}
		/// </summary>
		/// <param name="startRequest"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public Site AddStartRequest(Request startRequest)
		{
			_startRequests.Add(startRequest);
			if (Domain == null && startRequest.Url != null)
			{
				Domain = UrlUtils.GetDomain(startRequest.Url);
			}
			return this;
		}

		/// <summary>
		/// Set the interval between the processing of two pages. 
		/// Time unit is micro seconds. 
		/// </summary>
		public int SleepTime { get; set; } = 5000;

		/// <summary>
		/// Get or Set retry times immediately when download fail, 5 by default.
		/// </summary>
		/// <returns></returns>
		public int RetryTimes { get; set; } = 5;

		public IDictionary GetHeaders()
		{
			return _headers;
		}

		/// <summary>
		/// Put an Http header for downloader. 
		/// Use {@link #addCookie(string, string)} for cookie and {@link #setUserAgent(string)} for user-agent.
		/// </summary>
		public Site AddHeader(string key, string value)
		{
			if (_headers.Contains(key))
			{
				_headers[key] = value;
			}
			else
			{
				_headers.Add(key, value);
			}
			return this;
		}

		/// <summary>
		/// When cycleRetryTimes is more than 0, it will add back to scheduler and try download again. 
		/// </summary>
		public int CycleRetryTimes { get; set; } = 100000;

		/// <summary>
		/// Set or Get up httpProxy for this site
		/// </summary>
		public string HttpProxy { get; set; }

		/// <summary>
		/// Whether use gzip.  
		/// Default is true, you can set it to false to disable gzip.
		/// </summary>
		public bool IsUseGzip { get; set; }

		public ITask ToTask()
		{
			return new DefaultTask(Domain, this);
		}

		public override bool Equals(object o)
		{
			if (this == o) return true;
			if (o == null || GetType() != o.GetType()) return false;

			Site site = (Site)o;

			if (CycleRetryTimes != site.CycleRetryTimes) return false;
			if (RetryTimes != site.RetryTimes) return false;
			if (SleepTime != site.SleepTime) return false;
			if (Timeout != site.Timeout) return false;
			if (!AcceptStatCode?.Equals(site.AcceptStatCode) ?? site.AcceptStatCode != null)
				return false;
			if (!Encoding?.Equals(site.Encoding) ?? site.Encoding != null) return false;
			if (!_defaultCookies?.Equals(site._defaultCookies) ?? site._defaultCookies != null)
				return false;
			if (!Domain?.Equals(site.Domain) ?? site.Domain != null) return false;
			if (!_headers?.Equals(site._headers) ?? site._headers != null) return false;
			if (!_startRequests?.Equals(site._startRequests) ?? site._startRequests != null)
				return false;
			if (!UserAgent?.Equals(site.UserAgent) ?? site.UserAgent != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			int result = Domain?.GetHashCode() ?? 0;
			result = 31 * result + (UserAgent?.GetHashCode() ?? 0);
			result = 31 * result + (_defaultCookies?.GetHashCode() ?? 0);
			result = 31 * result + (Encoding?.GetHashCode() ?? 0);
			result = 31 * result + (_startRequests?.GetHashCode() ?? 0);
			result = 31 * result + SleepTime;
			result = 31 * result + RetryTimes;
			result = 31 * result + CycleRetryTimes;
			result = 31 * result + Timeout;
			result = 31 * result + (AcceptStatCode?.GetHashCode() ?? 0);
			result = 31 * result + (_headers?.GetHashCode() ?? 0);
			return result;
		}

		public override string ToString()
		{
			return "Site{" +
					"domain='" + Domain + '\'' +
					", userAgent='" + UserAgent + '\'' +
					", cookies=" + _defaultCookies +
					", charset='" + Encoding + '\'' +
					", startRequests=" + _startRequests +
					", sleepTime=" + SleepTime +
					", retryTimes=" + RetryTimes +
					", cycleRetryTimes=" + CycleRetryTimes +
					", timeOut=" + Timeout +
					", acceptStatCode=" + AcceptStatCode +
					", headers=" + _headers +
					'}';
		}

		/// <summary>
		/// Set httpProxyPool, string[0]:ip, string[1]:port
		/// </summary>
		/// <param name="httpProxyList"></param>
		/// <returns></returns>
		public Site SetHttpProxyPool(List<string[]> httpProxyList)
		{
			_httpProxyPool = new ProxyPool(httpProxyList);
			return this;
		}

		public ProxyPool GetHttpProxyPool()
		{
			return _httpProxyPool;
		}

		public HttpHost GetHttpProxyFromPool()
		{
			return _httpProxyPool.GetProxy();
		}

		public void ReturnHttpProxyToPool(HttpHost proxy, int statusCode)
		{
			_httpProxyPool.ReturnProxy(proxy, statusCode);
		}

		public Site SetProxyReuseInterval(int reuseInterval)
		{
			_httpProxyPool.SetReuseInterval(reuseInterval);
			return this;
		}
	}
}