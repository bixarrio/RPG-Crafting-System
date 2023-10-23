using GameDevTV.Saving;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, IJsonSaveable
    {
        LazyValue<float> mana;

        private void Awake() {
            mana = new LazyValue<float>(GetMaxMana);
        }

        private void Update() {
            if (mana.value < GetMaxMana())
            {
                mana.value += GetRegenRate() * Time.deltaTime;
                if (mana.value > GetMaxMana())
                {
                    mana.value = GetMaxMana();
                }
            }
        }

        public float GetMana()
        {
            return mana.value;
        }

        public float GetMaxMana()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        }

        public float GetRegenRate()
        {
            return GetComponent<BaseStats>().GetStat(Stat.ManaRegenRate);
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > mana.value)
            {
                return false;
            }
            mana.value -= manaToUse;
            return true;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(mana.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            mana.value = state.ToObject<float>();
        }
    }
}