using System.Collections.Generic;
using System.Threading;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Scheduler.Component;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Lib;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace Java2Dotnet.Spider.Extension.Scheduler
{
	/// <summary>
	/// Use Redis as url scheduler for distributed crawlers.
	/// </summary>
	public class RedisScheduler : DuplicateRemovedScheduler, IMonitorableScheduler, IDuplicateRemover
	{
		private readonly RedisManagerPool _pool;
		private readonly string _password;
		public static readonly string QueuePrefix = "queue-";
		public static readonly string TaskStatus = "task-status";
		public static readonly string SetPrefix = "set-";
		public static readonly string TaskList = "task";
		public static readonly string ItemPrefix = "item-";

		public RedisScheduler(string host, string password)
			: this(new RedisManagerPool(new List<string> { host }, new RedisPoolConfig { MaxPoolSize = 100 }), password)
		{
		}

		public override void Init(ITask task)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				redis.AddItemToSortedSet(TaskList, task.Identify, DateTimeUtil.GetCurrentTimeStamp());
			}
		}

		private RedisScheduler(RedisManagerPool pool, string password)
		{
			_password = password;
			_pool = pool;
			DuplicateRemover = this;
		}

		public void ResetDuplicateCheck(ITask task)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				redis.Remove(GetSetKey(task));
			}
		}

		private string GetSetKey(ITask task)
		{
			return SetPrefix + Encrypt.Md5Encrypt(task.Identify);
		}

		private string GetQueueKey(ITask task)
		{
			return QueuePrefix + Encrypt.Md5Encrypt(task.Identify);
		}

		public bool IsDuplicate(Request request, ITask task)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				bool isDuplicate = redis.SetContainsItem(GetSetKey(task), request.Url);
				if (!isDuplicate)
				{
					redis.AddItemToSet(GetSetKey(task), request.Url);
				}
				return isDuplicate;
			}
		}

		protected override void PushWhenNoDuplicate(Request request, ITask task)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				redis.AddItemToSortedSet(GetQueueKey(task), request.Url);

				// 没有必要判断浪费性能了, 这里不可能为空。最少会有一个层级数据 Grade
				//if (request.Extras != null && request.Extras.Count > 0)
				//{
					string field = Encrypt.Md5Encrypt(request.Url);
					string value = JsonConvert.SerializeObject(request);

					redis.SetEntryInHash(ItemPrefix + task.Identify, field, value);
					var value1 = redis.GetValueFromHash(ItemPrefix + task.Identify, field);

					// 验证数据是否存入成功
					for (int i = 0; i < 10 && value1 != value; ++i)
					{
						redis.SetEntryInHash(ItemPrefix + task.Identify, field, value);
						value1 = redis.GetValueFromHash(ItemPrefix + task.Identify, field);
						Thread.Sleep(150);
					}
				//}
			}
		}

		public override Request Poll(ITask task)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				string url = redis.PopItemWithLowestScoreFromSortedSet(GetQueueKey(task));
				if (url == null)
				{
					return null;
				}

				string hashId = ItemPrefix + task.Identify;
				string field = Encrypt.Md5Encrypt(url);

				string json = null;
				//redis 有可能取数据失败
				for (int i = 0; i < 10 && string.IsNullOrEmpty(json = redis.GetValueFromHash(hashId, field)); ++i)
				{
					Thread.Sleep(150);
				}

				if (!string.IsNullOrEmpty(json))
				{
					return JsonConvert.DeserializeObject<Request>(json);
				}

				// 严格意义上说不会走到这里, 一定会有JSON数据,详情看Push方法
				// 是否应该设为1级？
				Request request = new Request(url, 1, null);
				return request;
			}
		}

		public int GetLeftRequestsCount(ITask task)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				long size = redis.GetSortedSetCount(GetQueueKey(task));
				return (int)size;
			}
		}

		public int GetTotalRequestsCount(ITask task)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				long size = redis.GetSetCount(GetSetKey(task));
				return (int)size;
			}
		}
	}
}