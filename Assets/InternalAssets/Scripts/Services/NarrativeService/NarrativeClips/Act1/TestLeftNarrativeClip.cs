using System;
using InternalAssets.Scripts.Services.PlayableDirector;
using InternalAssets.Scripts.Services.PlayableDirector.Impls;
using UnityEngine;
using UnityEngine.Timeline;
using Zenject;

namespace InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1
{
    public class TestLeftNarrativeClip : AbstractNarrativeClip
    {
        [SerializeField] private TestLeftNarrativeClipData testLeftNarrativeClipData;
        [SerializeField] private UnityEngine.Playables.PlayableDirector playableDirector;
        
        public override void OnStart()
        {
            //playableDirector.playableAsset = testLeftNarrativeClipData.TimelineAsset;
            playableDirector.Play();
            playableDirector.stopped += TimelineEndCallback;
        }
        
        private void TimelineEndCallback(UnityEngine.Playables.PlayableDirector playableDirector)
        {
            playableDirector.stopped -= TimelineEndCallback;
            _onEndCallback?.Invoke();
        }
    }
}