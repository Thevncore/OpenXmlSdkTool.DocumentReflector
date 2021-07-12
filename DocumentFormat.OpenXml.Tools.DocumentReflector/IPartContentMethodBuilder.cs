namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal interface IPartContentMethodBuilder
	{
		int InitialIndent
		{
			get;
			set;
		}

		string MethodName
		{
			get;
			set;
		}

		string ParamName
		{
			get;
			set;
		}

		string Comment
		{
			get;
			set;
		}

		MethodChunk Build(ReflectContext context);
	}
}
