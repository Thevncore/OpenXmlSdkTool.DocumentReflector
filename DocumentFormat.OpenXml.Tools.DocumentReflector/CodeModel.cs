namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class CodeModel : ICodeModel
	{
		public virtual CodeChunk Imports
		{
			get;
			internal set;
		}

		public virtual NamespaceChunk Namespace
		{
			get;
			internal set;
		}
	}
}
