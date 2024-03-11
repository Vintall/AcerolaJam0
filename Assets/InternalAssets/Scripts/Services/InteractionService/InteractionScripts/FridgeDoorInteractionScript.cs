using DG.Tweening;
using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts
{
    public class FridgeDoorInteractionScript : AbstractObjectInteractionScript
    {
        private bool _isOpen = false;
        private Sequence fridgeDoorSequence;
        [SerializeField] private float closedAngle;
        [SerializeField] private float openedAngle;
        [SerializeField] private float animationDuration;
        [SerializeField] private string openInteractionText;
        [SerializeField] private string closedInteractionText;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openingAudioClip;
        [SerializeField] private AudioClip closingAudioClip;


        public override void Interact()
        {
            if(_isOpen)
                CloseDoor();
            else
                OpenDoor();

            _isOpen = !_isOpen;
        }

        public override string InteractionText => !_isOpen ? openInteractionText : closedInteractionText;

        private void OpenDoor()
        {
            KillSequence();
            audioSource.clip = openingAudioClip;
            audioSource.Play();
            var rotateVector = Vector3.forward * openedAngle;

            fridgeDoorSequence = DOTween.Sequence()
                .Append(transform.DOLocalRotate(rotateVector, animationDuration)
                    .SetEase(Ease.OutCubic));
        }

        private void CloseDoor()
        {
            KillSequence();
            audioSource.clip = closingAudioClip;
            audioSource.Play();
            var rotateVector = Vector3.forward * closedAngle;

            fridgeDoorSequence = DOTween.Sequence()
                .Append(transform.DOLocalRotate(rotateVector, animationDuration)
                    .SetEase(Ease.InCubic));
        }

        private void KillSequence()
        {
            if(fridgeDoorSequence == null)
                return;

            fridgeDoorSequence.Kill();
            fridgeDoorSequence = null;
        }
    }
}