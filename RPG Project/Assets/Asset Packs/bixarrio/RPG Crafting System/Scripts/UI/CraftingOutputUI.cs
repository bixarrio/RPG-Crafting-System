using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Crafting.UI
{
    public class CraftingOutputUI : MonoBehaviour, IDragSource<InventoryItem>, IPointerClickHandler
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

        // Remove items from the slot
        private void RemoveItemsFromSlot(int number)
        {
            craftedItemSlot.RemoveCraftedItem(number);

            if (craftedItemSlot.Amount <= 0)
            {
                itemIcon.SetItem(null);
            }

            ItemRemoved?.Invoke();
        }

        private void SendItemToInventory()
        {
            // Try to put the item(s) in the inventory
            var playerInventory = Inventory.GetPlayerInventory();
            if (playerInventory == null)
            {
                // No inventory found, bo nothing - should probably be an error
                return;
            }
            if (!playerInventory.HasSpaceFor(craftedItemSlot.Item))
            {
                // No space for this item, do nothing
                return;
            }
            if (!playerInventory.AddToFirstEmptySlot(craftedItemSlot.Item, craftedItemSlot.Amount))
            {
                // Could not add all the items, do nothing
                return;
            }

            // Item was sent to the inventory. Remove it from the slot
            RemoveItemsFromSlot(craftedItemSlot.Amount);
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
                // The slot is empty, do nothing
                return;
            }

            // Remove the item from the slot
            RemoveItemsFromSlot(number);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                // Not a right-click, do nothing
                return;
            }

            if (craftedItemSlot == null)
            {
                // The slot is empty, do nothing
                return;
            }

            SendItemToInventory();
        }
    }
}
