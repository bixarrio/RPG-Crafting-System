using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.Crafting.UI
{
    // UI for one recipe
    public class RecipeUI : MonoBehaviour
    {
        // An image to hold the resulting item's icon
        [SerializeField] Image recipeIcon;
        // A text field to hold the resulting item's name
        [SerializeField] TextMeshProUGUI recipeName;
        // The details panel
        private RecipeDetailsUI recipeDetails;

        // A reference to the recipe
        private Recipe recipe;

        // Set up the recipe
        public void Setup(Recipe recipe, RecipeDetailsUI recipeDetails)
        {
            // Keep a reference to the recipe
            this.recipe = recipe;
            // A reference to the details panel
            this.recipeDetails = recipeDetails;
            // Refresh the UI
            RefreshUI();
        }

        // Get the recipe I represent
        public Recipe GetRecipe()
        {
            return recipe;
        }

        // Hooked to the UI button
        public void OnSelect()
        {
            // Visually select the game object
            EventSystem.current.SetSelectedGameObject(gameObject);
            // Let the details panel know that a recipe was selected
            recipeDetails.RecipeSelected(recipe);
        }

        // Toggle the button
        public void SetEnabled(bool enabled)
        {
            var button = GetComponent<Button>();
            button.interactable = enabled;
        }

        // Refresh the UI
        private void RefreshUI()
        {
            // Get the resulting item (the wrapper)
            var resultingItem = recipe.GetResult();
            // Set the recipe icon
            recipeIcon.sprite = resultingItem.Item.GetIcon();
            // Set the recipe name - will include the amount if it's more than 1
            recipeName.text = resultingItem.GetRecipeName();
        }
    }
}
