using System;
using System.Collections.Generic;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class DomCodeChunk : CodeChunk
	{
		private Metacode last;

		public override void Append(params Metacode[] metacodes)
		{
			foreach (Metacode newMetacode in metacodes)
			{
				Append(newMetacode);
			}
		}

		public override void Append(Metacode newMetacode)
		{
			if (newMetacode == null)
			{
				throw new ArgumentNullException("newMetacode");
			}
			if (FirstChild == null)
			{
				FirstChild = newMetacode;
			}
			else
			{
				last.Next = newMetacode;
			}
			last = FindLast(newMetacode);
		}

		public override void Append(string text)
		{
			Append(new Metacode(text));
		}

		public override void Append(CodeChunk codeChunk)
		{
			if (codeChunk == null)
			{
				throw new ArgumentNullException("codeChunk");
			}
			if (codeChunk.FirstChild != null)
			{
				Append(codeChunk.FirstChild);
			}
		}

		public override void AppendLine()
		{
			Append(new LineBreaker());
		}

		public override void AppendLine(string text)
		{
			AppendLine(new Metacode(text));
		}

		public override void AppendLine(Metacode newMetacode)
		{
			if (newMetacode == null)
			{
				throw new ArgumentNullException("newMetacode");
			}
			Append(newMetacode);
			Append(new LineBreaker());
		}

		private static Metacode FindLast(Metacode start)
		{
			if (start == null)
			{
				return null;
			}
			Metacode metacode = start;
			while (metacode.Next != null)
			{
				metacode = metacode.Next;
			}
			return metacode;
		}

		public override IEnumerator<Metacode> GetEnumerator()
		{
			for (Metacode mc = FirstChild; mc != null; mc = mc.Next)
			{
				yield return mc;
			}
		}
	}
}
