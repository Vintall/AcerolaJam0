using InternalAssets.Scripts.NarrativeClips.Act1;
using InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1;
using UnityEngine;
using Zenject;

namespace InternalAssets.Scripts.Installers
{
    [CreateAssetMenu(menuName = "NarrativeClipsInstaller", fileName = "NarrativeClipsInstaller")]
    public class NarrativeClipsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private TestLeftNarrativeClipData testLeftNarrativeClipData;
        [SerializeField] private TestRightNarrativeClipData testRightNarrativeClipData;
        
        public override void InstallBindings()
        {
            Container.Bind<ITestLeftNarrativeClipData>().FromInstance(testLeftNarrativeClipData);
            Container.Bind<ITestRightNarrativeClipData>().FromInstance(testRightNarrativeClipData);
        }
    }
}
