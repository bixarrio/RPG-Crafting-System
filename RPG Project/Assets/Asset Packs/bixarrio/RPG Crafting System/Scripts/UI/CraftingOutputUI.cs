using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using System;
using UnityEngine;

namespace RPG.Crafting.UI
{
    public class CraftingOutputUI : MonoBehaviour, IDragSource<InventoryItem>
    {
        // Event that will fire when item(s) have been removed from this slot
        public event Action ItemRemoved;

        // The inventory icon component that manages inventory icons and stacked amounts
        [SerializeField] InventoryItemIcon itemIcon;

        // A reference to the crafted item slot
        private CraftedItemSlot craftedItemSlot;

        public void SetCraftedItemSlot(CraftedItemSlot creftedItemSlot)
        {
            this.craftedItemSlot = creftedItemSlot;
        }

        public void RefreshOutput()
        {
            var outputItem = default(InventoryItem);
            var outputAmount = 0;
            if (craftedItemSlot != null)
            {
                outputItem = craftedItemSlot.Item;
                outputAmount = craftedItemSlot.Amount;
            }
            itemIcon.SetItem(outputItem, outputAmount);
        }

        InventoryItem IDragSource<InventoryItem>.GetItem()
        {
            if (craftedItemSlot == null)
            {
                return null;
            }
            return craftedItemSlot.Item;
        }

        int IDragSource<InventoryItem>.GetNumber()
        {
            if (craftedItemSlot == null)
            {
                return 0;
            }
            return craftedItemSlot.Amount;
        }

        void IDragSource<InventoryItem>.RemoveItems(int number)
        {
            if (craftedItemSlot == null)
            {
                return;
            }

            craftedItemSlot.RemoveCraftedItem(number);

            if (craftedItemSlot.Amount <= 0)
            {
                itemIcon.SetItem(null);
            }

            ItemRemoved?.Invoke();
        }
    }
}
