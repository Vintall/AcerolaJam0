using System.Collections.Generic;
using HolmanPlayerController;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class FindFoodClip : PlayableNarrativeClip
    {
        [SerializeField] private Transform fridgeDoor;
        [SerializeField] private InputHandler playerInput;
        [SerializeField] private CameraFree _cameraFree;
        [SerializeField] private List<InteractableObject> _interactableObjects;
        [SerializeField] private RaycastService _raycastService;
        
        
        public override void OnStart()
        {
            playerInput.enabled = true;
            _cameraFree.enabled = true;
            _raycastService._onHit += OnRaycast;
        }

        protected override void EndClip()
        {
            _raycastService._onHit -= OnRaycast;
        }

        protected override void EndCallback()
        {
        
        }

        private void OnRaycast(RaycastHit[] result)
        {
            foreach (var raycastHit in result)
                foreach (var interactableObject in _interactableObjects)
                {
                    if (raycastHit.transform.gameObject.GetInstanceID() ==
                        interactableObject.gameObject.GetInstanceID())
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
            interactable.Outline.OutlineWidth = 5;
            _raycastService.MarkDisableOutline(interactable);
        }

        private void Interact(InteractableObject interactable)
        {
            interactable.ObjectInteractionScript.Interact();
        }
    }
}
