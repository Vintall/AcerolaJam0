using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using QuickOutline.Scripts;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act3
{
    public class RevisionClip : PlayableNarrativeClip
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

        [SerializeField] private GameObject shelfHighlight;
        [SerializeField] private GameObject revisionHighlightsParent;
        [SerializeField] private Transform textbookInteractable;
        [SerializeField] private Transform textbookHandheld;
        [SerializeField] private Transform dispensePosition;
        [SerializeField] private Transform hidePosition;
        [SerializeField] private Transform playerItemSlot;
        [SerializeField] private AudioSource paperCheckAudioSource;

        [SerializeField] private AudioSource sandpaperAudioSource;

        public override void OnStart()
        {
            DOTween.Sequence()
                .AppendInterval(1f)
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "The next transfer is near", 
                    charDuration, 
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialog(
                    "We need to conduct revision",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.5f)
                .Append(PrintDialog(
                    "Take it and check the tents",
                    charDuration,
                    workerPitch))
                .JoinCallback(() =>
                {
                    sandpaperAudioSource.Play();
                    textbookInteractable.DOMove(dispensePosition.position, 0.5f);
                })
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                    shelfHighlight.SetActive(true);
                    ServicesHolder.ObjectiveService.PrintObjective("Take the clipboard");
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
                    if(interactableObject.CustomTags.Contains("shelf") && !isRevisionDone)
                        return;
                    
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


        private bool isClipboardPicked = false;
        private bool isRevisionDone = false;
        private int revisionsLeft = 5;
        private void Interact(InteractableObject interactable)
        {
            interactable.ObjectInteractionScript.Interact();

            if (interactable.CustomTags.Contains("revision"))
            {
                ServicesHolder.ObjectiveService.ClearPanel();
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);

                --revisionsLeft;
                paperCheckAudioSource.Play();
                switch (revisionsLeft)
                {
                    case 4:
                        Left4Revisions();
                        break;
                    case 3:
                        Left3Revisions();
                        break;
                    case 2:
                        Left2Revisions();
                        break;
                    case 1:
                        Left1Revisions();
                        break;
                    case 0:
                        Left0Revisions();
                        break;
                }
            }

            if (interactable.CustomTags.Contains("shelf"))
            {
                textbookHandheld.parent = hidePosition;
                shelfHighlight.gameObject.SetActive(false);
                DOTween.Sequence()
                    .Append(textbookHandheld.DOMove(dispensePosition.position, 0.5f))
                    .AppendCallback(() =>
                    {
                        sandpaperAudioSource.Play();
                    })
                    .Append(textbookHandheld.DOMove(hidePosition.position, 0.5f))
                    .AppendCallback(() =>
                    {
                        DOTween.Sequence()
                            .AppendCallback(() =>
                            {
                                ServicesHolder.RaycastService._onHit -= OnRaycast;
                            })
                            .AppendInterval(1f)
                            .Append(ShowPanel())
                            .Join(PrintDialog(
                                "Alright, we've done enough for today.",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.7f)
                            .Append(PrintDialogWoCleaning(
                                "Alright, we've done enough for today. ",
                                "You can rest.",
                                charDuration,
                                workerPitch))
                            .AppendInterval(0.5f)
                            .Append(HidePanel())
                            .AppendCallback(() =>
                            {
                                EndClip();
                            });
                    });
            }
            
            if (interactable.CustomTags.Contains("textbook"))
            {
                shelfHighlight.SetActive(false);
                sandpaperAudioSource.Play();
                textbookInteractable.gameObject.SetActive(false);
                textbookHandheld.gameObject.SetActive(true);
                playerItemSlot.position = textbookHandheld.position;
                textbookHandheld.parent = playerItemSlot;
                textbookHandheld.localPosition = Vector3.zero;
                textbookHandheld.localEulerAngles = new Vector3(-150, 30, 60);
                ServicesHolder.ObjectiveService.PrintObjective("Check the tents");
                revisionHighlightsParent.gameObject.SetActive(true);
                isClipboardPicked = true;
            }
        }
        private void Left4Revisions()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit -= OnRaycast;
                })
                .AppendInterval(1f)
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "And what are we exchanging?",
                    charDuration,
                    characterPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .Append(ShowPanel(0.3f))
                .Join(PrintDialog("I don't know.",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "I don't know. ",
                    "Something called USB stick.",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "I don't know. Something called USB stick. ",
                    "It's memory related",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel(0.3f))
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                });
        }
        private void Left3Revisions()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit -= OnRaycast;
                })
                .AppendInterval(1f)
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "No, I mean...",
                    charDuration,
                    characterPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "No, I mean... ",
                    "What we giving them in exchange",
                    charDuration,
                    characterPitch))
                .AppendInterval(1f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .Append(ShowPanel(0.3f))
                .Join(PrintDialog(
                    "Some garbage, by people's standarts.",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "Some garbage, by people's standarts. ",
                    "They want to study us.",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialog(
                    "But...",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                });
        }
        private void Left2Revisions()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit -= OnRaycast;
                })
                .AppendInterval(1f)
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "But?",
                    charDuration,
                    characterPitch))
                .AppendInterval(1f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .AppendInterval(2f)
                .Append(ShowPanel(0.3f))
                .Join(PrintDialog(
                    "That garbage is only small part of the deal...",
                    charDuration,
                    workerPitch))
                .AppendInterval(2f)
                .Append(PrintDialogWoCleaning(
                    "That garbage is only small part of the deal... ",
                    "They wanted live specimens...",
                    charDuration,
                    workerPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "That garbage is only small part of the deal... They wanted live specimens... ",
                    "Of us",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .Append(ShowPanel(0.3f))
                .Append(PrintDialog(
                    "...",
                    charDuration,
                    characterPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                });
        }
        private void Left1Revisions()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit -= OnRaycast;
                })
                .AppendInterval(1f)
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "And you agreed?",
                    charDuration,
                    characterPitch))
                .AppendInterval(1f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .AppendInterval(2f)
                .Append(ShowPanel(0.3f))
                .Join(PrintDialog(
                    "Maybe heard about disappearance series in towns, so...",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialogWoCleaning(
                    "Maybe heard about disappearance series in towns, so... ",
                    "Yeah...",
                    charDuration,
                    workerPitch))
                .AppendInterval(1.7f)
                .Append(PrintDialogWoCleaning(
                    "Maybe heard about disappearance series in towns, so... Yeah... ",
                    "For almost two years",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .Append(ShowPanel(0.3f))
                .Append(PrintDialog(
                    "...",
                    charDuration,
                    characterPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                });
        }
        private void Left0Revisions()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit -= OnRaycast;
                })
                .AppendInterval(1f)
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "It's illegal.",
                    charDuration,
                    characterPitch))
                .AppendInterval(1.2f)
                .Append(PrintDialogWoCleaning(
                    "It's illegal. ",
                    "And immoral.",
                    charDuration,
                    characterPitch))
                .AppendInterval(1f)
                .Append(HidePanel(0.5f))
                .AppendCallback(ClearPanel)
                .Append(ShowPanel(0.3f))
                .Join(PrintDialog(
                    "Why do YOU care?",
                    charDuration,
                    workerPitch))
                .AppendInterval(1.5f)
                .Append(PrintDialogWoCleaning(
                    "Why do YOU care? ",
                    "Wake up, we financed by the government.",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialog(
                    "So that just how it goes.",
                    charDuration,
                    workerPitch))
                .AppendInterval(2.5f)
                .Append(PrintDialog(
                    "And don't tell me about that 'Immoral stuff'.",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialogWoCleaning(
                    "And don't tell me about that 'Immoral stuff'. ",
                    "You are here not because you are exactly a moral citizen. ",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialog(
                    "I've read your file",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(PrintDialog(
                    "We are all here like this.",
                    charDuration,
                    workerPitch))
                .AppendInterval(1f)
                .Append(HidePanel(0.3f))
                .AppendCallback(ClearPanel)
                .Append(ShowPanel(0.3f))
                .Append(PrintDialog(
                    "...",
                    charDuration,
                    characterPitch))
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .AppendInterval(3.5f)
                .AppendCallback(() =>
                {
                    ServicesHolder.GibberishService.SetVolume(0f);
                })
                .Append(ShowPanel())
                .Join(PrintDialog(
                    "*He seems to really belive...*",
                    charDuration,
                    characterPitch))
                .AppendInterval(1.5f)
                .Append(PrintDialog(
                    "*This is a chance to sabotage all of this...*",
                    charDuration,
                    characterPitch))
                .AppendInterval(1.5f)
                .Append(PrintDialog(
                    "*All I need is to print a message into network... *",
                    charDuration,
                    characterPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialogWoCleaning(
                    "*All I need is to print a message into network... ",
                    "Tell the people truth*",
                    charDuration,
                    characterPitch))
                .AppendInterval(0.7f)
                .Append(PrintDialog(
                    "*The computer...*",
                    charDuration,
                    characterPitch))
                .AppendCallback(() =>
                {
                    ServicesHolder.GibberishService.SetDefaultVolume();
                })
                .AppendInterval(0.5f)
                .Append(HidePanel())
                .AppendCallback(() =>
                {
                    ServicesHolder.RaycastService._onHit += OnRaycast;
                    ServicesHolder.ObjectiveService.PrintObjective("Bring the textbook back");
                    isRevisionDone = true;
                    shelfHighlight.gameObject.SetActive(true);
                });
        }
    }
}
