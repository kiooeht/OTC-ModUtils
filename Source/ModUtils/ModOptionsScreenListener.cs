using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Offworld.GameCore;

namespace ModUtils
{
	public class ModOptionsScreenListener : OptionsScreenListener
	{
		public static T Install<T>()
			where T : ModOptionsScreenListener
		{
			T ret = InjectorUtils.Install<T, OptionsScreenListener>();
			if (ret != null)
				Debug.Log("Replaced OptionsScreenListener");
			return ret;
		}

		public virtual void TryInstallInHudDirector()
		{
			if (modsInitialized && !installedInHUDDirector) {
				installedInHUDDirector = InstallInHUDDirector();
			}
		}

		public bool IsInstalledInHUDDirector
		{
			get
			{
				return installedInHUDDirector;
			}
		}

		private bool InstallInHUDDirector()
		{
			if (!object.ReferenceEquals(AppMain.gApp.HUDDirector, null)) {
				IBaseGameScreen[] screens = (IBaseGameScreen[])typeof(HUDDirector).GetField("screens", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(AppMain.gApp.HUDDirector);
				// loop over them incase the index changes in future updates
				for (int i = 0; i < screens.Length; ++i) {
					if (!object.ReferenceEquals(screens[i], null)) {
						if (screens[i].GetType() == typeof(OptionsScreenListener)) {
							screens[i] = this;
						}
					}
				}
				return true;
			}
			return false;
		}

		public static OptionsScreenListener Uninstall()
		{
			activeTab = 0;
			/*
			OptionsScreenListener ret = InjectorUtils.Uninstall<ModsOptionsScreenListener, OptionsScreenListener>();
			if (ret != null)
				Debug.Log("Uninstalled ModsOptionsScreenListener");
			return ret;
			//*/
			return null;
		}

		public bool UninstallFromHUDDirector()
		{
			if (AppMain.gApp.HUDDirector != null) {
				IBaseGameScreen[] screens = (IBaseGameScreen[])typeof(HUDDirector).GetField("screens", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(AppMain.gApp.HUDDirector);
				// loop over them incase the index changes in future updates
				for (int i = 0; i < screens.Length; ++i) {
					if (screens[i].GetType() == typeof(ModOptionsScreenListener)) {
						screens[i] = this;
					}
				}
				return true;
			}
			return false;
		}

		private bool installedInHUDDirector = false;
		private bool modsInitialized = false;
		private OptionsScreenListener optionslistener;
		private int numOptionsScreen = -1;

		protected AppMain APP
		{
			get { return AppMain.gApp; }
		}

		protected static int activeTab
		{
			get { return (int)typeof(ModOptionsScreenListener).BaseType.GetField("activeTab", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null); }
			set { typeof(ModOptionsScreenListener).BaseType.GetField("activeTab", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value); }
		}

		protected List<ToggleControl> tabToggles
		{
			get { return (List<ToggleControl>)typeof(ModOptionsScreenListener).BaseType.GetField("tabToggles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this); }
		}

		protected List<GameObject> tabs
		{
			get { return (List<GameObject>)typeof(ModOptionsScreenListener).BaseType.GetField("tabs", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this); }
		}

		protected Dictionary<OptionsScreenCommon.OptionsScreenUIItem, ToggleControl> toggles
		{
			get { return (Dictionary<OptionsScreenCommon.OptionsScreenUIItem, ToggleControl>)typeof(ModOptionsScreenListener).BaseType.GetField("toggles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this); }
		}

		protected Dictionary<OptionsScreenCommon.OptionsScreenUIItem, DropdownControl> dropdowns
		{
			get { return (Dictionary<OptionsScreenCommon.OptionsScreenUIItem, DropdownControl>)typeof(ModOptionsScreenListener).BaseType.GetField("dropdowns", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this); }
		}

		protected Dictionary<OptionsScreenCommon.OptionsScreenUIItem, SliderControl> sliders
		{
			get { return (Dictionary<OptionsScreenCommon.OptionsScreenUIItem, SliderControl>)typeof(ModOptionsScreenListener).BaseType.GetField("sliders", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this); }
		}

		protected static bool hasReloadedUI
		{
			get { return (bool)typeof(ModOptionsScreenListener).BaseType.GetField("hasReloadedUI", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null); }
			set { typeof(ModOptionsScreenListener).BaseType.GetField("hasReloadedUI", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value); }
		}

		protected int mSkipUpdates
		{
			get { return (int)typeof(ModOptionsScreenListener).BaseType.GetField("mSkipUpdates", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this); }
			set { typeof(ModOptionsScreenListener).BaseType.GetField("mSkipUpdates", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, value); }
		}


		public ModOptionsScreenListener()
		{
			numOptionsScreen = CheckOptionsScreenTabNUM();
		}

		private static int CheckOptionsScreenTabNUM()
		{
			Type TOptionsScreenTab = typeof(OptionsScreenListener).GetNestedType("OptionsScreenTab", BindingFlags.NonPublic | BindingFlags.Public);
			FieldInfo NUM_OPTIONS_SCREENS = TOptionsScreenTab.GetField("NUM_OPTIONS_SCREENS");
			return (int)NUM_OPTIONS_SCREENS.GetValue(null);
		}

		public new virtual void Update()
		{
			base.Update();

			if (!modsInitialized) {
				modsInitialized = ModTryInitialize();
				if (!modsInitialized)
					return;
			}
			TryInstallInHudDirector();
		}

		private bool ModTryInitialize()
		{
			foreach (var opt in UnityEngine.Object.FindObjectsOfType<OptionsScreenListener>()) {
				if (opt.GetType() == typeof(OptionsScreenListener))
					optionslistener = opt;
			}
			if (object.ReferenceEquals(optionslistener, null))
				return false;

			bool optionslistenerinit = (bool)typeof(ModOptionsScreenListener).BaseType.GetField("initialized", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
			if (!optionslistenerinit) {
				return false;
			}

			tabs.Add(this.createTabPanel(numOptionsScreen, numOptionsScreen == activeTab));

			string labelText = TabName();
			GameObject gameObject = AppMain.gApp.RenderManager.SpawnUIAsset(Globals.Infos.Globals.ASSET_PATH_UI_OPTIONS_TAB, optionslistener.tabToggleGroup.gameObject);
			ToggleControl component = gameObject.GetComponent<ToggleControl>();
			component.toggle.onValueChanged.RemoveAllListeners();
			int tabIndex = numOptionsScreen;
			component.toggle.onValueChanged.AddListener(delegate (bool boolValue)
			{
				this.switchTab(tabIndex);
			});
			component.SetData(null, new GuiActionInfo((int)OptionsScreenCommon.OptionsScreenUIItem.TABS, numOptionsScreen, 0), labelText, optionslistener.tabToggleGroup);
			MohawkUI.assignDefaultSounds(component);
			component.SetValue(tabIndex == activeTab, false);
			tabToggles.Add(component);

			if (!this.IsInGameOptions()) {
				MainScreenListener.MenuCamera menuCamera = Array.Find(this.mParent.menuCameras, (MainScreenListener.MenuCamera t) => t.screen == MainScreenListener.MenuScreen.Options);
				foreach (var x in menuCamera.camera.gameObject.GetComponentsInChildren<BaseScreenListener>()) {
					if (!x.GetType().IsSubclassOf(typeof(ModOptionsScreenListener))) {
						UnityEngine.Object.Destroy(x);
					}
				}

				if (typeof(MainScreenListener).GetField("currentListener", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mParent).GetType() == typeof(OptionsScreenListener)) {
					typeof(MainScreenListener).GetField("currentListener", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(mParent, this);
				}
			}

			Debug.Log("OptionsScreen initialized");
			return true;
		}

		private void RefreshUIValues()
		{
			InjectorUtils.CallPrivateBaseFunc(this, "RefreshUIValues");
			this.RefreshModValues();
		}

		public override bool OnHover(GuiActionInfo actionInfo, RectTransform rect)
		{
			if (actionInfo.data1 != MOD_ID)
				return base.OnHover(actionInfo, rect);
			return false;
		}

		public override void OnClick(GuiActionInfo actionInfo, RectTransform rect)
		{
			bool refreshUIValues = false;
			OptionsCommon.onOptionsButtonClick(actionInfo, out refreshUIValues, this.IsInGameOptions());
			if (refreshUIValues)
				this.RefreshUIValues();

			base.OnClick(actionInfo, rect);
		}

		private void ReloadUI()
		{
			MainScreenListener.SwitchAndLoadLastScreen();
			hasReloadedUI = true;
		}

		public override void OnCancelKeybindingPressed()
		{
			bool resolutionChanged;
			OptionsCommon.OnCancel(out resolutionChanged, IsInGameOptions());
			RefreshUIValues();

			base.OnCancelKeybindingPressed();
		}

		private void switchTab(int newTab)
		{
			InjectorUtils.CallPrivateBaseFunc(this, "switchTab", newTab);
		}

		private GameObject createTabPanel(int tab, bool isActive)
		{
			GameObject gameObject = null;
			if (tab == numOptionsScreen) {
				gameObject = AppMain.gApp.RenderManager.SpawnUIAsset(Globals.Infos.Globals.ASSET_PATH_UI_OPTIONS_PANEL, optionslistener.scrollablePanel);
				this.createModPanel(gameObject);
			}
			if (gameObject != null) {
				gameObject.SetActive(isActive);
			}
			return gameObject;
		}

		protected virtual void createModPanel(GameObject panel)
		{
			this.RefreshModValues();
		}

		protected virtual void RefreshModValues()
		{
		}

		protected virtual string TabName()
		{
			return "MOD";
		}

		public static int MOD_ID = (int)OptionsScreenCommon.OptionsScreenUIItem.NUM_ITEMS;

		public enum OptionsScreenUIItem
		{
		}

		public enum ButtonResponse
		{
			NONE,
			SAVE,
			CANCEL,
		}
	}
}

