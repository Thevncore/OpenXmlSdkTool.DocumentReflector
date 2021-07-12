using System;
using System.Globalization;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal sealed class CSharpPackageCodeGen : CSharpCodeGen
	{
		private OpenXmlPackage _package;

		private readonly string genPartsMethodName = "CreateParts";

		public CSharpPackageCodeGen(OpenXmlPackage package)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}
			_package = package;
			base.GlobalContext.UsedNamespaces.NoAliasNamespaces.Add(NamespaceCollector.GetMainNamespace(package));
		}

		protected override MethodChunk GenerateEntryMethod(int initIndent)
		{
			MethodChunk methodChunk = new MethodChunk();
			methodChunk.Head = BuildMethodHead(initIndent);
			methodChunk.Body = BuildMethodBody(_package, initIndent + 4);
			methodChunk.End = BuildMethodEnd(initIndent);
			MethodChunk methodChunk2 = methodChunk;
			MethodChunk methodChunk4 = (methodChunk2.Next = BuildPartsCreationMethods(initIndent));
			return methodChunk2;
		}

		private CodeChunk BuildMethodHead(int indent)
		{
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine(new Comment("// " + string.Format(CultureInfo.InvariantCulture, DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.PackageCodeEntryComment, new object[1]
			{
				_package.GetType().Name
			})));
			codeChunk.Append(new Indent(indent));
			codeChunk.Append(Keyword.Public, new Separator());
			codeChunk.Append(Keyword.Void, new Separator());
			codeChunk.Append("CreatePackage(");
			codeChunk.Append(Keyword.String);
			codeChunk.AppendLine(" filePath)");
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

		private CodeChunk BuildMethodBody(OpenXmlPackage package, int indent)
		{
			Type type = package.GetType();
			CodeChunk codeChunk = CodeChunk.CreateDefault();
			string str = "package";
			codeChunk.Append(new Indent(indent), Keyword.Using, new Metacode("("), new TypeMetacode(type.Name), new Metacode(" " + str), new Metacode(" = "), new TypeMetacode(type.Name), new Metacode(".Create(filePath, "), new TypeMetacode(GetPackageType(package)), new Metacode("))"), new LineBreaker(), new Indent(indent), new Metacode("{"), new LineBreaker());
			codeChunk.Append(new Indent(indent + 4));
			codeChunk.Append(new Metacode(genPartsMethodName), new Metacode("(" + str + ");"));
			codeChunk.AppendLine();
			codeChunk.Append(new Indent(indent));
			codeChunk.AppendLine("}");
			return codeChunk;
		}

		private MethodChunk BuildPartsCreationMethods(int indent)
		{
			IPartContainerMethodBuilder partContainerMethodBuilder = CSharpCodeGenFactory.CreateMethodBuilder(_package);
			partContainerMethodBuilder.InitialIndent = indent;
			partContainerMethodBuilder.MethodModifier = MethodAttributes.Private;
			partContainerMethodBuilder.MethodName = genPartsMethodName;
			ReflectContext reflectContext = new ReflectContext(base.GlobalContext.UsedNamespaces, new VarNameGenerator(), base.GlobalContext.BinaryData);
			reflectContext.IndentSize = base.GlobalContext.IndentSize;
			return partContainerMethodBuilder.Build(reflectContext);
		}

		private static string GetPackageType(OpenXmlPackage package)
		{
			WordprocessingDocument wordprocessingDocument;
			if ((wordprocessingDocument = package as WordprocessingDocument) != null)
			{
				return typeof(WordprocessingDocumentType).Name + "." + wordprocessingDocument.DocumentType;
			}
			PresentationDocument presentationDocument;
			if ((presentationDocument = package as PresentationDocument) != null)
			{
				return typeof(PresentationDocumentType).Name + "." + presentationDocument.DocumentType;
			}
			SpreadsheetDocument spreadsheetDocument;
			if ((spreadsheetDocument = package as SpreadsheetDocument) != null)
			{
				return typeof(SpreadsheetDocumentType).Name + "." + spreadsheetDocument.DocumentType;
			}
			throw new InvalidOperationException("Invalid document format.");
		}
	}
}
