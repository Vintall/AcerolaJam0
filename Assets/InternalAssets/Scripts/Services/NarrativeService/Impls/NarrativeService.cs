using UnityEngine;
using Zenject;

namespace InternalAssets.Scripts
{
    public class NarrativeService : INarrativeService, IInitializable
    {
        public NarrativeService()
        {
            Debug.Log("NarrativeService was initialized");
        }

        public void Initialize()
        {
            Debug.Log("NarrativeService was initialized in Initialize()");
        }
    }
}
