using System;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

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
        [SerializeField] private AudioSource tvChanel2;

        [SerializeField] private AudioSource ambientAudioSource;

        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private Transform playerStandUpTransform;

        [SerializeField] private AudioSource squeakAudioSource;
        
        [SerializeField] private Transform playerBody;

        [SerializeField] private List<Color> tvColors;
        [SerializeField] private Material tvPlaneMaterial;

        [SerializeField] private List<Light> onExplosionEnable;
        [SerializeField] private List<Light> onExplosionDisable;

        private Color tvColor;
        private bool isTVOn;

        private void Update()
        {
            TVColorChange();
        }

        private void TVColorChange()
        {
            if(!isTVOn)
                return;
            var newColor = tvColor + tvColor * Random.Range(-0.03f, 0.03f);
            tvPlaneMaterial.color = newColor;
            tvPlaneMaterial.SetColor("_Emission", newColor);
        }

        public override void OnStart()
        {
            playableDirector.stopped += OnPlayableDirectorStopped;
            
            var dialogService = ServicesHolder.UIDialogService;
            var standartDuration = 0.06f;
            tvPlaneMaterial.color = tvColors[0];

            DOTween.Sequence()
                .AppendInterval(0.3f)
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("Finally..", standartDuration))
                .AppendInterval(0.3f)
                .Append(dialogService.PrintDialogWoCleaning("Finally.. ", "Done with the cleaning", standartDuration))
                .AppendInterval(2f)
                .Append(dialogService.PrintDialog("Well...", standartDuration))
                .AppendInterval(0.3f)
                .Append(dialogService.PrintDialogWoCleaning("Well... ", "the day is ruined for sure.", standartDuration))
                .AppendInterval(0.3f)
                .Append(dialogService.PrintDialogWoCleaning("Well... the day is ruined for sure. ", "But it was worth it.",
                    standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialog("I might as well go and watch some TV.", standartDuration))
                .AppendInterval(0.5f)
                .Append(dialogService.HidePanel())
                .JoinCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                    ServicesHolder.ObjectiveService.PrintObjective(objectiveText, 0.05f);
                });
        }

        private int switchCount = 0;
        public void OnTVSwitch()
        {  
            tvSwitch.Play();
            isTVOn = true;
            if (switchCount == 0)
            {
                
                DOTween.Sequence()
                    .AppendCallback(() => { tvColor = tvColors[1]; })
                    .AppendInterval(tvSwitch.clip.length)
                    .AppendCallback(() =>
                    {
                        tvColor = tvColors[2];
                        
                        var dialogService = ServicesHolder.UIDialogService;
                        var standartDuration = 0.06f;

                        DOTween.Sequence()
                            .Append(dialogService.ShowPanel(0.1f))
                            .Join(dialogService.PrintDialog("A few more people are missing today. The mystery disappearance series, " +
                                                            "which began two years ago, seems to has no intention of ending.", 0.03f, 1.5f))
                            .AppendInterval(1f)
                            .Append(dialogService.PrintDialog("Nothing new...", standartDuration))
                            .AppendInterval(0.5f)
                            .Append(dialogService.HidePanel());
                    });
                switchCount = 1;
            }
            else if (switchCount == 1)
            {
                DOTween.Sequence()
                    .AppendCallback(() => { tvColor = tvColors[1]; })
                    .AppendInterval(tvSwitch.clip.length)
                    .AppendCallback(() =>
                    {
                        tvChanel2.Play();
                        tvColor = tvColors[3];
                    });
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
            tvChanel2.Stop();
            tvSwitch.Play();

            foreach (var lightSource in onExplosionDisable)
                lightSource.enabled = false;
            
            foreach (var lightSource in onExplosionEnable)
                lightSource.enabled = true;
            
            DOTween.Sequence()
                .AppendCallback(() => { tvColor = tvColors[1]; })
                .AppendInterval(tvSwitch.clip.length)
                .AppendCallback(() =>
                {
                    isTVOn = false;
                    tvPlaneMaterial.SetColor("_Emission", Color.black);
                    tvPlaneMaterial.color = tvColors[0];
                    tvColor = tvColors[0];
                });
            
            var dialogService = ServicesHolder.UIDialogService;
            var standartDuration = 0.06f;

            DOTween.Sequence()
                .AppendInterval(0.2f)
                .Append(dialogService.ShowPanel(0f))
                .Join(dialogService.PrintDialog("Jesus Christ!", 0.01f, 1.2f))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialogWoCleaning("Jesus Christ!", " What the hell was that", 0.03f, 1.2f))
                .AppendInterval(0.5f)
                .Append(dialogService.HidePanel());
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
                    squeakAudioSource.Play();
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