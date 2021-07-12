using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DocumentFormat.OpenXml.Framework
{
	[Serializable]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[GeneratedCode("xsd", "2.0.50727.42")]
	public class NamespaceMapDataMapItem
	{
		private string pseudoNSField;

		private string shortNSField;

		private string xmlNSField;

		private string apiNSField;

		[XmlAttribute]
		public string pseudoNS
		{
			get
			{
				return pseudoNSField;
			}
			set
			{
				pseudoNSField = value;
			}
		}

		[XmlAttribute]
		public string shortNS
		{
			get
			{
				return shortNSField;
			}
			set
			{
				shortNSField = value;
			}
		}

		[XmlAttribute]
		public string xmlNS
		{
			get
			{
				return xmlNSField;
			}
			set
			{
				xmlNSField = value;
			}
		}

		[XmlAttribute]
		public string apiNS
		{
			get
			{
				return apiNSField;
			}
			set
			{
				apiNSField = value;
			}
		}
	}
}
