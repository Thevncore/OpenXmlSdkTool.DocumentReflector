namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public abstract class Chunk<T>
	{
		public CodeChunk Head
		{
			get;
			set;
		}

		public CodeChunk End
		{
			get;
			set;
		}

		public T Body
		{
			get;
			set;
		}
	}
}
