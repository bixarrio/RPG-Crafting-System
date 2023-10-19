using System;
using UnityEngine;

namespace RPG.Crafting
{
    public class CraftingManager : MonoBehaviour
    {
        public static event Action<ICraftingTable> Interact;

        public static CraftingManager GetCraftingManager()
        {
            return FindObjectOfType<CraftingManager>();
        }

        public void NotifyInteraction(ICraftingTable craftingTable)
        {
            Interact?.Invoke(craftingTable);
        }
    }
}