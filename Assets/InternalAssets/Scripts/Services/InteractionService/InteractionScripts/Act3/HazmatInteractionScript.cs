using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts.Act3
{
    public class HazmatInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private string interactionText;

        public override void Interact()
        {
            gameObject.SetActive(false);
        }

        public override string InteractionText => interactionText;
    }
}