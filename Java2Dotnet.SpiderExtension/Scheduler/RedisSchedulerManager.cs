using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Utils;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace Java2Dotnet.Spider.Extension.Scheduler
{
	public class RedisSchedulerManager : ISchedulerManager
	{
		private readonly RedisManagerPool _pool;
		private readonly string _password;

		public RedisSchedulerManager(string host, string password)
		{
			_pool = new RedisManagerPool(host);
			_password = password;
		}

		public IDictionary<string, double> GetTaskList(int startIndex, int count)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				return redis.GetRangeWithScoresFromSortedSetDesc(RedisScheduler.TaskList, startIndex, startIndex + count);
			}
		}

		public void RemoveTask(string taskIdentify)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				string json = redis?.GetValueFromHash(RedisScheduler.TaskStatus, taskIdentify);
				if (!string.IsNullOrEmpty(json))
				{
					redis.Remove(GetQueueKey(taskIdentify));
					redis.Remove(GetSetKey(taskIdentify));
					redis.RemoveEntryFromHash(RedisScheduler.TaskStatus, taskIdentify);
					redis.RemoveEntryFromHash(RedisScheduler.ItemPrefix + taskIdentify, taskIdentify);

					redis.RemoveItemFromSortedSet(RedisScheduler.TaskList, taskIdentify);
				}
			}
		}

		private string GetSetKey(string identify)
		{
			return RedisScheduler.SetPrefix + Encrypt.Md5Encrypt(identify);
		}

		private string GetQueueKey(string identify)
		{
			return RedisScheduler.QueuePrefix + Encrypt.Md5Encrypt(identify);
		}

		public SpiderStatus GetTaskStatus(string taskIdentify)
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				string json = redis?.GetValueFromHash(RedisScheduler.TaskStatus, taskIdentify);
				if (!string.IsNullOrEmpty(json))
				{
					return JsonConvert.DeserializeObject<SpiderStatus>(json);
				}
			}
			return new SpiderStatus();
		}

		public void ClearDb()
		{
			using (var redis = _pool.GetClient())
			{
				redis.Password = _password;
				redis.FlushDb();
			}
		}
	}
}
