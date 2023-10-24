## RPG Crafting System
This is a simple crafting system for the GameDev.tv RPG course. This crafting system was initially inspired by the one used in Valheim.

### Version 0.4
* Moved a function from Inventory to an extension method. I forgot that I was the one that added it in the first place
* Added a flag to the `CraftingTable` to configure whether or not crafting can be cancelled
* Added functionality to the output slot. Right-clicking the items will send it to the inventory if there is space for it. For now it's in, but I will likely turn it into opt-in functionality

### Setup
#### Cursor Type
* Add a new entry to the `CursorType` enum and call it `Crafting`
* Add a new mapping to the `PlayerController` for this `CursorType` and select an icon that will represent crafting

#### Crafting Table
* Create a crafting table prefab with a model
* Add a `SaveableEntity` component
* Add the `CraftingTable` component

#### Crafting UI
* Add the `Crafting UI` prefab to the `UI Canvas` game object in the `Core` prefab

#### TimeKeeper
* Add the `TimeKeeper` prefab to the `Core` prefab
* Make sure all the scripts on the game object is ok. The prefab uses a script that has not been included in this package (`SaveableEntity`). If the scripts is reported as 'missing' delete the component and add the correct one. _In the prefab_, give the `TimeKeeper` a unique name, like 'timekeeper'

#### Recipes
* Make some recipes. Be sure to remove the demo recipes or it will appear in your game

### Notes
* I made an extension method for the inventory system to check if an item is in the inventory and get the amount if any. It's ever so slightly less performant, but saves me from having to change the `Inventory` class and explain here how to do it, since I can't add it to this package.
* This package is dependent on the RPG course - especially the inventory system. The UI uses sprites that are included in the course and are therefore not in this package. Sprites that are not in the course were either created by myself, or modified from course images (I made a horizontal scrollbar from the vertical ones) and are included here.
