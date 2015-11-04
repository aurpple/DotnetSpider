using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class CollectorPageModelPipeline : IPageModelPipeline
	{
		private readonly Dictionary<Type, List<dynamic>> _collector = new Dictionary<Type, List<dynamic>>();

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(Dictionary<Type, List<dynamic>> data, ITask task)
		{
			foreach (var pair in data)
			{
				Type type = pair.Key;
				if (_collector.ContainsKey(type))
				{
					_collector[type].AddRange(pair.Value);
				}
				else
				{
					_collector.Add(type, new List<dynamic>(pair.Value));
				}
			}
		}

		public Dictionary<Type, List<dynamic>> GetCollected()
		{
			return _collector;
		}
	}
}
