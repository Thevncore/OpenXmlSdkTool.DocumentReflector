using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class CodeDocument : BaseDocument
	{
		private ICodeModel _codeDom;

		public CodeDocument(ICodeModel codeDom)
		{
			if (codeDom == null)
			{
				throw new ArgumentNullException("codeDom");
			}
			_codeDom = codeDom;
		}

		public override IEnumerable<ILine> Lines()
		{
			foreach (DefaultLine item in Read(_codeDom.Imports))
			{
				yield return item;
			}
			yield return GetBlankLine();
			foreach (DefaultLine item2 in Read(_codeDom.Namespace))
			{
				yield return item2;
			}
		}

		private static DefaultLine GetBlankLine()
		{
			DefaultLine defaultLine = new DefaultLine();
			defaultLine.Append(new CodeSegment(new LineBreaker()));
			return defaultLine;
		}

		private static IEnumerable<DefaultLine> Read(NamespaceChunk chunk)
		{
			foreach (DefaultLine item in Read(chunk.Head))
			{
				yield return item;
			}
			foreach (DefaultLine item2 in Read(chunk.Body))
			{
				yield return item2;
			}
			foreach (DefaultLine item3 in Read(chunk.End))
			{
				yield return item3;
			}
		}

		private static IEnumerable<DefaultLine> Read(ClassChunk chunk)
		{
			foreach (DefaultLine item in Read(chunk.Head))
			{
				yield return item;
			}
			for (MethodChunk i = chunk.Body; i != null; i = i.Next)
			{
				foreach (DefaultLine item2 in Read(i))
				{
					yield return item2;
				}
				yield return GetBlankLine();
			}
			foreach (DefaultLine item3 in Read(chunk.End))
			{
				yield return item3;
			}
		}

		private static IEnumerable<DefaultLine> Read(MethodChunk chunk)
		{
			return Read(chunk.Head).Concat(Read(chunk.Body).Concat(Read(chunk.End)));
		}

		private static IEnumerable<DefaultLine> Read(CodeChunk chunk)
		{
			DefaultLine line = new DefaultLine();
			for (Metacode mc = chunk.FirstChild; mc != null; mc = mc.Next)
			{
				if (mc is LineBreaker)
				{
					yield return line;
					line = new DefaultLine();
				}
				line.Append(new CodeSegment(mc));
			}
			yield return line;
		}
	}
}
