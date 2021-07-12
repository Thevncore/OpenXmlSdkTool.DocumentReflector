using System;
using DocumentFormat.OpenXml.Packaging;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class FullCodeReflector : ICodeReflector
	{
		public ICodeModel Reflect(OpenXmlPackage package)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}
			CSharpPackageCodeGen cSharpPackageCodeGen = new CSharpPackageCodeGen(package);
			return cSharpPackageCodeGen.Generate();
		}

		public ICodeModel Reflect(OpenXmlPart part)
		{
			if (part == null)
			{
				throw new ArgumentNullException("part");
			}
			CSharpPartCodeGen cSharpPartCodeGen = new CSharpPartCodeGen(part);
			return cSharpPartCodeGen.Generate();
		}

		public ICodeModel Reflect(DataPart dataPart)
		{
			if (dataPart == null)
			{
				throw new ArgumentNullException("dataPart");
			}
			CSharpPartCodeGen cSharpPartCodeGen = new CSharpPartCodeGen(dataPart);
			return cSharpPartCodeGen.Generate();
		}

		public ICodeModel Reflect(OpenXmlElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			CSharpElementCodeGen cSharpElementCodeGen = new CSharpElementCodeGen(element);
			return cSharpElementCodeGen.Generate();
		}
	}
}
