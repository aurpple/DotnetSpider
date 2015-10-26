using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public enum CollectorType
	{
		Insert,
		Update
	}

	public sealed class CollectorPageModelToDbPipeline : IPageModelPipeline
	{
		private CollectorType _type;

		public long TotalCount => _totalCount.Value;

		public CollectorPageModelToDbPipeline(CollectorType type = CollectorType.Insert)
		{
			_type = type;
		}

		private readonly AutomicLong _totalCount = new AutomicLong(0);
		//private readonly Hashtable _collected = new Hashtable();
		private readonly ConcurrentDictionary<Type, IDataRepository> _cache = new ConcurrentDictionary<Type, IDataRepository>();

		public void Process(dynamic o, ITask task)
		{
			Type type = o.GetType();
			IDataRepository dataRepository = null;

			ArrayList result = new ArrayList();
			if (!type.IsGenericType)
			{
				if (_cache.ContainsKey(type))
				{
					dataRepository = _cache[type];
					_totalCount.Inc();
				}
				else
				{
					if (type.GetCustomAttribute(typeof(StoredAs)) != null)
					{
						dataRepository = new DataRepository(DbProviderUtil.GetProvider, type);
						dataRepository.CreateSheme();
						dataRepository.CreateTable();
						_cache.TryAdd(type, dataRepository);
					}
				}
				//_collected.Add(o, null);
				result.Add(o);
			}
			else
			{
				IList list = (IList)o;
				if (list.Count > 0)
				{
					type = list[0].GetType();

					if (_cache.ContainsKey(type))
					{
						dataRepository = _cache[type];
					}
					else
					{
						if (type.GetCustomAttribute(typeof(StoredAs)) != null)
						{
							dataRepository = new DataRepository(DbProviderUtil.GetProvider, type);
							dataRepository.CreateSheme();
							dataRepository.CreateTable();
							_cache.TryAdd(type, dataRepository);
						}
					}

					foreach (dynamic o1 in list)
					{
						//_collected.Add(o1, null);
						result.Add(o1);
						_totalCount.Inc();
					}
				}
			}

			switch (_type)
			{
				case CollectorType.Insert:
					{
						dataRepository?.Insert(result);
						break;
					}
				case CollectorType.Update:
					{
						dataRepository?.Update(result);
						break;
					}
			}
		}

		public ICollection GetCollected()
		{
			//return _collected.Keys;
			//SAVE HUGE OBJECTS TO MEMORY
			return null;
		}
	}
}
