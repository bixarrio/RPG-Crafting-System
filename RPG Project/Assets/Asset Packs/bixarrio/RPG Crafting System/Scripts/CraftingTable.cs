using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Control;
using RPG.Stats;
using System;
using UnityEngine;

namespace RPG.Crafting
{
    // The crafting system model
    public class CraftingTable : MonoBehaviour, ICraftingTable, IRaycastable, ISaveable
    {
        public event Action<ICraftingTable> OnInteract;
        public event Action CraftingStarted;
        public event Action<float> CraftingProgress;
        public event Action CraftingCompleted;
        public event Action CraftingCancelled;

        // The current crafting progress, if any
        public float CraftingPercentage { get; private set; } = 0;
        // The current crafting state
        public CraftingState CurrentState { get; private set; }
        // The current recipe (if any)
        public Recipe CurrentRecipe { get; private set; }
        public CraftedItemSlot CraftedOutput { get; private set; }

        // All the discovered recipes in the system
        private Recipe[] recipes;
        // The global time that crafting has started
        private float craftingStartTime;
        // The current recipe to be crafted

        // A reference to the time keeper
        private TimeKeeper timeKeeper;

        // An action to execute depending on the state - just a little state machine
        private Action currentAction;

        private void Awake()
        {
            // Finds all the recipes in the system. Assumes recipes are under /Resources/Recipes/
            recipes = Resources.LoadAll<Recipe>("Recipes");
            // Keep a reference to the time keeper
            timeKeeper = TimeKeeper.GetTimeKeeper();
            // Create the crafted output slot
            CraftedOutput = new CraftedItemSlot();
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

            // If the player presseed the left mouse button, open the UI
            if (Input.GetMouseButtonDown(0))
            {
                // Let the CraftingMediator know that we are being interacted with
                var craftingManager = CraftingMediator.GetCraftingManager();
                craftingManager.NotifyInteraction(this);
            }

            return true;
        }

        // Calculate the current crafting progress
        private float GetCraftingProgress(Recipe recipe)
        {
            // We calculate progress by subtracting the start time from the global time
            // We do it like this because we need to know how much time has passed since we started crafting
            // and we can't use the deltaTime because it stops giving us data when we aren't loaded
            var elapsedTime = timeKeeper.GetGlobalTime() - craftingStartTime;
            // we return a normalised value for the crafting progress
            return Mathf.Clamp01(elapsedTime / recipe.GetCraftDuration());
        }

        // The action that will execute when we are in the idle state
        private void IdleAction()
        {
            // Make sure the state is what it should be for this action
            if (CurrentState != CraftingState.Idle)
            {
                CurrentState = CraftingState.Idle;
            }
            // We do nothing else here
        }

        // The action that will execute when we are in the crafting state
        private void CraftingAction()
        {
            // Make sure the state is what it should be for this action
            if (CurrentState != CraftingState.Crafting)
            {
                CurrentState = CraftingState.Crafting;
            }

            // Determine the current progress
            CraftingPercentage = GetCraftingProgress(CurrentRecipe);

            // Report progress
            CraftingProgress?.Invoke(CraftingPercentage);

            // If we aren't done yet, we have nothing further to do
            if (CraftingPercentage < 1f)
            {
                return;
            }

            // If we are here, crafting is complete
            var craftedResult = CurrentRecipe.GetResult();
            if (CraftedOutput.CanAddItem(craftedResult))
            {
                // The crafted output matches the little inventory, add it (should always be the case)
                CraftedOutput.AddItem(craftedResult);
            }
            else
            {
                // We have a problem. This should never happen because we checked the output before allowing a craft
                Debug.LogError("Crafted an item that does not match the item in the output inventory");
            }

            // Crafting is complete
            CraftingCompleted?.Invoke();

            // Reset the progress
            CraftingPercentage = 0f;
            // Reset the crafting start time
            craftingStartTime = 0f;
            // Reset the recipe
            CurrentRecipe = default;
            // Set the state (the action will do it) to idle
            currentAction = IdleAction;
        }

        // A getter to get the discovered recipes
        Recipe[] ICraftingTable.GetRecipesList()
        {
            return recipes;
        }

