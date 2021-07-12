using System;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class LineBreaker : Metacode
	{
		public override string Text
		{
			get
			{
				return Environment.NewLine;
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
