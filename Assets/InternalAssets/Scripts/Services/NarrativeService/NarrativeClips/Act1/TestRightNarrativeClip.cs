using System;
using InternalAssets.Scripts.Services.PlayableDirector;
using UnityEngine;
using UnityEngine.Timeline;
using Zenject;

namespace InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1
{
    
    public class TestRightNarrativeClip : AbstractNarrativeClip
    {
        [SerializeField] private TestRightNarrativeClipData testRightNarrativeClipData;
        [SerializeField] private UnityEngine.Playables.PlayableDirector playableDirector;
        
        public override void OnStart()
        {
            //playableDirector.playableAsset = testRightNarrativeClipData.TimelineAsset;
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