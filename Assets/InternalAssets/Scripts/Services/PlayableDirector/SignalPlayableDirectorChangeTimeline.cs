using UnityEngine.Timeline;

namespace InternalAssets.Scripts.Services.PlayableDirector
{
    public class SignalPlayableDirectorChangeTimeline
    {
        public TimelineAsset Value;
        public SignalPlayableDirectorChangeTimeline(TimelineAsset timelineAsset)
        {
            Value = timelineAsset;
        }
    }
}