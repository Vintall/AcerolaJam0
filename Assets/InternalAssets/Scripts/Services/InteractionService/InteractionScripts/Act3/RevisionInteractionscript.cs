using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts.Act3
{
    public class RevisionInteractionscript : AbstractObjectInteractionScript
    {
        [SerializeField] private string _interactionText;

        public override void Interact()
        {
            transform.parent.gameObject.SetActive(false);
        }

        public override string InteractionText => _interactionText;
    }
}