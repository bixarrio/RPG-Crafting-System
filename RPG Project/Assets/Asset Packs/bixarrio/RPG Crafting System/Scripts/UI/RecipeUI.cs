using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Crafting.UI
{
    // UI for one recipe
    public class RecipeUI : MonoBehaviour
    {
        public event Action<RecipeUI> Selected;

        // An image to hold the resulting item's icon
        [SerializeField] Image recipeIcon;
        // A text field to hold the resulting item's name
        [SerializeField] TextMeshProUGUI recipeName;
        // Some selection stuff
        [Header("Selection")]
        [SerializeField] Image selectionImage;
        [SerializeField] Color normalColor;
        [SerializeField] Color selectedColor;

        // The selected state
        private bool isSelected;
        private bool isInteractable = true;

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
            // Only allow selection if we're interactable
            if (!isInteractable)
            {
                return;
            }
            // Set the visual
            SetSelected(true);
        }

        // Toggle the recipe interaction
        public void SetEnabled(bool enabled)
        {
            isInteractable = enabled;
        }

        // Set selection
        public void SetSelected(bool isSelected)
        {
            this.isSelected = isSelected;
            selectionImage.color = normalColor;
            if (isSelected)
            {
                // Set the selected color
                selectionImage.color = selectedColor;
                // Fire the event that we were selected
                Selected?.Invoke(this);
            }
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
