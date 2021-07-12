using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	public class ElementXmlDocument : BaseDocument
	{
		private List<DefaultLine> _lines;

		public ElementXmlDocument(OpenXmlElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			Preread(element);
		}

		private void Preread(OpenXmlElement element)
		{
			string outerXml = FormatXml(element.OuterXml);
			string[] lines = SeperateToLines(outerXml);
			_lines = GetModelLines(lines);
		}

		private static List<DefaultLine> GetModelLines(string[] lines)
		{
			List<DefaultLine> list = new List<DefaultLine>();
			foreach (string text in lines)
			{
				DefaultLine defaultLine = new DefaultLine();
				defaultLine.Append(new XmlSegment
				{
					Text = text,
					Tag = null
				});
				list.Add(defaultLine);
			}
			return list;
		}

		private static List<DefaultLine> GetModelLines(OpenXmlElement element, string[] lines)
		{
			bool isStartTag = true;
			List<DefaultLine> list = new List<DefaultLine>();
			foreach (string text in lines)
			{
				DefaultLine defaultLine = new DefaultLine();
				defaultLine.Append(new XmlSegment
				{
					Text = text,
					Tag = element
				});
				list.Add(defaultLine);
				element = GetNextElement(element, ref isStartTag);
			}
			return list;
		}

		private static OpenXmlElement GetNextElement(OpenXmlElement currentElement, ref bool isStartTag)
		{
			if (isStartTag && currentElement.FirstChild != null && !(currentElement is Text))
			{
				return currentElement.FirstChild;
			}
			if (currentElement.NextSibling() != null)
			{
				isStartTag = true;
				return currentElement.NextSibling();
			}
			isStartTag = false;
			return currentElement.Parent;
		}

		private static string FormatXml(string outerXml)
		{
			if (string.IsNullOrEmpty(outerXml))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.XmlResolver = null;
			StringReader input = new StringReader(outerXml);
			XmlReader reader = XmlReader.Create(input);
			xmlDocument.Load(reader);
			using XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(stringBuilder, CultureInfo.CurrentCulture));
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlDocument.WriteTo(xmlTextWriter);
			return stringBuilder.ToString();
		}

		private static string[] SeperateToLines(string outerXml)
		{
			return outerXml.Split('\n');
		}

		public override IEnumerable<ILine> Lines()
		{
			if (_lines == null)
			{
				yield break;
			}
			foreach (DefaultLine line in _lines)
			{
				yield return line;
			}
		}
	}
}
