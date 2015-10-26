using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Grammar
{
	public class Result
	{
		public dynamic Value { get; set; }
		public Type Type { get; set; }

		public Result(object value)
		{
			Value = value;
			if (value != null)
			{
				Type = value.GetType();
			}
		}

		public Result() { }
	}

	public class ModifyScriptVisitor : ModifyScriptBaseVisitor<Result>
	{
		public string Value;
		public bool NeedStop;

		private readonly Dictionary<string, Type> _supportType;
		private PropertyInfo _field;

		public ModifyScriptVisitor(string value, PropertyInfo field)
		{
			Value = value;

			_supportType = new Dictionary<string, Type> {
				{ "INT", typeof(int)},
				{ "INT64", typeof(long)},
				{ "FLOAT", typeof(float)},
				{ "STRING", typeof(string)} ,
				{ "BOOL", typeof(bool)} ,
				{ "Object", typeof(object)}
			};

			_field = field;
		}

		public override Result VisitInt(ModifyScriptParser.IntContext context)
		{
			return new Result() { Value = int.Parse(context.INT().GetText()), Type = _supportType["INT"] };
		}

		public override Result VisitString(ModifyScriptParser.StringContext context)
		{
			return new Result() { Value = ValueConverter.Convert(context.GetText()), Type = _supportType["STRING"] };
		}

		public override Result VisitFloat(ModifyScriptParser.FloatContext context)
		{
			return new Result() { Value = float.Parse(context.GetText()), Type = _supportType["FLOAT"] };
		}

		public override Result VisitMulDiv(ModifyScriptParser.MulDivContext context)
		{
			Result left = Visit(context.expr(0));
			Result right = Visit(context.expr(1));

			if (left.Value is ValueType && right.Value is ValueType)
			{
				if (context.op.Type == ModifyScriptParser.DIV)
				{
					if (right.Value == 0.0 || right.Value == 0)
					{
						throw new Exception("Dividend can not be 0");
					}
					dynamic result = left.Value / right.Value;
					return new Result() { Value = result, Type = result.GetType() };
				}
				else
				{
					dynamic result = left.Value * right.Value;
					return new Result() { Value = result, Type = result.GetType() };
				}
			}
			else
			{
				throw new Exception("Only value type can do calculate.");
			}
		}

		public override Result VisitAddSub(ModifyScriptParser.AddSubContext context)
		{
			Result left = Visit(context.expr(0));
			Result right = Visit(context.expr(1));

			if (context.op.Type == ModifyScriptParser.ADD)
			{
				if (((left.Value is ValueType) || left.Type == _supportType["STRING"]) && ((right.Value is ValueType) || right.Type == _supportType["STRING"]))
				{
					dynamic result = left.Value + right.Value;
					return new Result() { Value = result, Type = result.GetType() };
				}
				else
				{
					throw new Exception("Only value/string type can do calculate.");
				}
			}
			else
			{
				if (((left.Value is ValueType) || left.Type == _supportType["STRING"]) && ((right.Value is ValueType) || right.Type == _supportType["STRING"]))
				{
					dynamic result = left.Value - right.Value;
					return new Result() { Value = result, Type = result.GetType() };
				}
				else
				{
					throw new Exception("Only value/string type can do calculate.");
				}
			}
		}

		public override Result VisitParens(ModifyScriptParser.ParensContext context)
		{
			return Visit(context.expr());
		}

		public override Result VisitParam(ModifyScriptParser.ParamContext context)
		{
			var STRING = context.STRING();

			if (STRING != null)
			{
				return new Result() { Value = ValueConverter.Convert(STRING.ToString()), Type = _supportType["STRING"] };
			}

			if (context.expr() != null)
			{
				return Visit(context.expr());
			}

			if (context.BOOLEAN() != null)
			{
				return Visit(context.BOOLEAN());
			}

			return null;
		}

		public override Result VisitRegex(ModifyScriptParser.RegexContext context)
		{
			string regexString = ValueConverter.Convert(context.STRING().GetText());
			int index = int.Parse(context.INT().GetText());

			Regex regex = new Regex(regexString);
			Value = regex.Match(Value).Groups[index].Value;
			Result result = new Result() { Value = Value, Type = _supportType["STRING"] };

			return result;
		}

		public override Result VisitAppend(ModifyScriptParser.AppendContext context)
		{
			string appendString = ValueConverter.Convert(context.STRING().GetText());
			Value = Value + appendString;
			Result result = new Result() { Value = Value, Type = _supportType["STRING"] };

			return result;
		}

		public override Result VisitPrefix(ModifyScriptParser.PrefixContext context)
		{
			string prefixString = ValueConverter.Convert(context.STRING().GetText());
			Value = prefixString + Value;
			Result result = new Result() { Value = Value, Type = _supportType["STRING"] };

			return result;
		}

		public override Result VisitReplace(ModifyScriptParser.ReplaceContext context)
		{
			string oldValue = ValueConverter.Convert(context.STRING()[0].GetText());
			string newValue = ValueConverter.Convert(context.STRING()[1].GetText());
			Value = Value.Replace(oldValue, newValue);
			Result result = new Result() { Value = Value, Type = _supportType["STRING"] };

			return result;
		}

		public override Result VisitCompare(ModifyScriptParser.CompareContext context)
		{
			string value = context.GetText();
			return new Result() { Value = value, Type = _supportType["STRING"] };
		}

		public override Result VisitStoper(ModifyScriptParser.StoperContext context)
		{
			string compare = Visit(context.compare()).Value;
			string value = ValueConverter.Convert(context.STRING().GetText());

			if (_field.PropertyType == _supportType["INT"])
			{
				int compareValue;
				int originalValue;
				if (int.TryParse(value, out compareValue) && int.TryParse(Value, out originalValue))
				{
					switch (compare)
					{
						case ">":
							NeedStop = originalValue > compareValue;
							break;
						case "<":
							NeedStop = originalValue < compareValue;
							break;
						case "=":
							NeedStop = originalValue == compareValue;
							break;
					}
				}
				else
				{
					throw new SpiderExceptoin($"Can't covert stoper value or Property value to int");
				}
			}

			if (_field.PropertyType == _supportType["INT64"])
			{
				long compareValue;
				long originalValue;
				if (long.TryParse(value, out compareValue) && long.TryParse(Value, out originalValue))
				{
					switch (compare)
					{
						case ">":
							NeedStop = originalValue > compareValue;
							break;
						case "<":
							NeedStop = originalValue < compareValue;
							break;
						case "=":
							NeedStop = originalValue == compareValue;
							break;
					}
				}
				else
				{
					throw new SpiderExceptoin($"Can't covert stoper value or Property value to int");
				}
			}

			if (_field.PropertyType == _supportType["FLOAT"])
			{
				float compareValue;
				float originalValue;
				if (float.TryParse(value, out compareValue) && float.TryParse(Value, out originalValue))
				{
					switch (compare)
					{
						case ">":
							NeedStop = originalValue > compareValue;
							break;
						case "<":
							NeedStop = originalValue < compareValue;
							break;
						case "=":
							NeedStop = originalValue == compareValue;
							break;
					}
				}
				else
				{
					throw new SpiderExceptoin($"Can't covert stoper value or Property value to FLOAT");
				}
			}

			if (_field.PropertyType == _supportType["STRING"])
			{
				switch (compare)
				{
					case ">":
					case "<":
						throw new SpiderExceptoin($"Stoper can't set STRING > OR < STRING.");
					case "=":
						NeedStop = Value == value;
						break;
				}
			}

			if (_field.PropertyType == _supportType["BOOL"])
			{
				bool compareValue;
				bool originalValue;
				if (bool.TryParse(value, out compareValue) && bool.TryParse(Value, out originalValue))
				{
					switch (compare)
					{
						case ">":
						case "<":
							throw new SpiderExceptoin($"Stoper can't set BOOL > OR < BOOL.");
						case "=":
							NeedStop = originalValue == compareValue;
							break;
					}
				}
				else
				{
					throw new SpiderExceptoin($"Can't covert stoper value or Property value to BOOL");
				}
			}
			return null;
		}
	}
}
