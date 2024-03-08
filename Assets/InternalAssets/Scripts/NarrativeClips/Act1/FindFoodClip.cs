using System.Collections.Generic;
using HolmanPlayerController;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using InternalAssets.Scripts.Services.ObjectiveService;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class FindFoodClip : PlayableNarrativeClip
    {
        [SerializeField] private InputHandler playerInput;
        [SerializeField] private CameraFree _cameraFree;
        [SerializeField] private List<InteractableObject> _interactableObjects;
        [SerializeField] private RaycastService _raycastService;
        [SerializeField] private ObjectiveService objectiveService;
        [SerializeField] private string objectiveText;
        
        public override void OnStart()
        {
            playerInput.enabled = true;
            _cameraFree.enabled = true;
            _raycastService._onHit += OnRaycast;
            objectiveService.PrintObjective(objectiveText, 0.05f);
        }

        protected override void EndClip()
        {
            _raycastService._onHit -= OnRaycast;
            onEndCallback?.Invoke();
        }

        protected override void EndCallback()
        {
            EndClip();
        }

        private void OnRaycast(RaycastHit raycastHit)
        {
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
            if (interactable.CustomTags.Contains("food"))
            {
                Debug.Log("ObjectiveDone");
                EndCallback();
            }
        }
    }
}
