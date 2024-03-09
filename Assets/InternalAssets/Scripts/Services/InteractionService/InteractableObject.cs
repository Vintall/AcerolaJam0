using System;
using System.Collections.Generic;
using QuickOutline.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace InternalAssets.Scripts.Services.InteractionService
{
    [RequireComponent(typeof(Outline))]
    public class InteractableObject : MonoBehaviour
    {
        private Outline _outline;
        [SerializeField] private List<string> customTags;
        [SerializeField] private AbstractObjectInteractionScript objectInteractionScript;
        private HashSet<string> _customTags;
        
        public AbstractObjectInteractionScript ObjectInteractionScript => objectInteractionScript;
        public HashSet<string> CustomTags => _customTags;
        public string InteractionText => objectInteractionScript.InteractionText;
        public Color OutlineColor => _outline.OutlineColor;
        
        
        private void Awake()
        {
            gameObject.tag = "Interactable";
            _outline = GetComponent<Outline>();
            _customTags = new HashSet<string>();
            foreach (var customTag in customTags)
                _customTags.Add(customTag);
        }

        public virtual void SetOutlineColor(Color color)
        {
            _outline.OutlineColor = color;
        }

        public virtual void SetOutlineWidth(float width)
        {
            _outline.OutlineWidth = width;
        }
    }
}