using System;
using System.Text;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class Indent : Metacode
	{
		private const string WhiteSpace = " ";

		public int IndentChars
		{
			get;
			set;
		}

		public override string Text
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < IndentChars; i++)
				{
					stringBuilder.Append(" ");
				}
				return stringBuilder.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public Indent()
		{
			IndentChars = 4;
		}

		public Indent(int indentChars)
		{
			IndentChars = indentChars;
		}
	}
}
