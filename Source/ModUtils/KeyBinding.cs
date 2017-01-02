using System;
using System.Linq;
using System.Reflection;
using Offworld.GameCore;
using UnityEngine;

namespace ModUtils
{
	public static class KeyBindingUtils
	{
		public static void ReloadKeyBindingManager()
		{
			if (!Version.IsStandAloneServer) {
				InjectorUtils.SetPrivateMember(AppMain.gApp, "mKeyBindingManager", new MKeyBindingManager());
			}
		}

		public static KeyBindingType Get(string name)
		{
			return Globals.Infos.getType<KeyBindingType>(name);
		}
	}

	public static class ExtensionMethods
	{
		public static bool GetKey(this KeyBindingType eKeyBinding)
		{
			return AppMain.gApp.KeyBindingManager.CheckKeyBindingPressed(eKeyBinding);
		}

		public static bool GetKeyDown(this KeyBindingType eKeyBinding)
		{
			if (eKeyBinding == KeyBindingType.NONE) {
				return false;
			}

			KeyBinding mcKeyCodes = AppMain.gApp.KeyBindingManager.KeyBinding(eKeyBinding).mcKeyCodes;
			return mcKeyCodes.Any((KeyCombo combo) => (combo.Count - 1 == AppMain.gApp.KeyBindingManager.PressedKeys.Count)
				&& combo.All((KeyCode key) => key != KeyCode.None && MKeyBindingManager.CheckKeyPressed(key, Input.GetKey))
				&& combo.Count((KeyCode key) => key != KeyCode.None && MKeyBindingManager.CheckKeyPressed(key, Input.GetKeyDown)) >= 1);
		}

		public static bool GetKeyUp(this KeyBindingType eKeyBinding)
		{
			if (eKeyBinding == KeyBindingType.NONE) {
				return false;
			}

			KeyBinding mcKeyCodes = AppMain.gApp.KeyBindingManager.KeyBinding(eKeyBinding).mcKeyCodes;
			return mcKeyCodes.Any((KeyCombo combo) => (combo.Count - 1 == AppMain.gApp.KeyBindingManager.PressedKeys.Count)
				&& combo.All((KeyCode key) => key != KeyCode.None && MKeyBindingManager.CheckKeyPressed(key, new Func<KeyCode, bool>(x => Input.GetKey(x) || Input.GetKeyUp(x))))
				&& combo.Count((KeyCode key) => key != KeyCode.None && MKeyBindingManager.CheckKeyPressed(key, new Func<KeyCode, bool>(Input.GetKeyUp))) >= 1);
		}
	}
}
