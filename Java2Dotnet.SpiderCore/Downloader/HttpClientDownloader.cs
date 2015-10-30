using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using Java2Dotnet.Spider.Core.Proxy;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core.Downloader
{
	/// <summary>
	/// The http downloader based on HttpClient.
	/// </summary>
	[Synchronization]
	public class HttpClientDownloader : BaseDownloader
	{
		public override Page Download(Request request, ITask task)
		{
			if (task.Site == null)
			{
				return null;
			}

			Site site = task.Site;

			ICollection<int> acceptStatCode;
			Encoding charset = null;
			IDictionary headers = null;
			if (site != null)
			{
				acceptStatCode = site.AcceptStatCode;
				charset = site.Encoding;
				headers = site.GetHeaders();
			}
			else
			{
				acceptStatCode = new HashSet<int> { 200 };
			}
			//Logger.InfoFormat("Downloading page {0}", request.Url);

			int statusCode = 0;

			HttpWebResponse response = null;
			try
			{
				HttpWebRequest httpWebRequest = GetHttpWebRequest(request, site, headers);
				response = (HttpWebResponse)httpWebRequest.GetResponse();
				statusCode = (int)response.StatusCode;
				request.PutExtra(Request.StatusCode, statusCode);
				if (StatusAccept(acceptStatCode, statusCode))
				{
					Page page = HandleResponse(request, charset, response, statusCode);

					//customer verify
					if (DownloadVerifyEvent != null)
					{
						string msg = "";
						if (!DownloadVerifyEvent(page, ref msg))
						{
							throw new SpiderExceptoin(msg);
						}
					}

					// 结束后要置空, 这个值存到Redis会导置无限循环跑单个任务
					request.PutExtra(Request.CycleTriedTimes, null);

					httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
					OnSuccess(request);
					return page;
				}
				else
				{
					throw new SpiderExceptoin("Download failed.");
				}
			}
			finally
			{
				request.PutExtra(Request.StatusCode, statusCode);
				try
				{
					//ensure the connection is released back to pool
					//check:
					//EntityUtils.consume(httpResponse.getEntity());
					response?.Close();
				}
				catch (Exception e)
				{
					Logger.Warn("Close response fail.", e);
				}
			}
		}

		private bool StatusAccept(ICollection<int> acceptStatCode, int statusCode)
		{
			return acceptStatCode.Contains(statusCode);
		}

		private HttpWebRequest GeneratorCookie(HttpWebRequest httpWebRequest, Site site)
		{
			CookieContainer cookie = new CookieContainer();
			foreach (var entry in site.GetAllCookies())
			{
				string domain = entry.Key;
				var cookies = entry.Value;
				Uri uri = new Uri(domain);

				foreach (var keypair in cookies)
				{
					cookie.Add(uri, new Cookie(keypair.Key, keypair.Value));
				}
			}
			httpWebRequest.CookieContainer = cookie;
			return httpWebRequest;
		}

		private HttpWebRequest GetHttpWebRequest(Request request, Site site, IDictionary headers)
		{
			if (site == null) return null;

			HttpWebRequest httpWebRequest = SelectRequestMethod(request);

			httpWebRequest.UserAgent = site.UserAgent ??
									   "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:39.0) Gecko/20100101 Firefox/39.0Mozilla/5.0 (Windows NT 10.0; WOW64; rv:39.0) Gecko/20100101 Firefox/39.0";

			if (site.IsUseGzip)
			{
				httpWebRequest.Headers.Add("Accept-Encoding", "gzip");
			}

			// headers
			if (headers != null)
			{
				var enumerator = headers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var key = enumerator.Key;
					var value = enumerator.Value;
					httpWebRequest.Headers.Add(key.ToString(), value.ToString());
				}
			}

			// cookie
			httpWebRequest = GeneratorCookie(httpWebRequest, site);

			//check:
			httpWebRequest.Timeout = site.Timeout;
			httpWebRequest.ContinueTimeout = site.Timeout;
			httpWebRequest.ReadWriteTimeout = site.Timeout;
			httpWebRequest.AllowAutoRedirect = true;

			if (headers != null)
			{
				foreach (DictionaryEntry entry in headers)
				{
					httpWebRequest.Headers.Add(entry.Key.ToString(), entry.Value.ToString());
				}
			}

			if (site.GetHttpProxyPool().Enable)
			{
				HttpHost host = site.GetHttpProxyFromPool();
				httpWebRequest.Proxy = new WebProxy(host.Host, host.Port);
				request.PutExtra(Request.Proxy, host);
			}

			return httpWebRequest;
		}

		private HttpWebRequest SelectRequestMethod(Request request)
		{
			HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(request.Url);
			if (request.Method == null || request.Method.ToUpper().Equals(HttpConstant.Method.Get))
			{
				//default get
				webrequest.Method = HttpConstant.Method.Get;
				return webrequest;
			}
			if (request.Method.ToUpper().Equals(HttpConstant.Method.Post))
			{
				webrequest.Method = HttpConstant.Method.Post;
				return webrequest;
			}
			if (request.Method.ToUpper().Equals(HttpConstant.Method.Head))
			{
				webrequest.Method = HttpConstant.Method.Head;
				return webrequest;
			}
			if (request.Method.ToUpper().Equals(HttpConstant.Method.Put))
			{
				webrequest.Method = HttpConstant.Method.Put;
				return webrequest;
			}
			if (request.Method.ToUpper().Equals(HttpConstant.Method.Delete))
			{
				webrequest.Method = HttpConstant.Method.Delete;
				return webrequest;
			}
			if (request.Method.ToUpper().Equals(HttpConstant.Method.Trace))
			{
				webrequest.Method = HttpConstant.Method.Trace;
				return webrequest;
			}
			throw new ArgumentException("Illegal HTTP Method " + request.Method);
		}

		private Page HandleResponse(Request request, Encoding charset, HttpWebResponse response, int statusCode)
		{
			string content = GetContent(charset, response);
			if (string.IsNullOrEmpty(content))
			{
				throw new SpiderExceptoin($"Download {request.Url} failed.");
			}
			content = HttpUtility.UrlDecode(HttpUtility.HtmlDecode(content), charset);
			Page page = new Page(request);
			page.SetRawText(content);
			page.TargetUrl = response.ResponseUri.ToString();
			page.SetUrl(new PlainText(request.Url));
			page.SetStatusCode(statusCode);
			return page;
		}

		private string GetContent(Encoding charset, HttpWebResponse response)
		{
			byte[] contentBytes = GetContentBytes(response);

			if (charset == null)
			{
				Encoding htmlCharset = GetHtmlCharset(response.ContentType, contentBytes);
				if (htmlCharset != null)
				{
					return htmlCharset.GetString(contentBytes);
				}

				return Encoding.Default.GetString(contentBytes);
			}
			return charset.GetString(contentBytes);
		}

		private byte[] GetContentBytes(HttpWebResponse response)
		{
			Stream stream = null;

			//GZIIP处理  
			if (response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
			{
				//开始读取流并设置编码方式
				var tempStream = response.GetResponseStream();
				if (tempStream != null) stream = new GZipStream(tempStream, CompressionMode.Decompress);
			}
			else
			{
				//开始读取流并设置编码方式  
				stream = response.GetResponseStream();
			}

			MemoryStream resultStream = new MemoryStream();
			if (stream != null)
			{
				stream.CopyTo(resultStream);
				return resultStream.StreamToBytes();
			}
			return null;
		}

		private Encoding GetHtmlCharset(string contentType, byte[] contentBytes)
		{
			// charset
			// 1、encoding in http header Content-Type
			string value = contentType;
			var encoding = UrlUtils.GetEncoding(value);
			if (encoding != null)
			{
				Logger.DebugFormat("Auto get charset: {0}", encoding);
				return encoding;
			}
			// use default charset to decode first time
			Encoding defaultCharset = Encoding.Default;
			string content = defaultCharset.GetString(contentBytes);
			string charset = null;
			// 2、charset in meta
			if (!string.IsNullOrEmpty(content))
			{
				HtmlDocument document = new HtmlDocument();
				document.LoadHtml(content);
				HtmlNodeCollection links = document.DocumentNode.SelectNodes("//meta");
				if (links != null)
				{
					foreach (var link in links)
					{
						// 2.1、html4.01 <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
						string metaContent = link.Attributes["content"] != null ? link.Attributes["content"].Value : "";
						string metaCharset = link.Attributes["charset"] != null ? link.Attributes["charset"].Value : "";
						if (metaContent.IndexOf("charset", StringComparison.Ordinal) != -1)
						{
							metaContent = metaContent.Substring(metaContent.IndexOf("charset", StringComparison.Ordinal), metaContent.Length - metaContent.IndexOf("charset", StringComparison.Ordinal));
							charset = metaContent.Split('=')[1];
							break;
						}
						// 2.2、html5 <meta charset="UTF-8" />
						if (!string.IsNullOrEmpty(metaCharset))
						{
							charset = metaCharset;
							break;
						}
					}
				}
			}
			Logger.DebugFormat("Auto get charset: {0}", charset);
			// 3、todo use tools as cpdetector for content decode
			try
			{
				return Encoding.GetEncoding(string.IsNullOrEmpty(charset) ? "UTF-8" : charset);
			}
			catch
			{
				return Encoding.Default;
			}
		}
	}
}