using System;

namespace Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes
{
	/// <summary>
	/// 
	/// </summary>
	public class Scheme : Attribute
	{
		/// <summary>
		/// 
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public Scheme(string value)
		{
			Value = value;
		}
	}
}
