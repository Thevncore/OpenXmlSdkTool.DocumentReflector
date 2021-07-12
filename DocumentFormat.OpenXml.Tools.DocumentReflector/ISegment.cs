namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public interface ISegment
	{
		string Text
		{
			get;
			set;
		}

		object Tag
		{
			get;
			set;
		}
	}
}
