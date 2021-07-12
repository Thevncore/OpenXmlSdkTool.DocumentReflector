using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal static class STValueGenerator
	{
		private class STValue
		{
			public Type Type
			{
				get;
				set;
			}

			public Func<OpenXmlSimpleType, string> Value
			{
				get;
				set;
			}
		}

		private static List<STValue> STValueGenerators = new List<STValue>
		{
			new STValue
			{
				Type = typeof(OpenXmlSimpleType),
				Value = GenerateOpenXmlSimpleType
			},
			new STValue
			{
				Type = typeof(BooleanValue),
				Value = GenerateBooleanValue
			},
			new STValue
			{
				Type = typeof(ByteValue),
				Value = GenerateIntegerValue
			},
			new STValue
			{
				Type = typeof(DateTimeValue),
				Value = GenerateDateTimeValue
			},
			new STValue
			{
				Type = typeof(DecimalValue),
				Value = GenerateDecimalValue
			},
			new STValue
			{
				Type = typeof(DoubleValue),
				Value = GenerateDoubleValue
			},
			new STValue
			{
				Type = typeof(Int16Value),
				Value = GenerateIntegerValue
			},
			new STValue
			{
				Type = typeof(IntegerValue),
				Value = GenerateIntegerValue
			},
			new STValue
			{
				Type = typeof(Int32Value),
				Value = GenerateIntegerValue
			},
			new STValue
			{
				Type = typeof(Int64Value),
				Value = GenerateInt64Value
			},
			new STValue
			{
				Type = typeof(SByteValue),
				Value = GenerateIntegerValue
			},
			new STValue
			{
				Type = typeof(SingleValue),
				Value = GenerateSingleValue
			},
			new STValue
			{
				Type = typeof(UInt16Value),
				Value = GenerateUInt16Value
			},
			new STValue
			{
				Type = typeof(UInt32Value),
				Value = GenerateUInt32Value
			},
			new STValue
			{
				Type = typeof(UInt64Value),
				Value = GenerateUInt64Vaue
			},
			new STValue
			{
				Type = typeof(StringValue),
				Value = GenerateOpenXmlSimpleType
			},
			new STValue
			{
				Type = typeof(ListValue<StringValue>),
				Value = GenerateListValue
			},
			new STValue
			{
				Type = typeof(ListValue<Int32Value>),
				Value = GenerateListValue
			},
			new STValue
			{
				Type = typeof(ListValue<UInt32Value>),
				Value = GenerateListValue
			},
			new STValue
			{
				Type = typeof(ListValue<BooleanValue>),
				Value = GenerateListValue
			},
			new STValue
			{
				Type = typeof(HexBinaryValue),
				Value = GenerateOpenXmlSimpleType
			},
			new STValue
			{
				Type = typeof(Base64BinaryValue),
				Value = GenerateOpenXmlSimpleType
			},
			new STValue
			{
				Type = typeof(OnOffValue),
				Value = GenerateOnOffValue
			},
			new STValue
			{
				Type = typeof(TrueFalseValue),
				Value = GenerateOnOffValue
			},
			new STValue
			{
				Type = typeof(TrueFalseBlankValue),
				Value = GenerateOnOffValue
			}
		};

		private static string GenerateOpenXmlSimpleType(OpenXmlSimpleType value)
		{
			return "\"" + CSharpCodeGen.EscapeCSharpString(value.InnerText) + "\"";
		}

		private static string GenerateBooleanValue(OpenXmlSimpleType value)
		{
			bool value2 = ((BooleanValue)value).Value;
			if (value2)
			{
				return "true";
			}
			return "false";
		}

		private static string GenerateDateTimeValue(OpenXmlSimpleType value)
		{
			DateTimeValue dateTimeValue = (DateTimeValue)value;
			return "System.Xml.XmlConvert.ToDateTime(\"" + CSharpCodeGen.EscapeCSharpString(XmlConvert.ToString(dateTimeValue.Value, XmlDateTimeSerializationMode.RoundtripKind)) + "\", System.Xml.XmlDateTimeSerializationMode.RoundtripKind)";
		}

		private static string GenerateDecimalValue(OpenXmlSimpleType value)
		{
			return value.InnerText.Trim() + "M";
		}

		private static string GenerateDoubleValue(OpenXmlSimpleType value)
		{
			DoubleValue doubleValue = (DoubleValue)value;
			if (doubleValue.HasValue && !double.IsInfinity(doubleValue.Value) && !double.IsNaN(doubleValue.Value) && !double.IsNegativeInfinity(doubleValue.Value) && !double.IsPositiveInfinity(doubleValue.Value))
			{
				return value.InnerText + "D";
			}
			return "new DoubleValue() { InnerText = \"" + CSharpCodeGen.EscapeCSharpString(value.InnerText) + "\" }";
		}

		private static string GenerateIntegerValue(OpenXmlSimpleType value)
		{
			return value.InnerText;
		}

		private static string GenerateInt64Value(OpenXmlSimpleType value)
		{
			return value.InnerText + "L";
		}

		private static string GenerateSingleValue(OpenXmlSimpleType value)
		{
			SingleValue singleValue = (SingleValue)value;
			if (singleValue.HasValue && !float.IsInfinity(singleValue.Value) && !float.IsNaN(singleValue.Value) && !float.IsNegativeInfinity(singleValue.Value) && !float.IsPositiveInfinity(singleValue.Value))
			{
				return singleValue.InnerText + "F";
			}
			return "new SingleValue() { InnerText = \"" + CSharpCodeGen.EscapeCSharpString(value.InnerText) + "\"}";
		}

		private static string GenerateUInt16Value(OpenXmlSimpleType value)
		{
			return "(UInt16Value)" + value.InnerText.Trim() + "U";
		}

		private static string GenerateUInt32Value(OpenXmlSimpleType value)
		{
			return "(UInt32Value)" + value.InnerText.Trim() + "U";
		}

		private static string GenerateUInt64Vaue(OpenXmlSimpleType value)
		{
			return "(UInt64Value)" + value.InnerText.Trim() + "UL";
		}

		private static string GenerateOnOffValue(OpenXmlSimpleType value)
		{
			bool flag = false;
			OnOffValue onOffValue;
			TrueFalseValue trueFalseValue;
			TrueFalseBlankValue trueFalseBlankValue;
			if (((onOffValue = value as OnOffValue) != null && onOffValue.Value) || ((trueFalseValue = value as TrueFalseValue) != null && trueFalseValue.Value) || ((trueFalseBlankValue = value as TrueFalseBlankValue) != null && trueFalseBlankValue.Value))
			{
				return "true";
			}
			return "false";
		}

		private static string GenerateListValue(OpenXmlSimpleType value)
		{
			return "new ListValue<" + value.GetType().GetGenericArguments()[0].Name + ">() { InnerText = \"" + value.InnerText + "\" }";
		}

		public static string ReflectSimplyTypeValue(OpenXmlSimpleType value, NamespaceCollector ns)
		{
			Type t = value.GetType();
			ns.Collect(t);
			if (IsTypeOfEnum(t))
			{
				return TransformEnumValue(value, ns);
			}
			if (IsTypeOfListEnum(t))
			{
				return TransformListEnumValue(value, ns);
			}
			if (value.HasValue)
			{
				STValue sTValue = STValueGenerators.Find((STValue x) => x.Type == t);
				return sTValue.Value(value);
			}
			return "new " + value.GetType().Name + "() { InnerText = \"" + value.InnerText + "\"}";
		}

		private static string TransformEnumValue(OpenXmlSimpleType value, NamespaceCollector ns)
		{
			Type type = value.GetType().GetGenericArguments()[0];
			ns.Collect(type);
			string enumFieldName = GetEnumFieldName(type, value.InnerText);
			if (enumFieldName != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(ns.GetAliasWithDot(type.Namespace));
				stringBuilder.Append(type.Name);
				stringBuilder.Append(".");
				stringBuilder.Append(GetEnumFieldName(type, value.InnerText));
				return stringBuilder.ToString();
			}
			return "new EnumValue<" + type.Name + ">() { InnerText = \"" + value.InnerText + "\"}";
		}

		private static string GetEnumFieldName(Type enumType, string fieldTag)
		{
			FieldInfo fieldInfo = null;
			FieldInfo[] fields = enumType.GetFields();
			foreach (FieldInfo fieldInfo2 in fields)
			{
				string enumStringAttributeTypeName = Settings.Default.EnumStringAttributeTypeName;
				string enumStringAttributeValuePropertyName = Settings.Default.EnumStringAttributeValuePropertyName;
				foreach (Attribute item in from a in Attribute.GetCustomAttributes(fieldInfo2)
					where a.GetType().FullName == enumStringAttributeTypeName
					select a)
				{
					object value = item.GetType().GetProperty(enumStringAttributeValuePropertyName).GetValue(item, null);
					if (value.ToString() == fieldTag)
					{
						fieldInfo = fieldInfo2;
						break;
					}
				}
			}
			if (fieldInfo == null)
			{
				return null;
			}
			return fieldInfo.Name;
		}

		private static string TransformListEnumValue(OpenXmlSimpleType value, NamespaceCollector ns)
		{
			ns.Collect(typeof(ListValue<>));
			Type type = value.GetType().GetGenericArguments()[0];
			ns.Collect(type);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "new ListValue<EnumValue<{0}>> ", new object[1]
			{
				type.GetGenericArguments()[0].FullName
			}));
			stringBuilder.Append("{");
			stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, " InnerText = \"{0}\" ", new object[1]
			{
				value.InnerText
			}));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		private static bool IsTypeOfEnum(Type t)
		{
			if (t.IsGenericType)
			{
				return t.GetGenericTypeDefinition() == typeof(EnumValue<>);
			}
			return false;
		}

		private static bool IsTypeOfListEnum(Type t)
		{
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ListValue<>))
			{
				return t.GetGenericArguments()[0].IsGenericType;
			}
			return false;
		}
	}
}
