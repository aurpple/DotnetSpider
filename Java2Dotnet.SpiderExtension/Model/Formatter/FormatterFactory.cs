using System;
using System.Collections.Generic;

namespace Java2Dotnet.Spider.Extension.Model.Formatter
{
	public class FormatterFactory
	{
		private static readonly Dictionary<string, IObjectFormatter> BasicTypeFormatters = new Dictionary<string, IObjectFormatter>();

		static FormatterFactory()
		{
			BasicTypeFormatters.Add(Types.Int.FullName, new IntegerFormatter());
			BasicTypeFormatters.Add(Types.Double.FullName, new DoubleFormatter());
			BasicTypeFormatters.Add(Types.Float.FullName, new FloatFormatter());
			BasicTypeFormatters.Add(Types.Short.FullName, new ShortFormatter());
			BasicTypeFormatters.Add(Types.Long.FullName, new LongFormatter());

			BasicTypeFormatters.Add(Types.NullableInt.FullName, new NullableIntegerFormatter());
			BasicTypeFormatters.Add(Types.NullableDouble.FullName, new NullableDoubleFormatter());
			BasicTypeFormatters.Add(Types.NullableFloat.FullName, new NullableFloatFormatter());
			BasicTypeFormatters.Add(Types.NullableShort.FullName, new NullableShortFormatter());
			BasicTypeFormatters.Add(Types.NullableLong.FullName, new NullableLongFormatter());

			BasicTypeFormatters.Add(Types.Char.FullName, new CharactorFormatter());
			BasicTypeFormatters.Add(Types.Byte.FullName, new ByteFormatter());
			BasicTypeFormatters.Add(Types.Bool.FullName, new BooleanFormatter());
			BasicTypeFormatters.Add(Types.String.FullName, new StringFormatter());
			BasicTypeFormatters.Add(Types.Datetime.FullName, new DatetimeFormatter());
		}

		public static IObjectFormatter GetFormatter(Type type)
		{
			if (type.IsGenericType)
			{
				Type t1 = type.GenericTypeArguments[0];
				return GetFormatter(t1);
			}
			else
			{
				return GetTerminalFormatter(type);
			}
		}

		public static IObjectFormatter GetTerminalFormatter(Type type)
		{
			if (BasicTypeFormatters.ContainsKey(type.FullName))
			{
				return BasicTypeFormatters[type.FullName];
			}
			else
			{
				return null;
			}
		}
	}
}
