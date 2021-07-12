using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class ReflectedAttribute
	{
		public PropertyInfo Prop
		{
			get;
			private set;
		}

		public OpenXmlSimpleType Value
		{
			get;
			private set;
		}

		public ReflectedAttribute(PropertyInfo prop, OpenXmlSimpleType value)
		{
			if (prop == null)
			{
				throw new ArgumentNullException("prop");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Prop = prop;
			Value = value;
		}

		public static IEnumerable<ReflectedAttribute> GetReflectedAttributes(OpenXmlElement element)
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
			PropertyInfo[] array = (from p in element.GetType().GetProperties(bindingFlags)
				where p.PropertyType == typeof(OpenXmlSimpleType) || p.PropertyType.IsSubclassOf(typeof(OpenXmlSimpleType))
				select p).ToArray();
			List<ReflectedAttribute> list = new List<ReflectedAttribute>();
			PropertyInfo[] array2 = array;
			foreach (PropertyInfo propertyInfo in array2)
			{
				OpenXmlSimpleType openXmlSimpleType = element.GetType().InvokeMember(propertyInfo.Name, bindingFlags | BindingFlags.GetProperty, Type.DefaultBinder, element, null, CultureInfo.InvariantCulture) as OpenXmlSimpleType;
				if (openXmlSimpleType != null)
				{
					list.Add(new ReflectedAttribute(propertyInfo, openXmlSimpleType));
				}
			}
			return list;
		}

		public static CodeChunk ReflectAttribute(string propName, OpenXmlSimpleType stValue, NamespaceCollector ns)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(propName);
			codeChunk.Append(" = ");
			codeChunk.Append(new StringMetacode(STValueGenerator.ReflectSimplyTypeValue(stValue, ns)));
			return codeChunk;
		}
	}
}
