using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Java2Dotnet.Spider.Scripts
{
	public class ScriptProcessorBuilder
	{
		private static readonly Language DefaultLanguage = Scripts.Language.Javascript;
		private static readonly List<Assembly> AssemblyList = new List<Assembly>();
		private Language _language0 = DefaultLanguage;

		private string _script0;

		private int _threadNum = 1;

		static ScriptProcessorBuilder()
		{
			AssemblyList.AddRange(AppDomain.CurrentDomain.GetAssemblies());
		}

		private ScriptProcessorBuilder()
		{
		}

		public static ScriptProcessorBuilder Custom()
		{
			return new ScriptProcessorBuilder();
		}

		public ScriptProcessorBuilder Language(Language language)
		{
			_language0 = language;
			return this;
		}

		public ScriptProcessorBuilder ScriptFromFile(string fileName)
		{
			try
			{

				_script0 = File.ReadAllText(fileName);
			}
			catch (IOException e)
			{
				//wrap IOException because I prefer a runtime exception...
				throw new ArgumentException(e.ToString());
			}
			return this;
		}

		public ScriptProcessorBuilder ScriptFromClassPathFile(string fileName)
		{
			try
			{
				StringBuilder builder = new StringBuilder();
				using (StreamReader sr = new StreamReader(GetScriptStream(fileName)))
				{
					string line;
					// Read and display lines from the file until the end of 
					// the file is reached.
					while ((line = sr.ReadLine()) != null)
					{
						builder.AppendLine(line);
					}
				}
				_script0 = builder.ToString();
			}
			catch (IOException e)
			{
				//wrap IOException because I prefer a runtime exception...
				throw new ArgumentException(e.ToString());
			}
			return this;
		}

		private Stream GetScriptStream(string fileName)
		{
			List<Assembly> selfAssemblyList = AssemblyList.FindAll(a => a.FullName.Contains("Java2Dotnet.Spider"));

			foreach (var ass in selfAssemblyList)
			{
				Stream stream = ass.GetManifestResourceStream(fileName);
				if (stream != null)
				{
					return stream;
				}
			}
			throw new ArgumentException("Script path error.");
		}

		public ScriptProcessorBuilder Script(string script)
		{
			_script0 = script;
			return this;
		}

		public ScriptProcessorBuilder Thread(int threadNum)
		{
			_threadNum = threadNum;
			return this;
		}

		public ScriptProcessor Build()
		{
			return new ScriptProcessor(_language0, _script0, _threadNum);
		}
	}
}