using RPG.Crafting.UI;
using UnityEngine;

namespace RPG.Crafting
{
    // The crafting system presenter
    public class CraftingSystem : MonoBehaviour
    {
        // Reference to the crafting UI
        [SerializeField] CraftingUI craftingUI;

        // A reference to the current crafting table
        private ICraftingTable currentCraftingTable;

        private void Awake()
        {
            // Subscribe to the crafting mediator's interact event
            CraftingMediator.Interact += OnCraftingInteraction;
        }

        private void OnDestroy()
        {
            // Unsubscribe from the crafting mediator's interact event
            CraftingMediator.Interact -= OnCraftingInteraction;
        }

        private void Update()
        {
            // If we have no crafting table, just do nothing
            if (currentCraftingTable == null)
            {
                return;
            }
            // Update the crafting table's state
            currentCraftingTable.UpdateState();
        }

        public Recipe[] GetRecipesList()
        {
            return currentCraftingTable.GetRecipesList();
        }

        public CraftedItemSlot GetCraftedOutput()
        {
            // If we have no crafting table, return null
            if (currentCraftingTable == null)
            {
                return null;
            }
            // Return the output (or null if there's nothing)
            return currentCraftingTable.CraftedOutput;
        }

        public bool CanCraftRecipe(Recipe recipe)
        {
            return currentCraftingTable.CanCraftRecipe(recipe);
        }

        public void StartCrafting(Recipe recipe)
        {
            // Tell the crafting table to start the crafting process
            currentCraftingTable.CraftRecipe(recipe);
        }

        public void CancelCrafting()
        {
            // Tell the crafting table to cancel the crafting process
            currentCraftingTable.CancelCrafting();
        }

        public void CloseCrafting()
        {
            // Unsubscribe to all the events
            currentCraftingTable.CraftingStarted -= OnCraftingStarted;
            currentCraftingTable.CraftingProgress -= OnCraftingProgress;
            currentCraftingTable.CraftingCompleted -= OnCraftingCompleted;
            currentCraftingTable.CraftingCancelled -= OnCraftingCancelled;
            // Cleanup the UI
            craftingUI.Cleanup();
            // remove the reference to the crafting table
            currentCraftingTable = default;
        }

        private void OnCraftingInteraction(ICraftingTable craftingTable)
        {
            // Keep a reference to the current crafting table
            currentCraftingTable = craftingTable;
            // Subscribe to all the events
            currentCraftingTable.CraftingStarted += OnCraftingStarted;
            currentCraftingTable.CraftingProgress += OnCraftingProgress;
            currentCraftingTable.CraftingCompleted += OnCraftingCompleted;
            currentCraftingTable.CraftingCancelled += OnCraftingCancelled;
            // Show the crafting UI
            craftingUI.ShowCraftingUI(this);
            // If we have output, the UI need to show it
            if (currentCraftingTable.CraftedOutput != null)
            {
                craftingUI.CraftingCompleted(currentCraftingTable.CraftedOutput.GetCraftingItem());
            }
            // Let the UI know we are crafting
            if (currentCraftingTable.CurrentState == CraftingState.Crafting)
            {
                craftingUI.CraftingStarted(currentCraftingTable.CurrentRecipe);
            }
        }

        private void OnCraftingStarted()
        {
            craftingUI.CraftingStarted(currentCraftingTable.CurrentRecipe);
        }
        private void OnCraftingProgress(float progress)
        {
            craftingUI.CraftingProgress(progress);
        }
        private void OnCraftingCompleted()
        {
            craftingUI.CraftingCompleted(currentCraftingTable.CraftedOutput.GetCraftingItem());
        }
        private void OnCraftingCancelled()
        {
            craftingUI.CraftingCancelled();
        }

    }
}
