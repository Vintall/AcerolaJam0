using UnityEngine;
using Zenject;

namespace InternalAssets.Scripts
{
    public class FirstActInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<NarrativeService>().AsSingle().NonLazy();
        }
    }
}