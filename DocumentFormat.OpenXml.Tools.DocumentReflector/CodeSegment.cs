using System;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class CodeSegment : ISegment
	{
		public string Text
		{
			get
			{
				return ((Metacode)Tag).Text;
			}
			set
			{
				((Metacode)Tag).Text = value;
			}
		}

		public object Tag
		{
			get;
			set;
		}

		public CodeSegment(Metacode code)
		{
			if (code == null)
			{
				throw new ArgumentNullException("code");
			}
			Tag = code;
		}
	}
}
