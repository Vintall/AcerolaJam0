using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts
{
    public class BeerCanInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private string interactionText;
        
        public override string InteractionText => interactionText;
        
        public override void Interact()
        {
            Debug.Log("BeerInteract");
        }
    }
}