using System;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace InternalAssets.Scripts.Services.UIServices
{
    public class UIDialogService : MonoBehaviour
    {
        [SerializeField] private RectTransform dialogPanel;
        [SerializeField] private TMP_Text dialogText;

        [SerializeField] private float panelSlideDuration;
        [SerializeField] private float awaitDuration;
        private Sequence _printSequence;

        private void Awake()
        {
            ClearPanel();
        }

        public void ShowPanel()
        {
            
        }
        
        public void HidePanel()
        {
            var minimizePosition = -Screen.width / 3;
            var hidePosition = minimizePosition - Screen.width * 0.02f;
            
            _printSequence = DOTween.Sequence()
                .Append(dialogPanel.DOAnchorPosY(hidePosition, panelSlideDuration)
                    .SetEase(Ease.OutCubic));
        }

        public void ClearPanel()
        {
            _printSequence.Kill();
            _printSequence = null;
            dialogText.text = "";
        }

        public void HideImmediately()
        {
            var minimizePosition = -Screen.width / 3;
            var hidePosition = minimizePosition - Screen.width * 0.02f;

            dialogPanel.DOAnchorPosY(hidePosition, 0);
        }
        
        public void PrintDialog(string text, float symbolPrintDuration)
        {
            if(_printSequence != null)
                return;
            
            _printSequence = DOTween.Sequence()
                .Append(dialogPanel.DOAnchorPosY(0, panelSlideDuration)
                    .SetEase(Ease.OutCubic));

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < text.Length; ++i)
            {
                _printSequence
                    .AppendCallback(() =>
                    {
                        stringBuilder.Append(text[stringBuilder.Length]);
                        dialogText.text = stringBuilder.ToString();
                    })
                    .AppendInterval(symbolPrintDuration);
            }

            _printSequence
                .AppendInterval(awaitDuration)
                .AppendCallback(HidePanel);
        }
        
    }
}