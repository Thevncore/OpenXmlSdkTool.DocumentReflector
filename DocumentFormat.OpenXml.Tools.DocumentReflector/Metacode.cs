using System.Diagnostics;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	[DebuggerDisplay("Text = {Text}")]
	public class Metacode
	{
		private string _text;

		public virtual string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public virtual Metacode Next
		{
			get;
			set;
		}

		public Metacode()
		{
		}

		public Metacode(string codeText)
		{
			_text = codeText;
		}
	}
}
