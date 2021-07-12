using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public abstract class BaseDocument : IDocument, IEnumerable<ILine>, IEnumerable
	{
		private Collection<IDocumentListener> _listeners = new Collection<IDocumentListener>();

		public Collection<IDocumentListener> Listeners => _listeners;

		public virtual void Notify()
		{
			foreach (IDocumentListener listener in _listeners)
			{
				listener.CommitUpdate();
			}
		}

		public abstract IEnumerable<ILine> Lines();

		public IEnumerator<ILine> GetEnumerator()
		{
			return Lines().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
