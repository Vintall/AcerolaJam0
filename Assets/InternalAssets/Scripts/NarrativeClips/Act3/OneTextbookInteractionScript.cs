using InternalAssets.Scripts.Services.InteractionService;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act3
{
    public class OneTextbookInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private string _interactionText;

        public override void Interact()
        {
            
        }

        public override string InteractionText => _interactionText;
    }
}