using System.Collections.Generic;
using System.Globalization;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class BinaryDataCache
	{
		private IDictionary<string, string> _dataFields = new Dictionary<string, string>();

		private readonly string _getBinaryDataMethodName = "GetBinaryDataStream";

		public IDictionary<string, string> DataFields => _dataFields;

		public string GetBinaryDataMethodName => _getBinaryDataMethodName;

		public string InsertDataField(string fieldName, ref string fieldData)
		{
			_dataFields.Add(fieldName, fieldData);
			return string.Format(CultureInfo.InvariantCulture, "{0}({1})", new object[2]
			{
				GetBinaryDataMethodName,
				fieldName
			});
		}

		public CodeChunk GenerateBinaryDataRegion(int initIndent, int indentDelta)
		{
			if (_dataFields.Count == 0)
			{
				return null;
			}
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(initIndent));
			codeChunk.AppendLine("#region Binary Data");
			codeChunk.Append(GenerateDataFieldsCode(DataFields, initIndent));
			MethodChunk methodChunk = GenerateGetDataStreamMethod(initIndent, indentDelta);
			codeChunk.Append(methodChunk.Head);
			codeChunk.Append(methodChunk.Body);
			codeChunk.Append(methodChunk.End);
			codeChunk.AppendLine();
			codeChunk.Append(new Indent(initIndent));
			codeChunk.AppendLine("#endregion");
			return codeChunk;
		}

		private static CodeChunk GenerateDataFieldsCode(IDictionary<string, string> dataFields, int initIndent)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			foreach (KeyValuePair<string, string> dataField in dataFields)
			{
				codeChunk.Append(new Indent(initIndent), Keyword.Private, new Separator(), Keyword.String, new Separator());
				codeChunk.Append(dataField.Key);
				codeChunk.Append(" = \"");
				codeChunk.Append(dataField.Value);
				codeChunk.AppendLine("\";");
				codeChunk.AppendLine();
			}
			return codeChunk;
		}

		private MethodChunk GenerateGetDataStreamMethod(int initIndent, int indentDelta)
		{
			MethodChunk methodChunk = new MethodChunk();
			int num = initIndent;
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(num));
			codeChunk.Append(Keyword.Private, new Separator());
			codeChunk.Append("System.IO.Stream ");
			codeChunk.Append(GetBinaryDataMethodName + "(");
			codeChunk.Append(Keyword.String, new Separator());
			codeChunk.Append("base64String");
			codeChunk.AppendLine(")");
			codeChunk.Append(new Indent(num));
			codeChunk.AppendLine("{");
			num += indentDelta;
			CodeChunk codeChunk2 = CodeChunk.CreateDefault();
			codeChunk2.Append(new Indent(num));
			codeChunk2.Append(Keyword.Return, new Separator(), Keyword.New, new Separator());
			codeChunk2.AppendLine("System.IO.MemoryStream(System.Convert.FromBase64String(base64String));");
			num -= indentDelta;
			CodeChunk codeChunk3 = CodeChunk.CreateDefault();
			codeChunk3.Append(new Indent(num));
			codeChunk3.AppendLine("}");
			methodChunk.Head = codeChunk;
			methodChunk.Body = codeChunk2;
			methodChunk.End = codeChunk3;
			return methodChunk;
		}
	}
}
