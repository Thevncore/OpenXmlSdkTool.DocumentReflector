using System.Windows.Forms;

namespace DocumentFormat.OpenXml.Tools.DocumentReflector.Commands
{
	internal class FontSettingCommand : ICommand
	{
		public void Execute(object arg)
		{
			IToolSettingService service = ToolSingleton.Instance.Services.GetService<IToolSettingService>();
			if (service != null)
			{
				FontDialog fontDialog = new FontDialog();
				fontDialog.ShowColor = false;
				fontDialog.ShowEffects = false;
				fontDialog.Font = service.FontSetting;
				if (fontDialog.ShowDialog() == DialogResult.OK)
				{
					service.FontSetting = fontDialog.Font;
					service.Save();
				}
			}
		}
	}
}
