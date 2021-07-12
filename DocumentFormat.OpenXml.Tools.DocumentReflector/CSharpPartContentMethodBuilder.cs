using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class CSharpPartContentMethodBuilder : IPartContentMethodBuilder
	{
		private object _part;

		public int InitialIndent
		{
			get;
			set;
		}

		public string MethodName
		{
			get;
			set;
		}

		public string ParamName
		{
			get;
			set;
		}

		public string Comment
		{
			get;
			set;
		}

		public CSharpPartContentMethodBuilder(object part)
		{
			_part = part;
		}

		public MethodChunk Build(ReflectContext context)
		{
			return BuildGeneratePartContentMethod(_part, MethodName, ParamName, Comment, context);
		}

		private MethodChunk BuildGeneratePartContentMethod(object part, string methodName, string paramName, string comment, ReflectContext context)
		{
			OpenXmlPart openXmlPart = part as OpenXmlPart;
			if (openXmlPart == null)
			{
				return BuildBinaryPartMethod(part, methodName, paramName, comment, context);
			}
			if (IsStrongTypedPart(openXmlPart))
			{
				return BuildStrongTypedPartMethod(openXmlPart, methodName, paramName, comment, context);
			}
			if (OpenXmlPackageHelper.GetOpenXmlPartType(openXmlPart) == OpenXmlPartType.RawXml || part is CoreFilePropertiesPart)
			{
				return BuildRawXmlPartMethod(openXmlPart, methodName, paramName, comment, context);
			}
			return BuildBinaryPartMethod(openXmlPart, methodName, paramName, comment, context);
		}

		private MethodChunk BuildStrongTypedPartMethod(OpenXmlPart part, string methodName, string paramName, string comment, ReflectContext context)
		{
			Type type = part.GetType();
			using (OpenXmlReader openXmlReader = OpenXmlReader.Create(part))
			{
				if (openXmlReader.Read())
				{
					OpenXmlElement openXmlElement = openXmlReader.LoadCurrentElement();
					MethodChunk methodChunk = new MethodChunk();
					CodeChunk codeChunk = CodeChunk.CreateDefault();
					if (!string.IsNullOrEmpty(comment))
					{
						codeChunk.Append(new Indent(InitialIndent));
						codeChunk.AppendLine(new Comment("// " + comment));
					}
					codeChunk.Append(new Indent(InitialIndent));
					codeChunk.Append(Keyword.Private, new Separator(), Keyword.Void, new Separator());
					codeChunk.Append(methodName + "(");
					codeChunk.Append(new TypeMetacode(type.Name), new Separator());
					codeChunk.AppendLine(paramName + ")");
					codeChunk.Append(new Indent(InitialIndent));
					codeChunk.AppendLine("{");
					int num = InitialIndent + context.IndentSize;
					IElementCodeBuilder elementCodeBuilder = CSharpCodeGenFactory.CreateElementCodeBuilder(openXmlElement, context);
					elementCodeBuilder.InitialIndent = num;
					string elementVariableName;
					CodeChunk codeChunk2 = elementCodeBuilder.Build(out elementVariableName);
					Type rootType = openXmlElement.GetType();
					PropertyInfo propertyInfo = (from p in type.GetProperties()
						where p.PropertyType == rootType
						select p).FirstOrDefault();
					codeChunk2.AppendLine();
					codeChunk2.Append(new Indent(num));
					codeChunk2.Append(paramName + "." + propertyInfo.Name + " = " + elementVariableName);
					codeChunk2.AppendLine(";");
					CodeChunk codeChunk3 = CodeChunk.CreateDefault();
					codeChunk3.Append(new Indent(InitialIndent));
					codeChunk3.AppendLine("}");
					methodChunk.Head = codeChunk;
					methodChunk.Body = codeChunk2;
					methodChunk.End = codeChunk3;
					return methodChunk;
				}
			}
			return null;
		}

		private MethodChunk BuildRawXmlPartMethod(OpenXmlPart part, string methodName, string paramName, string comment, ReflectContext context)
		{
			MethodChunk methodChunk = new MethodChunk();
			methodChunk.Head = CodeChunk.CreateDefault();
			if (!string.IsNullOrEmpty(comment))
			{
				methodChunk.Head.Append(new Indent(InitialIndent));
				methodChunk.Head.AppendLine(new Comment("// " + comment));
			}
			methodChunk.Head.Append(new Indent(InitialIndent), Keyword.Private, new Separator());
			methodChunk.Head.Append(Keyword.Void, new Separator());
			methodChunk.Head.Append(methodName + "(");
			methodChunk.Head.Append(new TypeMetacode(part.GetType().Name), new Separator());
			methodChunk.Head.AppendLine(paramName + ")");
			methodChunk.Head.Append(new Indent(InitialIndent));
			methodChunk.Head.AppendLine(new Metacode("{"));
			methodChunk.End = CodeChunk.CreateDefault();
			methodChunk.End.Append(new Indent(InitialIndent));
			methodChunk.End.AppendLine("}");
			int indentChars = InitialIndent + context.IndentSize;
			methodChunk.Body = CodeChunk.CreateDefault();
			methodChunk.Body.Append(new Indent(indentChars));
			string text = string.Empty;
			string text2 = "System.Xml.XmlTextWriter";
			using (StreamReader streamReader = new StreamReader(part.GetStream()))
			{
				text = CSharpCodeGen.EscapeCSharpString(streamReader.ReadToEnd());
			}
			methodChunk.Body.Append(text2 + " writer = new " + text2 + "(");
			methodChunk.Body.AppendLine(paramName + ".GetStream(System.IO.FileMode.Create), System.Text.Encoding.UTF8);");
			methodChunk.Body.Append(new Indent(indentChars));
			methodChunk.Body.Append("writer.WriteRaw(\"");
			methodChunk.Body.Append(text);
			methodChunk.Body.AppendLine("\");");
			methodChunk.Body.Append(new Indent(indentChars));
			methodChunk.Body.AppendLine("writer.Flush();");
			methodChunk.Body.Append(new Indent(indentChars));
			methodChunk.Body.AppendLine("writer.Close();");
			return methodChunk;
		}

		private MethodChunk BuildBinaryPartMethod(object partObj, string methodName, string paramName, string comment, ReflectContext context)
		{
			Type type = partObj.GetType();
			string fieldName = paramName + "Data";
			Stream stream = null;
			OpenXmlPart openXmlPart = partObj as OpenXmlPart;
			stream = ((openXmlPart == null) ? (partObj as DataPart).GetStream() : openXmlPart.GetStream());
			string text;
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				byte[] inArray = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				string fieldData = Convert.ToBase64String(inArray);
				text = context.BinaryData.InsertDataField(fieldName, ref fieldData);
			}
			MethodChunk methodChunk = new MethodChunk();
			methodChunk.Head = CodeChunk.CreateDefault();
			if (!string.IsNullOrEmpty(comment))
			{
				methodChunk.Head.Append(new Indent(InitialIndent));
				methodChunk.Head.AppendLine(new Comment("// " + comment));
			}
			methodChunk.Head.Append(new Indent(InitialIndent), Keyword.Private, new Separator());
			methodChunk.Head.Append(Keyword.Void, new Separator());
			methodChunk.Head.Append(methodName + "(");
			methodChunk.Head.Append(new TypeMetacode(type.Name), new Separator());
			methodChunk.Head.AppendLine(paramName + ")");
			methodChunk.Head.Append(new Indent(InitialIndent));
			methodChunk.Head.AppendLine(new Metacode("{"));
			methodChunk.End = CodeChunk.CreateDefault();
			methodChunk.End.Append(new Indent(InitialIndent));
			methodChunk.End.AppendLine("}");
			int indentChars = context.IndentSize + InitialIndent;
			methodChunk.Body = CodeChunk.CreateDefault();
			methodChunk.Body.Append(new Indent(indentChars));
			methodChunk.Body.Append("System.IO.Stream data = ");
			methodChunk.Body.Append(text);
			methodChunk.Body.AppendLine(";");
			methodChunk.Body.Append(new Indent(indentChars));
			methodChunk.Body.Append(paramName);
			methodChunk.Body.AppendLine(".FeedData(data);");
			methodChunk.Body.Append(new Indent(indentChars));
			methodChunk.Body.AppendLine("data.Close();");
			return methodChunk;
		}

		private static bool IsStrongTypedPart(OpenXmlPart part)
		{
			if (part == null)
			{
				return false;
			}
			if (OpenXmlPackageHelper.GetOpenXmlPartType(part) == OpenXmlPartType.StrongTyped)
			{
				return !(part is CoreFilePropertiesPart);
			}
			return false;
		}
	}
}
