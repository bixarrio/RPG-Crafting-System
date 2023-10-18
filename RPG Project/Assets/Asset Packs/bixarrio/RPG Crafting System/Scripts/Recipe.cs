using GameDevTV.Inventories;
using System;
using UnityEngine;

namespace RPG.Crafting
{
    [CreateAssetMenu(menuName = "Crafting / Recipe", order = 0)]
    public class Recipe : ScriptableObject
    {
        // A list of all the required ingredients
        [SerializeField] CraftingItem[] ingredients;
        // The resulting item
        [SerializeField] CraftingItem resultingItem;
        // How long it will take (in seconds) to craft this item
        [SerializeField] float craftDuration = 1f;
        // Required level to craft this item
        [SerializeField] int requiredLevel = 0;

        // Validate the recipe
        private void OnValidate()
        {
            // Check if we have the same ingredient multiple times
            for (int i = 0; i < ingredients.Length - 1; i++)
            {
                for(int j = i + 1; j < ingredients.Length; j++)
                {
                    if (object.ReferenceEquals(ingredients[i].Item, ingredients[j].Item))
                    {
                        Debug.LogError($"Recipe Ingredients contain the same item ({ingredients[i].Item}) multiple times", this);
                    }
                }
            }
        }

        // A getter to return the ingredients
        public CraftingItem[] GetIngredients()
        {
            return ingredients;
        }
        // A getter to return the resulting item
        public CraftingItem GetResult()
        {
            return resultingItem;
        }
        // A getter to return the crafting duration
        public float GetCraftDuration()
        {
            return craftDuration;
        }
        // A getter to return the required level
        public int GetRequiredLevel()
        {
            return requiredLevel;
        }
    }

    [Serializable]
    // A wrapper to hold an inventory item and amount
    public class CraftingItem
    {
        // The inventory item
        public InventoryItem Item;
        // The amount
        public int Amount = 1;

        // Will return the resulting item's display name, and append the number
        // of resulting items that will be crafted if the amount is greater than 1
        // eg.
        //     Hunting Bow
        //     Flaming Arrow x20
        public string GetRecipeName()
        {
            var recipeName = Item.GetDisplayName();
            if (Amount > 1)
            {
                recipeName = $"{recipeName} x{Amount}";
            }
            return recipeName;
        }
    }
}
