using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ModUtils
{
	public class AutoReloader
	{
		private FileSystemWatcher dllWatcher;

		public AutoReloader(string modpath)
		{
			dllWatcher = new FileSystemWatcher(modpath, "*.dll");
			dllWatcher.NotifyFilter = NotifyFilters.LastWrite;
			dllWatcher.Changed += new FileSystemEventHandler(OnChanged);
			dllWatcher.EnableRaisingEvents = true;

			var usm = AppMain.gApp.UserScriptManager;
			foreach (Assembly a in (List<Assembly>)InjectorUtils.GetPrivateMember(usm, "loadedAssemblies")) {
				Debug.Log(a.GetName().Name);
			}
		}

		private void OnChanged(object source, FileSystemEventArgs e)
		{
			Debug.Log(e.FullPath);
			//InjectorUtils.CallPrivateStaticFunc<Assembly>(typeof(UserScriptManager), "LoadAssembly", e.FullPath);
		}
	}
}
