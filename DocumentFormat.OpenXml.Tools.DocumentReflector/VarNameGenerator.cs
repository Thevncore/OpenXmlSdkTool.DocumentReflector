using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class VarNameGenerator
	{
		private List<string> existedVars = new List<string>();

		public string NewVarName(string name)
		{
			existedVars.Add(name);
			int num = existedVars.Where((string n) => n.Equals(name)).Count();
			string arg = ((name.Length == 1) ? name.ToLower(CultureInfo.InvariantCulture) : (char.ToLower(name[0], CultureInfo.InvariantCulture) + name.Substring(1)));
			return arg + num;
		}

		public string NewVarName(Type t)
		{
			return NewVarName(t.Name);
		}

		public string GetMethodName(string name)
		{
			existedVars.Add(name);
			int num = existedVars.Where((string n) => n.Equals(name)).Count();
			string arg = ((name.Length == 1) ? name.ToUpper(CultureInfo.InvariantCulture) : (char.ToUpper(name[0], CultureInfo.InvariantCulture) + name.Substring(1)));
			return arg + num;
		}

		public void Reset()
		{
			existedVars.Clear();
		}
	}
}
