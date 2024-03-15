using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts
{
    public class CouchInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private string interactionText;

        public override void Interact()
        {
            
        }

        public override string InteractionText => interactionText;
    }
}