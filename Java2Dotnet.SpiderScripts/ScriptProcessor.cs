using System;
using System.IO;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Processor;

namespace Java2Dotnet.Spider.Scripts
{
	public class ScriptProcessor : IPageProcessor
	{
		private readonly string _defines;

		private readonly string _script;

		private readonly Language _language;

		public ScriptProcessor(Language language, string script, int threadNum)
		{
			if (language == null || script == null)
			{
				throw new ArgumentException("language and script must not be null!");
			}
			_language = language;

			try
			{
				StringBuilder builder = new StringBuilder();
				using (StreamReader sr = new StreamReader(GetType().Assembly.GetManifestResourceStream(language.GetDefineFile())))
				{
					string line;
					// Read and display lines from the file until the end of 
					// the file is reached.
					while ((line = sr.ReadLine()) != null)
					{
						builder.AppendLine(line);
					}
				}
				_defines = builder.ToString();
			}
			catch (IOException e)
			{
				throw new ArgumentException(e.ToString());
			}

			_script = script;
		}

		public void Process(Page page)
		{
			if (_language == Language.Javascript)
			{
				//engine.eval(defines + "\n" + script, context);
				//                        NativeObject o = (NativeObject) engine.get("result");
				//                        if (o != null) {
				//                            for (Object o1 : o.getIds()) {
				//                                string key = string.valueOf(o1);
				//                                page.getResultItems().put(key, NativeObject.getProperty(o, key));
				//                            }
				//                        }
				string realScript = _defines + Environment.NewLine + _script;

				Jurassic.ScriptEngine engine = new Jurassic.ScriptEngine();
				engine.EnableExposedClrTypes = true;
				engine.SetGlobalValue("page", page);
				engine.SetGlobalValue("config", Site);
				engine.Execute(realScript);
			}
			else if (_language == Language.Python)
			{
				//RubyHash oRuby = (RubyHash)engine.eval(defines + "\n" + script, context);
				//Iterator itruby = oRuby.entrySet().iterator();
				//while (itruby.hasNext())
				//{
				//	Map.Entry pairs = (Map.Entry)itruby.next();
				//	page.getResultItems().put(pairs.getKey().toString(), pairs.getValue());
				//}
			}
			else if (_language == Language.Ruby)
			{
				//engine.eval(defines + "\n" + script, context);
				//PyDictionary oJython = (PyDictionary)engine.get("result");
				//Iterator it = oJython.entrySet().iterator();
				//while (it.hasNext())
				//{
				//	Map.Entry pairs = (Map.Entry)it.next();
				//	page.getResultItems().put(pairs.getKey().toString(), pairs.getValue());
				//}
			}
		}

		public Site Site { get; } = new Site();
	}
}