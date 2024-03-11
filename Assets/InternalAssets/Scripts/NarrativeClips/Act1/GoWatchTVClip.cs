using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.Playables;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class GoWatchTVClip : PlayableNarrativeClip
    {
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private Transform couchCameraSitTransform;

        [SerializeField] private Transform dummyCamera;
        [SerializeField] private float sitAnimationDuration;
        [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;

        [SerializeField] private AudioSource rumbleAudioSource;
        [SerializeField] private AudioSource explosionAudioSource;
        [SerializeField] private AudioSource tvSwitch;

        [SerializeField] private AudioSource ambientAudioSource;

        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private Transform playerStandUpTransform;

        [SerializeField] private Transform playerBody;

        [SerializeField] private List<Color> tvColors;
        [SerializeField] private Material tvPlaneMaterial;
        
        public override void OnStart()
        {
            ServicesHolder.RaycastService._onHit += OnRaycast;
            ServicesHolder.ObjectiveService.PrintObjective(objectiveText, 0.05f);
            playableDirector.stopped += OnPlayableDirectorStopped;
        }

        private int switchCount = 0;
        public void OnTVSwitch()
        {  
            tvSwitch.Play();

            if (switchCount == 0)
            {
                DOTween.Sequence()
                    .AppendCallback(() => { tvPlaneMaterial.color = tvColors[1]; })
                    .AppendInterval(tvSwitch.clip.length)
                    .AppendCallback(() => { tvPlaneMaterial.color = tvColors[2]; });
                switchCount = 1;
            }
            else if (switchCount == 1)
            {
                DOTween.Sequence()
                    .AppendCallback(() => { tvPlaneMaterial.color = tvColors[1]; })
                    .AppendInterval(tvSwitch.clip.length)
                    .AppendCallback(() => { tvPlaneMaterial.color = tvColors[3]; });
            }
        }

        public void OnSceneChange() //Eye closed
        {
            ambientAudioSource.DOFade(0, 0.2f);
        }

        public void OnExplosion()
        {
            rumbleAudioSource.Play();
            explosionAudioSource.Play();
            tvSwitch.Play();

            DOTween.Sequence()
                .AppendCallback(() => { tvPlaneMaterial.color = tvColors[1]; })
                .AppendInterval(tvSwitch.clip.length)
                .AppendCallback(() => { tvPlaneMaterial.color = tvColors[0]; });
        }

        public void OnExplosionReaction()
        {
            var randomVelocity = new Vector3()
            {
                x = Random.Range(0.5f, 1f),
                y = Random.Range(0.5f, 1f)
            };

            cinemachineImpulseSource.GenerateImpulseWithVelocity(randomVelocity);
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
            BackToPlayerAnimation();
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
            if (interactable.CustomTags.Contains("couch"))
            {
                ServicesHolder.ObjectiveService.ClearPanel();
                
                ServicesHolder.PlayerInputService.enabled = false;
                ServicesHolder.PlayerCameraService.enabled = false;

                ServicesHolder.RaycastService.serviceEnabled = false;
                ServicesHolder.RaycastService.DisableOutline(interactable);
                cinemachineVirtualCamera.gameObject.SetActive(true);
                SitOnCouchAnimation();
            }
        }

        private void SitOnCouchAnimation()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    dummyCamera.transform.position = ServicesHolder.PlayerCameraService.transform.position;
                    dummyCamera.transform.rotation = ServicesHolder.PlayerCameraService.transform.rotation;
                    dummyCamera.gameObject.SetActive(true);
                    ServicesHolder.PlayerCameraService.gameObject.SetActive(false);
                })
                .Insert(0, dummyCamera.DOMove(couchCameraSitTransform.position,
                    sitAnimationDuration))
                .Insert(0,
                    dummyCamera.transform.DORotate(couchCameraSitTransform.eulerAngles,
                        sitAnimationDuration))
                .AppendCallback(() =>
                {
                    playableDirector.Play();
                });
        }

        private void BackToPlayerAnimation()
        {
            //playerBody.transform.eulerAngles = Vector3.up * playerStandUpTransform.eulerAngles.y;
            //ServicesHolder.PlayerCameraService.transform.eulerAngles =
            //    Vector3.right * playerStandUpTransform.eulerAngles.x;
            
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    
                })
                .Insert(0, dummyCamera.DOMove(ServicesHolder.PlayerCameraService.transform.position,
                    sitAnimationDuration))
                .Insert(0,
                    dummyCamera.transform.DORotate(ServicesHolder.PlayerCameraService.transform.eulerAngles,
                        sitAnimationDuration))
                .AppendCallback(() =>
                {
                    dummyCamera.gameObject.SetActive(false);
                    ServicesHolder.PlayerCameraService.gameObject.SetActive(true);
                    ServicesHolder.PlayerInputService.enabled = true;
                    ServicesHolder.PlayerCameraService.enabled = true;
                    cinemachineVirtualCamera.gameObject.SetActive(false);
                    
                    EndCallback();
                });
        }
    }
}