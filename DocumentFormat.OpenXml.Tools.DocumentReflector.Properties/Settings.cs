using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
	[CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default => defaultInstance;

		[DebuggerNonUserCode]
		[UserScopedSetting]
		[DefaultSettingValue("Microsoft Sans Serif, 8.25pt")]
		public Font Font
		{
			get
			{
				return (Font)this["Font"];
			}
			set
			{
				this["Font"] = value;
			}
		}

		[DefaultSettingValue("Value")]
		[DebuggerNonUserCode]
		[ApplicationScopedSetting]
		public string EnumStringAttributeValuePropertyName => (string)this["EnumStringAttributeValuePropertyName"];

		[DebuggerNonUserCode]
		[UserScopedSetting]
		public StringCollection FilterElementType
		{
			get
			{
				return (StringCollection)this["FilterElementType"];
			}
			set
			{
				this["FilterElementType"] = value;
			}
		}

		[DebuggerNonUserCode]
		[ApplicationScopedSetting]
		[DefaultSettingValue("Gray")]
		public Color FilteredNodeColor => (Color)this["FilteredNodeColor"];

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("DocumentFormat.OpenXml.Wordprocessing")]
		public string WordprocessingNamespace => (string)this["WordprocessingNamespace"];

		[ApplicationScopedSetting]
		[DefaultSettingValue("DocumentFormat.OpenXml.Presentation")]
		[DebuggerNonUserCode]
		public string PresentationNamespace => (string)this["PresentationNamespace"];

		[DefaultSettingValue("DocumentFormat.OpenXml.Spreadsheet")]
		[DebuggerNonUserCode]
		[ApplicationScopedSetting]
		public string SpreadsheetNamespace => (string)this["SpreadsheetNamespace"];

		[UserScopedSetting]
		[DefaultSettingValue("Control,F")]
		[DebuggerNonUserCode]
		public string FindShortKey
		{
			get
			{
				return (string)this["FindShortKey"];
			}
			set
			{
				this["FindShortKey"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("DocumentFormat.OpenXml.EnumStringAttribute")]
		[ApplicationScopedSetting]
		public string EnumStringAttributeTypeName => (string)this["EnumStringAttributeTypeName"];
	}
}
