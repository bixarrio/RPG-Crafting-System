using GameDevTV.Inventories;
using System;

namespace RPG.Crafting
{
    [Serializable]
    // A wrapper to hold an inventory item and amount
    public class CraftingItem
    {
        // The inventory item
        public InventoryItem Item;
        // The amount
        public int Amount = 1;

        // Will return the resulting item's display name, and append the number
        // of resulting items that will be crafted if the amount is greater than 1
        // eg.
        //     Hunting Bow
        //     Flaming Arrow x20
        public string GetRecipeName()
        {
            var recipeName = Item.GetDisplayName();
            if (Amount > 1)
            {
                recipeName = $"{recipeName} x{Amount}";
            }
            return recipeName;
        }
    }
}
