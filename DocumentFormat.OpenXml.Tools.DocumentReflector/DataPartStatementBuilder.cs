using System;
using DocumentFormat.OpenXml.Packaging;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class DataPartStatementBuilder : IElementCodeBuilder
	{
		private DataPart dataPart;

		private string paramName;

		private ReflectContext context;

		public int InitialIndent
		{
			get;
			set;
		}

		public DataPartStatementBuilder(DataPart part, ReflectContext context, string packageVariableName)
		{
			dataPart = part;
			this.context = context;
			paramName = packageVariableName;
		}

		public CodeChunk Build(out string generatedVariableName)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			int initialIndent = InitialIndent;
			codeChunk.Append(new Indent(initialIndent));
			Type type = dataPart.GetType();
			codeChunk.Append(type.Name);
			codeChunk.Append(new Separator());
			string text = context.Variables.NewVarName(type);
			codeChunk.Append(text);
			codeChunk.Append(" = ");
			codeChunk.Append(paramName);
			codeChunk.Append(".CreateMediaDataPart(\"");
			codeChunk.Append(dataPart.ContentType);
			codeChunk.Append("\"");
			codeChunk.Append(new Separator());
			string text2 = dataPart.Uri.ToString();
			int num = text2.LastIndexOf('.');
			if (num > 0 && num < text2.Length - 1)
			{
				string text3 = text2.Substring(num + 1);
				codeChunk.Append(", \"");
				codeChunk.Append(text3);
				codeChunk.Append("\"");
			}
			codeChunk.AppendLine(");");
			string fieldName = text + "Data";
			string fieldData = CSharpCodeGenHelper.EncodePart2Base64(dataPart);
			string str = context.BinaryData.InsertDataField(fieldName, ref fieldData);
			codeChunk.Append(new Indent(initialIndent));
			codeChunk.Append("System.IO.Stream ");
			string text4 = text + "Stream";
			codeChunk.Append(text4);
			codeChunk.Append(" = " + str);
			codeChunk.AppendLine(";");
			codeChunk.Append(new Indent(initialIndent));
			codeChunk.Append(text);
			codeChunk.AppendLine(".FeedData(" + text4 + ");");
			codeChunk.Append(new Indent(initialIndent));
			codeChunk.AppendLine(text4 + ".Close();");
			generatedVariableName = text;
			return codeChunk;
		}
	}
}
