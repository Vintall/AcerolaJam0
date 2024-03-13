using System;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using InternalAssets.Scripts.Services.UIServices;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act3
{
    public class WellcomeNoobieClip : PlayableNarrativeClip
    {
        [SerializeField] private float workerPitch = 1.5f;
        [SerializeField] private float characterPitch = 1;
        [SerializeField] private float charDuration = 0.06f;
        [SerializeField] private GameObject outCollider;
        
        private Sequence ShowPanel() => 
            ServicesHolder.UIDialogService.ShowPanel();
        private Sequence HidePanel(float duration) => 
            ServicesHolder.UIDialogService.HidePanel(duration);
        private Sequence ShowPanel(float duration) => 
            ServicesHolder.UIDialogService.ShowPanel(duration);
        private Sequence HidePanel() => 
            ServicesHolder.UIDialogService.HidePanel();
        private Sequence PrintDialog(string text, float symbolPrintDuration, float pitch = 1) =>
            ServicesHolder.UIDialogService.PrintDialog(text, symbolPrintDuration, pitch);
        private Sequence PrintDialogWoCleaning(string oldText, string text, float symbolPrintDuration,
            float pitch = 1) =>
            ServicesHolder.UIDialogService.PrintDialogWoCleaning(oldText, text, symbolPrintDuration, pitch);

        private void ClearPanel() => ServicesHolder.UIDialogService.ClearPanel();
        
        private Sequence AddDialog(string text, float symbolPrintDuration, float pitch = 1) =>
            ServicesHolder.UIDialogService.AddDialog(text, symbolPrintDuration, pitch);

        [SerializeField] private AudioSource hazmatAudioSource;
        
        public override void OnStart()
        {
            var dialogService = ServicesHolder.UIDialogService;
            //var standartDuration = 0.06f;

            DOTween.Sequence()
                .AppendInterval(2f)
                .Append(ShowPanel())
                .Join(PrintDialog("Oh, hi. ", charDuration, workerPitch))
                .AppendInterval(0.5f)
                .Append(PrintDialogWoCleaning("Oh, hi. ", "You are a new guy", charDuration, workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialog("You're kinda late!", charDuration, workerPitch))
                .AppendInterval(0.3f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .Append(ShowPanel(0.3f))
                .Append(PrintDialog("What", charDuration, characterPitch))
                .Append(PrintDialog("Erm,", charDuration, characterPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning("Erm,", " yeah.", charDuration, characterPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning("Erm, yeah.", " Sorry", charDuration, characterPitch))
                .AppendInterval(1f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .Append(ShowPanel(0.3f))
                .Append(PrintDialog("Common, pun on a suit.", charDuration, workerPitch))
                .AppendInterval(0.2f)
                .Append(PrintDialogWoCleaning("Common, pun on a suit. ", "We're already behind schedule", charDuration, workerPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .JoinCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                    ServicesHolder.ObjectiveService.PrintObjective("Put on a hazmat suit");
                });
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
                hazmatAudioSource.Play();
                ServicesHolder.ObjectiveService.ClearPanel();
                outCollider.SetActive(false);
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);
                EndCallback();
            }
        }
    }
}