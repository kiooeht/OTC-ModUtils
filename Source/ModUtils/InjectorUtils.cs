using System;
using System.Linq;
using System.Reflection;

namespace ModUtils
{
	public static class InjectorUtils
	{
		public static T Install<T, Old>()
			where T : UnityEngine.Component
			where Old : UnityEngine.MonoBehaviour
		{
			var oldComp = UnityEngine.Object.FindObjectOfType<Old>();
			if (oldComp == null)
				return null;
			var go = oldComp.gameObject;
			var newComp = go.AddComponent<T>();
			if (newComp == null)
				return null;
			InjectorUtils.CopyAll(newComp, oldComp);
			oldComp.enabled = false;
			return newComp;
		}

		public static Old Uninstall<T, Old>()
			where T : UnityEngine.Component
			where Old : UnityEngine.MonoBehaviour
		{
			var oldComp = UnityEngine.Object.FindObjectOfType<Old>();
			if (oldComp == null)
				return null;
			var newComp = UnityEngine.Object.FindObjectOfType<T>();
			if (newComp == null)
				return null;
			InjectorUtils.CopyAll(oldComp, newComp);
			oldComp.enabled = true;
			UnityEngine.Object.Destroy(newComp);
			return oldComp;
		}

		public static void CopyAll<T, Old>(T dest, Old source)
		{
			FieldInfo[] objfields = typeof(Old).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo fi in objfields) {
				var field = typeof(T).GetField(fi.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null) {
					field = typeof(T).BaseType.GetField(fi.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				}
				if (field != null) {
					field.SetValue(dest, fi.GetValue(source));
				} else {
					Debug.Log(string.Format("ERROR: Could not find field {0} in type {1}", fi.Name, typeof(T).Name));
				}
			}
		}

		public static object CallPrivateFunc<T>(T obj, string funcname, params object[] args)
		{
			Type[] types = args.Select(x => x.GetType()).ToArray();
			MethodInfo methodInfo = typeof(T).GetMethod(
				funcname,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
				null,
				types,
				null);
			if (methodInfo == null) {
				Debug.Log("No method " + typeof(T).Name + "." + funcname + " found");
				return null;
			}

			return methodInfo.Invoke(obj, args);
		}

		public static object CallPrivateBaseFunc<T>(T obj, string funcname, params object[] args)
		{
			Type[] types = args.Select(x => x.GetType()).ToArray();
			MethodInfo methodInfo = typeof(T).BaseType.GetMethod(
				funcname,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
				null,
				types,
				null);
			if (methodInfo == null) {
				Debug.Log("No method " + funcname + " found");
				return null;
			}

			return methodInfo.Invoke(obj, args);
		}

		public static object CallPrivateStaticFunc(Type T, string funcname, params object[] args)
		{
			Type[] types = args.Select(x => x.GetType()).ToArray();
			MethodInfo methodInfo = T.GetMethod(
				funcname,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static,
				null,
				types,
				null);
			if (methodInfo == null) {
				Debug.Log("No method {0}.{1} found", T.Name, funcname);
				return null;
			}

			return methodInfo.Invoke(null, args);
		}

		public static object GetPrivateMember<T>(T obj, string name)
		{
			FieldInfo fi = typeof(T).GetField(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			if (fi == null) {
				Debug.Log("No member {0}.{1} found", typeof(T).Name, name);
				return null;
			}
			return fi.GetValue(obj);
		}

		public static void SetPrivateMember<T>(T obj, string name, object value)
		{
			FieldInfo fi = typeof(T).GetField(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			if (fi == null) {
				Debug.Log("No member {0}.{1} found", typeof(T).Name, name);
				throw new Exception("No member " + typeof(T).Name + "." + name + " found");
			}
			fi.SetValue(obj, value);
		}
	}
}

