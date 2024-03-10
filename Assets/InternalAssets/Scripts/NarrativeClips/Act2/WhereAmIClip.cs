using System;
using Cinemachine;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

namespace InternalAssets.Scripts.NarrativeClips.Act2
{
    public class WhereAmIClip : PlayableNarrativeClip
    {
        [SerializeField] private Transform eyeMaskUp;
        [SerializeField] private Transform eyeMaskDown;
        [SerializeField] private float initialAwaitDuration;
        [SerializeField] private PlayableDirector playableDirector;
        
        [SerializeField] private GameObject phone;
        [SerializeField] private ItemSlot.Impls.ItemSlot itemSlot;

        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform playerCamera;
        [SerializeField] private Transform dummyCamera;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private CinemachineImpulseSource impulseSource;

        [SerializeField] private Vector3 playerAfterFallPosition;
        [SerializeField] private Vector3 playerAfterFallRotation;

        [SerializeField] private Transform fallStartPoint;
        [SerializeField] private float fallCameraPositioningDuration;
        
        public override void OnStart()
        {
            eyeMaskUp.position = Vector3.zero;
            eyeMaskDown.position = Vector3.zero;

            ServicesHolder.PlayerInputService.enabled = false;
            ServicesHolder.PlayerCameraService.enabled = false;
            playableDirector.stopped += OnPlayableDirectorStopped;
            
            
            DOTween.Sequence()
                .AppendInterval(initialAwaitDuration)
                .AppendCallback(() =>
                {
                    ServicesHolder.UIInteractionService.SetInteractionData("Press F to turn on light", Color.gray);
                    isF = true;
                });
        }

        private bool isF = false;
        private void Update()
        {
            if(!isF)
                return;

            if (Input.GetKeyDown(KeyCode.F))
            {
                ServicesHolder.UIInteractionService.ClearData();
                isF = false;
                phone.SetActive(true);
                itemSlot.enabled = true;
                ServicesHolder.PlayerInputService.enabled = true;
                ServicesHolder.PlayerCameraService.enabled = true;

                ServicesHolder.CollisionService.TriggerEnter += MyOnTriggerEnter;
            }
        }

        private void MyOnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("TriggerFall"))
                return;
            
            ServicesHolder.CollisionService.TriggerEnter -= MyOnTriggerEnter;
            
            ServicesHolder.PlayerInputService.enabled = false; //Control off
            ServicesHolder.PlayerCameraService.enabled = false;
            
            dummyCamera.position = playerCamera.position; // Camera exchange
            dummyCamera.eulerAngles = playerCamera.eulerAngles;
            ServicesHolder.PlayerCameraService.gameObject.SetActive(false);
            dummyCamera.gameObject.SetActive(true);
            virtualCamera.gameObject.SetActive(true);

            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    OnRumbleSignal();
                })
                .Insert(0, dummyCamera.DOMove(fallStartPoint.position, fallCameraPositioningDuration))
                .Insert(0, dummyCamera.DORotate(fallStartPoint.eulerAngles, fallCameraPositioningDuration))
                .AppendCallback(() =>
                {
                    playableDirector.Play();
                });
        }
        
        public void OnRumbleSignal()
        {
            var randomVelocity = new Vector3()
            {
                x = Random.Range(0.5f, 1f),
                y = Random.Range(0.5f, 1f)
            };
            
            impulseSource.GenerateImpulseWithVelocity(randomVelocity);
        }

        private void OnPlayableDirectorStopped(PlayableDirector pd)
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;

            phone.SetActive(false);
            playerBody.transform.position = playerAfterFallPosition;
            playerBody.transform.eulerAngles = Vector3.up * playerAfterFallRotation.y;
            playerCamera.eulerAngles = Vector3.right * playerAfterFallRotation.x + Vector3.up * playerAfterFallRotation.y;
            
            ServicesHolder.PlayerCameraService.gameObject.SetActive(true);
            dummyCamera.gameObject.SetActive(false);
            virtualCamera.gameObject.SetActive(false);
            
            DOTween.Sequence()
                .AppendInterval(initialAwaitDuration)
                .AppendCallback(() =>
                {
                    ServicesHolder.UIInteractionService.SetInteractionData("Press F to turn on light", Color.gray);
                    isF = true;
                    ServicesHolder.PlayerInputService.enabled = true;
                    ServicesHolder.PlayerCameraService.enabled = true;
                });
        }
        

        protected override void EndClip()
        {
            onEndCallback?.Invoke();
        }

        protected override void EndCallback()
        {
            EndClip();
        }
    }
}