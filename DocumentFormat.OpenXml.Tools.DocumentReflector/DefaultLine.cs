using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class DefaultLine : ILine, IEnumerable<ISegment>, IEnumerable
	{
		private List<ISegment> _segments = new List<ISegment>();

		public void Append(ISegment segment)
		{
			if (segment == null)
			{
				throw new ArgumentNullException("segment");
			}
			_segments.Add(segment);
		}

		public void Insert(ISegment segment, int index)
		{
			throw new NotImplementedException();
		}

		public void Remove(int index)
		{
			throw new NotImplementedException();
		}

		public string GetText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ISegment segment in _segments)
			{
				stringBuilder.Append(segment.Text);
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return GetText();
		}

		public IEnumerator<ISegment> GetEnumerator()
		{
			return Segments().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerable<ISegment> Segments()
		{
			foreach (ISegment segment in _segments)
			{
				yield return segment;
			}
		}
	}
}
