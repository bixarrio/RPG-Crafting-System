using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Crafting
{
    public class CraftedItemSlot
    {
        public InventoryItem Item { get; private set; }
        public int Amount { get; private set; }

        public CraftedItemSlot(InventoryItem item, int amount)
        {
            Item = item;
            Amount = amount;
        }
        public CraftedItemSlot(CraftingItem craftingItem) : this(craftingItem.Item, craftingItem.Amount) { }

        public bool AddItem(CraftingItem craftingItem)
        {
            // Some checks if the item is already set
            if (!CanAddItem(craftingItem))
            {
                return false;
            }

            // We can add the item here
            if (Item == null)
            {
                Item = craftingItem.Item;
                Amount = 0;
            }
            Amount += craftingItem.Amount;

            return true;
        }

        public bool CanAddItem(CraftingItem craftingItem)
        {
            // If there's nothing in here, we're good to go
            if (Item == null)
            {
                return true;
            }

            // Some checks if the item is already set
            if (Item != null)
            {
                // If these aren't the same item, we cannot add them together
                if (!object.ReferenceEquals(Item, craftingItem.Item))
                {
                    return false;
                }
                // If the item cannot stack, we cannot add them together
                if (!Item.IsStackable())
                {
                    return false;
                }
            }

            // If we're here, the items match and can be added together
            return true;
        }

        public CraftingItem GetCraftingItem()
        {
            return new CraftingItem
            {
                Item = Item,
                Amount = Amount,
            };
        }
    }
}
