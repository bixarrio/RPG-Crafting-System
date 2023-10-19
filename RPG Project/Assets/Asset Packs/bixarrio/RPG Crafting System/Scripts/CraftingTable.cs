using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Control;
using System;
using System.Collections;
using UnityEngine;

namespace RPG.Crafting
{
    public class CraftingTable : MonoBehaviour, IRaycastable, ISaveable, ICraftingTable
    {
        public event Action CraftingStarted;
        public event Action<float> CraftingProgress;
        public event Action CraftingCompleted;

        // All the discovered recipes in the system
        private Recipe[] recipes;
        // Time when crafting started
        private float craftingStartTime;

        private void Awake()
        {
            // Finds all the recipes in the system. Assumes recipes are under /Resources/Recipes/
            recipes = Resources.LoadAll<Recipe>("Recipes");
        }

        public CursorType GetCursorType()
        {
            // Return the 'crafting' cursor type
            return CursorType.Crafting;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            // If there are no discovered recipes, return false
            if (recipes == null || recipes.Length == 0)
            {
                return false;
            }

            // If the player pressed the left mouse button, show the crafting UI
            if (Input.GetMouseButtonDown(0))
            {
                var craftingSystem = CraftingSystem.GetCraftingSystem();
                craftingSystem.ShowCrafting(this);
            }

            return true;
        }

        // A static helper that checks if the player has all the
        // required ingredients of a recipe in their inventory
        public static bool CanCraftRecipe(Recipe recipe)
        {
            // If we receive no recipe, return false
            if (recipe == null)
            {
                return false;
            }

            // Get the player inventory
            var playerInventory = Inventory.GetPlayerInventory();
            // If we found no inventory, return false
            if (playerInventory == null)
            {
                return false;
            }
            
            // Go through each ingredient and check the player's inventory for that ingredient
            foreach (var craftingItem in recipe.GetIngredients())
            {
                var hasIngredient = playerInventory.HasItem(craftingItem.Item, out int amountInInventory);
                // If the player does not have the ingredient, return false
                if (!hasIngredient)
                {
                    return false;
                }
                // If the player does not have enough of an ingredient, return false
                if (amountInInventory < craftingItem.Amount)
                {
                    return false;
                }
            }

            // If we got to here, the player has all the ingredients required. return true
            return true;
        }

        private IEnumerator CraftingRoutine(Recipe recipe)
        {
            yield return null;
        }

        Recipe[] ICraftingTable.GetRecipesList() => recipes;
        bool ICraftingTable.CanCraftRecipe(Recipe recipe) => CanCraftRecipe(recipe);
        void ICraftingTable.CraftRecipe(Recipe recipe)
        {
            craftingStartTime = CraftingSystem.GetCraftingSystem().GetGlobalCraftingTime();
            StartCoroutine(CraftingRoutine(recipe));
        }

        void ICraftingTable.CancelCrafting() => StopAllCoroutines();

        object ISaveable.CaptureState() => throw new NotImplementedException();
        void ISaveable.RestoreState(object state) => throw new NotImplementedException();
    }

    public interface ICraftingTable
    {
        event Action CraftingStarted;
        event Action<float> CraftingProgress;
        event Action CraftingCompleted;

        Recipe[] GetRecipesList();
        bool CanCraftRecipe(Recipe recipe);
        void CraftRecipe(Recipe recipe);
        void CancelCrafting();
    }
}
