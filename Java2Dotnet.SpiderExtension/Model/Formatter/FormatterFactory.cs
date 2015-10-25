using System;
using System.Collections.Generic;

namespace Java2Dotnet.Spider.Extension.Model.Formatter
{
	public class FormatterFactory
	{
		private static readonly Dictionary<string, Type> BasicTypeFormatters = new Dictionary<string, Type>();

		static FormatterFactory()
		{
			BasicTypeFormatters.Add(Types.Int.FullName, typeof(IntegerFormatter));
			BasicTypeFormatters.Add(Types.Double.FullName, typeof(DoubleFormatter));
			BasicTypeFormatters.Add(Types.Float.FullName, typeof(FloatFormatter));
			BasicTypeFormatters.Add(Types.Short.FullName, typeof(ShortFormatter));
			BasicTypeFormatters.Add(Types.Long.FullName, typeof(LongFormatter));

			BasicTypeFormatters.Add(Types.NullableInt.FullName, typeof(NullableIntegerFormatter));
			BasicTypeFormatters.Add(Types.NullableDouble.FullName, typeof(NullableDoubleFormatter));
			BasicTypeFormatters.Add(Types.NullableFloat.FullName, typeof(NullableFloatFormatter));
			BasicTypeFormatters.Add(Types.NullableShort.FullName, typeof(NullableShortFormatter));
			BasicTypeFormatters.Add(Types.NullableLong.FullName, typeof(NullableLongFormatter));

			BasicTypeFormatters.Add(Types.Char.FullName, typeof(CharactorFormatter));
			BasicTypeFormatters.Add(Types.Byte.FullName, typeof(ByteFormatter));
			BasicTypeFormatters.Add(Types.Bool.FullName, typeof(BooleanFormatter));
			BasicTypeFormatters.Add(Types.String.FullName, typeof(StringFormatter));
			BasicTypeFormatters.Add(Types.Datetime.FullName, typeof(DatetimeFormatter));
		}

		public static Type GetFormatter(Type type)
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

		public static Type GetTerminalFormatter(Type type)
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
