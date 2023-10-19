using GameDevTV.Saving;
using RPG.Crafting.UI;
using UnityEngine;

namespace RPG.Crafting
{
    public class CraftingSystem : MonoBehaviour, ISaveable
    {
        // Reference to the crafting UI
        [SerializeField] CraftingUI craftingUI;

        // A global crafting value that will be used by crafting tables
        private float globalCraftingTime = 0f;

        // A reference to the current crafting table
        private ICraftingTable currentCraftingTable;

        private void FixedUpdate()
        {
            // Continuously update the global crafting time
            globalCraftingTime += Time.fixedDeltaTime;
        }

        // Convenience for getting the crafting system.
        public static CraftingSystem GetCraftingSystem()
        {
            return FindObjectOfType<CraftingSystem>();
        }

        // Show the crafting UI
        public void ShowCrafting(ICraftingTable craftingTable)
        {
            // Keep a reference to the current crafting table
            currentCraftingTable = craftingTable;
            // Show the crafting UI
            craftingUI.ShowCraftingUI(this);
        }

        public Recipe[] GetRecipesList()
        {
            return currentCraftingTable.GetRecipesList();
        }

        public void StartCrafting(Recipe recipe)
        {
            // Tell the crafting table to start the crafting process
            currentCraftingTable.CraftRecipe(recipe);
        }

        // A getter to return the global crafting time
        public float GetGlobalCraftingTime()
        {
            return globalCraftingTime;
        }

        object ISaveable.CaptureState()
        {
            // Save the global crafting time
            return globalCraftingTime;
        }

        void ISaveable.RestoreState(object state)
        {
            // Restore the global crafting time
            globalCraftingTime = (float)state;
        }
    }
}
