using DG.Tweening;
using HolmanPlayerController;
using InternalAssets.Scripts.Services;
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

        public void AnimationEvent1()
        {
            var dialogService = ServicesHolder.UIDialogService;
            var standartDuration = 0.06f;
            
            DOTween.Sequence()
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("Damn. What a mess. ", standartDuration))
                .AppendInterval(1.5f)
                .Append(dialogService.PrintDialog("And already evening... ", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.HidePanel());
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
