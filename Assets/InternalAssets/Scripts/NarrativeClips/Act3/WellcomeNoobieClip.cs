using System;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act3
{
    public class WellcomeNoobieClip : PlayableNarrativeClip
    {
        [SerializeField] private float workerPitch = 1.1f;
        [SerializeField] private float characterPitch = 1;
        [SerializeField] private float charDuration = 0.06f; 
        public override void OnStart()
        {
            var dialogService = ServicesHolder.UIDialogService;
            //var standartDuration = 0.06f;
            ;

            DOTween.Sequence()
                .AppendInterval(2f)
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("Oh, hi. You are a new guy?", charDuration, workerPitch))
                .AppendInterval(0.7f)
                .Append(dialogService.PrintDialogWoCleaning("Oh, hi. You are a new guy? ", "You're late", charDuration, workerPitch))
                .AppendInterval(1f)
                .Append(dialogService.HidePanel(0.1f))
                .Append(dialogService.ShowPanel(0.1f))
                .Append(dialogService.PrintDialog("Wha", charDuration, characterPitch))
                .Append(dialogService.PrintDialog("Erm,", charDuration, characterPitch))
                .AppendInterval(0.7f)
                .Append(dialogService.AddDialog(" yeah", charDuration, characterPitch))
                .AppendInterval(0.7f)
                .Append(dialogService.AddDialog(" sure", charDuration, characterPitch));
                
            ServicesHolder.RaycastService._onHit += OnRaycast;
        }

        protected override void EndClip()
        {
            ServicesHolder.RaycastService._onHit -= OnRaycast;
            onEndCallback?.Invoke();
        }

        protected override void EndCallback()
        {
            EndClip();
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
            
            if (interactable.CustomTags.Contains("hazmat"))
            {
                ServicesHolder.ObjectiveService.ClearPanel();
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);
            }
        }
    }
}