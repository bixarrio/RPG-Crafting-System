using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Crafting.UI
{
    // This is the complete Crafting UI
    public class CraftingUI : MonoBehaviour
    {
        // The container that will hold all the recipes
        [SerializeField] Transform recipesListContainer;
        // The prefab that represents each recipe
        [SerializeField] RecipeUI recipePrefab;
        // The details panel
        [SerializeField] RecipeDetailsUI recipeDetails;
        [SerializeField] CraftingOutputUI recipeOutput;

        // A reference to the crafting system
        private CraftingSystem craftingSystem;
        // All the recipes in the list
        private List<RecipeUI> recipesInList = new List<RecipeUI>();

        private void Awake()
        {
            // Disable the carfting ui
            gameObject.SetActive(false);

            // Remove all children from the recipe list container
            CleanupRecipesList();
        }

        public void ShowCraftingUI(CraftingSystem craftingSystem)
        {
            // Keep a reference to the crafting system
            this.craftingSystem = craftingSystem;
            // Attach the crafting system to the recipe details
            recipeDetails.AttachCraftingSystem(craftingSystem);
            // Populate the recipes list
            PopulateRecipesList(craftingSystem.GetRecipesList());
            // Show the UI
            gameObject.SetActive(true);
        }

        public void HideCraftingUI()
        {
            // tell the crafting system we are closing
            craftingSystem.CloseCrafting();
            // Hide the UI
            gameObject.SetActive(false);
        }

        public void CraftingStarted(Recipe currentRecipe)
        {
            foreach (var recipe in recipesInList)
            {
                if (recipe.GetRecipe().GetRecipeID() == currentRecipe.GetRecipeID())
                    recipe.OnSelect();
                recipe.SetEnabled(false);
            }
            recipeDetails.SetCrafting(true);
        }
        public void CraftingCancelled()
        {
            foreach (var recipe in recipesInList)
            {
                recipe.SetEnabled(true);
            }
            recipeDetails.SetCrafting(false);
        }
        public void CraftingCompleted(CraftingItem output)
        {
            foreach (var recipe in recipesInList)
            {
                recipe.SetEnabled(true);
            }
            recipeDetails.SetCrafting(false);
            recipeOutput.SetOutput(output);
        }
        public void CraftingProgress(float progress)
        {
            recipeDetails.UpdateProgress(progress);
        }

        private void CleanupRecipesList()
        {
            // Destroy all the recipe items we know of
            foreach (var recipe in recipesInList)
            {
                recipe.transform.SetParent(null);
                Destroy(recipe.gameObject);
            }
            // clear the list
            recipesInList.Clear();

            // If there are no more items in the container, we are done
            if (recipesListContainer.childCount == 0)
            {
                return;
            }

            // If we still have children, destroy those too
            foreach (Transform child in recipesListContainer)
            {
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        private void PopulateRecipesList(Recipe[] recipesList)
        {
            // Remove all children from the recipe list container
            CleanupRecipesList();
            // Go through each recipe, create it's representation and add it to the list
            foreach (var recipe in recipesList)
            {
                var recipeUI = Instantiate(recipePrefab, recipesListContainer);
                recipeUI.Setup(recipe, recipeDetails);
                recipesInList.Add(recipeUI);
            }
        }
    }
}
