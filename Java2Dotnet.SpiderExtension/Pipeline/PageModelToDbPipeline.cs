using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model.Formatter;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public enum OperateType
	{
		Insert,
		Update
	}

	public sealed class PageModelToDbPipeline : IPageModelPipeline
	{
		private readonly OperateType _operateType;
		public long TotalCount => _totalCount.Value;
		private readonly ConcurrentDictionary<Type, IDataRepository> _cache = new ConcurrentDictionary<Type, IDataRepository>();

		public PageModelToDbPipeline(OperateType operateType = OperateType.Insert)
		{
			_operateType = operateType;
		}

		private readonly AutomicLong _totalCount = new AutomicLong(0);

		public void Process(dynamic data, ITask task)
		{
			if (data == null)
			{
				return;
			}

			Type type = data.GetType();
			IDataRepository dataRepository = null;

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
					else
					{
						throw new SpiderExceptoin("Didn't define TableName( StoreAs attribute) in the Type: " + type.FullName);
					}
				}
			}
			else
			{
				IList list = (IList)data;
				if (list.Count > 0)
				{
					type = type.GetGenericTypeDefinition();

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
						else
						{
							throw new SpiderExceptoin("Didn't define TableName( StoreAs attribute) in the Type: " + type.FullName);
						}
					}

					for (int i = 0; i < list.Count; ++i)
					{
						//_collected.Add(o1, null);
						//result.Add(o1);
						_totalCount.Inc();
					}
				}
			}

			switch (_operateType)
			{
				case OperateType.Insert:
					{
						dataRepository?.Insert(data);
						break;
					}
				case OperateType.Update:
					{
						dataRepository?.Update(data);
						break;
					}
			}
		}
	}
}
