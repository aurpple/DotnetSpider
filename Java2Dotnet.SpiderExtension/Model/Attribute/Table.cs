using System;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Table : System.Attribute
	{
		public Table(string schema, string name)
		{
			Name = name;
			Schema = schema;
		}

		public string Name { get; set; }
		public string Schema { get; set; }
	}
}
