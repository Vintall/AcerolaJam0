using DG.Tweening;
using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private ParticleSystem electricityParticleSystem;
        
        public override void OnStart()
        {
            ServicesHolder.RaycastService._onHit += OnRaycast;
            
            ServicesHolder.ObjectiveService.PrintObjective("Send a message to internet. Then sabotage the station.");
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
                        teleporterAudioSource.Play();
                        electricityAudioSource.Play();
                        electricityParticleSystem.Play();
                        keyboardAudioSource.Play();
                        ServicesHolder.PlayerInputService.enabled = false;
                    })
                    .AppendCallback(() =>
                    {
                        SceneManager.LoadScene(4);
                    });
            }
        }
    }
}