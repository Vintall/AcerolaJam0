using System.Collections.Generic;
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

        private int pilesLeftToClean;
        
        public override void OnStart()
        {
            ServicesHolder.PlayerInputService.enabled = true;
            ServicesHolder.PlayerCameraService.enabled = true;
            ServicesHolder.RaycastService._onHit += OnRaycast;
            ServicesHolder.ObjectiveService.PrintObjective(objectiveText, 0.05f);
            pilesLeftToClean = pilesCleanUpRequirement;
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
                    playableDirector.Play();
                }
            }
        }
    }
}