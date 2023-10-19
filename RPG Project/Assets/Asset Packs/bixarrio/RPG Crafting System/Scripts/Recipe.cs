using GameDevTV.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace RPG.Crafting
{
    [CreateAssetMenu(menuName = "Crafting / Recipe", order = 0)]
    public class Recipe : ScriptableObject, ISerializationCallbackReceiver
    {
        // A unique identifier for the recipe
        [SerializeField] string recipeID;
        // A list of all the required ingredients
        [SerializeField] CraftingItem[] ingredients;
        // The resulting item
        [SerializeField] CraftingItem resultingItem;
        // How long it will take (in seconds) to craft this item
        [SerializeField] float craftDuration = 1f;

        static Dictionary<string, Recipe> recipeLookupCache = default;

        public static Recipe GetFromID(string recipeID)
        {
            if (recipeLookupCache == null)
            {
                recipeLookupCache= new Dictionary<string, Recipe>();
                var recipeList = Resources.LoadAll<Recipe>("Recipes");
                foreach(var recipe in recipeList)
                {
                    if (recipeLookupCache.ContainsKey(recipe.recipeID))
                    {
                        Debug.LogError($"Looks like there's a duplicate RPG.Crafting.Recipe ID for objects: {recipeLookupCache[recipe.recipeID]} and {recipe}");
                        continue;
                    }
                    recipeLookupCache[recipe.recipeID] = recipe;
                }
            }

            if (string.IsNullOrWhiteSpace(recipeID) || !recipeLookupCache.ContainsKey(recipeID))
            {
                return null;
            }
            return recipeLookupCache[recipeID];
        }

        // A getter to return the unique id
        public string GetRecipeID()
        {
            return recipeID;
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

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(recipeID))
            {
                recipeID = Guid.NewGuid().ToString();
            }
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // We do nothing, but we need this here
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
