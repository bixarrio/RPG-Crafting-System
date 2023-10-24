using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Crafting
{
    public static class InventoryExtensions
    {
        // A helper to check if an inventory has a specific item and, if so, how many of it
        public static bool HasItem(this Inventory inventory, InventoryItem item, out int amount)
        {
            amount = 0;
            var hasItem = false;

            var inventorySize = inventory.GetSize();
            for (var i = 0; i < inventorySize; i++)
            {
                var testItem = inventory.GetItemInSlot(i);
                if (!object.ReferenceEquals(item, testItem))
                {
                    continue;
                }
                hasItem = true;
                amount += inventory.GetNumberInSlot(i);
            }

            return hasItem;
        }

        public static int RemoveItem(this Inventory inventory, InventoryItem item, int number)
        {
            if (item == null)
            {
                return number;
            }

            while (number > 0)
            {
                int slot = FindFirstInSlot(inventory, item);
                if (slot < 0)
                {
                    break;
                }
                int amountToRemove = Mathf.Min(number, inventory.GetNumberInSlot(slot));
                number -= amountToRemove;
                inventory.RemoveFromSlot(slot, amountToRemove);
            }

            return number;
        }

        public static int FindFirstInSlot(this Inventory inventory, InventoryItem item)
        {
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (object.ReferenceEquals(inventory.GetItemInSlot(i), item))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
