using System;
using System.Collections.Generic;
using System.Globalization;
using DocumentFormat.OpenXml.Framework;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class NamespaceCollector
	{
		private List<string> _namespaces = new List<string>();

		public IList<string> _noAliasNamespace = new List<string>(new string[3]
		{
			"DocumentFormat.OpenXml",
			"DocumentFormat.OpenXml.Packaging",
			"System.Linq"
		});

		public IList<string> NoAliasNamespaces => _noAliasNamespace;

		public NamespaceCollector()
		{
		}

		public NamespaceCollector(string mainNamespace)
			: this()
		{
			_noAliasNamespace.Add(mainNamespace);
		}

		public IEnumerable<string> GetNamespaces()
		{
			foreach (string ns in _namespaces)
			{
				string alias = GetAlias(ns);
				yield return string.IsNullOrEmpty(alias) ? ns : $"{alias} = {ns}";
			}
		}

		public void Reset()
		{
			_namespaces.Clear();
		}

		public static string GetMainNamespace(OpenXmlPackage package)
		{
			if (package is WordprocessingDocument)
			{
				return Settings.Default.WordprocessingNamespace;
			}
			if (package is PresentationDocument)
			{
				return Settings.Default.PresentationNamespace;
			}
			if (package is SpreadsheetDocument)
			{
				return Settings.Default.SpreadsheetNamespace;
			}
			return null;
		}

		public void Collect(Type type)
		{
			if (!_namespaces.Contains(type.Namespace))
			{
				_namespaces.Add(type.Namespace);
			}
		}

		public void Collect(string ns)
		{
			if (!_namespaces.Contains(ns))
			{
				_namespaces.Add(ns);
			}
		}

		public string GetAlias(string ns)
		{
			if (_noAliasNamespace.Contains(ns))
			{
				return string.Empty;
			}
			try
			{
				string text = OoxNamespaceMap.ApiNamespace2ShortNamespace(ns);
				return (text.Length == 1) ? text.ToUpper(CultureInfo.InvariantCulture) : (char.ToUpper(text[0], CultureInfo.InvariantCulture) + text.Substring(1));
			}
			catch (KeyNotFoundException)
			{
				int num = ns.LastIndexOf('.');
				if (num >= 0)
				{
					return ns.Substring(num + 1);
				}
				return ns;
			}
		}

		public string GetAliasWithDot(string ns)
		{
			string alias = GetAlias(ns);
			if (!string.IsNullOrEmpty(alias))
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.", new object[1]
				{
					alias
				});
			}
			return string.Empty;
		}
	}
}
