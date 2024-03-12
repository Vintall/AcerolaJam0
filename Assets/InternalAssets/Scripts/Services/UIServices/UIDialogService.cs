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
            HidePanel(0);
        }

        public Sequence ShowPanel()
        {
            return DOTween.Sequence()
                .Append(dialogPanel.DOAnchorPosY(0, panelSlideDuration)
                    .SetEase(Ease.OutCubic));
        }
        public Sequence ShowPanel(float duration)
        {
            return DOTween.Sequence()
                .Append(dialogPanel.DOAnchorPosY(0, duration)
                    .SetEase(Ease.OutCubic));
        }
        
        public Sequence HidePanel()
        {
            var minimizePosition = -Screen.width / 3;
            var hidePosition = minimizePosition - Screen.width * 0.02f;
            
            return DOTween.Sequence()
                .Append(dialogPanel.DOAnchorPosY(hidePosition, panelSlideDuration)
                    .SetEase(Ease.OutCubic));
        }
        
        public Sequence HidePanel(float duration)
        {
            var minimizePosition = -Screen.width / 3;
            var hidePosition = minimizePosition - Screen.width * 0.02f;
            
            return DOTween.Sequence()
                .Append(dialogPanel.DOAnchorPosY(hidePosition, duration)
                    .SetEase(Ease.OutCubic));
        }

        public void ClearPanel()
        {
            _printSequence.Kill();
            _printSequence = null;
            dialogText.text = "";
        }

        public Sequence PrintDialog(string text, float symbolPrintDuration, float pitch = 1)
        {
            if (_printSequence != null)
            {
                _printSequence.Kill();
                ClearPanel();
                _printSequence = null;
            }

            _printSequence = DOTween.Sequence()
                .AppendCallback(() => ServicesHolder.GibberishService.PlayStream(pitch));
            
            
            var stringBuilder = new StringBuilder(text.Length);
            for (var i = 0; i < text.Length; ++i)
            {
                _printSequence
                    .AppendCallback(() =>
                    {
                        stringBuilder.Append(text[stringBuilder.Length]);
                        dialogText.text = stringBuilder.ToString();
                    })
                    .AppendInterval(symbolPrintDuration);
                
                if (i == text.Length - 2)
                    _printSequence
                        .AppendCallback(() =>
                        {
                            ServicesHolder.GibberishService.StopStream();
                        });
            }
            return _printSequence;
        }
        
        public Sequence PrintDialogWoCleaning(string oldText, string text, float symbolPrintDuration, float pitch = 1)
        {
            if (_printSequence != null)
            {
                _printSequence.Kill();
                _printSequence = null;
            }
            var stringBuilder = new StringBuilder(text.Length);

            _printSequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.GibberishService.PlayStream(pitch);
                    stringBuilder.Append(oldText);
                });
            
            
            
            for (var i = 0; i < text.Length; ++i)
            {
                _printSequence
                    .AppendCallback(() =>
                    {
                        stringBuilder.Append(text[stringBuilder.Length - oldText.Length]);
                        dialogText.text = stringBuilder.ToString();
                    })
                    .AppendInterval(symbolPrintDuration);
                
                if (i == text.Length - 2)
                    _printSequence
                        .AppendCallback(() =>
                        {
                            ServicesHolder.GibberishService.StopStream();
                        });
            }
            return _printSequence;
        }

        public Sequence AddDialog(string text, float symbolPrintDuration, float pitch = 1)
        {
            if (_printSequence != null)
            {
                _printSequence.Kill();
                _printSequence = null;
            }
            var stringBuilder = new StringBuilder(text.Length);
            var oldText = dialogText.text;
            
            _printSequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    ServicesHolder.GibberishService.PlayStream(pitch);
                    stringBuilder.Append(oldText);
                });
            
            
            
            for (var i = 0; i < text.Length; ++i)
            {
                _printSequence
                    .AppendCallback(() =>
                    {
                        stringBuilder.Append(text[stringBuilder.Length - oldText.Length]);
                        dialogText.text = stringBuilder.ToString();
                    })
                    .AppendInterval(symbolPrintDuration);
                
                if (i == text.Length - 2)
                    _printSequence
                        .AppendCallback(() =>
                        {
                            ServicesHolder.GibberishService.StopStream();
                        });
            }
            return _printSequence;
        }
    }
}