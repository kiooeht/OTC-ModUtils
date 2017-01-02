# OTC-ModUtils
Modding Utilities library for Offworld Trading Company.

### Debug
Helper functions for printing debug output to the game log (output.txt) or in-game chat window.
Specifying mod name will prepend each message with the mod name.
```CSharp
public override void Initialize()
{
  ModUtils.Debug.ModName = "MyMod";
  ModUtils.Debug.Log("foobar");
}
```
Output:
```
00:03 - [MyMod] foobar
```

### KeyBindingUtils
Functions for getting and using custom hotkeys.
`Data/key-binding-add.xml`:
```XML
<Root>
	<Entry>
		<zType>KEYBINDING_MY_HOTKEY</zType>
		<zClass>KEYBINDCLASS_CAMERA_CONTROL</zClass>
		<zText>TEXT_KEYBINDING_MY_HOTKEY</zText>
		<Hotkey>CTRL+U</Hotkey>
		<OSXHotkey/>
		<bOnKeyUp>0</bOnKeyUp>
		<bOnKeyHold>0</bOnKeyHold>
	</Entry>
</Root>
```
```CSharp
using ModUtils;
public static KeyBindingType KEYBINDING_MY_HOTKEY = KeyBindingType.NONE;

public override void Initialize()
{
  KEYBINDING_MY_HOTKEY = KeyBindingUtils.Get("KEYBINDING_MY_HOTKEY");
  KeyBindingUtils.ReloadKeyBindingManager();
}

public override void Update()
{
  if (KEYBINDING_MY_HOTKEY.GetKeyDown()) {
    ...
  }
}
```
_**Note**: Currently you must call `KeyBindingUtils.ReloadKeyBindingManager()` in a mod's `Initialize()` and `Shutdown()` functions to use custom hotkeys._

### InjectorUtils
Collection of functions for:
 * Replacing Unity scripts with your own.
 * Accessing or changing private variables.
 * Calling private functions.