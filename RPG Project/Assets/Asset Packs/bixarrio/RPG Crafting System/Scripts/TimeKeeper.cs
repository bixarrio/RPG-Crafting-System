using GameDevTV.Saving;
using UnityEngine;

namespace RPG
{
    public class TimeKeeper : MonoBehaviour, ISaveable
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

        object ISaveable.CaptureState()
        {
            return globalTime;
        }
        void ISaveable.RestoreState(object state)
        {
            globalTime = (float)state;
        }
    }
}