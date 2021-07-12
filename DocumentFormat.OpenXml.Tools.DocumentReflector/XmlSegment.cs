namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class XmlSegment : ISegment
	{
		private OpenXmlElement _element;

		public string Text
		{
			get;
			set;
		}

		public object Tag
		{
			get
			{
				return _element;
			}
			set
			{
				_element = value as OpenXmlElement;
			}
		}
	}
}
