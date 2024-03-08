using System;
using UnityEngine;

namespace InternalAssets.Scripts.Services.NarrativeService.Impls
{
    public abstract class AbstractNarrativeClip : MonoBehaviour, INarrativeClip
    {
        [SerializeField] protected int nextClipId;
        [SerializeField] protected int clipId;
        protected Action onEndCallback;

        public abstract void OnStart();

        public int NextClipId => nextClipId;

        public int ClipId => clipId;

        public event Action OnEndCallback
        {
            add => onEndCallback += value;
            remove => onEndCallback -= value;
        }

        protected abstract void EndClip();
    }
}