using System.Drawing;
using System.Numerics;
using Cinemachine;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

namespace InternalAssets.Scripts.NarrativeClips.Act2
{
    public class StrangeDeviceClip : PlayableNarrativeClip
    {
        [SerializeField] private Transform strangeDevice;
        [SerializeField] private Transform strangeDeviceAnimated;
        [SerializeField] private Transform playerCamera;
        [SerializeField] private Transform dummyCamera;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private ParticleSystem deviceParticleSystem;
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private float cameraTranslationDuration;
        [SerializeField] private AudioSource ambientAudioSource;
        
        [SerializeField] private Transform animationStartTransform;
        
        public override void OnStart()
        {
            ServicesHolder.RaycastService._onHit += OnRaycast;
            virtualCamera.gameObject.SetActive(true);
            playableDirector.stopped += OnPlayableDirectorStopped;
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

        private void OnPlayableDirectorStopped(PlayableDirector pd)
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;

            SceneManager.LoadScene(3);
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

        public void AnimationTrigger1()
        {
            var dialogService = ServicesHolder.UIDialogService;
            var standartDuration = 0.04f;

            DOTween.Sequence()
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("The core is melted completely!", standartDuration, 0.8f))
                .AppendInterval(0.3f)
                .Append(dialogService.PrintDialogWoCleaning("The core is melted completely! ",
                    "What was you thinking?...", standartDuration, 0.8f))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialog(
                    "After the pulse, the ground all over the city started spilling into here.", standartDuration,
                    0.8f))
                .AppendInterval(0.5f)
                .Append(dialogService.PrintDialogWoCleaning(
                    "After the pulse, the ground all over the city started spilling into here. ",
                    "There's no way to keep the complex a secret.", standartDuration, 0.8f))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialog("I need to tell them.", standartDuration, 0.8f))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialogWoCleaning("I need to tell them. ", "And fast.", standartDuration,
                    0.8f))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialog("Where the hell is my teleporter?", standartDuration, 0.8f))
                .Append(dialogService.HidePanel())
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("Teleporter, huh?", 0.06f, 1f))
                .AppendInterval(0.3f)
                .Append(dialogService.PrintDialogWoCleaning("Teleporter, huh? ", "I guess, thaw will do.", 0.06f,
                    1f));
        }

        public void AnimationTrigger2()
        {
            deviceParticleSystem.Play();
        }
        
        private void Interact(InteractableObject interactable)
        {
            interactable.ObjectInteractionScript.Interact();
            if (interactable.CustomTags.Contains("gadget"))
            {
                ServicesHolder.ObjectiveService.ClearPanel();
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);
                
                ServicesHolder.PlayerInputService.enabled = false;
                ServicesHolder.PlayerCameraService.enabled = false;
                
                dummyCamera.position = playerCamera.position; // Camera exchange
                dummyCamera.eulerAngles = playerCamera.eulerAngles;
                ServicesHolder.PlayerCameraService.gameObject.SetActive(false);
                dummyCamera.gameObject.SetActive(true);
                virtualCamera.gameObject.SetActive(true);
                
                strangeDevice.gameObject.SetActive(false);
                strangeDeviceAnimated.gameObject.SetActive(true);

                var dialogService = ServicesHolder.UIDialogService;
                var standartDuration = 0.06f;
            
                DOTween.Sequence()
                    .Append(dialogService.ShowPanel())
                    .Join(dialogService.PrintDialog("What is this.", standartDuration))
                    .AppendInterval(0.3f)
                    .Append(dialogService.PrintDialogWoCleaning("What is this. ", "Never saw anything like that.", standartDuration))
                    .AppendInterval(1f)
                    .Append(dialogService.HidePanel());
                
                DOTween.Sequence()
                    .Insert(0, dummyCamera.DOMove(animationStartTransform.position, cameraTranslationDuration))
                    .Insert(0, dummyCamera.DORotate(animationStartTransform.eulerAngles, cameraTranslationDuration))
                    .Insert(0, ambientAudioSource.DOFade(0, cameraTranslationDuration))
                    .AppendCallback(() =>
                    {
                        playableDirector.Play();
                    });
            }
        }
    }
}