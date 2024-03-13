using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

namespace InternalAssets.Scripts.NarrativeClips.Act3
{
    public class GatherItemsToTransferClip : PlayableNarrativeClip
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

        [SerializeField] private AudioSource keyboardAudioSource;
        [SerializeField] private AudioSource electricityAudioSource;
        [SerializeField] private AudioSource teleporterAudioSource;
        [SerializeField] private VisualEffect electricityVFX;
        [SerializeField] private AudioSource ambientAudioSource;
        
        public override void OnStart()
        {
            ServicesHolder.RaycastService._onHit += OnRaycast;
            
            ServicesHolder.ObjectiveService.PrintObjective("Send a message");
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
            
            if (interactable.CustomTags.Contains("keyboard"))
            {
                ServicesHolder.ObjectiveService.ClearPanel();
                ServicesHolder.RaycastService.MarkDisableOutline(interactable);
                ServicesHolder.RaycastService._onHit -= OnRaycast;

                DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        keyboardAudioSource.Play();
                        ServicesHolder.PlayerInputService.enabled = false;
                    })
                    .AppendInterval(3f)
                    .AppendCallback(() =>
                    {
                        teleporterAudioSource.Play();
                    })
                    .AppendInterval(2f)
                    .Append(ShowPanel())
                    .Join(PrintDialog(
                        "Huh?",
                        charDuration,
                        characterPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Huh? ",
                        "Who the hell are you?",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1.7f)
                    .Append(PrintDialogWoCleaning(
                        "Huh? Who the hell are you? ",
                        "Stay where you are!",
                        charDuration,
                        workerPitch + 0.1f))
                    .AppendInterval(0.5f)
                    .Append(HidePanel(0.1f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.2f))
                    .Append(PrintDialog(
                        "Wait, don't shoot. I'm just a new guy.",
                        charDuration * 0.7f,
                        1.7f))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "Wait, don't shoot. I'm just a new guy. ",
                        "They said they would warn about me",
                        charDuration * 0.7f,
                        1.7f))
                    .AppendInterval(0.5f)
                    .Append(HidePanel(0.1f))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0.3f))
                    .Join(PrintDialog(
                        "We already had one today.",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "We already had one today. ",
                        "Show me your ID.",
                        charDuration,
                        workerPitch))
                    .AppendInterval(0.7f)
                    .Append(PrintDialogWoCleaning(
                        "We already had one today. Show me your ID. ",
                        "Immediately",
                        charDuration,
                        workerPitch))
                    .AppendInterval(3f)
                    .Append(PrintDialog(
                        "Oh no...",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1.5f)
                    .Append(PrintDialogWoCleaning(
                        "Oh no...",
                        "That can't be good.",
                        charDuration,
                        workerPitch))
                    .AppendInterval(1.5f)
                    .Append(PrintDialog(
                        "You peace of",
                        charDuration,
                        workerPitch))
                    .Append(HidePanel(0))
                    .AppendCallback(ClearPanel)
                    .Append(ShowPanel(0))
                    .Append(PrintDialog(
                        "Wait, I can expl",
                        charDuration,
                        characterPitch))
                    .Append(HidePanel())
                    .JoinCallback(() =>
                    {
                        electricityAudioSource.Play();
                        electricityVFX.gameObject.SetActive(true);
                        electricityVFX.Play();
                        ambientAudioSource.DOFade(0, 2.5f);
                        electricityAudioSource.DOFade(0, 2.5f);
                    })
                    .AppendInterval(1.5f)
                    .AppendCallback(() =>
                    {
                        SceneManager.LoadScene(4);
                    });
            }
        }
    }
}