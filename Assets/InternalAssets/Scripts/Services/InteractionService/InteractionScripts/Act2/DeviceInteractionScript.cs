using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts.Act2
{
    public class DeviceInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private string interactionText;

        public override void Interact()
        {
            
        }

        public override string InteractionText => interactionText;
    }
}