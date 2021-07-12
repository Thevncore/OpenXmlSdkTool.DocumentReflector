using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Commands;
using DocumentFormat.OpenXml.Tools.DocumentReflector.Properties;
using DocumentFormat.OpenXml.Tools.DocumentReflector.View;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector
{
	internal class DocumentReflector : IToolAddin
	{
		private const string _actionMenuId = "Actions";

		private const string _settingsMenuId = "Settings";

		private const string _reflectMenuItemId = "Reflect";

		private const string _packageInspectorTabId = "PackageInspector";

		internal const string ReflectorWindowId = "DocumentReflectorWindow";

		private ICommandMenuItem _menuItem;

		private ICommandToolbarItem _toolbarItem;

		private ICommand _reflectCommand;

		private IPackageObject _lastReflectedNode;

		private Type[] _typesCanReflect = new Type[4]
		{
			typeof(IPackageDocument),
			typeof(IPackageElement),
			typeof(IPackageDataPart),
			typeof(IPackagePart)
		};

		public DocumentReflector()
		{
			_reflectCommand = new ReflectCommand(this);
		}

		public void Load()
		{
			if (ToolSingleton.Instance.Services.GetService<IPackageService>() != null)
			{
				AddCommandBarItems();
				RegisterHotKeys();
				HookContextMenu();
				HandleActiveToolPadChanged();
				HandleActiveWindowChanged();
				HandlePackageServiceEvents();
				HandleFontSettingChanged();
			}
		}

		internal IWorkbenchWindow GetOrCreateReflectorWindow()
		{
			IWorkbenchWindow w = ToolSingleton.Instance.Workbench.Windows["DocumentReflectorWindow"];
			if (w == null)
			{
				w = ToolSingleton.Instance.Workbench.Windows.Add("DocumentReflectorWindow", DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectWindowText, null);
				w.Closed += delegate
				{
					(w.Content as ReflectorControlWpfAdapter).Reflector.CancelReflecting();
				};
				HandleLockedChanged(w);
			}
			return w;
		}

		private void AddCommandBarItems()
		{
			ICommandBar<ICommandMenuItem> menu = ToolSingleton.Instance.Workbench.Menu;
			if (menu.Items.Contains("Actions"))
			{
				_menuItem = menu.Items["Actions"].Items.AddItem("Reflect", DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectMenuItemHeader, _reflectCommand);
				_menuItem.ShortcutKeyText = "Ctrl+Alt+R";
				_menuItem.Icon = DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectIcon;
			}
			ICommandBar<ICommandToolbarItem> toolbar = ToolSingleton.Instance.Workbench.Toolbar;
			_toolbarItem = toolbar.Items.AddItem("Reflect", DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectToolbarItemHeader, _reflectCommand);
			_toolbarItem.ToolTip = DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectToolbarItemTooltip;
			_toolbarItem.Icon = DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectIcon;
			ChangeCommandBarItemsEnabled();
			if (menu.Items.Contains("Settings"))
			{
				menu.Items["Settings"].Items.AddItem("FontSetting", DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.FontSettingMenuHeader, new FontSettingCommand());
			}
		}

		private void RegisterHotKeys()
		{
			IKeyBindingService service = ToolSingleton.Instance.Services.GetService<IKeyBindingService>();
			if (service != null)
			{
				service.Add("R", "Control,Alt", _reflectCommand);
				service.Add("F", "Control", new FindCommand());
			}
		}

		private void HookContextMenu()
		{
			IContextService service = ToolSingleton.Instance.Services.GetService<IContextService>();
			if (service == null)
			{
				return;
			}
			IContextProvider contextProvider = service.Providers.Where(delegate(IContextProvider e)
			{
				Type[] typesCanReflect = _typesCanReflect;
				foreach (Type value in typesCanReflect)
				{
					if (e.SupportedTypes.Contains(value))
					{
						return true;
					}
				}
				return false;
			}).FirstOrDefault();
			if (contextProvider == null)
			{
				return;
			}
			contextProvider.RequestingContextData += delegate(object sender, RequestingContextDataEventArgs e)
			{
				if (CanReflect(e.Context))
				{
					e.ContextMenu.Items.AddItem("Reflect", DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectMenuItemHeader, _reflectCommand).Icon = DocumentFormat.OpenXml.Tools.DocumentReflector.Properties.Resources.ReflectIcon;
				}
			};
		}

		private bool CanReflect(object context)
		{
			if (context == null)
			{
				return false;
			}
			Type type = context.GetType();
			Type[] typesCanReflect = _typesCanReflect;
			foreach (Type type2 in typesCanReflect)
			{
				if (type.GetInterface(type2.Name) != null)
				{
					return true;
				}
			}
			return false;
		}

		[Conditional("DEBUG")]
		private void HandleToolLoaded()
		{
			ToolSingleton.Instance.ToolLoaded += delegate(object sender, ToolLoadedEventArgs e)
			{
				for (int i = 1; i < e.Args.Length; i++)
				{
					if (e.Args[i] == "-r" || e.Args[i] == "/r")
					{
						if (_reflectCommand != null)
						{
							_reflectCommand.Execute(null);
						}
						break;
					}
				}
			};
		}

		private void HandleActiveWindowChanged()
		{
			ToolSingleton.Instance.Workbench.Windows.ActiveWindowChanged += delegate
			{
				ChangeCommandBarItemsEnabled();
				TriggerReflect();
			};
		}

		private void HandleActiveToolPadChanged()
		{
			ToolSingleton.Instance.Workbench.ToolPads.ActiveWindowChanged += delegate
			{
				ChangeCommandBarItemsEnabled();
				TriggerReflect();
			};
		}

		private void HandlePackageServiceEvents()
		{
			IPackageService packageService = ToolSingleton.Instance.Services.GetService<IPackageService>();
			packageService.SelectedItemChanged += delegate
			{
				ChangeCommandBarItemsEnabled();
				TriggerReflect();
			};
			packageService.FileClosing += delegate
			{
				IWorkbenchWindow workbenchWindow = ToolSingleton.Instance.Workbench.Windows["DocumentReflectorWindow"];
				if (workbenchWindow != null)
				{
					ReflectorControlWpfAdapter reflectorControlWpfAdapter = workbenchWindow.Content as ReflectorControlWpfAdapter;
					reflectorControlWpfAdapter.Reflector.CancelReflecting();
					ToolSingleton.Instance.Workbench.Windows.Remove(workbenchWindow);
				}
			};
		}

		private void HandleLockedChanged(IWorkbenchWindow window)
		{
			window.LockedChanged += delegate
			{
				TriggerReflect();
			};
		}

		private void TriggerReflect()
		{
			IPackageService service = ToolSingleton.Instance.Services.GetService<IPackageService>();
			if (service.SelectedItem != null && IsReflectAgainNeeded())
			{
				_reflectCommand.Execute(null);
				_lastReflectedNode = service.SelectedItem;
			}
		}

		private bool IsReflectAgainNeeded()
		{
			IWorkbenchWindow workbenchWindow = ToolSingleton.Instance.Workbench.Windows["DocumentReflectorWindow"];
			bool flag = workbenchWindow != null && workbenchWindow == ToolSingleton.Instance.Workbench.Windows.ActiveWindow;
			bool flag2 = workbenchWindow != null && workbenchWindow.Lockable && !workbenchWindow.Locked;
			bool flag3 = ToolSingleton.Instance.Workbench.ToolPads.ActiveWindow.ID == "PackageInspector";
			IPackageService service = ToolSingleton.Instance.Services.GetService<IPackageService>();
			bool result = service.SelectedItem != _lastReflectedNode;
			if (flag && flag2 && flag3)
			{
				return result;
			}
			return false;
		}

		private void ChangeCommandBarItemsEnabled()
		{
			if (ToolSingleton.Instance.Workbench.ToolPads.ActiveWindow != null && ToolSingleton.Instance.Workbench.ToolPads.ActiveWindow.ID == "PackageInspector" && ToolSingleton.Instance.Services.GetService<IPackageService>().SelectedItem != null)
			{
				if (_menuItem != null)
				{
					_menuItem.Enabled = true;
				}
				if (_toolbarItem != null)
				{
					_toolbarItem.Enabled = true;
				}
			}
			else
			{
				if (_menuItem != null)
				{
					_menuItem.Enabled = false;
				}
				if (_toolbarItem != null)
				{
					_toolbarItem.Enabled = false;
				}
			}
		}

		private static void HandleFontSettingChanged()
		{
			IToolSettingService settingService = ToolSingleton.Instance.Services.GetService<IToolSettingService>();
			if (settingService == null)
			{
				return;
			}
			settingService.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == "FontSetting")
				{
					IWorkbenchWindow workbenchWindow = ToolSingleton.Instance.Workbench.Windows.Where((IWorkbenchWindow w) => w.ID == "DocumentReflectorWindow").FirstOrDefault();
					if (workbenchWindow != null)
					{
						(workbenchWindow.Content as ReflectorControlWpfAdapter).Reflector.Font = settingService.FontSetting;
					}
				}
			};
		}
	}
}
