using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal abstract class CSharpCodeGen : ICodeGenerator
	{
		protected const int DefaultIndent = 4;

		private string _generatedNsName = "GeneratedCode";

		private string _generatedClassName = "GeneratedClass";

		private ReflectContext _globalContext;

		protected ReflectContext GlobalContext
		{
			get
			{
				if (_globalContext == null)
				{
					_globalContext = new ReflectContext();
					_globalContext.IndentSize = 4;
				}
				return _globalContext;
			}
		}

		public virtual string GeneratedNamespaceName
		{
			get
			{
				return _generatedNsName;
			}
			set
			{
				_generatedNsName = value;
			}
		}

		public virtual string GeneratedClassName
		{
			get
			{
				return _generatedClassName;
			}
			set
			{
				_generatedClassName = value;
			}
		}

		public ICodeModel Generate()
		{
			CodeModel codeModel = new CodeModel();
			codeModel.Namespace = GenerateNs();
			codeModel.Imports = TransformNamespaces(GlobalContext.UsedNamespaces);
			return codeModel;
		}

		internal static string EscapeCSharpString(string text)
		{
			if (text == null)
			{
				return string.Empty;
			}
			bool flag = false;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '\0':
				case '\a':
				case '\b':
				case '\t':
				case '\n':
				case '\v':
				case '\f':
				case '\r':
				case '"':
				case '\'':
				case '\\':
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return text;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char value in text)
			{
				int num = "'\"\\\0\a\b\f\n\r\t\v".IndexOf(value);
				if (num >= 0)
				{
					stringBuilder.Append('\\');
					stringBuilder.Append("'\"\\0abfnrtv"[num]);
				}
				else
				{
					stringBuilder.Append(value);
				}
			}
			return stringBuilder.ToString();
		}

		protected virtual MethodChunk GenerateEntryMethod(int initIndent)
		{
			return new MethodChunk();
		}

		protected static string BuildAccessModifier(MethodAttributes accessModifier)
		{
			string text = "";
			text = (((accessModifier & MethodAttributes.Public) == MethodAttributes.Public) ? "public " : (((accessModifier & MethodAttributes.Private) == MethodAttributes.Private) ? "private " : (((accessModifier & MethodAttributes.Assembly) != MethodAttributes.Assembly) ? "" : "internal ")));
			if ((accessModifier & MethodAttributes.Static) == MethodAttributes.Static)
			{
				text += "static ";
			}
			return text;
		}

		protected static void CollectNamespace(NamespaceCollector ns, Type type)
		{
			ns.Collect(type);
		}

		private NamespaceChunk GenerateNs()
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(Keyword.Namespace, new Separator());
			codeChunk.AppendLine(GeneratedNamespaceName);
			codeChunk.AppendLine("{");
			CodeChunk codeChunk2 = CodeChunk.CreateDefault();
			codeChunk2.AppendLine("}");
			NamespaceChunk namespaceChunk = new NamespaceChunk();
			namespaceChunk.Head = codeChunk;
			namespaceChunk.End = codeChunk2;
			namespaceChunk.Body = GenerateClass(4);
			return namespaceChunk;
		}

		private ClassChunk GenerateClass(int indent)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(indent));
			codeChunk.Append(Keyword.Public, new Separator());
			codeChunk.Append(Keyword.Class, new Separator());
			codeChunk.AppendLine(GeneratedClassName);
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("{");
			MethodChunk body = GenerateEntryMethod(4 + indent);
			CodeChunk codeChunk2 = CodeChunk.CreateDefault();
			CodeChunk codeChunk3 = GlobalContext.BinaryData.GenerateBinaryDataRegion(8, 4);
			if (codeChunk3 != null)
			{
				codeChunk2.Append(codeChunk3);
			}
			codeChunk2.AppendLine();
			codeChunk2.Append(new Indent(indent));
			codeChunk2.AppendLine("}");
			ClassChunk classChunk = new ClassChunk();
			classChunk.Head = codeChunk;
			classChunk.Body = body;
			classChunk.End = codeChunk2;
			return classChunk;
		}

		[Conditional("DEBUG")]
		private static void CheckIsValidCSharpName(string name)
		{
			string pattern = "[a-zA-Z][\\w|_]*";
			Regex regex = new Regex(pattern);
			regex.Match(name);
		}

		private static CodeChunk TransformNamespaces(NamespaceCollector nc)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			foreach (string @namespace in nc.GetNamespaces())
			{
				TransformNs(@namespace, codeChunk);
			}
			return codeChunk;
		}

		private static void TransformNs(string ns, CodeChunk chunk)
		{
			chunk.Append(Keyword.Using, new Separator());
			chunk.Append(ns);
			chunk.AppendLine(";");
		}
	}
}
