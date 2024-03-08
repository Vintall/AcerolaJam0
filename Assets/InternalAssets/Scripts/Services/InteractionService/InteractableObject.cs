using System;
using System.Collections.Generic;
using QuickOutline.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace InternalAssets.Scripts.Services.InteractionService
{
    [RequireComponent(typeof(Outline), typeof(AbstractObjectInteractionScript))]
    public class InteractableObject : MonoBehaviour
    {
        private Outline _outline;
        [SerializeField] private List<string> customTags;
        [SerializeField] private AbstractObjectInteractionScript objectInteractionScript;
        private HashSet<string> _customTags;

        public Outline Outline => _outline;
        public AbstractObjectInteractionScript ObjectInteractionScript => objectInteractionScript;
        public HashSet<string> CustomTags => _customTags;


        private void Awake()
        {
            gameObject.tag = "Interactable";
            _outline = GetComponent<Outline>();
            _customTags = new HashSet<string>();
            foreach (var customTag in customTags)
                _customTags.Add(customTag);
        }
    }
}