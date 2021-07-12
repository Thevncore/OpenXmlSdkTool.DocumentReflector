using System.Linq;
using DocumentFormat.OpenXml.Tools.DocumentReflector.View;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector.Commands
{
	internal class FindCommand : ICommand
	{
		static FindCommand()
		{
			ToolSingleton.Instance.Workbench.Windows.ActiveWindowChanged += delegate
			{
				CloseFindWindow();
			};
		}

		private static void CloseFindWindow()
		{
			ToolSingleton.Instance.Services.GetService<IFindService>()?.TerminateFind("DocumentReflectorWindow");
		}

		public void Execute(object paramenter)
		{
			IWorkbenchWindow workbenchWindow = ToolSingleton.Instance.Workbench.Windows.Where((IWorkbenchWindow e) => e.ID == "DocumentReflectorWindow").FirstOrDefault();
			if (workbenchWindow != null && ToolSingleton.Instance.Workbench.Windows.ActiveWindow == workbenchWindow)
			{
				(workbenchWindow.Content as ReflectorControlWpfAdapter).Reflector.InvokeFind();
			}
		}
	}
}
