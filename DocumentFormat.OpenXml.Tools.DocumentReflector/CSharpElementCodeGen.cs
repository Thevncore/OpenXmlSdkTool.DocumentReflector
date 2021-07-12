using System;
using System.Globalization;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal sealed class CSharpElementCodeGen : CSharpCodeGen
	{
		private OpenXmlElement _element;

		public CSharpElementCodeGen(OpenXmlElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			_element = element;
			base.GlobalContext.UsedNamespaces.NoAliasNamespaces.Add(element.GetType().Namespace);
		}

		protected override MethodChunk GenerateEntryMethod(int initIndent)
		{
			MethodChunk methodChunk = new MethodChunk();
			methodChunk.Head = BuildMethodHead(_element, initIndent, base.GlobalContext);
			methodChunk.Body = BuildMethodBody(_element, initIndent + 4, base.GlobalContext.UsedNamespaces);
			methodChunk.End = BuildMethodEnd(initIndent);
			return methodChunk;
		}

		private static CodeChunk BuildMethodHead(OpenXmlElement element, int indent, ReflectContext context)
		{
			Type type = element.GetType();
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine(new Comment("// " + string.Format(CultureInfo.InvariantCulture, DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ElementCodeEntryComment, new object[1]
			{
				type.Name
			})));
			codeChunk.Append(new Indent(indent));
			codeChunk.Append(Keyword.Public, new Separator());
			codeChunk.Append(new Metacode(context.UsedNamespaces.GetAliasWithDot(type.Namespace)), new TypeMetacode(type.Name));
			codeChunk.Append(" Generate" + type.Name);
			codeChunk.AppendLine("()");
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("{");
			return codeChunk;
		}

		private static CodeChunk BuildMethodEnd(int indent)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("}");
			return codeChunk;
		}

		private static CodeChunk BuildMethodBody(OpenXmlElement element, int indent, NamespaceCollector ns)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			Type type = element.GetType();
			CSharpCodeGen.CollectNamespace(ns, type);
			IElementCodeBuilder elementCodeBuilder = CSharpCodeGenFactory.CreateElementCodeBuilder(element, new ReflectContext(ns)
			{
				IndentSize = 4
			});
			elementCodeBuilder.InitialIndent = indent;
			codeChunk.Append(elementCodeBuilder.Build(out var elementVariableName));
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("return " + elementVariableName + ";");
			return codeChunk;
		}
	}
}
