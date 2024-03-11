using System.Collections.Generic;
using UnityEngine;

namespace InternalAssets.Scripts.Services.InteractionService.InteractionScripts
{
    public class PileOfMessInteractionScript : AbstractObjectInteractionScript
    {
        [SerializeField] private List<GameObject> spawnableObjects;
        [SerializeField] private string interactionText;
        [SerializeField] private AudioSource audioSource;

        public override void Interact()
        {
            audioSource.Play();
            gameObject.SetActive(false);
            foreach (var spawnableObject in spawnableObjects)
                spawnableObject.SetActive(true);
        }

        public override string InteractionText => interactionText;
    }
}