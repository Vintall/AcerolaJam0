using System;

namespace InternalAssets.Scripts.Services.PlayableDirector.Impls
{
    public class SignalPlayableDirectorPlay
    {
        public Action<UnityEngine.Playables.PlayableDirector> Value;
        public SignalPlayableDirectorPlay(Action<UnityEngine.Playables.PlayableDirector> action)
        {
            Value = action;
        }
    }
}