using DocumentFormat.OpenXml.Packaging;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal static class CSharpCodeGenFactory
	{
		public static ICodeGenerator CreateCodeGenerator(object openXmlObject)
		{
			OpenXmlPackage package;
			if ((package = openXmlObject as OpenXmlPackage) != null)
			{
				return new CSharpPackageCodeGen(package);
			}
			OpenXmlPart part;
			if ((part = openXmlObject as OpenXmlPart) != null)
			{
				return new CSharpPartCodeGen(part);
			}
			OpenXmlElement element;
			if ((element = openXmlObject as OpenXmlElement) != null)
			{
				return new CSharpElementCodeGen(element);
			}
			return null;
		}

		public static IPartContainerMethodBuilder CreateMethodBuilder(OpenXmlPartContainer partContainer)
		{
			OpenXmlPackage package;
			if ((package = partContainer as OpenXmlPackage) != null)
			{
				return new CSharpPartCodeGen(package);
			}
			OpenXmlPart part;
			if ((part = partContainer as OpenXmlPart) != null)
			{
				return new CSharpPartCodeGen(part);
			}
			return null;
		}

		public static IPartContentMethodBuilder CreatePartContentMethodBuilder(object part)
		{
			return new CSharpPartContentMethodBuilder(part);
		}

		public static IElementCodeBuilder CreateElementCodeBuilder(OpenXmlElement element, ReflectContext context)
		{
			return new ElementCSharpCodeBuilder(element, context);
		}

		public static IElementCodeBuilder CreateDataPartCodeBuilder(DataPart part, ReflectContext context, string packageVariable)
		{
			return new DataPartStatementBuilder(part, context, packageVariable);
		}
	}
}
