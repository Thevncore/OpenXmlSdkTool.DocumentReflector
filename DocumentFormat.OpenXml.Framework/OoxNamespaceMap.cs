using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;

namespace DocumentFormat.OpenXml.Framework
{
	public static class OoxNamespaceMap
	{
		private static readonly StringDictionary apiNamespace;

		private static readonly StringDictionary xmlNamespace;

		private static readonly StringDictionary pseudoUri2ApiNamespace;

		private static readonly StringDictionary apiNamespace2ShortNamespace;

		private static readonly XmlNamespaceManager xmlNamespaceManager;

		public static XmlNamespaceManager XmlNamespaceManager => xmlNamespaceManager;

		static OoxNamespaceMap()
		{
			apiNamespace = new StringDictionary();
			xmlNamespace = new StringDictionary();
			pseudoUri2ApiNamespace = new StringDictionary();
			apiNamespace2ShortNamespace = new StringDictionary();
			xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			LoadNamespaceMap();
		}

		public static string ApiNamespace(string xmlNamespaceUri)
		{
			if (xmlNamespaceUri == null || xmlNamespaceUri.Length <= 0)
			{
				throw new ArgumentNullException("xmlNamespaceUri");
			}
			return apiNamespace[xmlNamespaceUri];
		}

		public static string XmlNamespace(string apiNamespace)
		{
			if (apiNamespace == null || apiNamespace.Length <= 0)
			{
				throw new ArgumentNullException("apiNamespace");
			}
			return xmlNamespace[apiNamespace];
		}

		public static string ApiNamespace2ShortNamespace(string shortNamespace)
		{
			if (shortNamespace == null || shortNamespace.Length <= 0)
			{
				throw new ArgumentNullException(shortNamespace);
			}
			return apiNamespace2ShortNamespace[shortNamespace];
		}

		internal static string PseudoUri2ApiNamespace(string pseudoNamespace)
		{
			if (pseudoNamespace == null || pseudoNamespace.Length <= 0)
			{
				throw new ArgumentNullException("pseudoNamespace");
			}
			return pseudoUri2ApiNamespace[pseudoNamespace];
		}

		private static bool LoadNamespaceMap()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(NamespaceMapData));
			using (MemoryStream stream = new MemoryStream(Resources.NamespaceMapData))
			{
				NamespaceMapData namespaceMapData = (NamespaceMapData)xmlSerializer.Deserialize(stream);
				NamespaceMapDataMapItem[] items = namespaceMapData.Items;
				foreach (NamespaceMapDataMapItem namespaceMapDataMapItem in items)
				{
					xmlNamespace[namespaceMapDataMapItem.apiNS] = namespaceMapDataMapItem.xmlNS;
					apiNamespace[namespaceMapDataMapItem.xmlNS] = namespaceMapDataMapItem.apiNS;
					pseudoUri2ApiNamespace[namespaceMapDataMapItem.pseudoNS] = namespaceMapDataMapItem.apiNS;
					apiNamespace2ShortNamespace[namespaceMapDataMapItem.apiNS] = namespaceMapDataMapItem.shortNS;
					xmlNamespaceManager.AddNamespace(namespaceMapDataMapItem.shortNS, namespaceMapDataMapItem.xmlNS);
				}
			}
			return true;
		}
	}
}
