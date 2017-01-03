using System;
namespace ModUtils
{
	public static class OptionsCommon
	{
		public static void onOptionsButtonClick(GuiActionInfo actionInfo, out bool refreshUIValues, bool isInGame)
		{
			refreshUIValues = false;
			switch ((OptionsScreenCommon.OptionsScreenUIItem)actionInfo.data1) {
			case OptionsScreenCommon.OptionsScreenUIItem.OK:
				ModOptionsSave.gModOptions.Save();
				break;
			case OptionsScreenCommon.OptionsScreenUIItem.CANCEL:
				OnCancel(out refreshUIValues, isInGame);
				break;
			}
		}

		public static void OnCancel(out bool refreshUIValues, bool isInGame)
		{
			refreshUIValues = true;
			ModOptionsSave.gModOptions.ReloadModOptions(isInGame);
		}
	}
}
