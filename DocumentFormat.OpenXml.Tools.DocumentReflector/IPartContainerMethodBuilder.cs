using System.Reflection;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal interface IPartContainerMethodBuilder
	{
		string MethodName
		{
			get;
			set;
		}

		MethodAttributes MethodModifier
		{
			get;
			set;
		}

		int InitialIndent
		{
			get;
			set;
		}

		MethodChunk Build(ReflectContext context);
	}
}
