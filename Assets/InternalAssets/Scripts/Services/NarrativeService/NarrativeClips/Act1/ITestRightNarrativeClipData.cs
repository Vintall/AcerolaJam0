using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1
{
    public interface ITestRightNarrativeClipData
    {
        TimelineAsset TimelineAsset { get; }
        public int NextClipId { get; }
        public int ClipId { get; }
    }

        
}