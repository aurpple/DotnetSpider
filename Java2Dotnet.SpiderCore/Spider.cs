using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Core.Processor;
using Java2Dotnet.Spider.Core.Proxy;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Utils;
using log4net;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Entrance of a crawler. 
	/// A spider contains four modules: Downloader, Scheduler, PageProcessor and
	/// Pipeline. 
	/// Every module is a field of Spider.  
	/// The modules are defined in interface.  
	/// You can customize a spider with various implementations of them.  
	/// Examples:  
	/// A simple crawler:  
	/// Spider.create(new SimplePageProcessor("http://my.oschina.net/",
	/// "http://my.oschina.net/*blog/*")).run(); 
	/// Store results to files by FilePipeline:  
	/// Spider.create(new SimplePageProcessor("http://my.oschina.net/",
	/// "http://my.oschina.net/*blog/*"))  
	/// .pipeline(new FilePipeline("/data/temp/webmagic/")).run();  
	/// Use FileCacheQueueScheduler to store urls and cursor in files, so that a
	/// Spider can resume the status when shutdown.  
	/// Spider.create(new SimplePageProcessor("http://my.oschina.net/",
	/// "http://my.oschina.net/*blog"))  
	/// .scheduler(new FileCacheQueueScheduler("/data/temp/webmagic/cache/")).run();  
	/// </summary>
	public class Spider : ITask
	{
		[DllImport("User32.dll ", EntryPoint = "FindWindow")]
		private static extern int FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll ", EntryPoint = "GetSystemMenu")]
		private extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
		[DllImport("user32.dll ", EntryPoint = "RemoveMenu")]
		private extern static int RemoveMenu(IntPtr hMenu, int nPos, int flags);

		protected IDownloader Downloader { get; set; }
		protected IList<IPipeline> Pipelines { get; set; } = new List<IPipeline>();
		protected IPageProcessor PageProcessor { get; set; }
		protected HashSet<Request> StartRequests { get; set; }
		protected IScheduler Scheduler { get; set; } = new QueueScheduler();
		public int ThreadNum { get; set; } = 1;
		protected static ILog Logger;

		protected readonly static int StatInit = 0;
		protected readonly static int StatRunning = 1;
		protected readonly static int StatStopped = 2;
		protected static readonly int StatFinished = 3;

		protected readonly AutomicLong Stat = new AutomicLong(StatInit);

		protected bool ExitWhenComplete { get; set; } = true;
		protected bool SpawnUrl { get; set; } = true;
		protected bool DestroyWhenExit { get; set; } = true;
		protected CountableThreadPool ThreadPool { get; set; }

		private bool _registConsoleCtrlHandler;
		private IList<ISpiderListener> _spiderListeners;
		private readonly AutomicLong _pageCount = new AutomicLong(0);
		private DateTime _startTime = DateTime.MinValue;
		private DateTime _endTime = DateTime.MinValue;
		private int _waitCountLimit = 20;
		private int _waitCount;
		private string _identify;
		private readonly Site _site;

		/// <summary>
		/// Create a spider with pageProcessor.
		/// </summary>
		/// <param name="pageProcessor"></param>
		/// <returns></returns>
		public static Spider Create(IPageProcessor pageProcessor)
		{
			return new Spider(null, pageProcessor);
		}

		/// <summary>
		/// Create a spider with pageProcessor.
		/// </summary>
		/// <param name="identify"></param>
		/// <param name="pageProcessor"></param>
		/// <returns></returns>
		public static Spider Create(string identify, IPageProcessor pageProcessor)
		{
			return new Spider(identify, pageProcessor);
		}

		/// <summary>
		/// Create a spider with pageProcessor.
		/// </summary>
		/// <param name="identify"></param>
		/// <param name="pageProcessor"></param>
		protected Spider(string identify, IPageProcessor pageProcessor)
		{
			Logger = LogManager.GetLogger(typeof(Spider));
			_waitCount = 0;
			PageProcessor = pageProcessor;
			_site = pageProcessor.Site;
			StartRequests = pageProcessor.Site.GetStartRequests();
			_identify = identify;
		}

		/// <summary>
		/// Set startUrls of Spider. 
		/// Prior to startUrls of Site.
		/// </summary>
		/// <param name="startUrls"></param>
		/// <returns></returns>
		public Spider StartUrls(IList<string> startUrls)
		{
			CheckIfRunning();
			StartRequests = new HashSet<Request>(UrlUtils.ConvertToRequests(startUrls));
			return this;
		}

		/// <summary>
		/// Set startUrls of Spider. 
		/// Prior to startUrls of Site.
		/// </summary>
		/// <param name="startRequests"></param>
		/// <returns></returns>
		public Spider StartRequest(IList<Request> startRequests)
		{
			CheckIfRunning();
			StartRequests = new HashSet<Request>(startRequests);
			return this;
		}

		public string Identify
		{
			get
			{
				if (_identify != null)
				{
					return _identify;
				}
				if (_site != null)
				{
					return _site.Domain;
				}
				_identify = Guid.NewGuid().ToString();
				return _identify;
			}
		}

		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public bool ShowControl { get; set; }

		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public bool SaveStatusInRedis { get; set; }

		/// <summary>
		/// Set scheduler for Spider
		/// </summary>
		/// <param name="scheduler"></param>
		/// <returns></returns>
		public Spider SetScheduler(IScheduler scheduler)
		{
			CheckIfRunning();
			IScheduler oldScheduler = Scheduler;
			Scheduler = scheduler;
			if (oldScheduler != null)
			{
				Request request;
				while ((request = oldScheduler.Poll(this)) != null)
				{
					Scheduler.Push(request, this);
				}
			}
			return this;
		}

		/// <summary>
		/// Add a pipeline for Spider
		/// </summary>
		/// <param name="pipeline"></param>
		/// <returns></returns>
		public Spider AddPipeline(IPipeline pipeline)
		{
			CheckIfRunning();
			Pipelines.Add(pipeline);
			return this;
		}

		/// <summary>
		/// Set pipelines for Spider
		/// </summary>
		/// <param name="pipelines"></param>
		/// <returns></returns>
		public Spider SetPipelines(List<IPipeline> pipelines)
		{
			CheckIfRunning();
			Pipelines = pipelines;
			return this;
		}

		/// <summary>
		/// Clear the pipelines set
		/// </summary>
		/// <returns></returns>
		public Spider ClearPipeline()
		{
			Pipelines = new List<IPipeline>();
			return this;
		}

		/// <summary>
		/// Set the downloader of spider
		/// </summary>
		/// <param name="downloader"></param>
		/// <returns></returns>
		public Spider SetDownloader(IDownloader downloader)
		{
			CheckIfRunning();
			Downloader = downloader;
			return this;
		}

		protected void InitComponent()
		{
			Scheduler.Init(this);

			if (Downloader == null)
			{
				Downloader = new HttpClientDownloader();
			}

			Downloader.SetThreadNum(ThreadNum);

			if (Pipelines.Count == 0)
			{
				Pipelines.Add(new FilePipeline());
			}
			if (ThreadPool == null || ThreadPool.IsShutdown)
			{
				ThreadPool = new CountableThreadPool(ThreadNum);
			}
			if (StartRequests != null)
			{
				if (StartRequests.Count > 0)
				{
					Parallel.ForEach(StartRequests, new ParallelOptions() { MaxDegreeOfParallelism = 100 }, request =>
					{
						Scheduler.Push((Request)request.Clone(), this);
					});

					ClearStartRequests();
					Logger.InfoFormat("Push Request to Scheduler success.");
				}

				Logger.InfoFormat("Push Zero Request to Scheduler.");
			}

			if (!_registConsoleCtrlHandler)
			{
				Console.Title = Identify;
				Console.CancelKeyPress += Console_CancelKeyPress;
				_registConsoleCtrlHandler = true;

				//根据控制台标题找控制台
				int windowHandler = FindWindow(null, Identify);
				//找关闭按钮
				IntPtr closeMenu = GetSystemMenu((IntPtr)windowHandler, IntPtr.Zero);
				int SC_CLOSE = 0xF060;
				//关闭按钮禁用
				RemoveMenu(closeMenu, SC_CLOSE, 0x0);
			}
		}

		private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Stop();
			while (Stat.Value != StatFinished)
			{
				Thread.Sleep(1500);
			}
		}

		public void Run()
		{
			//Stopwatch watch = new Stopwatch();
			//watch.Start();

			// 必须开启多线程限制
			System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;

			CheckRunningStat();

			Logger.Info("Spider " + Identify + " InitComponent...");
			InitComponent();

			//IMonitorableScheduler monitor = (IMonitorableScheduler)Scheduler;

			Logger.Info("Spider " + Identify + " Started!");

			bool firstTask = false;
			while (Stat.Value == StatRunning)
			{
				Request request = Scheduler.Poll(this);

				if (request == null)
				{
					if (ThreadPool.GetThreadAlive() == 0 && ExitWhenComplete)
					{
						break;
					}

					if (_waitCount > _waitCountLimit)
					{
						break;
					}

					// wait until new url added
					WaitNewUrl();
				}
				else
				{
					if (_startTime == DateTime.MinValue)
					{
						_startTime = DateTime.Now;
					}

					_waitCount = 0;

					ThreadPool.Execute((obj, cts) =>
					{
						//Logger.Info(
						//	$"Left: {monitor.GetLeftRequestsCount(this)} Total: {monitor.GetTotalRequestsCount(this)} AliveThread: {ThreadPool.GetThreadAlive()} ThreadNum: {ThreadPool.GetThreadNum()}");
						var request1 = obj as Request;
						if (request1 != null)
						{
							try
							{
								ProcessRequest(request1, cts);
								OnSuccess(request1);
								Uri uri = new Uri(request1.Url);
								Logger.Info($"Request: { HttpUtility.HtmlDecode(HttpUtility.UrlDecode(uri.Query))} Sucess.");
								return 1;
							}
							catch (Exception e)
							{
								OnError(request1);
								Logger.Error("Request " + request1.Url + " failed.", e);
								return -1;
							}
							finally
							{
								if (_site.GetHttpProxyPool().Enable)
								{
									_site.ReturnHttpProxyToPool((HttpHost)request1.GetExtra(Request.Proxy), (int)request1.GetExtra(Request.StatusCode));
								}
								_pageCount.Inc();
							}
						}

						return 0;
					}, request);

					if (!firstTask)
					{
						Thread.Sleep(3000);
						firstTask = true;
					}
				}
			}

			ThreadPool.WaitToEnd();

			// release some resources
			if (DestroyWhenExit)
			{
				Close();
			}

			_endTime = DateTime.Now;

			OnClose();

			//watch.Stop();

			//Logger.Info("Cost time:" + (float)watch.ElapsedMilliseconds / 1000);
			Stat.Set(StatFinished);
		}

		protected void OnClose()
		{
			if (_spiderListeners != null && _spiderListeners.Count > 0)
			{
				foreach (ISpiderListener spiderListener in _spiderListeners)
				{
					spiderListener.OnClose();
				}
			}
		}

		protected void OnError(Request request)
		{
			if (_spiderListeners != null && _spiderListeners.Count > 0)
			{
				foreach (ISpiderListener spiderListener in _spiderListeners)
				{
					spiderListener.OnError(request);
				}
			}
		}

		protected void OnSuccess(Request request)
		{
			if (_spiderListeners != null && _spiderListeners.Count > 0)
			{
				foreach (ISpiderListener spiderListener in _spiderListeners)
				{
					spiderListener.OnSuccess(request);
				}
			}
		}

		private void CheckRunningStat()
		{
			while (true)
			{
				long statNow = Stat.Value;
				if (statNow == StatRunning)
				{
					throw new SpiderExceptoin("Spider is already running!");
				}
				if (Stat.CompareAndSet(statNow, StatRunning))
				{
					break;
				}
			}
		}

		public void Close()
		{
			DestroyEach(Downloader);
			DestroyEach(PageProcessor);
			foreach (IPipeline pipeline in Pipelines)
			{
				DestroyEach(pipeline);
			}
			ThreadPool.Shutdown();
		}

		private void DestroyEach(object obj)
		{
			var disposable = obj as IDisposable;
			if (disposable != null)
			{
				try
				{
					disposable.Dispose();
				}
				catch (Exception e)
				{
					Logger.Warn(e);
				}
			}
		}

		/// <summary>
		/// Process specific urls without url discovering.
		/// </summary>
		/// <param name="urls"></param>
		public void Test(params string[] urls)
		{
			InitComponent();
			if (urls.Length > 0)
			{
				foreach (string url in urls)
				{
					ProcessRequest(new Request(url, null));
				}
			}
		}

		protected void ProcessRequest(Request request, CancellationTokenSource cts = null)
		{
			cts?.Cancel();

			//Stopwatch watch = new Stopwatch();
			//watch.Start();

			// 下载页面
			Page page = Downloader.Download(request, this);

			//watch.Stop();
			//Logger.Info("dowloader cost time:" + watch.ElapsedMilliseconds);

			cts?.Cancel();
			if (page == null)
			{
				Sleep(_site.SleepTime);
				OnError(request);
				return;
			}
			// for cycle retry, 这个下载出错时, 会把自身Request扔回TargetUrls中做重复任务。所以此时，targetRequests只有本身
			// 而不需要考虑 MissTargetUrls的情况
			if (page.IsNeedCycleRetry())
			{
				ExtractAndAddRequests(page, true);
				Sleep(_site.SleepTime);
				return;
			}

			//watch = new Stopwatch();
			//watch.Start();

			// 解析页面数据
			PageProcessor.Process(page);

			//watch.Stop();
			//Logger.Info("process cost time:" + watch.ElapsedMilliseconds);

			cts?.Cancel();

			if (page.MissTargetUrls)
			{
				Logger.Info($"Stoper trigger worked on this page.");
				ExtractAndAddRequests(page, SpawnUrl);
			}

			cts?.Cancel();

			//watch = new Stopwatch();
			//watch.Start();

			if (!page.GetResultItems().IsSkip)
			{
				foreach (IPipeline pipeline in Pipelines)
				{
					pipeline.Process(page.GetResultItems(), this);
				}
				//cts?.Cancel();
			}
			else
			{
				Logger.Warn($"Request {request.Url} 's result count is zero.");
			}

			//watch.Stop();
			//Logger.Info("pipeline cost time:" + watch.ElapsedMilliseconds);

			Sleep(_site.SleepTime);
		}

		protected void Sleep(int time)
		{
			Thread.Sleep(time);
		}

		protected void ExtractAndAddRequests(Page page, bool spawnUrl)
		{
			if (spawnUrl && page.GetTargetRequests() != null && page.GetTargetRequests().Count > 0)
			{
				foreach (Request request in page.GetTargetRequests())
				{
					AddRequest(request);
				}
			}
		}

		private void AddRequest(Request request)
		{
			if (_site.Domain == null && request?.Url != null)
			{
				_site.Domain = UrlUtils.GetDomain(request.Url);
			}
			Scheduler.Push(request, this);
		}

		protected void CheckIfRunning()
		{
			if (Stat.Value == StatRunning)
			{
				throw new SpiderExceptoin("Spider is already running!");
			}
		}

		public void RunAsync()
		{
			Task.Factory.StartNew(Run).ContinueWith(t =>
			{
				if (t.Exception != null)
				{
					Logger.Error(t.Exception);
				}
			});
		}

		/// <summary>
		/// Add urls to crawl.
		/// </summary>
		/// <param name="urls"></param>
		/// <returns></returns>
		public Spider AddUrl(params string[] urls)
		{
			foreach (string url in urls)
			{
				AddRequest(new Request(url, null));
			}
			return this;
		}

		public Spider AddUrl(ICollection<string> urls)
		{
			foreach (string url in urls)
			{
				AddRequest(new Request(url, null));
			}
			return this;
		}

		/// <summary>
		/// Download urls synchronizing.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="urls"></param>
		/// <returns></returns>
		public IList<T> GetAll<T>(params string[] urls)
		{
			DestroyWhenExit = false;
			SpawnUrl = false;

			foreach (Request request in UrlUtils.ConvertToRequests(urls))
			{
				AddRequest(request);
			}
			ICollectorPipeline collectorPipeline = GetCollectorPipeline();
			Pipelines.Add(collectorPipeline);
			Run();
			SpawnUrl = true;
			DestroyWhenExit = true;
			ICollection collection = collectorPipeline.GetCollected();
			return (from object o in collection select (T)o).ToList();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void ClearStartRequests()
		{
			//Request tmpTequest;
			//while (StartRequests.TryTake(out tmpTequest))
			//{
			//	tmpTequest.Dispose();
			//}
			StartRequests.Clear();
			GC.Collect();
		}

		protected virtual ICollectorPipeline GetCollectorPipeline()
		{
			return new ResultItemsCollectorPipeline();
		}

		public T Get<T>(string url)
		{
			IList<T> resultItemses = GetAll<T>(url);
			if (resultItemses != null && resultItemses.Count > 0)
			{
				return resultItemses[0];
			}
			return default(T);
		}

		/// <summary>
		/// Add urls with information to crawl.
		/// </summary>
		/// <param name="requests"></param>
		/// <returns></returns>
		public Spider AddRequest(params Request[] requests)
		{
			foreach (Request request in requests)
			{
				AddRequest(request);
			}
			return this;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void WaitNewUrl()
		{
			//double check
			//if (ThreadPool.GetThreadAlive() == 0 && ExitWhenComplete)
			//{
			//	return;
			//}

			Thread.Sleep(1000);
			++_waitCount;
		}

		public void Start()
		{
			RunAsync();
		}

		public void Stop()
		{
			if (Stat.CompareAndSet(StatRunning, StatStopped))
			{
				Logger.Info("Spider " + Identify + " stop success!");
			}
			else
			{
				Logger.Info("Spider " + Identify + " stop fail!");
			}
		}

		/// <summary>
		/// Start with more than one threads
		/// </summary>
		/// <param name="threadNum"></param>
		/// <returns></returns>
		public virtual Spider SetThreadNum(int threadNum)
		{
			CheckIfRunning();
			ThreadNum = threadNum;
			if (threadNum <= 0)
			{
				throw new ArgumentException("threadNum should be more than one!");
			}
			return this;
		}

		//check: looks useless
		///**
		// * start with more than one threads
		// *
		// * @param threadNum
		// * @return this
		// 
		//public Spider thread(ExecutorService executorService, int threadNum)
		//{
		//	checkIfRunning();
		//	this.threadNum = threadNum;
		//	if (threadNum <= 0)
		//	{
		//		throw new ArgumentException("threadNum should be more than one!");
		//	}
		//	return this;
		//}

		public bool IsExitWhenComplete()
		{
			return ExitWhenComplete;
		}

		/// <summary>
		/// Exit when complete.  
		/// True: exit when all url of the site is downloaded. 
		/// False: not exit until call stop() manually. 
		/// </summary>
		/// <param name="exitWhenComplete"></param>
		/// <returns></returns>
		public Spider SetExitWhenComplete(bool exitWhenComplete)
		{
			ExitWhenComplete = exitWhenComplete;
			return this;
		}

		public bool IsSpawnUrl()
		{
			return SpawnUrl;
		}

		/// <summary>
		/// Get page count downloaded by spider.
		/// </summary>
		/// <returns></returns>
		public long GetPageCount()
		{
			return _pageCount.Value;
		}

		/// <summary>
		/// Get running status by spider.
		/// </summary>
		/// <returns></returns>
		public Status GetStatus()
		{
			return StatusFromValue((int)Stat.Value);
		}

		public enum Status
		{
			Init = 0, Running = 1, Stopped = 2, Finished = 3
		}

		public Status StatusFromValue(int value)
		{
			return Enum.GetValues(typeof(Status)).Cast<Status>().FirstOrDefault(status => (int)status == value);
		}

		/// <summary>
		/// Get thread count which is running
		/// </summary>
		/// <returns></returns>
		public int GetThreadAliveCount()
		{
			if (ThreadPool == null)
			{
				return 0;
			}
			return ThreadPool.GetThreadAlive();
		}

		/// <summary>
		/// Whether add urls extracted to download. 
		/// Add urls to download when it is true, and just download seed urls when it is false.  
		/// DO NOT set it unless you know what it means!
		/// </summary>
		/// <param name="spawnUrl"></param>
		/// <returns></returns>
		public Spider SetSpawnUrl(bool spawnUrl)
		{
			SpawnUrl = spawnUrl;
			return this;
		}

		public Site Site => _site;

		public IList<ISpiderListener> GetSpiderListeners()
		{
			return _spiderListeners;
		}

		public Spider SetSpiderListeners(IList<ISpiderListener> spiderListeners)
		{
			_spiderListeners = spiderListeners;
			return this;
		}

		public DateTime GetStartTime()
		{
			return _startTime;
		}

		public DateTime GetEndOrCurrentTime()
		{
			return _endTime == DateTime.MinValue ? DateTime.Now : _endTime;
		}

		public IScheduler GetScheduler()
		{
			return Scheduler;
		}

		/// <summary>
		/// Set wait time when no url is polled.
		/// </summary>
		/// <param name="emptySleepTime"></param>
		public void SetEmptySleepTime(int emptySleepTime)
		{
			if (emptySleepTime > 10000)
			{
				_waitCountLimit = emptySleepTime / 1000;
			}
			else
			{
				throw new SpiderExceptoin("Sleep time should be large than 10000.");
			}
		}
	}
}