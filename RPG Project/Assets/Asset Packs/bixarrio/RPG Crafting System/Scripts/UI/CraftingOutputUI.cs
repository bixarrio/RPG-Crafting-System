using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using UnityEngine;

namespace RPG.Crafting.UI
{
    public class CraftingOutputUI : MonoBehaviour, IDragSource<InventoryItem>
    {
        [SerializeField] InventoryItemIcon itemIcon;

        private int amount;
        private InventoryItem outputItem;

        public void SetOutput(CraftingItem output)
        {
            amount = output.Amount;
            outputItem = output.Item;
            itemIcon.SetItem(outputItem, amount);
        }

        InventoryItem IDragSource<InventoryItem>.GetItem()
        {
            return outputItem;
        }

        int IDragSource<InventoryItem>.GetNumber()
        {
            return amount;
        }

        void IDragSource<InventoryItem>.RemoveItems(int number)
        {
            amount -= number;
            if (amount <= 0)
            {
                outputItem = null;
                itemIcon.SetItem(null);
            }
        }
    }
}
