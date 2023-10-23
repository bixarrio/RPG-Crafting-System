using GameDevTV.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG
{
    // We use a custom timekeeper to keep track of time. The built-in one
    // is initialised when the game starts and we don't necessarily want that.
    // This time will keep consistent time, giving us a continuous value  between
    // game sessions/saves
    public class TimeKeeper : MonoBehaviour, IJsonSaveable
    {
        // A global time value that can be used by systems that require it
        private float globalTime = 0f;

        private void Update()
        {
            // Continuously update the global time
            globalTime += Time.deltaTime;
        }

        // Convenience for getting the time keeper
        public static TimeKeeper GetTimeKeeper()
        {
            return FindObjectOfType<TimeKeeper>();
        }

        // A getter to return the global time
        public float GetGlobalTime()
        {
            return globalTime;
        }

        JToken IJsonSaveable.CaptureAsJToken()
        {
            // Save the global time
            return JToken.FromObject(globalTime);
        }
        void IJsonSaveable.RestoreFromJToken(JToken state)
        {
            // Restore the global time
            globalTime = state.ToObject<float>();
        }
    }
}