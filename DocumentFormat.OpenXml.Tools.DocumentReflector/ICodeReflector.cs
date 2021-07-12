using DocumentFormat.OpenXml.Packaging;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public interface ICodeReflector
	{
		ICodeModel Reflect(OpenXmlPackage package);

		ICodeModel Reflect(OpenXmlPart part);

		ICodeModel Reflect(OpenXmlElement element);
	}
}
