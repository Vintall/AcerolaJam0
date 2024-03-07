using UnityEngine.Timeline;

namespace InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1
{
    public interface ITestLeftNarrativeClipData
    {
        TimelineAsset TimelineAsset { get; }
        int NextClipId { get; }
        int ClipId { get; }
    }
}