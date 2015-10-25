using System;

namespace Java2Dotnet.Spider.Extension.Grammar
{
	public class ValueConverter
	{
		public static object Convert(string type, object value)
		{
			object result = null;
			switch (type)
			{
				case "string":
					{
						result = value.ToString();
						break;
					}
				case "int":
					{
						int i;
						if (int.TryParse(value.ToString(), out i))
						{
							result = i;
						}
						break;
					}
				case "float":
					{
						float f;
						if (float.TryParse(value.ToString(), out f))
						{
							result = f;
						}
						break;
					}
				case "double":
					{
						double f;
						if (double.TryParse(value.ToString(), out f))
						{
							result = f;
						}
						break;
					}
			}
			if (result != null)
			{
				return result;
			}
			else
			{
				throw new  Exception("Unknow value type.");
			}
		}

		public static string Convert(string value)
		{
			return value.Substring(1, value.Length - 2);
		}
	}
}
