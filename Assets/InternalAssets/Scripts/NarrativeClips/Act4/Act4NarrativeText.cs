using DG.Tweening;
using InternalAssets.Scripts.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InternalAssets.Scripts.NarrativeClips.Act4
{
    public class Act4NarrativeText : MonoBehaviour
    {
        #region UIDialogService

        private Sequence ShowPanel() =>
            ServicesHolder.UIDialogService.ShowPanel();

        private Sequence HidePanel(float duration) =>
            ServicesHolder.UIDialogService.HidePanel(duration);

        private Sequence ShowPanel(float duration) =>
            ServicesHolder.UIDialogService.ShowPanel(duration);

        private Sequence HidePanel() =>
            ServicesHolder.UIDialogService.HidePanel();

        private Sequence PrintDialog(string text, float symbolPrintDuration, float pitch = 1) =>
            ServicesHolder.UIDialogService.PrintDialog(text, symbolPrintDuration, pitch);

        private Sequence PrintDialogWoCleaning(string oldText, string text, float symbolPrintDuration,
            float pitch = 1) =>
            ServicesHolder.UIDialogService.PrintDialogWoCleaning(oldText, text, symbolPrintDuration, pitch);

        private void ClearPanel() => ServicesHolder.UIDialogService.ClearPanel();

        #endregion
        
        public void PrintFirst()
        {
            DOTween.Sequence()
                .Append(ShowPanel())
                .Join(PrintDialog("Haha",
                    0.08f,
                    1.7f))
                .AppendInterval(0.5f)
                .Append(HidePanel());
        }

        public void PrintSecond()
        {
            DOTween.Sequence()
                .Append(ShowPanel())
                .Join(PrintDialog("What a bullshit",
                    0.08f,
                    1.7f))
                .AppendInterval(0.5f)
                .Append(HidePanel());
        }

        public void OnEnd()
        {
            SceneManager.LoadScene(5);
        }
    }
}