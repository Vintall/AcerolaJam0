using System;

namespace InternalAssets.Scripts
{
    public interface INarrativeClip
    {
        void OnStart();
        int NextClipId { get; }
        int ClipId { get; }
        event Action OnEndCallback;
    }
}