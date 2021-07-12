using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class ElementCSharpCodeBuilder : IElementCodeBuilder
	{
		private OpenXmlElement _element;

		private ReflectContext _context;

		public int InitialIndent
		{
			get;
			set;
		}

		public ElementCSharpCodeBuilder(OpenXmlElement element, ReflectContext context)
		{
			_element = element;
			_context = context;
		}

		public CodeChunk Build(out string elementVariableName)
		{
			return ReflectElement(_element, InitialIndent, _context, out elementVariableName);
		}

		private static CodeChunk ReflectElement(OpenXmlElement element, int indent, ReflectContext context, out string elementVariableName)
		{
			OpenXmlUnknownElement element2;
			if ((element2 = element as OpenXmlUnknownElement) != null)
			{
				return ReflectUnknownElement(element2, indent, context, out elementVariableName);
			}
			return ReflectKnownElement(element, indent, context, out elementVariableName);
		}

		private static CodeChunk ReflectUnknownElement(OpenXmlUnknownElement element, int indent, ReflectContext context, out string elementVariableName)
		{
			Type type = element.GetType();
			context.UsedNamespaces.Collect(type);
			string text = context.Variables.NewVarName(type);
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(indent));
			codeChunk.Append(context.UsedNamespaces.GetAliasWithDot(type.Namespace));
			codeChunk.Append(new TypeMetacode(type.Name), new Separator());
			codeChunk.Append(text + " = ");
			codeChunk.Append(type.Name + ".CreateOpenXmlUnknownElement(\"" + CSharpCodeGen.EscapeCSharpString(element.OuterXml) + "\");");
			codeChunk.AppendLine();
			elementVariableName = text;
			return codeChunk;
		}

		private static CodeChunk ReflectKnownElement(OpenXmlElement element, int indent, ReflectContext context, out string elementVariableName)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			Type type = element.GetType();
			context.UsedNamespaces.Collect(type);
			string text = context.Variables.NewVarName(type);
			codeChunk.Append(new Indent(indent));
			codeChunk.Append(context.UsedNamespaces.GetAliasWithDot(type.Namespace));
			codeChunk.Append(new TypeMetacode(type.Name), new Separator());
			codeChunk.Append(text + " = ");
			codeChunk.Append(Keyword.New, new Separator());
			codeChunk.Append(new Metacode(context.UsedNamespaces.GetAliasWithDot(type.Namespace)), new TypeMetacode(type.Name));
			if (element is OpenXmlMiscNode && element.LocalName == "#comment")
			{
				codeChunk.Append(new Metacode("(System.Xml.XmlNodeType.Comment)"));
			}
			else
			{
				codeChunk.Append(new Metacode("()"));
			}
			codeChunk.Append(GenerateClassInitializer(element, context.UsedNamespaces));
			codeChunk.AppendLine(new Metacode(";"));
			codeChunk.Append(GenerateNamespaceDeclarations(element, indent, text));
			codeChunk.Append(GenerateExtendedAttributes(element, indent, text));
			OpenXmlLeafTextElement openXmlLeafTextElement;
			if ((openXmlLeafTextElement = element as OpenXmlLeafTextElement) != null)
			{
				codeChunk.Append(new Indent(indent));
				codeChunk.Append(text + ".Text = ");
				string text2 = "\"" + CSharpCodeGen.EscapeCSharpString(openXmlLeafTextElement.Text) + "\";";
				codeChunk.AppendLine(new StringMetacode(text2));
			}
			else if (element.FirstChild != null)
			{
				List<string> list = new List<string>();
				foreach (OpenXmlElement item in element)
				{
					if (WillGenerateOnePlusLines(item))
					{
						codeChunk.AppendLine();
					}
					codeChunk.Append(ReflectElement(item, indent, context, out var elementVariableName2));
					list.Add(elementVariableName2);
				}
				codeChunk.AppendLine();
				foreach (string item2 in list)
				{
					codeChunk.Append(new Indent(indent));
					codeChunk.Append(string.Format(CultureInfo.InvariantCulture, "{0}.Append({1})", new object[2]
					{
						text,
						item2
					}));
					codeChunk.AppendLine(";");
				}
			}
			elementVariableName = text;
			return codeChunk;
		}

		private static bool WillGenerateOnePlusLines(OpenXmlElement element)
		{
			if (element.FirstChild == null && element.MCAttributes == null && element.ExtendedAttributes.Count() <= 0)
			{
				return element.NamespaceDeclarations.Count() > 0;
			}
			return true;
		}

		private static CodeChunk GenerateClassInitializer(OpenXmlElement element, NamespaceCollector ns)
		{
			ns.Collect(element.GetType());
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			bool flag = false;
			IEnumerable<ReflectedAttribute> reflectedAttributes = ReflectedAttribute.GetReflectedAttributes(element);
			foreach (ReflectedAttribute item in reflectedAttributes)
			{
				if (item != reflectedAttributes.First())
				{
					codeChunk.Append(", ");
				}
				else
				{
					flag = true;
					codeChunk.Append("{ ");
				}
				codeChunk.Append(ReflectedAttribute.ReflectAttribute(item.Prop.Name, item.Value, ns));
			}
			if (element.MCAttributes != null)
			{
				Type type = element.MCAttributes.GetType();
				ns.Collect(type);
				if (flag)
				{
					codeChunk.Append(", ");
				}
				else
				{
					codeChunk.Append("{ ");
				}
				codeChunk.Append("MCAttributes = ");
				codeChunk.Append(Keyword.New);
				codeChunk.Append(" ");
				codeChunk.Append(new TypeMetacode(type.Name));
				codeChunk.Append("(){ ");
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
				PropertyInfo[] properties = type.GetProperties(bindingFlags);
				bool flag2 = false;
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					StringValue stringValue = type.InvokeMember(propertyInfo.Name, bindingFlags | BindingFlags.GetProperty, Type.DefaultBinder, element.MCAttributes, null, CultureInfo.InvariantCulture) as StringValue;
					if (stringValue != null && stringValue.HasValue)
					{
						if (flag2)
						{
							codeChunk.Append(", ");
						}
						codeChunk.Append(propertyInfo.Name + " = \"" + CSharpCodeGen.EscapeCSharpString(stringValue.Value) + "\"");
						flag2 = true;
					}
				}
				codeChunk.Append(" } ");
			}
			if (flag || element.MCAttributes != null)
			{
				codeChunk.Append(" }");
			}
			return codeChunk;
		}

		private static CodeChunk GenerateNamespaceDeclarations(OpenXmlElement element, int initIndent, string varName)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			foreach (KeyValuePair<string, string> namespaceDeclaration in element.NamespaceDeclarations)
			{
				codeChunk.Append(new Indent(initIndent));
				codeChunk.Append(varName + ".AddNamespaceDeclaration(\"");
				codeChunk.Append(namespaceDeclaration.Key);
				codeChunk.Append("\", \"");
				codeChunk.Append(namespaceDeclaration.Value);
				codeChunk.AppendLine("\");");
			}
			return codeChunk;
		}

		private static CodeChunk GenerateExtendedAttributes(OpenXmlElement element, int indent, string varName)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			foreach (OpenXmlAttribute extendedAttribute in element.ExtendedAttributes)
			{
				codeChunk.Append(new Indent(indent));
				codeChunk.Append(varName + ".SetAttribute(");
				codeChunk.Append(Keyword.New, new Separator(), new TypeMetacode(typeof(OpenXmlAttribute).Name));
				codeChunk.Append(string.Format(CultureInfo.InvariantCulture, "(\"{0}\", \"{1}\", \"{2}\", \"{3}\")", CSharpCodeGen.EscapeCSharpString(extendedAttribute.Prefix), CSharpCodeGen.EscapeCSharpString(extendedAttribute.LocalName), CSharpCodeGen.EscapeCSharpString(extendedAttribute.NamespaceUri), CSharpCodeGen.EscapeCSharpString(extendedAttribute.Value)));
				codeChunk.AppendLine(");");
			}
			return codeChunk;
		}
	}
}
