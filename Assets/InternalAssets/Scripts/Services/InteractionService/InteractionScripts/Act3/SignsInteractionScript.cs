using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts.Act3
{
    public class SignsInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private string interactionText;

        public override void Interact()
        {
        }

        public override string InteractionText => interactionText;
    }
}