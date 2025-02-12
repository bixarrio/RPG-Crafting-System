﻿using System;
using UnityEngine;

namespace RPG.Crafting
{
    public class CraftingMediator : MonoBehaviour
    {
        public static event Action<ICraftingTable> Interact;

        public static CraftingMediator GetCraftingMediator()
        {
            return FindObjectOfType<CraftingMediator>();
        }

        public void NotifyInteraction(ICraftingTable craftingTable)
        {
            Interact?.Invoke(craftingTable);
        }
    }
}