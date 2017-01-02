using System;
using Offworld.AppCore;
using Offworld.GameCore;
using UnityEngine;

namespace ModUtils
{
	public static class Debug
	{
		private static string modName;

		public static string ModName
		{
			get { return modName; }
			set { modName = value; }
		}

		private static string Pre
		{
			get {
				if (string.IsNullOrEmpty(ModName))
					return "";
				return "[" + ModName + "] ";
			}
		}

		public static void Msg(object msg)
		{
			if (AppGlobals.GameHUDHelpers.IsHUDReady())
				AppGlobals.GameHUDHelpers.DisplayChatMessage(PlayerType.NONE, false, Pre + msg.ToString());
		}

		public static void Msg(string format, params object[] args)
		{
			Msg((object)string.Format(format, args));
		}

		public static void Log(object msg)
		{
			UnityEngine.Debug.Log(Pre + msg.ToString());
		}

		public static void Log(string format, params object[] args)
		{
			UnityEngine.Debug.Log(Pre + string.Format(format, args));
		}

		public static void All(object msg)
		{
			Log(msg);
			Msg(msg);
		}

		public static void All(string format, params object[] args)
		{
			Log(format, args);
			Msg(format, args);
		}


		public static void LogComponents(GameObject gameObject)
		{
			foreach (Component comp in gameObject.GetComponents(typeof(Component))) {
				Debug.Log(comp.name + " " + comp.GetHashCode() + " " + comp.GetType().Name);
			}
		}

		public static void LogChildren(GameObject gameObject, Action<GameObject> action = null)
		{
			for (int i = 0; i < gameObject.transform.childCount; ++i) {
				Debug.Log(i);
				GameObject child = gameObject.transform.GetChild(i).gameObject;
				Debug.Log(child.name + " " + child.GetHashCode() + " " + child.GetType().Name);
				if (action != null) {
					Debug.Log("+++");
					action(child);
					Debug.Log("---");
				}
			}
		}

		public static void LogChildrenComponents(GameObject gameObject)
		{
			LogChildren(gameObject, LogComponents);
		}
	}
}
