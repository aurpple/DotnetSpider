using System;
using Java2Dotnet.Spider.Extension.Model.Formatter;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	/// <summary>
	/// Define how the result string is convert to an object for field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class Formatter : System.Attribute
	{
		public Formatter(string[] value, Type type = null, IObjectFormatter formatter = null)
		{
			Value = value;
			SubType = type;
			ObjectFormatter = formatter;
		}

		/// <summary>
		/// Set formatter params.
		/// </summary>
		public string[] Value { get; set; }

		/// <summary>
		/// Specific the class of field of class of elements in collection for field. 
		/// It is not necessary to be set because we can detect the class by class of field,
		/// unless you use a collection as a field. 
		/// </summary>
		public Type SubType;

		/// <summary>
		/// If there are more than one formatter for a class, just specify the implement.
		/// </summary>
		public IObjectFormatter ObjectFormatter { get; set; }
	}
}
