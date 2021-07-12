using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = (resourceMan = new ResourceManager("DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources", typeof(Resources).Assembly));
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string CopyAllCodeMenuItemHeader => ResourceManager.GetString("CopyAllCodeMenuItemHeader", resourceCulture);

		internal static string CopySelectionMenuItemHeader => ResourceManager.GetString("CopySelectionMenuItemHeader", resourceCulture);

		internal static string ElementCodeEntryComment => ResourceManager.GetString("ElementCodeEntryComment", resourceCulture);

		internal static string ExportCodeDialogFilter => ResourceManager.GetString("ExportCodeDialogFilter", resourceCulture);

		internal static string FileCorrupted => ResourceManager.GetString("FileCorrupted", resourceCulture);

		internal static string FillInBinaryPartComment => ResourceManager.GetString("FillInBinaryPartComment", resourceCulture);

		internal static string FindInCodeTitle => ResourceManager.GetString("FindInCodeTitle", resourceCulture);

		internal static string FindInXmlTitle => ResourceManager.GetString("FindInXmlTitle", resourceCulture);

		internal static string FindMenuItemHeader => ResourceManager.GetString("FindMenuItemHeader", resourceCulture);

		internal static string FontSettingMenuHeader => ResourceManager.GetString("FontSettingMenuHeader", resourceCulture);

		internal static byte[] NamespaceMapData
		{
			get
			{
				object @object = ResourceManager.GetObject("NamespaceMapData", resourceCulture);
				return (byte[])@object;
			}
		}

		internal static string NotFoundMessage => ResourceManager.GetString("NotFoundMessage", resourceCulture);

		internal static string PackageCodeEntryComment => ResourceManager.GetString("PackageCodeEntryComment", resourceCulture);

		internal static string PartCodeEntryComment => ResourceManager.GetString("PartCodeEntryComment", resourceCulture);

		internal static string PartCodeMethodComment => ResourceManager.GetString("PartCodeMethodComment", resourceCulture);

		internal static string PartHasWrongDataPartReferenceRelationshipComment => ResourceManager.GetString("PartHasWrongDataPartReferenceRelationshipComment", resourceCulture);

		internal static string ReflectComplete => ResourceManager.GetString("ReflectComplete", resourceCulture);

		internal static Icon ReflectIcon
		{
			get
			{
				object @object = ResourceManager.GetObject("ReflectIcon", resourceCulture);
				return (Icon)@object;
			}
		}

		internal static string ReflectingCancelled => ResourceManager.GetString("ReflectingCancelled", resourceCulture);

		internal static string ReflectionInProgress => ResourceManager.GetString("ReflectionInProgress", resourceCulture);

		internal static string ReflectMenuItemHeader => ResourceManager.GetString("ReflectMenuItemHeader", resourceCulture);

		internal static string ReflectToolbarItemHeader => ResourceManager.GetString("ReflectToolbarItemHeader", resourceCulture);

		internal static string ReflectToolbarItemTooltip => ResourceManager.GetString("ReflectToolbarItemTooltip", resourceCulture);

		internal static string ReflectWindowText => ResourceManager.GetString("ReflectWindowText", resourceCulture);

		internal Resources()
		{
		}
	}
}
