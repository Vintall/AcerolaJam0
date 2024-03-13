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

        [SerializeField] private AudioSource paperTwist;
        [SerializeField] private AudioSource sandpaper;
        
        public override void OnStart()
        {
            DOTween.Sequence()
                .AppendInterval(2f)
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "Normally, that place is flooded with people. ", 
                    charDuration, 
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialog(
                    "But right now there is only two of us.",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "But right now there is only two of us. ",
                    "The rest",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "But right now there is only two of us. The rest ",
                    "... erm ...",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "But right now there is only two of us. The rest ... erm ...",
                    " kinda busy",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .AppendInterval(5f)
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "Oh, you done",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialog(
                    "There is a documents I need",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "There is a documents I need, ",
                    "it is in the next dome.",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialog(
                    "Get me those, please",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .JoinCallback(() =>
                {
                    textbooksHighlight.SetActive(true);
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                    ServicesHolder.ObjectiveService.PrintObjective("Get the documents");
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
                paperTwist.Play();
                isTextbooksPicked = true;
                textbooksHighlight.SetActive(false);
                
                ServicesHolder.ObjectiveService.ClearPanel();
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);
                clipboards.gameObject.SetActive(false);
                clipboardsHandheld.gameObject.SetActive(true);
                clipboardsHandheld.transform.parent = playerItemSlot;
                playerItemSlot.position = clipboardsHandheld.position;
                clipboardsHandheld.localPosition = Vector3.zero;

                DOTween.Sequence()
                    .Append(ShowPanel())
                    .Join(PrintDialog(
                        "Oh, hey",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Oh, hey, ",
                        "there is a computer in here",
                        charDuration,
                        characterPitch))
                    .AppendInterval(1.5f)
                    .Append(HidePanel(0.3f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Join(PrintDialog(
                        "Ok?",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.5f)
                    .Append(HidePanel(0.3f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Join(PrintDialog(
                        "I mean",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "I mean, ",
                        "Is it working?",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.5f)
                    .Append(HidePanel(0.3f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Join(PrintDialog(
                        "Yeah, sure",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Yeah, sure. ",
                        "Feel free",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.5f)
                    .Append(HidePanel())
                    .AppendInterval(5f)
                    .Append(ShowPanel())
                    .Join(PrintDialog(
                        "Actually ...",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1f)
                    .Append(PrintDialogWoCleaning(
                        "Actually ... ",
                        "This base is",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Actually ... This base is ",
                        "kinda",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Actually ... This base is kinda ",
                        "origin of staff like that",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1f)
                    .Append(HidePanel(0.3f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Join(PrintDialog(
                        "You mean",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "You mean, ",
                        "computers?",
                        charDuration,
                        characterPitch))
                    .AppendInterval(1f)
                    .Append(PrintDialog(
                        "You invented computers?",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.3f)
                    .Append(HidePanel(0.3f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Join(PrintDialog(
                        "Well ...",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Well ... ",
                        "Not exactly invented...",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1f)
                    .Append(HidePanel())
                    .AppendCallback(ClearPanel)
                    .AppendInterval(3f)
                    .Append(ShowPanel())
                    .Join(PrintDialog(
                        "Did you hear all that stories",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Did you hear all that stories ",
                        "about",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Did you hear all that stories about ",
                        "UFO and stuff",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1f)
                    .Append(HidePanel())
                    .AppendInterval(2f)
                    .Append(ShowPanel(0.3f))
                    .Join(PrintDialog(
                        "Most of it",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Most of it, ",
                        "of course",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Most of it, of course, ",
                        "Just a stories",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1.5f)
                    .Append(PrintDialog(
                        "But there was a contact",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "But there was a contact. ",
                        "Around two years ago",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(HidePanel(0.3f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Append(PrintDialog(
                        "Ahah",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Ahah, ",
                        "no way it's true",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Ahah, no way it's true ",
                        "You're just messing with me, dude",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.7f)
                    .Append(HidePanel())
                    .AppendCallback(() =>
                    {
                        ServicesHolder.RaycastService._onHit += OnRaycast;
                        ServicesHolder.ObjectiveService.PrintObjective("Bring the documents");
                        shelfHighlight.SetActive(true);
                    });
            }

            if (interactable.CustomTags.Contains("shelf") && isTextbooksPicked)
            {
                sandpaper.Play();
                shelfHighlight.SetActive(false);
                clipboardsHandheld.parent = hidePosition;
                isTextbooksPicked = false;
                ServicesHolder.RaycastService._onHit -= OnRaycast;
                DOTween.Sequence()
                    .Append(clipboardsHandheld.DOMove(dispensePosition.position, 0.3f))
                    .Append(clipboardsHandheld.DOMove(hidePosition.position, 0.3f))
                    .AppendCallback(() =>
                    {
                        DOTween.Sequence()
                            .Append(ShowPanel())
                            .Join(PrintDialog(
                                "Where do you think we took all that stuff?",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.7f)
                            .Append(PrintDialogWoCleaning(
                                "Where do you think we took all that stuff? ",
                                "Invented by ourself. No way.",
                                charDuration,
                                workerPitch))
                            .AppendInterval(1f)
                            .Append(PrintDialog(
                                "What about your teleporter?",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.7f)
                            .Append(PrintDialogWoCleaning(
                                "What about your teleporter? ",
                                "Also invented?",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.7f)
                            .Append(HidePanel(0.3f))
                            .AppendCallback(ClearPanel)
                            .Append(ShowPanel(0.3f))
                            .Join(PrintDialog(
                                "Hmm",
                                charDuration,
                                characterPitch))
                            .AppendInterval(1f)
                            .Append(HidePanel(0.3f))
                            .AppendCallback(ClearPanel)
                            .Append(ShowPanel(0.3f))
                            .Join(PrintDialog(
                                "Teleporter was one of the first technology we got.",
                                charDuration,
                                workerPitch))
                            .AppendInterval(1f)
                            .Append(PrintDialog(
                                "But we never gave it to masses.",
                                charDuration,
                                characterPitch))
                            .AppendInterval(0.7f)
                            .Append(PrintDialogWoCleaning(
                                "But we never gave it to masses. ",
                                "It's too powerfull.",
                                charDuration,
                                workerPitch))
                            .AppendInterval(1.5f)
                            .Append(PrintDialog(
                                "There actually a lot of stuff in overall use.",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.5f)
                            .Append(PrintDialogWoCleaning(
                                "But there actually a lot of stuff in overall use. ",
                                "Like ",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.5f)
                            .Append(PrintDialogWoCleaning(
                                "But there actually a lot of stuff in overall use. Like ",
                                "microwaves,",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.7f)
                            .Append(PrintDialogWoCleaning(
                                "But there actually a lot of stuff in overall use. Like microwaves, ",
                                "computers",
                                charDuration,
                                workerPitch))
                            .AppendInterval(1f)
                            .Append(PrintDialogWoCleaning(
                                "But there actually a lot of stuff in overall use. Like microwaves, computers.",
                                "You name it",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.5f)
                            .Append(HidePanel())
                            .AppendCallback(() =>
                            {
                                EndCallback();
                            });
                    });
            }
        }
    }
}