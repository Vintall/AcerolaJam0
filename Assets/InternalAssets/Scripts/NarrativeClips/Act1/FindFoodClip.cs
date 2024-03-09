using System.Collections.Generic;
using HolmanPlayerController;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using InternalAssets.Scripts.Services.UIServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class FindFoodClip : PlayableNarrativeClip
    {
        public override void OnStart()
        {
            ServicesHolder.PlayerInputService.enabled = true;
            ServicesHolder.PlayerCameraService.enabled = true;
            ServicesHolder.RaycastService._onHit += OnRaycast;
            ServicesHolder.ObjectiveService.PrintObjective(objectiveText, 0.05f);
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
            if (interactable.CustomTags.Contains("food"))
            {
                ServicesHolder.ObjectiveService.ClearPanel();
                EndCallback();
            }
        }
    }
}
