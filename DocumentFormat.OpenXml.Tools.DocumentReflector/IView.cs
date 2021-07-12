using System;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public interface IView : IDocumentListener
	{
		IDocument Document
		{
			get;
			set;
		}

		event EventHandler Click;

		void Clear();

		void Draw();
	}
}
