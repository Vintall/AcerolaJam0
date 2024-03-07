using InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1;
using InternalAssets.Scripts.Services.PlayableDirector;
using InternalAssets.Scripts.Services.PlayableDirector.Impls;
using InternalAssets.Scripts.Services.SceneLoadingService.Impls;
using UnityEngine.SceneManagement;
using Zenject;

namespace InternalAssets.Scripts.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //SignalBusInstaller.Install(Container);
            
            InstallClips();
            InstallServices();
            InstallSignals();
        }

        private void InstallServices()
        {
            Container.BindInterfacesTo<NarrativeService>().AsSingle().NonLazy();
        }
        private void InstallClips()
        {
            Container.BindInterfacesTo<TestLeftNarrativeClip>();
            Container.BindInterfacesTo<TestRightNarrativeClip>();
        }
        private void InstallSignals()
        {
            Container.DeclareSignal<SignalPlayableDirectorChangeTimeline>();
            Container.DeclareSignal<SignalPlayableDirectorPlay>();
        }
    }
}