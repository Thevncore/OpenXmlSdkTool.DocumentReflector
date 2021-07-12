namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public interface ICodeModel
	{
		CodeChunk Imports
		{
			get;
		}

		NamespaceChunk Namespace
		{
			get;
		}
	}
}
