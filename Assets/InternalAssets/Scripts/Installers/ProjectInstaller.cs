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
            InstallClips();
            InstallServices();
            InstallSignals();
        }

        private void InstallServices()
        {
        }
        private void InstallClips()
        {
        }
        private void InstallSignals()
        {
        }
    }
}