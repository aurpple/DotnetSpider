using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Java2Dotnet.Spider.Core.Utils;
using log4net;

namespace Java2Dotnet.Spider.Core.Pipeline
{
	/// <summary>
	/// Store results in files.
	/// </summary>
	[Synchronization]
	public class FilePipeline : FilePersistentBase, IPipeline
	{
		private readonly ILog _logger = LogManager.GetLogger(typeof(FilePipeline));

		/// <summary>
		/// create a FilePipeline with default path"/data/webmagic/"
		/// </summary>
		public FilePipeline()
		{
			SetPath("/data/dotnetspider/");
		}

		public FilePipeline(string path)
		{
			SetPath(path);
		}

		public virtual void Process(ResultItems resultItems, ITask task)
		{
			string filePath = BasePath + PathSeperator + task.Identify + PathSeperator;
			try
			{
				FileInfo file = PrepareFile(filePath + Encrypt.Md5Encrypt(resultItems.Request.Url) + ".html");
				using (StreamWriter printWriter = new StreamWriter(file.OpenWrite(), Encoding.UTF8))
				{
					printWriter.WriteLine("url:\t" + resultItems.Request.Url);

					foreach (DictionaryEntry entry in resultItems.GetAll())
					{
						var value = entry.Value as IList;
						if (value != null)
						{
							IList list = value;
							printWriter.WriteLine(entry.Key + ":");
							foreach (var o in list)
							{
								printWriter.WriteLine(o);
							}
						}
						else
						{
							printWriter.WriteLine(entry.Key + ":\t" + entry.Value);
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.Warn("Write file error.", e);
				throw;
			}
		}
	}
}