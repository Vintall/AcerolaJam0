using System;
using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act2
{
    public class WhereAmIClip : PlayableNarrativeClip
    {
        [SerializeField] private Transform eyeMaskUp;
        [SerializeField] private Transform eyeMaskDown;
        [SerializeField] private float initialAwaitDuration;

        [SerializeField] private GameObject phone;
        [SerializeField] private ItemSlot.Impls.ItemSlot itemSlot;
        
        
        public override void OnStart()
        {
            eyeMaskUp.position = Vector3.zero;
            eyeMaskDown.position = Vector3.zero;

            ServicesHolder.PlayerInputService.enabled = false;
            ServicesHolder.PlayerCameraService.enabled = false;
            
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
            }
        }

        protected override void EndClip()
        {
            
        }

        protected override void EndCallback()
        {
            
        }
    }
}