## RPG Crafting System
This is a simple crafting system for the GameDev.tv RPG course. This crafting system was initially inspired by the one used in Valheim.

### Version 0.3
This version is a complete overhaul of the crafting system.

The architecure has shifted to a Model-View-Presenter pattern, which is far better suited here.

The crafting system can now continue to craft after the player has closed the crafting UI. This allows for recipes that may take several minutes (and even longer) to craft.

Crafting will continue even when the player leaves the scene and the crafting table has been unloaded. In order to achieve this, a `TimeKeeper` is introduced to keep track of the game time. The `TimeKeeper` is independent of the crafting system and can be used by other systems with similar requirements.

Crafted items no longer automatically appear in the player's inventory when completed. A dedicated one-slot inventory has been added to the crafting table. Crafted items will go into this slot and the player can collect these items by dragging it into the inventory.

With this one-slot inventory, some new rules have been added to the crafting. The crafting rules are:
* As before, the player must have the required ingredients in their inventory, and
* the player must be on or above the required level (if any), and
* The output slot must be empty, or
* The item in the output slot must be the same item that the player is trying to craft, and
* The item in the output slot must be stackable.

The crafting UI had a change as well and now incorporates the player's inventory.

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

#### Recipes
* Make some recipes. Be sure to remove the demo recipes or it will appear in your game

### Notes
* I made an extension method for the inventory system to check if an item is in the inventory and get the amount if any. It's ever so slightly less performant, but saves me from having to change the `Inventory` class and explain here how to do it, since I can't add it to this package.
* This package is dependent on the RPG course - especially the inventory system. The UI uses sprites that are included in the course and are therefore not in this package. Sprites that are not in the course were either created by myself, or modified from course images (I made a horizontal scrollbar from the vertical ones) and are included here.
