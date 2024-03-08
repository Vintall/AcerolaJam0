using System;

namespace InternalAssets.Scripts.Services.NarrativeService
{
    public interface INarrativeClip
    {
        void OnStart();
        int NextClipId { get; }
        int ClipId { get; }
        event Action OnEndCallback;
    }
}