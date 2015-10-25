using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
		private readonly Dictionary<string, Type> _supportType;

		public ModifyScriptVisitor(string value)
		{
			Value = value;

			_supportType = new Dictionary<string, Type> {
				{ "INT", typeof(int)},
				{ "FLOAT", typeof(float)},
				{ "STRING", typeof(string)} ,
				{ "BOOL", typeof(bool)} ,
				{ "Object", typeof(object)}
			};
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

		
	}
}
