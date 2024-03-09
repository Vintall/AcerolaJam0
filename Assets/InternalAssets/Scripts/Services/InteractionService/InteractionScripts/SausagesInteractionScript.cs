using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts
{
    public class SausagesInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private string interactionText;
        
        public override string InteractionText => interactionText;
        
        public override void Interact()
        {
            gameObject.SetActive(false);
            Debug.Log("SausagesInteract");
        }
    }
}