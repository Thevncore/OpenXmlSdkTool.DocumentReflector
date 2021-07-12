using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DocumentFormat.OpenXml.Framework
{
	[Serializable]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[XmlType(AnonymousType = true)]
	[DebuggerStepThrough]
	[XmlRoot(Namespace = "urn:Microsfot:Office:DocumentFormat:OpenXml:Framework:NamespaceMapData", IsNullable = false)]
	public class NamespaceMapData
	{
		private NamespaceMapDataMapItem[] itemsField;

		[XmlElement("mapItem")]
		public NamespaceMapDataMapItem[] Items
		{
			get
			{
				return itemsField;
			}
			set
			{
				itemsField = value;
			}
		}
	}
}
