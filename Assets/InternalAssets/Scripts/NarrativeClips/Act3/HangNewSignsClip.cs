using DG.Tweening;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using QuickOutline.Scripts;
using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts.Act3
{
    public class HangNewSignsClip : PlayableNarrativeClip
    {
        [SerializeField] private float workerPitch = 1.1f;
        [SerializeField] private float characterPitch = 1;
        [SerializeField] private float charDuration = 0.06f;

        #region UIDialogService
        private Sequence ShowPanel() => 
            ServicesHolder.UIDialogService.ShowPanel();
        private Sequence HidePanel(float duration) => 
            ServicesHolder.UIDialogService.HidePanel(duration);
        private Sequence ShowPanel(float duration) => 
            ServicesHolder.UIDialogService.ShowPanel(duration);
        private Sequence HidePanel() => 
            ServicesHolder.UIDialogService.HidePanel();
        private Sequence PrintDialog(string text, float symbolPrintDuration, float pitch = 1) =>
            ServicesHolder.UIDialogService.PrintDialog(text, symbolPrintDuration, pitch);
        private Sequence PrintDialogWoCleaning(string oldText, string text, float symbolPrintDuration,
            float pitch = 1) =>
            ServicesHolder.UIDialogService.PrintDialogWoCleaning(oldText, text, symbolPrintDuration, pitch);
        private void ClearPanel() => ServicesHolder.UIDialogService.ClearPanel();
        #endregion

        [SerializeField] private Transform signsParent;
        [SerializeField] private Transform[] signs;
        [SerializeField] private Outline[] signPlaces;
        [SerializeField] Transform playerItemSlot;
        [SerializeField] private Transform dispensePosition;
        
        
        public override void OnStart()
        {
            DOTween.Sequence()
                .AppendInterval(2f)
                .Append(ShowPanel())
                .Join(PrintDialog("Take those an hang them around entrance.", charDuration, workerPitch))
                .JoinCallback(() =>
                {
                    //TODO scratch sound
                    signsParent.DOMove(dispensePosition.position, 0.5f);
                })
                .AppendInterval(0.5f)
                .Append(PrintDialogWoCleaning(
                    "Take those an hang them around entrance. ", 
                    "There is a visor in your suit. I'v marked positions", 
                    charDuration, 
                    workerPitch))
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                    ServicesHolder.ObjectiveService.PrintObjective("Take the signs");
                })
                .AppendInterval(0.5f)
                .Append(HidePanel());
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

        private int signsCount;
        private void Interact(InteractableObject interactable)
        {
            interactable.ObjectInteractionScript.Interact();
            
            if (interactable.CustomTags.Contains("signs"))
            {
                ServicesHolder.ObjectiveService.ClearPanel();
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);
                signsParent.tag = "Untagged";
                signsParent.parent = playerItemSlot;
                playerItemSlot.position = signsParent.position;
                signsParent.localPosition = Vector3.zero;
                signsCount = 3;
                signPlaces[0].gameObject.SetActive(true);
                signPlaces[1].gameObject.SetActive(true);
                signPlaces[2].gameObject.SetActive(true);
            }
            if (interactable.CustomTags.Contains("signplace"))
            {
                signs[signsCount - 1].gameObject.SetActive(false);
                --signsCount;
                if (signsCount == 0)
                {
                    Destroy(signsParent.gameObject);
                    EndCallback();
                }
            }
        }
    }
}