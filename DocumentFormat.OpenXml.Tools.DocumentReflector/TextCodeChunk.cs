using System.Collections.Generic;
using System.Text;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class TextCodeChunk : CodeChunk
	{
		private const string _lineBreaker = "\r\n";

		private Metacode _firstChild;

		private StringBuilder _builder;

		public override Metacode FirstChild
		{
			get
			{
				if (_firstChild == null)
				{
					_firstChild = new Metacode();
				}
				_firstChild.Text = _builder.ToString();
				return _firstChild;
			}
		}

		public TextCodeChunk()
		{
			_builder = new StringBuilder();
		}

		public override void Append(string text)
		{
			_builder.Append(text);
		}

		public override void Append(Metacode newMetacode)
		{
			_builder.Append(newMetacode.Text);
		}

		public override void Append(CodeChunk codeChunk)
		{
			foreach (Metacode item in codeChunk)
			{
				_builder.Append(item.Text);
			}
		}

		public override void Append(params Metacode[] metacodes)
		{
			foreach (Metacode metacode in metacodes)
			{
				_builder.Append(metacode.Text);
			}
		}

		public override void AppendLine()
		{
			Append("\r\n");
		}

		public override void AppendLine(string text)
		{
			Append(text);
			AppendLine();
		}

		public override void AppendLine(Metacode newMetacode)
		{
			Append(newMetacode);
			AppendLine();
		}

		public override IEnumerator<Metacode> GetEnumerator()
		{
			yield return FirstChild;
		}
	}
}
