using RPG.Crafting.UI;
using RPG.Stats;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Crafting
{
    // The crafting system presenter
    public class CraftingSystem : MonoBehaviour
    {
        [Header("Crafting UI")]
        // A reference to the window so we can open/close it when needed
        [SerializeField] GameObject craftingWindow;
        
        [Header("Recipes List")]
        // The container that will hold all the recipes
        [SerializeField] Transform recipesListContainer;
        // The prefab that represents each recipe
        [SerializeField] RecipeUI recipePrefab;
        
        [Header("Recipe Details")]
        // An image to hold the resulting item's icon
        [SerializeField] Image recipeIcon;
        // A texct field to hold the resulting item's name (and amount)
        [SerializeField] TextMeshProUGUI recipeName;
        // A text field to hold the resulting item's description
        [SerializeField] TextMeshProUGUI recipeDescription;
        // A text field to hold the level requirement for this recipe (will be hidden if met)
        [SerializeField] TextMeshProUGUI recipeLevelRequirement;
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

        [Header("Recipe Output")]
        // The output slot - it's a single-slot, one-way inventory (only take out, no putting in)
        [SerializeField] CraftingOutputUI recipeOutput;

        // A reference to the selected recipe
        private Recipe recipe;
        // A reference to the current crafting table
        private ICraftingTable currentCraftingTable;
        // All the recipes in the list
        private List<RecipeUI> recipesInList = new List<RecipeUI>();

        private void Awake()
        {
            // Subscribe to the crafting mediator's interact event
            // This will let the system know when a crafting table
            // has been interacted with
            CraftingMediator.Interact += OnCraftingInteraction;
            // Hide the UI
            craftingWindow.SetActive(false);
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

        // Executed when a crafting table is interacted with
        private void OnCraftingInteraction(ICraftingTable craftingTable)
        {
            // Keep a reference to the current crafting table
            currentCraftingTable = craftingTable;
            // Subscribe to all the events
            currentCraftingTable.CraftingStarted += OnCraftingStarted;
            currentCraftingTable.CraftingProgress += OnCraftingProgress;
            currentCraftingTable.CraftingCompleted += OnCraftingCompleted;
            currentCraftingTable.CraftingCancelled += OnCraftingCancelled;

            // Initialise
            InitialiseCrafting();
            // Populate the recipes list
            PopulateRecipesList(GetRecipesList());
            // Setup the output
            recipeOutput.SetCraftedItemSlot(craftingTable.CraftedOutput);
            // Subscribe to the event so we know when the item was removed
            recipeOutput.ItemRemoved += OnOutputRemoved;
            // Show the UI
            craftingWindow.SetActive(true);

            // If we have output, the UI need to show it
            if (currentCraftingTable.CraftedOutput != null)
            {
                CraftingCompleted(currentCraftingTable.CraftedOutput.GetCraftedItem());
            }

            // If we are busy crafting, let the UI know
            if (currentCraftingTable.CurrentState == CraftingState.Crafting)
            {
                CraftingStarted(currentCraftingTable.CurrentRecipe);
            }

            // If we are _not_ crafting, default to the first item selected
            if (currentCraftingTable.CurrentState != CraftingState.Crafting)
            {
                var recipeUI = recipesListContainer.GetChild(0);
                recipeUI.GetComponent<RecipeUI>().SetSelected(true);
            }
        }

        // Bound to the 'Close Button'
        public void CloseCrafting()
        {
            if (!craftingWindow.gameObject.activeSelf)
            {
                // If the crafting UI is already closed, there's no need to clean it up
                return;
            }

            // Unsubscribe to all the events
            currentCraftingTable.CraftingStarted -= OnCraftingStarted;
            currentCraftingTable.CraftingProgress -= OnCraftingProgress;
            currentCraftingTable.CraftingCompleted -= OnCraftingCompleted;
            currentCraftingTable.CraftingCancelled -= OnCraftingCancelled;
            recipeOutput.ItemRemoved -= OnOutputRemoved;
            
            // Cleanup the UI
            Cleanup();
            // Close the Crafting UI
            craftingWindow.SetActive(false);
            // remove the reference to the crafting table
            currentCraftingTable = default;
        }

        // Bound to the craft button
        public void CraftRecipe()
        {
            // Tell the crafting table to start the crafting process
            currentCraftingTable.CraftRecipe(recipe);
        }

        // Bound to the progress image (clicking the image will cancel crafting)
        public void CancelCrafting()
        {
            // Tell the crafting table to cancel the crafting process
            currentCraftingTable.CancelCrafting();
        }

        private void PopulateRecipesList(Recipe[] recipesList)
        {
            // Remove all children from the recipe list container
            CleanupRecipesList();
            // Go through each recipe, create it's representation and add it to the list
            foreach (var recipe in recipesList)
            {
                // Create the instance
                var recipeUI = Instantiate(recipePrefab, recipesListContainer);
                // Setup the instance with its recipe
                recipeUI.Setup(recipe);
                // Subscribe to it's selected event
                recipeUI.Selected += OnRecipeSelected;
                // Add it to the list so we can track it
                recipesInList.Add(recipeUI);
                // Move the craftable recipes to the top
                if (CanCraftRecipe(recipe))
                {
                    recipeUI.transform.SetAsFirstSibling();
                }
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

        private Recipe[] GetRecipesList()
        {
            // Ask the crafting table for all the recipes it can craft
            return currentCraftingTable.GetRecipesList();
        }

        private CraftedItemSlot GetCraftedOutput()
        {
            // If we have no crafting table, return null
            if (currentCraftingTable == null)
            {
                return null;
            }
            // Get the output (or null if there's nothing) from the crafting table
            return currentCraftingTable.CraftedOutput;
        }

        private bool CanCraftRecipe(Recipe recipe)
        {
            // Ask the crafting table if we can craft this recipe
            return currentCraftingTable.CanCraftRecipe(recipe);
        }

        private void Cleanup()
        {
            // Cleanup the recipes list
            CleanupRecipesList();

            // Cleanup the ingredients list
            CleanupIngredientsList();

            // Disable the craft button
            craftButton.interactable = false;
            // Hide the craft button
            craftButton.gameObject.SetActive(true);

            // Reset the crafting progress image
            craftProgressImage.fillAmount = 0f;
            // Hide the progress image
            craftProgressContainer.SetActive(false);

            // Refresh the output slot
            recipeOutput.RefreshOutput();
        }

        private void RefreshDetails()
        {
            // Remove all children from the ingredient list container
            CleanupIngredientsList();

            // Hide the image
            recipeIcon.gameObject.SetActive(false);
            // Hide the name
            recipeName.gameObject.SetActive(false);
            // Hide the description
            recipeDescription.gameObject.SetActive(false);
            // Initially, disable crafting
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

            // Set the level requirement text
            recipeLevelRequirement.text = $"Requires Level {recipe.GetRequiredLevel()}";
            // If the required level is greater than the player's level, show the required level text
            recipeLevelRequirement.gameObject.SetActive(!PlayerMeetsLevelCriteria(recipe));

            // Show the craft button
            craftButton.gameObject.SetActive(true);
            // Make the craft button interactable _if_ the player can craft this recipe
            craftButton.interactable = CanCraftRecipe(recipe);
        }

        private void InitialiseCrafting()
        {
            // Show the craft button
            craftButton.gameObject.SetActive(true);
            // Initially disable the craft button
            craftButton.interactable = false;

            // Reset the progress image
            craftProgressImage.fillAmount = 0f;
            // Hide the progress image
            craftProgressContainer.SetActive(false);
        }

        private void CleanupRecipesList()
        {
            // Destroy all the recipe items we know of
            foreach (var recipe in recipesInList)
            {
                recipe.Selected -= OnRecipeSelected;
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

        private void CleanupIngredientsList()
        {
            // Destroy all the child items in the ingredients list
            foreach (Transform child in ingredientsListContainer)
            {
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        private void CraftingStarted(Recipe currentRecipe)
        {
            foreach (var recipe in recipesInList)
            {
                if (recipe.GetRecipe().GetRecipeID() == currentRecipe.GetRecipeID())
                    recipe.OnSelect();
                recipe.SetEnabled(false);
            }
            SetCrafting(true);
        }

        private void CraftingCompleted(CraftingItem output)
        {
            ResetCrafting();
            recipeOutput.RefreshOutput();
        }

        private void RecipeSelected(Recipe recipe)
        {
            // Keep a reference to the selected recipe
            this.recipe = recipe;
            // Reset
            RefreshDetails();
            // Make this game object active
            gameObject.SetActive(true);
        }

        private void SetCrafting(bool crafting)
        {
            // Toggle the craft button
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
                RefreshDetails();
            }
        }

        private void ResetCrafting()
        {
            foreach (var recipe in recipesInList)
            {
                recipe.SetEnabled(true);
            }
            SetCrafting(false);
        }

        private bool PlayerMeetsLevelCriteria(Recipe recipe)
        {
            var baseStates = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            var playerLevel = baseStates.GetLevel();
            return playerLevel >= recipe.GetRequiredLevel();
        }

        private void OnCraftingStarted()
        {
            CraftingStarted(currentCraftingTable.CurrentRecipe);
        }
        private void OnCraftingProgress(float progress)
        {
            // Update the crafting progress
            craftProgressImage.fillAmount = progress;
        }
        private void OnCraftingCompleted()
        {
            CraftingCompleted(currentCraftingTable.CraftedOutput.GetCraftedItem());
        }
        private void OnCraftingCancelled()
        {
            ResetCrafting();
        }

        private void OnRecipeSelected(RecipeUI recipeUI)
        {
            // Unselect all the other recipe visuals
            foreach (var recipe in recipesInList)
            {
                if (!object.ReferenceEquals(recipe, recipeUI))
                    recipe.SetSelected(false);
            }
            // Update the details with the selected recipe
            RecipeSelected(recipeUI.GetRecipe());
        }

        private void OnOutputRemoved()
        {
            // Refresh the details
            RefreshDetails();
        }
    }
}
