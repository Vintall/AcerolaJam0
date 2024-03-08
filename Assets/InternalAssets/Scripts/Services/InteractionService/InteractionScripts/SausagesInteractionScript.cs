using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts
{
    public class SausagesInteractionScript : AbstractObjectInteractionScript
    {
        public override void Interact()
        {
            gameObject.SetActive(false);
            Debug.Log("SausagesInteract");
        }
    }
}