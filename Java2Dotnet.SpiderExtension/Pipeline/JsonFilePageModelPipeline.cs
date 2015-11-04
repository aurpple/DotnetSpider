using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.Model;
using Newtonsoft.Json;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// Store results objects (page models) to files in JSON format.
	/// Use model.getKey() as file name if the model implements HasKey.
	/// Otherwise use SHA1 as file name.
	/// </summary>
	public class JsonFilePageModelPipeline : FilePersistentBase, IPageModelPipeline
	{
		private readonly log4net.ILog _logger;

		/// <summary>
		/// New JsonFilePageModelPipeline with default path "/data/webmagic/"
		/// </summary>
		public JsonFilePageModelPipeline()
		{
			_logger = log4net.LogManager.GetLogger(GetType());
			SetPath("/data/dotnetspider/");
		}

		public JsonFilePageModelPipeline(string path)
		{
			SetPath(path);
		}

		public void Process(Dictionary<Type, List<dynamic>> data, ITask task)
		{
			foreach (var pair in data)
			{
				string path = BasePath + "/" + task.Identify + "/";
				try
				{
					string filename = path + pair.Key.FullName + ".json";

					FileInfo file = PrepareFile(filename);
					using (StreamWriter printWriter = new StreamWriter(file.OpenWrite(), Encoding.UTF8))
					{
						printWriter.WriteLine(JsonConvert.SerializeObject(pair.Value));
					}
				}
				catch (Exception e)
				{
					_logger.Warn("write file error", e);
					throw;
				}
			}
		}
	}
}