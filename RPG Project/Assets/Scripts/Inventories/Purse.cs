using System;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG.Inventories {
    public class Purse : MonoBehaviour, IJsonSaveable, IItemStore
    {
        [SerializeField] float startingBalance = 400f;

        float balance = 0;

        public event Action onChange;

        private void Awake() {
            balance = startingBalance;
        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            if (onChange != null)
            {
                onChange();
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(balance);
        }

        public void RestoreFromJToken(JToken state)
        {
            balance = state.ToObject<float>();
        }

        public int AddItems(InventoryItem item, int number)
        {
            if (item is CurrencyItem)
            {
                UpdateBalance(item.GetPrice() * number);
                return number;
            }
            return 0;
        }
    }
}