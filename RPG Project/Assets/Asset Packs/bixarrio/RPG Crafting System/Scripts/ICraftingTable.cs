using System;

namespace RPG.Crafting
{
    public enum CraftingState
    {
        Idle,
        Crafting
    }

    public interface ICraftingTable
    {
        event Action<ICraftingTable> OnInteract;
        event Action CraftingStarted;
        event Action<float> CraftingProgress;
        event Action CraftingCompleted;
        event Action CraftingCancelled;

        Recipe CurrentRecipe { get; }
        CraftedItemSlot CraftedOutput { get; }
        CraftingState CurrentState { get; }
        float CraftingPercentage { get; }

        Recipe[] GetRecipesList();
        bool CanCraftRecipe(Recipe recipe);
        void CraftRecipe(Recipe recipe);
        void CancelCrafting();
        void UpdateState();
    }
}