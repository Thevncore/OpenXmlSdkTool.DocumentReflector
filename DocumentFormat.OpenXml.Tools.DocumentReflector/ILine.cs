using System.Collections;
using System.Collections.Generic;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public interface ILine : IEnumerable<ISegment>, IEnumerable
	{
		IEnumerable<ISegment> Segments();

		void Append(ISegment segment);

		void Insert(ISegment segment, int index);

		void Remove(int index);

		string GetText();
	}
}
