using UnityEngine;
using UnityEngine.Timeline;

namespace InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1
{
    [CreateAssetMenu(menuName = "NarrativeClipsData/Act1/Cutscene/TestLeft", fileName = "TestLeftNarrativeClipData")]
    public class TestLeftNarrativeClipData : ScriptableObject, ITestLeftNarrativeClipData
    {
        [SerializeField] protected TimelineAsset timelineAsset;
        [SerializeField] private int nextClipId;
        [SerializeField] private int clipId;

        public TimelineAsset TimelineAsset => timelineAsset;

        public int NextClipId => nextClipId;

        public int ClipId => clipId;
    }
}