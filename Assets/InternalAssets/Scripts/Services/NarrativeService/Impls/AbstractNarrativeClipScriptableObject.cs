using System;
using UnityEngine;

namespace InternalAssets.Scripts.Services.NarrativeService.Impls
{
    public abstract class AbstractNarrativeClipScriptableObject : ScriptableObject
    {
        [SerializeField] protected int nextClipId;
        [SerializeField] protected int clipId;
        protected Action _onEndCallback;

        public abstract void OnStart();

        public abstract void OnEnd();

        public int NextClipId => nextClipId;

        public int ClipId => clipId;

        public event Action OnEndCallback
        {
            add => _onEndCallback += value;
            remove => _onEndCallback -= value;
        }
    }
}