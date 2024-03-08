using System;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace InternalAssets.Scripts.Services.ObjectiveService
{
    public class ObjectiveService : MonoBehaviour
    {
        [SerializeField] private RectTransform objectivePanel;
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private float panelSlideDuration;
        [SerializeField] private float awaitDuration;
        private Sequence printSequence;

        private void Awake()
        {
            ClearPanel();
        }

        public void PrintObjective(string text, float symbolPrintDuration)
        {
            if(printSequence != null)
                return;
            
            printSequence = DOTween.Sequence()
                .Append(objectivePanel.DOAnchorPosX(0, panelSlideDuration)
                    .SetEase(Ease.OutCubic));

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < text.Length; ++i)
            {
                printSequence
                    .AppendCallback(() =>
                    {
                        stringBuilder.Append(text[stringBuilder.Length]);
                        objectiveText.text = stringBuilder.ToString();
                    })
                    .AppendInterval(symbolPrintDuration);
            }

            printSequence
                .AppendInterval(awaitDuration)
                .AppendCallback(MinimizePanel);
        }

        public void MinimizePanel()
        {
            var minimizePosition = Screen.width / 3;
            
            printSequence = DOTween.Sequence()
                .Append(objectivePanel.DOAnchorPosX(minimizePosition, panelSlideDuration)
                    .SetEase(Ease.OutCubic));
        }

        public void HidePanel()
        {
            var minimizePosition = Screen.width / 3;
            var hidePosition = minimizePosition + Screen.width * 0.02f;
            
            printSequence = DOTween.Sequence()
                .Append(objectivePanel.DOAnchorPosX(hidePosition, panelSlideDuration)
                    .SetEase(Ease.OutCubic));
        }

        public void ClearPanel()
        {
            var minimizePosition = Screen.width / 3;
            var hidePosition = minimizePosition + Screen.width * 0.02f;

            objectivePanel.DOAnchorPosX(hidePosition, 0);
            objectiveText.text = "";
        }
    }
}