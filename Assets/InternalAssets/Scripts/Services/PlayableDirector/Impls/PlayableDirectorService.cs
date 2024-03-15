using System;
using UnityEngine;
using UnityEngine.Timeline;
using Zenject;

namespace InternalAssets.Scripts.Services.PlayableDirector.Impls
{
    public class PlayableDirectorService : MonoBehaviour, IPlayableDirectorService
    {
        [SerializeField] UnityEngine.Playables.PlayableDirector playableDirector;
        private SignalBus _signalBus;

        private static PlayableDirectorService instance;
        public static PlayableDirectorService Instance => instance;
        private void Awake()
        {
            instance = this;
        }

        public void Play()
        {
            playableDirector.Play();
        }

        public void Play(Action<UnityEngine.Playables.PlayableDirector> onStopCallback)
        {
            playableDirector.stopped += onStopCallback;
            Play();
        }

        public void ChangeTimeline(TimelineAsset timelineAsset)
        {
            playableDirector.playableAsset = timelineAsset;
            playableDirector.RebuildGraph();
        }
    }
}