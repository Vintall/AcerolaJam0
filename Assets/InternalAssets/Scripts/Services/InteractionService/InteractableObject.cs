using System;
using QuickOutline.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace InternalAssets.Scripts.Services.InteractionService
{
    [RequireComponent(typeof(Outline), typeof(AbstractObjectInteractionScript))]
    public class InteractableObject : MonoBehaviour
    {
        private Outline _outline;
        [SerializeField] private AbstractObjectInteractionScript objectInteractionScript;

        public Outline Outline => _outline;
        public AbstractObjectInteractionScript ObjectInteractionScript => objectInteractionScript;
        
        private void Awake()
        {
            gameObject.tag = "Interactable";
            _outline = GetComponent<Outline>();
        }
    }
}