        // Start the crafting process
        void ICraftingTable.CraftRecipe(Recipe recipe)
        {
            // Set the state of the table
            // Reset progress
            CraftingPercentage = 0f;
            // Set the start time to whatever the global time is
            craftingStartTime = timeKeeper.GetGlobalTime();
            // Set the current recipe to craft
            CurrentRecipe = recipe;

            // Set the state (the action will do it) to crafting
            currentAction = CraftingAction;

            // Remove the ingredients from the player's inventory
            var playerInventory = Inventory.GetPlayerInventory();
            foreach(var ingredient in recipe.GetIngredients())
            {
                playerInventory.RemoveItem(ingredient.Item, ingredient.Amount);
            }

            // Crafting has started
            CraftingStarted?.Invoke();
        }

        // Cancel the crafting process
        void ICraftingTable.CancelCrafting()
        {
            // Replace the items in the player's inventory
            var playerInventory = Inventory.GetPlayerInventory();
            foreach (var ingredient in CurrentRecipe.GetIngredients())
            {
                playerInventory.AddToFirstEmptySlot(ingredient.Item, ingredient.Amount);
            }

            // Reset progress
            CraftingPercentage = 0f;
            // Reset the start time
            craftingStartTime = 0f;
            // Reset the current recipe
            CurrentRecipe = default;

            // Set the state (the action will do it) to idle
            currentAction = IdleAction;

            // Crafting was cancelled
            CraftingCancelled?.Invoke();
        }

        // Checks if the player has all the
        // required ingredients of a recipe in their inventory
        bool ICraftingTable.CanCraftRecipe(Recipe recipe)
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

            // Check the player level - we assume the inventory and stats are on the same object = as per course
            var baseStats = playerInventory.GetComponent<BaseStats>();
            // If we found no base stats, we have a problem. Return false (should probably be an error)
            if (baseStats == null)
            {
                return false;
            }
            // If the player's level is below the requirement, return false
            if (baseStats.GetLevel() < recipe.GetRequiredLevel())
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

            // If we got to here, the player has all the ingredients required.
            // Check if we have matching output
            if (CraftedOutput == null || CraftedOutput.Item == null)
            {
                // We have no output, return true
                return true;
            }

            // Compare the output to the recipe's output
            var recipeOutput = recipe.GetResult();
            if (!object.ReferenceEquals(CraftedOutput.Item, recipeOutput.Item))
            {
                return false;
            }

            // Check if we can stack the items
            if (!CraftedOutput.Item.IsStackable())
            {
                return false;
            }

            return true;
        }

        // Will be called by the CraftingSystem to update the table progress
        // the table doesn't have to update its progress if it's not being looked at
        void ICraftingTable.UpdateState()
        {
            // execute the current action
            currentAction?.Invoke();
        }

        object ISaveable.CaptureState()
        {
            // Save the state, start time, recipe, and output (if any)
            var saveData = new CraftingTableSaveData
            {
                CraftingState = CurrentState,
                CraftingStartTime = craftingStartTime
            };
            if (CurrentRecipe != null)
            {
                saveData.RecipeID = CurrentRecipe.GetRecipeID();
            }
            if (CraftedOutput != null)
            {
                saveData.OutputItemID = CraftedOutput.Item.GetItemID();
                saveData.OutputAmount = CraftedOutput.Amount;
            }
            return saveData;
        }
        void ISaveable.RestoreState(object state)
        {
            // Restore the state, start time, recipe and output (if any)
            var saveData = (CraftingTableSaveData)state;

            craftingStartTime = 0;
            CurrentRecipe = default;
            CurrentState = saveData.CraftingState;
            currentAction = IdleAction;

            // If we saved an output item, restore it
            if (!string.IsNullOrWhiteSpace(saveData.OutputItemID) && saveData.OutputAmount > 0)
            {
                CraftedOutput = new CraftedItemSlot(InventoryItem.GetFromID(saveData.OutputItemID), saveData.OutputAmount);
            }

            // If we are idle, we are done here
            if (CurrentState == CraftingState.Idle)
            {
                return;
            }

            // If we are here, we were crafting something the last time we got saved
            // Update the table state to continue crafting
            craftingStartTime = saveData.CraftingStartTime;
            CurrentRecipe = Recipe.GetFromID(saveData.RecipeID);
            currentAction = CraftingAction;
        }

        [Serializable]
        struct CraftingTableSaveData
        {
            public CraftingState CraftingState;
            public float CraftingStartTime;
            public string RecipeID;
            public string OutputItemID;
            public int OutputAmount;
        }
    }
}