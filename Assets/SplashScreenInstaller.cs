using System.Collections;
using System.Collections.Generic;
using InternalAssets.Scripts.Services.PlayableDirector.Impls;
using InternalAssets.Scripts.Services.SceneLoadingService.Impls;
using UnityEngine;
using Zenject;

public class SplashScreenInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        
        Container.BindInterfacesTo<SceneLoadingService>().AsSingle().NonLazy();
    }
}
