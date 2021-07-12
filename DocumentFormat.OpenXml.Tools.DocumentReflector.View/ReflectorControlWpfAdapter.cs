using System.Windows.Forms.Integration;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector.View
{
	internal class ReflectorControlWpfAdapter : WindowsFormsHost
	{
		public ReflectorControl Reflector => base.Child as ReflectorControl;

		public ReflectorControlWpfAdapter(ReflectorControl control)
		{
			base.Child = control;
		}
	}
}
