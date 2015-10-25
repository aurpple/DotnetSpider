using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Extension.Scheduler;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace Java2Dotnet.Spider.Extension.Monitor
{
	public class RedisStatusUpdater
	{
		private static RedisManagerPool _pool;
		private readonly ISpiderStatus _spiderStatus;
		private readonly Core.Spider _spider;
		private readonly string _password;

		public RedisStatusUpdater(Core.Spider spider, ISpiderStatus spiderStatus)
		{
			_spider = spider;
			_spiderStatus = spiderStatus;
			string host = ConfigurationManager.AppSettings["redishost"];
			_password = ConfigurationManager.AppSettings["redishostpass"];
			if (!string.IsNullOrEmpty(host))
			{
				_pool = new RedisManagerPool(host);
			}
		}

		public void Run()
		{
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					try
					{
						UpdateStatus();
					}
					catch (Exception)
					{
						// ignored
					}

					Thread.Sleep(1000);
				}
				// ReSharper disable once FunctionNeverReturns
			});
		}

		public void UpdateStatus()
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				object status = new
				{
					_spiderStatus.Name,
					_spiderStatus.ErrorPageCount,
					_spiderStatus.LeftPageCount,
					_spiderStatus.PagePerSecond,
					_spiderStatus.StartTime,
					_spiderStatus.EndTime,
					_spiderStatus.Status,
					_spiderStatus.SuccessPageCount,
					_spiderStatus.ThreadCount,
					_spiderStatus.TotalPageCount,
					_spiderStatus.AliveThreadCount
				};
				redis.SetEntryInHash(RedisScheduler.TaskStatus, _spider.Identify, JsonConvert.SerializeObject(status));
			}
		}
	}
}
