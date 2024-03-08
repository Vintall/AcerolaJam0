using HolmanPlayerController;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.Playables;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class WakeUpClip : CutsceneNarrativeClip
    {
        [SerializeField] private InputHandler _playerInput;
        [SerializeField] private CameraFree _cameraFree;
        
        public override void OnStart()
        {
            _playerInput.enabled = false;
            _cameraFree.enabled = false;
            playableDirector.Play();
            playableDirector.stopped += EndCallback;
        }

        protected override void EndClip()
        {
            onEndCallback?.Invoke();
        }

        protected override void EndCallback(PlayableDirector pD)
        {
            playableDirector.stopped -= EndCallback;
            EndClip();
        }
    }
}
