using System;
using System.IO;

namespace Java2Dotnet.Spider.Core.Utils
{
	/// <summary>
	/// Base object of file persistence.
	/// </summary>
	public class FilePersistentBase : ContextBoundObject
	{
		protected string BasePath;

		public static string PathSeperator = "/";

		public void SetPath(string path)
		{
			if (!path.EndsWith(PathSeperator))
			{
				path += PathSeperator;
			}
			BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
		}

		public FileInfo GetFile(string fullName)
		{
			CheckAndMakeParentDirecotry(fullName);
			return new FileInfo(fullName);
		}

		public void CheckAndMakeParentDirecotry(string fullName)
		{
			string path = Path.GetDirectoryName(fullName);

			if (path != null)
			{
				DirectoryInfo directory = new DirectoryInfo(path);
				if (!directory.Exists)
				{
					directory.Create();
				}
			}
		}
	}
}
