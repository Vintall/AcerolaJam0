using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace InternalAssets.Scripts.Services.UIServices
{
    public class ObjectiveService : MonoBehaviour
    {
        [SerializeField] private RectTransform objectivePanel;
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private float panelSlideDuration;
        [SerializeField] private float awaitDuration;
        private Sequence _printSequence;

        private void Awake()
        {
            ClearPanel();
        }

        public void PrintObjective(string text, float symbolPrintDuration)
        {
            if(_printSequence != null)
                return;
            
            _printSequence = DOTween.Sequence()
                .Append(objectivePanel.DOAnchorPosX(0, panelSlideDuration)
                    .SetEase(Ease.OutCubic));

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < text.Length; ++i)
            {
                _printSequence
                    .AppendCallback(() =>
                    {
                        ServicesHolder.SoundService.PlayTypingSound();
                    })
                    .AppendInterval(symbolPrintDuration)
                    .AppendCallback(() =>
                    {
                        stringBuilder.Append(text[stringBuilder.Length]);
                        objectiveText.text = stringBuilder.ToString();
                    });
            }

            _printSequence
                .AppendInterval(awaitDuration)
                .AppendCallback(MinimizePanel);
        }

        public void MinimizePanel()
        {
            var minimizePosition = Screen.width / 3;
            
            _printSequence = DOTween.Sequence()
                .Append(objectivePanel.DOAnchorPosX(minimizePosition, panelSlideDuration)
                    .SetEase(Ease.OutCubic));
        }

        public void HidePanel()
        {
            var minimizePosition = Screen.width / 3;
            var hidePosition = minimizePosition + Screen.width * 0.02f;
            
            _printSequence = DOTween.Sequence()
                .Append(objectivePanel.DOAnchorPosX(hidePosition, panelSlideDuration)
                    .SetEase(Ease.OutCubic));
        }

        public void ClearPanel()
        {
            var minimizePosition = Screen.width / 3;
            var hidePosition = minimizePosition + Screen.width * 0.02f;

            objectivePanel.DOAnchorPosX(hidePosition, 0);
            objectiveText.text = "";

            if (_printSequence != null)
            {
                _printSequence.Kill();
                _printSequence = null;
            }
        }
    }
}