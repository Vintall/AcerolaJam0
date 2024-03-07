using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1
{
    [CreateAssetMenu(menuName = "NarrativeClipsData/Act1/Cutscene/TestRight", fileName = "TestRightNarrativeClipData")]
    public class TestRightNarrativeClipData : ScriptableObject, ITestRightNarrativeClipData
    {
        [SerializeField] private int nextClipId;
        [SerializeField] private int clipId;
        [SerializeField] private TimelineAsset timelineAsset;

        public TimelineAsset TimelineAsset => timelineAsset;

        public int NextClipId => nextClipId;

        public int ClipId => clipId;

        public event Action OnEndCallback;
    }
}