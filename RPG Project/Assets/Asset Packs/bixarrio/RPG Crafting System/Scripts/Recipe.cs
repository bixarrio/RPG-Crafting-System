using System;
using System.Collections.Generic;
using UnityEngine;

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
        // The player level requirement for this recipe. 0 = Always craftable
        [SerializeField] int levelRequired = 0;

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
        // A getter to return the required level
        public float GetRequiredLevel()
        {
            return levelRequired;
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
}
