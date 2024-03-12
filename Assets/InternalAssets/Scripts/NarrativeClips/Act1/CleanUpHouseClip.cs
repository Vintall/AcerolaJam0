using System.Collections.Generic;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.Playables;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class CleanUpHouseClip : PlayableNarrativeClip
    {
        [SerializeField] private int pilesCleanUpRequirement;
        [SerializeField] private PlayableDirector playableDirector;

        [SerializeField] private List<GameObject> autoCleanDeSpawnObjects;
        [SerializeField] private List<GameObject> autoCleanSpawnObjects;
        [SerializeField] private AudioSource sighAudioSource;
        [SerializeField] private AudioSource ambientAudioSource;
        
        
        private int pilesLeftToClean;
        
        public override void OnStart()
        {
            ServicesHolder.PlayerInputService.enabled = true;
            ServicesHolder.PlayerCameraService.enabled = true;
            
            
            var dialogService = ServicesHolder.UIDialogService;
            var standartDuration = 0.06f;

            DOTween.Sequence()
                .AppendInterval(2f)
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("What a mess...", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialog("The least favorite thing, when throwing a party... ",
                    standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialogWoCleaning("The least favorite thing, when throwing a party... ",
                    "Cleaning afterwards", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.HidePanel())
                .JoinCallback(() =>
                {
                    ServicesHolder.ObjectiveService.PrintObjective(objectiveText, 0.05f);
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                });
            
            pilesLeftToClean = pilesCleanUpRequirement;
        }

        protected override void EndClip()
        {
            
            onEndCallback?.Invoke();
        }

        protected override void EndCallback()
        {
            EndClip();
        }

        public void OnPlayableDirectorEndSignal()
        {
            EndCallback();
        }
        
        public void OnAutoClean()
        {
            foreach (var obj in autoCleanSpawnObjects)
                obj.SetActive(true);

            foreach (var obj in autoCleanDeSpawnObjects)
                obj.SetActive(false);

            
        }
        
        private void OnRaycast(RaycastHit raycastHit)
        {
            foreach (var interactableObject in interactableObjects)
            {
                var sameInstanceID = raycastHit.transform.gameObject.GetInstanceID() ==
                                     interactableObject.gameObject.GetInstanceID();
                var parent = raycastHit.transform.parent;
                var hasInteractableParent = parent.CompareTag("Interactable");
                
                var parentSameInstanceID = parent.gameObject.GetInstanceID() ==
                                           interactableObject.gameObject.GetInstanceID();
                
                if (sameInstanceID || (hasInteractableParent && parentSameInstanceID)) // TODO Possible bag
                {
                    Outline(interactableObject);

                    if (Input.GetKeyDown(KeyCode.E))
                        Interact(interactableObject);

                    return;
                }
            }
        }

        private void Outline(InteractableObject interactable)
        {
            interactable.SetOutlineWidth(5);
            ServicesHolder.UIInteractionService.SetInteractionData(interactable.InteractionText, 
                interactable.OutlineColor);
            
            ServicesHolder.RaycastService.MarkDisableOutline(interactable);
        }

        private void Interact(InteractableObject interactable)
        {
            interactable.ObjectInteractionScript.Interact();
            if (interactable.CustomTags.Contains("mess"))
            {
                --pilesLeftToClean;

                if (pilesLeftToClean == 0)
                {
                    ServicesHolder.ObjectiveService.ClearPanel();
                    ServicesHolder.RaycastService._onHit -= OnRaycast;
                    var dialogService = ServicesHolder.UIDialogService;
                    var standartDuration = 0.06f;

                    DOTween.Sequence()
                        .AppendInterval(1f)
                        .Append(dialogService.ShowPanel())
                        .Join(dialogService.PrintDialog("Damn... That's a lot of mess.", standartDuration))
                        .AppendInterval(0.5f)
                        .Append(dialogService.PrintDialogWoCleaning("Damn... That's a lot of mess.", " I'll be doing it for a while", standartDuration))
                        .JoinCallback(() =>
                        {
                            ambientAudioSource.DOFade(0, 1f);
                            sighAudioSource.Play();
                            playableDirector.Play();
                        })
                        .AppendInterval(0.5f)
                        .Append(dialogService.HidePanel());
                }
            }
        }
    }
}