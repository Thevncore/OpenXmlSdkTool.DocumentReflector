using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public interface IDocument : IEnumerable<ILine>, IEnumerable
	{
		Collection<IDocumentListener> Listeners
		{
			get;
		}

		IEnumerable<ILine> Lines();

		void Notify();
	}
}
