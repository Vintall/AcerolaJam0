using QuickOutline.Scripts;
using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts.Act3
{
    public class SignsPlacesInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private string interactionText;
        [SerializeField] private Transform sign;
        public override void Interact()
        {
            sign.gameObject.SetActive(true);
            gameObject.SetActive(false);
            transform.parent.GetComponent<Outline>().enabled = false;
        }

        public override string InteractionText => interactionText;
    }
}