using DG.Tweening;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using QuickOutline.Scripts;
using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts.Act3
{
    public class HangNewSignsClip : PlayableNarrativeClip
    {
        [SerializeField] private float workerPitch = 1.5f;
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
        [SerializeField] private AudioSource metalScratch;
        [SerializeField] private AudioSource metalPickup;
        [SerializeField] private AudioSource fenceSound;
        [SerializeField] private GameObject shelfHighlight;
        
        
        public override void OnStart()
        {
            DOTween.Sequence()
                .AppendInterval(2f)
                .Append(ShowPanel())
                .Join(PrintDialog("Take those an hang them around entrance.", charDuration, workerPitch))
                .JoinCallback(() =>
                {
                    metalScratch.Play();
                    signsParent.DOMove(dispensePosition.position, 0.5f);
                })
                .AppendInterval(0.5f)
                .Append(PrintDialogWoCleaning(
                    "Take those an hang them around entrance. ", 
                    "There is a visor in your suit. I've marked positions", 
                    charDuration, 
                    workerPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                    shelfHighlight.SetActive(true);
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

        private int signsCount;
        private void Interact(InteractableObject interactable)
        {
            interactable.ObjectInteractionScript.Interact();
            
            if (interactable.CustomTags.Contains("signs"))
            {
                shelfHighlight.SetActive(false);
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
                metalPickup.Play();

                DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        ServicesHolder.RaycastService._onHit -= OnRaycast;
                    })
                    .Append(ShowPanel())
                    .Join(PrintDialog(
                        "While you're busy, I'll bring you up to speed.", 
                        charDuration, 
                        workerPitch))
                    .AppendInterval(1f)
                    .Append(PrintDialog(
                        "What they already told you about staff around here?", 
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.5f)
                    .Append(HidePanel(0.3f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Join(PrintDialog(
                        "Not much",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.5f)
                    .Append(HidePanel(0.3f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Append(PrintDialog(
                        "Well, that's ok.",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Well, that's ok. ",
                        "Everyone's a little nervous about information leaks. ",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1.5f)
                    .Append(PrintDialog(
                        "Just look at this place.",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Just look at this place. ",
                        "No wonder ...",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.5f)
                    .Append(HidePanel())
                    .AppendCallback(() =>
                    {
                        ServicesHolder.RaycastService._onHit += OnRaycast;
                    });
            }
            if (interactable.CustomTags.Contains("signplace"))
            {
                signs[signsCount - 1].gameObject.SetActive(false);
                --signsCount;
                fenceSound.Play();
                switch (signsCount)
                {
                    case 2:
                        OnHangFirstSign();
                        break;
                    case 1:
                        OnHangSecondSign();
                        break;
                }
                
                if (signsCount == 0)
                {
                    Destroy(signsParent.gameObject);
                    EndCallback();
                }
            }
        }
        
        private void OnHangFirstSign()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit -= OnRaycast;
                    ServicesHolder.GibberishService.SetVolume(0f);
                })
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "*Oh my god...",
                    charDuration,
                    characterPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "*Oh my god... ",
                    "What the hell have I gotten myself into...*",
                    charDuration,
                    characterPitch))
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                    ServicesHolder.GibberishService.SetDefaultVolume();
                });
        }

        private void OnHangSecondSign()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit -= OnRaycast;
                })
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "We'll be changing up the post each week.",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "We'll be changing up the post each week. ",
                    "For the next week, you sit here and I work.",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialog(
                    "Company policy.",
                    charDuration,
                    workerPitch))
                .AppendInterval(1.3f)
                .Append(PrintDialogWoCleaning(
                    "Company policy. ",
                    "Something about security",
                    charDuration,
                    workerPitch))
                .AppendInterval(1.8f)
                .Append(PrintDialogWoCleaning(
                    "Company policy. Something about security, ",
                    "I guess",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(HidePanel())
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                });
        }
    }
}