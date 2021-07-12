namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal interface IElementCodeBuilder
	{
		int InitialIndent
		{
			get;
			set;
		}

		CodeChunk Build(out string elementVariableName);
	}
}
