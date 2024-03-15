using System;
using TMPro;
using UnityEngine;

namespace InternalAssets.Scripts.Services.UIServices
{
    public class UIInteractionService : MonoBehaviour
    {
        [SerializeField] private TMP_Text interactionText;

        private void Awake()
        {
            ClearData();
        }

        public void SetInteractionData(string text, Color color)
        {
            SetInteractionText(text);
            SetInteractionColor(color);
        }

        public void ClearData()
        {
            interactionText.text = "";
            interactionText.color = Color.white;
        }
        
        private void SetInteractionText(string text)
        {
            interactionText.text = text;
        }

        private void SetInteractionColor(Color color)
        {
            interactionText.color = color;
        }
    }
}