using System;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Column : System.Attribute
	{
		public Column(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}
