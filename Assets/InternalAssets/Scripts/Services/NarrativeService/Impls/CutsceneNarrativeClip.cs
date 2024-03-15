using InternalAssets.Scripts.Services.PlayableDirector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace InternalAssets.Scripts.Services.NarrativeService.Impls
{
    public abstract class CutsceneNarrativeClip : AbstractNarrativeClip
    {
        [SerializeField] protected UnityEngine.Playables.PlayableDirector playableDirector;
        protected abstract void EndCallback(UnityEngine.Playables.PlayableDirector pD);
    }
}