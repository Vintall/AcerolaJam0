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
        [SerializeField] private Transform itemSlotObject;

        [SerializeField] private Transform playerBody;
        [SerializeField] private Transform playerCamera;
        [SerializeField] private Transform dummyCamera;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private CinemachineImpulseSource impulseSource;

        [SerializeField] private Vector3 playerAfterFallPosition;
        [SerializeField] private Vector3 playerAfterFallRotation;

        [SerializeField] private Transform fallStartPoint;
        [SerializeField] private float fallCameraPositioningDuration;

        [SerializeField] private Vector3 phoneHidePosition;
        [SerializeField] private Vector3 phoneHideRotation;
        [SerializeField] private float phoneHideDuration;
        [SerializeField] private AudioSource ambientAudioSource;
        
        public override void OnStart()
        {
            eyeMaskUp.position = Vector3.zero;
            eyeMaskDown.position = Vector3.zero;

            ServicesHolder.PlayerInputService.enabled = false;
            ServicesHolder.PlayerCameraService.enabled = false;
            playableDirector.stopped += OnPlayableDirectorStopped;
            virtualCamera.gameObject.SetActive(true);

            ambientAudioSource.volume = 0;
            
            var dialogService = ServicesHolder.UIDialogService;
            var standartDuration = 0.08f;
            
            DOTween.Sequence()
                .Append(ambientAudioSource.DOFade(0.1f, 2f))
                .AppendInterval(4f)
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("I'm alive? ", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.HidePanel())
                .AppendInterval(3f)
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("Where am I?", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.HidePanel())
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
                    
                })
                .Insert(0, dummyCamera.DOMove(fallStartPoint.position, fallCameraPositioningDuration))
                .Insert(0, dummyCamera.DORotate(fallStartPoint.eulerAngles, fallCameraPositioningDuration))
                .AppendCallback(() =>
                {
                    var dialogService = ServicesHolder.UIDialogService;
                    DOTween.Sequence()
                        .AppendInterval(0.8f)
                        .Append(dialogService.ShowPanel(0.1f))
                        .Join(dialogService.PrintDialog("Not agaaain!", 0.03f, 1.1f))
                        .AppendInterval(0.3f)
                        .Append(dialogService.HidePanel());
                    
                    playableDirector.Play();
                });
        }

        public void OnWallKickSignal()
        {
            phone.SetActive(false);
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
            
            playerBody.transform.position = playerAfterFallPosition;
            playerBody.transform.eulerAngles = Vector3.up * playerAfterFallRotation.y;
            playerCamera.eulerAngles = Vector3.right * playerAfterFallRotation.x + Vector3.up * playerAfterFallRotation.y;
            
            ServicesHolder.PlayerCameraService.gameObject.SetActive(true);
            dummyCamera.gameObject.SetActive(false);
            virtualCamera.gameObject.SetActive(false);
            
            var dialogService = ServicesHolder.UIDialogService;
            var standartDuration = 0.06f;
            
            DOTween.Sequence()
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("Eventually, I'm just fall to my death.. ", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialog("And how the hell am I going to get out? ", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.HidePanel())
                .AppendInterval(initialAwaitDuration)
                .AppendCallback(() =>
                {
                    ServicesHolder.UIInteractionService.SetInteractionData("Press F to turn on light", Color.gray);
                    isF = true;
                    ServicesHolder.PlayerInputService.enabled = true;
                    ServicesHolder.PlayerCameraService.enabled = true;
                    ServicesHolder.CollisionService.TriggerEnter += CheckClipChangeTrigger;
                });
        }

        private void CheckClipChangeTrigger(Collider other)
        {
            if(!other.CompareTag("ClipChange"))
                return;
            
            
            var dialogService = ServicesHolder.UIDialogService;
            var standartDuration = 0.06f;
            
            DOTween.Sequence()
                .Append(dialogService.ShowPanel())
                .Join(dialogService.PrintDialog("What...", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.PrintDialogWoCleaning("What...", " What is this place?", standartDuration))
                .AppendInterval(1f)
                .Append(dialogService.HidePanel());
            
            ServicesHolder.CollisionService.TriggerEnter -= CheckClipChangeTrigger;
            itemSlot.enabled = false;
            DOTween.Sequence()
                .Insert(0, itemSlotObject.DOMove(phoneHidePosition, phoneHideDuration))
                .Insert(0, itemSlotObject.DORotate(phoneHideRotation, phoneHideDuration))
                .AppendCallback(() =>
                {
                    phone.SetActive(false);
                    EndCallback();
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