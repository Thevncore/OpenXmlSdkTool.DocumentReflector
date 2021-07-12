namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class Keyword : Metacode
	{
		private const string _public = "public";

		private const string _private = "private";

		private const string _static = "static";

		private const string _void = "void";

		private const string _new = "new";

		private const string _return = "return";

		private const string _var = "var";

		private const string _namespace = "namespace";

		private const string _class = "class";

		private const string _using = "using";

		private const string _string = "string";

		public static Keyword Using => new Keyword("using");

		public static Keyword Class => new Keyword("class");

		public static Keyword Namespace => new Keyword("namespace");

		public static Keyword Public => new Keyword("public");

		public static Keyword Private => new Keyword("private");

		public static Keyword Static => new Keyword("static");

		public static Keyword Void => new Keyword("void");

		public static Keyword New => new Keyword("new");

		public static Keyword Var => new Keyword("var");

		public static Keyword Return => new Keyword("return");

		public static Keyword String => new Keyword("string");

		public Keyword(string text)
			: base(text)
		{
		}
	}
}
