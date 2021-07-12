using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public abstract class CodeChunk : IEnumerable<Metacode>, IEnumerable
	{
		public virtual Metacode FirstChild
		{
			get;
			protected set;
		}

		public virtual CodeChunk Next
		{
			get;
			set;
		}

		public static CodeChunk CreateDefault()
		{
			return new DomCodeChunk();
		}

		public abstract void Append(string text);

		public abstract void Append(Metacode newMetacode);

		public abstract void Append(CodeChunk codeChunk);

		public abstract void Append(params Metacode[] metacodes);

		public abstract void AppendLine();

		public abstract void AppendLine(string text);

		public abstract void AppendLine(Metacode newMetacode);

		public abstract IEnumerator<Metacode> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (IEnumerator<Metacode> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Metacode current = enumerator.Current;
					stringBuilder.Append(current.Text);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
