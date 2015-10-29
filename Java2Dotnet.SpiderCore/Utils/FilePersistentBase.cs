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

		protected static string PathSeperator = "/";

		protected void SetPath(string path)
		{
			if (!path.EndsWith(PathSeperator))
			{
				path += PathSeperator;
			}
			BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
		}

		public static FileInfo PrepareFile(string fullName)
		{
			CheckAndMakeParentDirecotry(fullName);
			return new FileInfo(fullName);
		}

		private static void CheckAndMakeParentDirecotry(string fullName)
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
