using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using QuickOutline.Scripts;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act3
{
    public class GetTextbooksClip : PlayableNarrativeClip
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


        [SerializeField] private Transform clipboards;
        [SerializeField] private Transform clipboardsHandheld;
        [SerializeField] Transform playerItemSlot;
        [SerializeField] private GameObject textbooksHighlight;
        [SerializeField] private GameObject shelfHighlight;

        [SerializeField] private Transform dispensePosition;
        [SerializeField] private Transform hidePosition;
        
        public override void OnStart()
        {
            var dialogService = ServicesHolder.UIDialogService;
            //var standartDuration = 0.06f;

            textbooksHighlight.SetActive(true);
            ServicesHolder.RaycastService._onHit += OnRaycast;
            
            return;
            
            DOTween.Sequence()
                .AppendInterval(2f)
                .Append(ShowPanel())
                .Join(PrintDialog("Take those an hang them around entrance.", charDuration, workerPitch))
                .JoinCallback(() =>
                {
                    
                })
                .AppendInterval(0.5f)
                .Append(PrintDialogWoCleaning(
                    "Take those an hang them around entrance. ", 
                    "There is a visor in your suit. I'v marked positions", 
                    charDuration, 
                    workerPitch))
                .AppendCallback(() =>
                {
                    
                    ServicesHolder.ObjectiveService.PrintObjective("Take the signs");
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

        private bool isTextbooksPicked = false;
        private void Interact(InteractableObject interactable)
        {
            interactable.ObjectInteractionScript.Interact();
            
            if (interactable.CustomTags.Contains("textbooks"))
            {
                isTextbooksPicked = true;
                textbooksHighlight.SetActive(false);
                shelfHighlight.SetActive(true);
                ServicesHolder.ObjectiveService.ClearPanel();
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);
                clipboards.gameObject.SetActive(false);
                clipboardsHandheld.gameObject.SetActive(true);
                clipboardsHandheld.transform.parent = playerItemSlot;
                playerItemSlot.position = clipboardsHandheld.position;
                clipboardsHandheld.localPosition = Vector3.zero;
            }

            if (interactable.CustomTags.Contains("shelf") && isTextbooksPicked)
            {
                shelfHighlight.SetActive(false);
                clipboardsHandheld.parent = hidePosition;
                DOTween.Sequence()
                    .Append(clipboardsHandheld.DOMove(dispensePosition.position, 0.3f))
                    .Append(clipboardsHandheld.DOMove(hidePosition.position, 0.3f))
                    .AppendCallback(() =>
                    {
                        EndCallback();
                    });
            }
        }
    }
}