using System.IO;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Core.Utils;
using Newtonsoft.Json;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// Store results to files in JSON format.
	/// </summary>
	public class JsonFilePipeline : FilePersistentBase, IPipeline
	{
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(JsonFilePipeline));

		/// <summary>
		/// New JsonFilePageModelPipeline with default path "/data/webmagic/"
		/// </summary>
		public JsonFilePipeline()
		{
			SetPath("/data/dotnetspider");
		}

		public JsonFilePipeline(string path)
		{
			SetPath(path);
		}

		public void Process(ResultItems resultItems, ITask task)
		{
			string path = BasePath + "/" + task.Identify + "/";
			try
			{
				FileInfo file = PrepareFile(path + Encrypt.Md5Encrypt(resultItems.Request.Url) + ".json");
				using (StreamWriter printWriter = new StreamWriter(file.OpenWrite(), Encoding.UTF8))
				{
					printWriter.WriteLine(JsonConvert.SerializeObject(resultItems.GetAll()));
				}
			}
			catch (IOException e)
			{
				Logger.Warn("write file error", e);
				throw;
			}
		}
	}
}
