using System;
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
			SetPath("/data/webmagic/");
		}

		public JsonFilePageModelPipeline(string path)
		{
			SetPath(path);
		}

		public void Process(object o, ITask task)
		{
			string path = BasePath + "/" + task.Identify + "/";
			try
			{
				string filename;
				var key = o as IHasKey;
				if (key != null)
				{
					filename = path + key.Key + ".json";
				}
				else
				{
					//check
					filename = path + Encrypt.Md5Encrypt(o.ToString()) + ".json";
				}
				FileInfo file = GetFile(filename);
				using (StreamWriter printWriter = new StreamWriter(file.OpenWrite(), Encoding.UTF8))
				{
					printWriter.WriteLine(JsonConvert.SerializeObject(o));
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