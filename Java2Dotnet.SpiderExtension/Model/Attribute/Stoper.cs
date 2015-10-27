using Java2Dotnet.Spider.Extension.Utils;
using System;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	[AttributeUsage(AttributeTargets.Property)]
	public class Stoper : System.Attribute
	{
		public Operator Op { get; set; }

		public string Value { get; set; }

		public Stoper(Operator op, string value)
		{
			Op = op;
			Value = value;
		}

		public bool NeedStop(dynamic converted)
		{
			try
			{
				bool needStop = false;

				Type type = converted.GetType();

				if (type.Name == "Int")
				{
					int compareValue;
					if (int.TryParse(Value, out compareValue))
					{
						switch (Op)
						{
							case Operator.Greater:
								needStop = converted > compareValue;
								break;
							case Operator.Less:
								needStop = converted < compareValue;
								break;
							case Operator.Equal:
								needStop = converted == compareValue;
								break;
						}
					}
					else
					{
						throw new SpiderExceptoin($"Can't covert stoper value to int");
					}
				}

				if (type.Name == "Int64")
				{
					long compareValue;
					if (long.TryParse(Value, out compareValue))
					{
						switch (Op)
						{
							case Operator.Greater:
								needStop = converted > compareValue;
								break;
							case Operator.Less:
								needStop = converted < compareValue;
								break;
							case Operator.Equal:
								needStop = converted == compareValue;
								break;
						}
					}
					else
					{
						throw new SpiderExceptoin($"Can't covert stoper value or Property value to int");
					}
				}

				if (type.Name == "Float")
				{
					float compareValue;
					if (float.TryParse(Value, out compareValue))
					{
						switch (Op)
						{
							case Operator.Greater:
								needStop = converted > compareValue;
								break;
							case Operator.Less:
								needStop = converted < compareValue;
								break;
							case Operator.Equal:
								needStop = converted == compareValue;
								break;
						}
					}
					else
					{
						throw new SpiderExceptoin($"Can't covert stoper value or Property value to FLOAT");
					}
				}

				if (type.Name == "String")
				{
					switch (Op)
					{
						case Operator.Greater:
						case Operator.Less:
							throw new SpiderExceptoin($"Stoper can't set STRING > OR < STRING.");
						case Operator.Equal:
							needStop = Value == converted;
							break;
					}
				}

				if (type.Name == "Bool")
				{
					bool compareValue;
					if (bool.TryParse(Value, out compareValue))
					{
						switch (Op)
						{
							case Operator.Greater:
							case Operator.Less:
								throw new SpiderExceptoin($"Stoper can't set BOOL > OR < BOOL.");
							case Operator.Equal:
								needStop = converted == compareValue;
								break;
						}
					}
					else
					{
						throw new SpiderExceptoin($"Can't covert stoper value or Property value to BOOL");
					}
				}

				return needStop;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
