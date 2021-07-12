using System.ComponentModel;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;
using DocumentFormat.OpenXml.Tools.DocumentReflector.View;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class ReflectCommand : ICommand
	{
		private DocumentReflector _owner;

		public ReflectCommand(DocumentReflector owner)
		{
			_owner = owner;
		}

		public void Execute(object arg)
		{
			IPackageObject selectedPackageNode = GetSelectedPackageNode();
			if (selectedPackageNode != null)
			{
				IWorkbenchWindow orCreateReflectorWindow = _owner.GetOrCreateReflectorWindow();
				ToolSingleton.Instance.Workbench.Windows.ActiveWindow = orCreateReflectorWindow;
				if (orCreateReflectorWindow.Content == null)
				{
					orCreateReflectorWindow.Content = CreateReflectorHost();
				}
				ReflectorControl reflector = (orCreateReflectorWindow.Content as ReflectorControlWpfAdapter).Reflector;
				ShowStatus(DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectionInProgress);
				Reflect(reflector, selectedPackageNode);
			}
		}

		private static void Reflect(ReflectorControl reflector, IPackageObject item)
		{
			IPackageDocument packageDocument;
			IPackagePart packagePart;
			IPackageElement packageElement;
			IPackageDataPart packageDataPart;
			if ((packageDocument = item as IPackageDocument) != null)
			{
				reflector.ReflectAsync(packageDocument.OpenXmlPackage);
			}
			else if ((packagePart = item as IPackagePart) != null)
			{
				reflector.ReflectAsync(packagePart.OpenXmlPart);
			}
			else if ((packageElement = item as IPackageElement) != null)
			{
				reflector.ReflectAsync(packageElement.OpenXmlElement);
			}
			else if ((packageDataPart = item as IPackageDataPart) != null)
			{
				reflector.ReflectAsync(packageDataPart.DataPart);
			}
		}

		private static IPackageObject GetSelectedPackageNode()
		{
			return ToolSingleton.Instance.Services.GetService<IPackageService>()?.SelectedItem;
		}

		private static ReflectorControlWpfAdapter CreateReflectorHost()
		{
			ReflectorControlWpfAdapter host = new ReflectorControlWpfAdapter(new ReflectorControl());
			IToolSettingService service = ToolSingleton.Instance.Services.GetService<IToolSettingService>();
			if (service != null)
			{
				host.Reflector.Font = service.FontSetting;
			}
			host.Reflector.ReflectCompleted += delegate(object sender, AsyncCompletedEventArgs e)
			{
				if (e.Cancelled)
				{
					ShowStatus(DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectingCancelled);
				}
				else
				{
					if (e.Error != null)
					{
						throw e.Error;
					}
					ShowStatus(DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectComplete);
					IToolSettingService service2 = ToolSingleton.Instance.Services.GetService<IToolSettingService>();
					if (service2 != null)
					{
						host.Reflector.Font = service2.FontSetting;
					}
				}
			};
			return host;
		}

		private static void ShowStatus(string text)
		{
			ToolSingleton.Instance.Services.GetService<IPromptingMessageService>()?.ShowWindowOwnStatus(text, "DocumentReflectorWindow");
		}
	}
}
