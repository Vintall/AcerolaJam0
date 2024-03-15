using Cinemachine;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class CrackInTheYardClip : PlayableNarrativeClip
    {
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private Transform crackActive;
        [SerializeField] private Transform crackNotActive;
        [SerializeField] private Transform peekOrigin;
        [SerializeField] private AudioSource explosionAudioSource;
        [SerializeField] private AudioSource explosionRumbleAudioSource;
        [SerializeField] private AudioSource crackRumbleAudioSource;
        [SerializeField] private Transform rumble;

        [SerializeField] private AudioSource explosionCrack;
        [SerializeField] private AudioSource explosionRumble;
        
        [SerializeField] private Transform dummyCamera;
        [SerializeField] private Transform peekPositionTransform;
        [SerializeField] private float peekDuration;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
        
        public override void OnStart()
        {
            ServicesHolder.RaycastService.serviceEnabled = true;
            ServicesHolder.RaycastService._onHit += OnRaycast;
            
            playableDirector.stopped += OnPlayableDirectorStopped;
            
            explosionAudioSource.Play();
            explosionRumbleAudioSource.Play();
            crackRumbleAudioSource.Play();

            crackActive.gameObject.SetActive(true);
            crackNotActive.gameObject.SetActive(false);
            
            cinemachineVirtualCamera.gameObject.SetActive(true);
            rumble.position = Vector3.zero;
            rumble.eulerAngles = Vector3.zero;
            
            DOTween.Sequence()
                .AppendInterval(1f)
                .AppendCallback(() =>
                {
                    ServicesHolder.ObjectiveService.PrintObjective(objectiveText, 0.05f);
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

        public void OnRumbleSignal()
        {
            var randomVelocity = new Vector3()
            {
                x = Random.Range(0.5f, 1f),
                y = Random.Range(0.5f, 1f)
            };
            
            cinemachineImpulseSource.GenerateImpulseWithVelocity(randomVelocity);
        }

        public void OnEndSignal()
        {
            DOTween.Sequence()
                .Insert(0, explosionCrack.DOFade(0, 0.3f))
                .Insert(0, explosionRumble.DOFade(0, 0.3f))
                .AppendCallback(() =>
                {
                    playableDirector.Stop();
                    SceneManager.LoadScene(2);
                    EndCallback();
                });
        }
        
        private void OnPlayableDirectorStopped(PlayableDirector pd)
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;
            //EndCallback();
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
            if (interactable.CustomTags.Contains("crack"))
            {
                ServicesHolder.ObjectiveService.ClearPanel();
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);
                
                ServicesHolder.PlayerInputService.enabled = false;
                ServicesHolder.PlayerCameraService.enabled = false;

                dummyCamera.position = ServicesHolder.PlayerCameraService.transform.position;
                dummyCamera.eulerAngles = ServicesHolder.PlayerCameraService.transform.eulerAngles;
                
                ServicesHolder.PlayerCameraService.gameObject.SetActive(false);
                dummyCamera.gameObject.SetActive(true);

                var peekRotation = new Vector3(peekPositionTransform.eulerAngles.x, dummyCamera.eulerAngles.y,
                    peekPositionTransform.eulerAngles.z);
                
                DOTween.Sequence()
                    .Insert(0, dummyCamera.DOMove(peekPositionTransform.position, peekDuration))
                    .Insert(0, dummyCamera.DORotate(peekRotation, peekDuration))
                    .AppendCallback(() =>
                    {
                        peekOrigin.position = dummyCamera.position;
                        peekOrigin.eulerAngles = dummyCamera.eulerAngles;
                        dummyCamera.position = Vector3.zero;
                        dummyCamera.eulerAngles = Vector3.zero;
                        playableDirector.Play();
                    });
                
                var dialogService = ServicesHolder.UIDialogService;
                var standartDuration = 0.06f;

                DOTween.Sequence()
                    .Append(dialogService.ShowPanel())
                    .Join(dialogService.PrintDialog("What is that.", standartDuration, 1.1f))
                    .AppendInterval(0.5f)
                    .Append(dialogService.HidePanel())
                    .AppendInterval(2.5f)
                    .Append(dialogService.ShowPanel(0))
                    .Append(dialogService.PrintDialog("Oh, that can't be good", standartDuration / 2, 1f))
                    .AppendInterval(0.5f)
                    .Append(dialogService.HidePanel());
            }
        }
    }
}