namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class ReflectContext
	{
		public int IndentSize
		{
			get;
			set;
		}

		public NamespaceCollector UsedNamespaces
		{
			get;
			private set;
		}

		public VarNameGenerator Variables
		{
			get;
			private set;
		}

		public BinaryDataCache BinaryData
		{
			get;
			private set;
		}

		public ReflectContext()
			: this(new NamespaceCollector(), new VarNameGenerator(), new BinaryDataCache())
		{
		}

		public ReflectContext(NamespaceCollector ns)
			: this(ns, new VarNameGenerator(), new BinaryDataCache())
		{
		}

		public ReflectContext(NamespaceCollector ns, VarNameGenerator varGen, BinaryDataCache binData)
		{
			UsedNamespaces = ns;
			Variables = varGen;
			BinaryData = binData;
		}

		public ReflectContext(ReflectContext src)
		{
			IndentSize = src.IndentSize;
			UsedNamespaces = src.UsedNamespaces;
			BinaryData = src.BinaryData;
			Variables = src.Variables;
		}
	}
}
