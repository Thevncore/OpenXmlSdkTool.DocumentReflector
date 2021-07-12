using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal static class CSharpCodeGenHelper
	{
		public static MethodChunk BuildMethod(string methodName, IDictionary<string, string> paramList, string returnType, bool isPublic, int indent, string comment)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			if (!string.IsNullOrEmpty(comment))
			{
				codeChunk.Append(new Indent(indent));
				codeChunk.AppendLine(new Comment("// " + comment));
			}
			codeChunk.Append(new Indent(indent));
			codeChunk.Append(Keyword.Private, new Separator());
			if (string.IsNullOrEmpty(returnType))
			{
				codeChunk.Append(Keyword.Void);
			}
			else
			{
				codeChunk.Append(new TypeMetacode(returnType));
			}
			codeChunk.Append(new Separator());
			codeChunk.Append(methodName + "(");
			bool flag = true;
			foreach (KeyValuePair<string, string> param in paramList)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					codeChunk.Append(", ");
				}
				codeChunk.Append(new TypeMetacode(param.Key), new Separator());
				codeChunk.Append(param.Value);
			}
			codeChunk.AppendLine(")");
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("{");
			CodeChunk codeChunk2 = CodeChunk.CreateDefault();
			codeChunk2.Append(new Indent(indent));
			codeChunk2.AppendLine("}");
			MethodChunk methodChunk = new MethodChunk();
			methodChunk.Head = codeChunk;
			methodChunk.Body = CodeChunk.CreateDefault();
			methodChunk.End = codeChunk2;
			return methodChunk;
		}

		public static string EncodePart2Base64(OpenXmlPart part)
		{
			return Encode2Base64(part.GetStream());
		}

		public static string EncodePart2Base64(DataPart part)
		{
			return Encode2Base64(part.GetStream());
		}

		private static string Encode2Base64(Stream stream)
		{
			using BinaryReader binaryReader = new BinaryReader(stream);
			byte[] inArray = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
			return Convert.ToBase64String(inArray);
		}
	}
}
