using System.Collections;
using System.Collections.Specialized;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Object contains extract results. 
	/// It is contained in Page and will be processed in pipeline.
	/// </summary>
	public class ResultItems
	{
		private readonly IDictionary _fields = new ListDictionary();

		public dynamic Get(string key)
		{
			return _fields.Contains(key) ? _fields[key] : null;
		}

		public ResultItems Put(string key, dynamic value)
		{
			_fields.Add(key, value);
			return this;
		}

		public IDictionary GetAll() => _fields;

		public Request  Request { get; set; }

		/// <summary>
		/// Whether to skip the result. 
		/// Result which is skipped will not be processed by Pipeline.
		/// </summary>
		public bool IsSkip { get; set; }

		public override string ToString()
		{
			return "ResultItems{" +
					"fields=" + _fields +
					", request=" + Request +
					", skip=" + IsSkip +
					'}';
		}
	}
}