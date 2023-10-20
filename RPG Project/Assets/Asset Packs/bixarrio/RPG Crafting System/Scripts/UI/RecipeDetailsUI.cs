using GameDevTV.Inventories;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Crafting.UI
{
    // Details of the recipe. Also does the crafting
    public class RecipeDetailsUI : MonoBehaviour
    {
        // An event that will fire when a recipe has been crafted
        public static event Action RecipeCrafted;

        // An image to hold the resulting item's icon
        [SerializeField] Image recipeIcon;
        // A texct field to hold the resulting item's name (and amount)
        [SerializeField] TextMeshProUGUI recipeName;
        // A text field to hold the resulting item's description
        [SerializeField] TextMeshProUGUI recipeDescription;
        // The container that will hold all the ingredients
        [SerializeField] Transform ingredientsListContainer;
        // The prefab that represents each ingredient
        [SerializeField] IngredientUI ingredientPrefab;
        // The crafting button
        [SerializeField] Button craftButton;
        // A 'filled' image that will serve as the crafting progress bar
        [SerializeField] Image craftProgressImage;
        // The container holding the 'progress' image
        [SerializeField] GameObject craftProgressContainer;

        // A reference to the recipe
        private Recipe recipe;
        // A reference to the crafting table
        private CraftingSystem craftingSystem;

        private void Awake()
        {
            // Hide the image
            recipeIcon.gameObject.SetActive(false);
            // Hide the name
            recipeName.gameObject.SetActive(false);
            // Hide the description
            recipeDescription.gameObject.SetActive(false);
            // Reset
            RefreshUI();
        }

        public void AttachCraftingSystem(CraftingSystem craftingSystem)
        {
            // Keep a reference to the crafting system
            this.craftingSystem = craftingSystem;
            // Disable crafting (initially)
            InitialiseCrafting();
        }

        // a recipe was selected
        public void RecipeSelected(Recipe recipe)
        {
            // Keep a reference to the selected recipe
            this.recipe = recipe;
            // Reset
            RefreshUI();
            // Make this game object active
            gameObject.SetActive(true);
        }

        // Hooked to the UI button
        public void CraftRecipe()
        {
            // Start the crafting
            craftingSystem.StartCrafting(recipe);
        }

        // Hooked to the progress image (clicking the image will cancel crafting)
        public void CancelCrafting()
        {
            // Stop crafting and reset
            craftingSystem.CancelCrafting();
        }

        public void SetCrafting(bool crafting)
        {
            // toggle the craft button
            craftButton.interactable = !crafting;
            // Hide the craft button
            craftButton.gameObject.SetActive(!crafting);

            // Reset the crafting progress image
            craftProgressImage.fillAmount = 0f;
            // Make the progress image visible
            craftProgressContainer.SetActive(crafting);

            // Refresh the UI if we are not crafting
            if (!crafting)
            {
                RefreshUI();
            }
        }

        public void UpdateProgress(float progress)
        {
            // Update the crafting progress
            craftProgressImage.fillAmount = progress;
        }

        public void Cleanup()
        {
            // Cleanup the ingredients list
            CleanupIngredientsList();

            // toggle the craft button
            craftButton.interactable = false;
            // Hide the craft button
            craftButton.gameObject.SetActive(true);

            // Reset the crafting progress image
            craftProgressImage.fillAmount = 0f;
            // Make the progress image visible
            craftProgressContainer.SetActive(false);
        }

        private void RefreshUI()
        {
            // Remove all children from the ingredient list container
            CleanupIngredientsList();

            // Initialliy, disable crafting
            InitialiseCrafting();

            // If we have no recipe, we're done
            if (recipe == null)
            {
                return;
            }

            // Get the resulting item for this recipe
            var resultingItem = recipe.GetResult();
            // Set the resulting item icon
            recipeIcon.sprite = resultingItem.Item.GetIcon();
            // Set the resulting item name (and amount)
            recipeName.text = resultingItem.GetRecipeName();
            // Set the resulting item description
            recipeDescription.text = resultingItem.Item.GetDescription();

            // Populate the ingredients list
            PopulateIngredientsList(recipe.GetIngredients());

            // Show the recipe icon
            recipeIcon.gameObject.SetActive(true);
            // Show the recipe name (and amount)
            recipeName.gameObject.SetActive(true);
            // Show the recipe description
            recipeDescription.gameObject.SetActive(true);

            // Show the craft button
            craftButton.gameObject.SetActive(true);
            // Make the craft button interactable _if_ the player can craft this recipe
            craftButton.interactable = craftingSystem.CanCraftRecipe(recipe);
        }

        private void InitialiseCrafting()
        {
            // Reset the progress image
            craftProgressImage.fillAmount = 0f;
            // Hide the progress image
            craftProgressContainer.SetActive(false);

            // Show the craft button
            craftButton.gameObject.SetActive(true);
            // Initially disable the craft button
            craftButton.interactable = false;
        }

        private void CleanupIngredientsList()
        {
            // Destroy all the child items in the ingredients list
            foreach (Transform child in ingredientsListContainer)
            {
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        private void PopulateIngredientsList(CraftingItem[] ingredients)
        {
            // Remove all children from the ingredient list container
            CleanupIngredientsList();

            // Go through each ingredient, create it's representation and add it to the list
            foreach (var ingredient in ingredients)
            {
                var ingredientUI = Instantiate(ingredientPrefab, ingredientsListContainer);
                ingredientUI.Setup(ingredient);
            }
        }
    }
}
