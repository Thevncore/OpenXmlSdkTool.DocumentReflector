using System;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class Separator : Metacode
	{
		private const string WhiteSpace = " ";

		public override string Text
		{
			get
			{
				return " ";
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
