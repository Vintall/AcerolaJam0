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
        
        
        public override void Interact()
        {
            if(_isOpen)
                CloseDoor();
            else
                OpenDoor();

            _isOpen = !_isOpen;
        }

        private void OpenDoor()
        {
            KillSequence();

            var rotateVector = Vector3.forward * openedAngle;

            fridgeDoorSequence = DOTween.Sequence()
                .Append(transform.DOLocalRotate(rotateVector, animationDuration)
                    .SetEase(Ease.OutCubic));
        }

        private void CloseDoor()
        {
            KillSequence();
            
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