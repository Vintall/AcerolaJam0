using System;
using UnityEngine.Timeline;

namespace InternalAssets.Scripts.Services.PlayableDirector
{
    public interface IPlayableDirectorService
    {
        void Play();
        void Play(Action<UnityEngine.Playables.PlayableDirector> onStopCallback);
        void ChangeTimeline(TimelineAsset timelineAsset);
    }
}