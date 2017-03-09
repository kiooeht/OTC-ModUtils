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
}

public override void PostUpdate()
{
  if (KEYBINDING_MY_HOTKEY.GetKeyDown()) {
    ...
  }
}
```
_**Note**: Hotkeys must be checked in the PostUpdate() function rather than Update() to work properly._

### Options Menu
Classes for creating options menu tabs and saving your options to file.

 * `ModOptionsScreenListener`
 * `ModOptionsSave`
 * `IOptionsSave`

See the mod [UI+](http://steamcommunity.com/sharedfiles/filedetails/?id=819109474) on the Steam Workshop (source is included) for an example on how to use these.

### InjectorUtils
Collection of functions for:
 * Replace the MainGameEventListener.
 * Replacing Unity scripts with your own.
 * Accessing or changing private variables.
 * Calling private functions.
